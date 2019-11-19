using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Util.TypeExtensions;

namespace Repository.RabbitMQ
{
    public class EntityChange
    {
        public string EntityType { get; set; }
        public string TableName { get; set; }
        public string Operate { get; set; }
        public DateTimeOffset ChangeTime { get; set; }
        public List<PropertyChange> Changes { get; set; }

        public class PropertyChange
        {
            public string PropertyName { get; set; }
            public string PropertyType { get; set; }
            public string ColumnName { get; set; }
            public string ColumnType { get; set; }
            public bool IsModified { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
        }
    }

    public interface IEntityHistoryTrackable
    {
        void StartRecord();
        void StopRecord();
    }

    public interface IEntityHistoryRecorder : IEntityHistoryTrackable
    {
        object DiscoveryChanges(DbContext dbContext);
        Task RecordHistory(object change, bool isSoftDelete);
        Task<Task> RecordHistory(object change, bool isSoftDelete, Task contextSaveChangeTask);
    }

    public class EntityHistoryRecorder : IEntityHistoryRecorder, IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IBasicProperties _properties;
        private readonly ConcurrentQueue<byte[]> _entityHistoryPublishQueue;
        private Task _publishTask;
        private CancellationTokenSource _cancellationTokenSource;
        private int _publishTimes;
        private ILogger _logger;

        private class ChangedEntity
        {
            public EntityEntry Entry { get; set; }

            public EntityState State { get; set; }

            public Dictionary<PropertyEntry, EntityChange.PropertyChange> Changes { get; set; }
        }

        public EntityHistoryRecorder(ILogger<EntityHistoryRecorder> logger, IOptions<EntityHistoryRecorderOptions> optionsAccessor)
        {
            _logger = logger;
            logger.LogInformation("正在连接RabbitMQ并启动实体变更推送任务……");
            Console.WriteLine("正在连接RabbitMQ并启动实体变更推送任务……");
            _publishTimes = 0;
            _entityHistoryPublishQueue = new ConcurrentQueue<byte[]>();
            _connectionFactory = new ConnectionFactory()
            {
                HostName = optionsAccessor.Value.HostName,
                AutomaticRecoveryEnabled = optionsAccessor.Value.AutomaticRecoveryEnabled,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(optionsAccessor.Value.NetworkRecoveryIntervalSeconds),
                TopologyRecoveryEnabled = optionsAccessor.Value.TopologyRecoveryEnabled
            };//创建连接工厂
            _connection = _connectionFactory.CreateConnection();//创建连接
            _channel = _connection.CreateModel(); //创建通道

            //声明队列（幂等队列）
            _channel.QueueDeclare(queue: "IdentityServerEntityHistory",//队列名称
                durable: true,//是否持久化队列
                exclusive: false,//是否私有化队列
                autoDelete: false,//是否自动删除
                arguments: null);//参数

            _channel.ConfirmSelect();

            _channel.BasicAcks += (sender, args) => { };
            _channel.BasicReturn += (sender, args) =>
            {
                _entityHistoryPublishQueue.Enqueue(args.Body);
            };

            // 将消息标记为持久性。
            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;

            _cancellationTokenSource = new CancellationTokenSource();
            _publishTask = new Task(() => Publish(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            try
            {
                _publishTask.Start();
                logger.LogInformation("已启动实体变更推送任务");
                Console.WriteLine("已启动实体变更推送任务");
            }
            catch (Exception e)
            {
                logger.LogError(e, "实体变更推送任务启动失败");
            }
        }

        private void Publish(CancellationToken cancellationToken)
        {
            while (true)
            {
                var publishCount = 0;
                while (_entityHistoryPublishQueue.TryDequeue(out var msg))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _entityHistoryPublishQueue.Enqueue(msg);
                        return;
                    }
                    try
                    {
                        _channel.BasicPublish(exchange: "",
                            routingKey: "IdentityServerEntityHistory",
                            basicProperties: _properties,
                            body: msg,
                            mandatory: true);

                        publishCount++;
                    }
                    catch(Exception e)
                    {
                        _entityHistoryPublishQueue.Enqueue(msg);
                        _logger.LogError(e, $"第{_publishTimes}次推送实体变更：推送失败！");
                        throw;
                    }
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                _publishTimes++;
                _logger.LogDebug($"第{_publishTimes}次推送实体变更：本次已推送 {publishCount} 条实体变更（每条推送可能包含多个实体变更）");
                Thread.Sleep(1000 * 10);
            }
        }

        public virtual object DiscoveryChanges(DbContext dbContext)
        {
            return dbContext.ChangeTracker.Entries().Where(e =>
                    new[] {EntityState.Added, EntityState.Deleted, EntityState.Modified}.Contains(e.State))
                .Select(e => new ChangedEntity()
                {
                    Entry = e,
                    State = e.State,
                    Changes = e.Properties.ToDictionary(p => p,
                        p =>
                        {
                            var converter = (p.Metadata.FindAnnotation("ValueConverter")?.Value as ValueConverter);
                            var o = p.OriginalValue != null && converter != null
                                ? converter.ConvertToProvider(p.OriginalValue).ToString()
                                : p.Metadata.ClrType == typeof(byte[]) && p.OriginalValue != null
                                    ? $"0x{(p.OriginalValue as byte[]).ToHexString()}"
                                    : p.OriginalValue?.ToString();
                            var n = p.CurrentValue != null && converter != null
                                ? converter.ConvertToProvider(p.CurrentValue).ToString()
                                : p.Metadata.ClrType == typeof(byte[]) && p.OriginalValue != null
                                    ? $"0x{(p.CurrentValue as byte[]).ToHexString()}"
                                    : p.CurrentValue?.ToString();
                            return new EntityChange.PropertyChange
                            {
                                ColumnName = p.Metadata.Relational().ColumnName,
                                ColumnType = p.Metadata.Relational().ColumnType,
                                PropertyName = p.Metadata.Name,
                                PropertyType = p.Metadata.ClrType.FullName,
                                IsModified = p.IsModified,
                                OldValue = o,
                                NewValue = n
                            };
                        })
                }).ToList();
        }

        public virtual Task RecordHistory(object change, bool isSoftDelete)
        {
            return Task.Run(() =>
            {
                var changes = change as List<ChangedEntity>;
                var toSend = changes?.Select(c =>
                {
                    var operate = string.Empty;
                    switch (c.State)
                    {
                        case EntityState.Added:
                            operate = "Insert";
                            break;
                        case EntityState.Modified:
                            operate = "Update";
                            break;
                        case EntityState.Deleted:
                            operate = isSoftDelete ? "SoftDelete" : "HardDelete";
                            break;
                    }

                    return new EntityChange
                    {
                        TableName = c.Entry.Metadata.Relational().TableName,
                        EntityType = c.Entry.Metadata.Name,
                        Operate = operate,
                        ChangeTime = DateTimeOffset.Now,
                        Changes = c.Changes.Select(p =>
                        {
                            var converter = (p.Key.Metadata.FindAnnotation("ValueConverter")?.Value as ValueConverter);
                            var n = p.Key.CurrentValue != null && converter != null
                                ? converter.ConvertToProvider(p.Key.CurrentValue).ToString()
                                : p.Key.Metadata.ClrType == typeof(byte[]) && p.Key.CurrentValue != null
                                    ? $"0x{(p.Key.CurrentValue as byte[]).ToHexString()}"
                                    : p.Key.CurrentValue?.ToString();

                            return new EntityChange.PropertyChange
                            {
                                PropertyName = p.Value.PropertyName,
                                PropertyType = p.Value.PropertyType,
                                ColumnName = p.Value.ColumnName,
                                ColumnType = p.Value.ColumnType,
                                IsModified = p.Value.IsModified,
                                OldValue = p.Value?.OldValue,
                                NewValue = n
                            };
                        }).ToList()
                    };
                }).ToList();

                if (toSend?.Any() == true)
                {
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(toSend));
                    _entityHistoryPublishQueue.Enqueue(body);
                }
            });
        }

