using System.Collections.Generic;
using MongoDB.Driver;
using Repository.RabbitMQ;

namespace EntityHistoryMQReceive
{
    public class MongoDB
    {
        private readonly IMongoCollection<EntityChange> _collection;

        private static MongoDB _mongoDb;

        private static readonly object Locker = new object();
        public static MongoDB GetCollection()
        {
            if (_mongoDb == null)
            {
                lock (Locker)
                {
                    if (_mongoDb == null)
                    {
                        _mongoDb = new MongoDB();
                    }
                }
            }

            return _mongoDb;
        }

        private MongoDB()
        {
            //建立连接
            var services = new List<MongoServerAddress>();
            services.Add(new MongoServerAddress("localhost", 27017));

            var settings = new MongoClientSettings
            {
                Servers = services
            };

            var client = new MongoClient(settings);

            //建立数据库
            var database = client.GetDatabase("IdentityServerDemo");

            //建立collection
            _collection = database.GetCollection<EntityChange>("EntityHistory");
        }

        public void Insert(IEnumerable<EntityChange> entityChanges)
        {
            _collection.InsertMany(entityChanges);
        }
    }
}
