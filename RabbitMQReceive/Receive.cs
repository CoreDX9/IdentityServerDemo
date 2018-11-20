using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repository.RabbitMQ;

namespace EntityHistoryMQReceive
{
    /// <summary>
    /// 基本消息接收
    /// </summary>
    public class Receive
    {
        private static string[] _includeProperties = {"Id", "RowVersion" };
        public static void ReceiveMsg()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //声明队列，持久性
                channel.QueueDeclare(queue: "IdentityServerEntityHistory",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // 告知RabbitMQ，在未收到当前Worker的消息确认信号时，不再分发给消息，确保公平调度。
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] 正在接收消息……");

                var consumer = new EventingBasicConsumer(channel);//声明消费者并绑定通道
                                                                  //定义收到消息后要执行的消息处理函数
                consumer.Received += (sender, arg) =>
                {
                    //记录历史，如果发生错误，应答退回消息重新处理并休眠线程10秒
                    try
                    {
                        var body = arg.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"[x] 收到消息：\r\n{message}");

                        //还原消息为对象
                        var msgObj = JsonConvert.DeserializeObject<List<EntityChange>>(message);
                        //对更新剔除没有发生变化的属性（Id和RowVersion要保留，用于定位记录和时间顺序）
                        if (msgObj.Count > 0)
                        {
                            msgObj.ForEach(o =>
                                o.Changes = o.Operate == "Update"
                                    ? o.Changes.Where(c => c.IsModified || _includeProperties.Contains(c.PropertyName))
                                        .ToList()
                                    : o.Changes);
                            //把历史记录写入MongoDB
                            MongoDB.GetCollection().Insert(msgObj);
                        }

                        Console.WriteLine("[x] 已处理消息");

                        // 手动发送消息确认信号。
                        channel.BasicAck(deliveryTag: arg.DeliveryTag, multiple: false);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[x] 发生异常：\r\n" + e.Message);
                        channel.BasicNack(deliveryTag: arg.DeliveryTag, multiple: false, requeue: true);
                        Thread.Sleep(1000 * 10);
                    }

                };

                //启动消息监听
                // autoAck:false - 关闭自动消息确认，调用`BasicAck`方法进行手动消息确认。
                // autoAck:true  - 开启自动消息确认，当消费者接收到消息后就自动发送ack信号，无论消息是否正确处理完毕。
                channel.BasicConsume(queue: "IdentityServerEntityHistory",//监听的队列
                    autoAck: false,//是否自动确认消息
                    consumer: consumer);//消费者

                Console.WriteLine("按 [enter（回车）] 退出应用");
                Console.ReadLine();
            }
        }
    }
}