        public virtual Task<Task> RecordHistory(object change, bool isSoftDelete, Task contextSaveChangeTask)
        {
            if (contextSaveChangeTask == null)
            {
                return Task.Run<Task>(() => RecordHistory(change, isSoftDelete));
            }

            return Task.Run<Task>(() =>
            {
                contextSaveChangeTask.Wait();
                return RecordHistory(change, isSoftDelete);
            });
        }

        public void StartRecord()
        {
            try
            {
                _publishTask.Start();
            }
            catch
            {
                if (_publishTask == null)
                {
                    _cancellationTokenSource?.Dispose();

                    _cancellationTokenSource = new CancellationTokenSource();
                    _publishTask = new Task(() => Publish(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
                    _publishTask.Start();
                }
                else if (new[] {TaskStatus.Canceled, TaskStatus.Faulted, TaskStatus.RanToCompletion}.Contains(_publishTask.Status))
                {
                    _publishTask.Dispose();
                    _cancellationTokenSource.Dispose();

                    _cancellationTokenSource = new CancellationTokenSource();
                    _publishTask = new Task(() => Publish(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
                    _publishTask.Start();
                }
            }
        }

        public void StopRecord()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            using (_cancellationTokenSource)
            using (_publishTask)
            using (_channel)
            using (_connection)
            {
                _cancellationTokenSource.Cancel();

                Console.WriteLine("正在等待RabbitMQ实体变更推送任务退出……");
                _logger.LogInformation("正在等待RabbitMQ实体变更推送任务退出……");

                _publishTask.Wait();

                if (_entityHistoryPublishQueue.Count < 1)
                {
                    _logger.LogInformation($"RabbitMQ实体变更推送任务已退出");
                }
                else
                {
                    _logger.LogWarning($"RabbitMQ实体变更推送任务已退出，剩余{_entityHistoryPublishQueue.Count}条消息未推送！");
                }
            }
        }
    }
}
