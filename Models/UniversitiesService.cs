using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TelegramOnlyBot.Models
{
    public class UniversitiesService
    {
        IMongoCollection<Universities> Universities; /// коллекция в базе данных
        public UniversitiesService()
        {
            /// строка подключения
            string connectionString = "mongodb://localhost:27017";
            var connection = new MongoUrlBuilder(connectionString);
            /// получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            /// получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase("Telegram");
            /// обращаемся к коллекции TimeTable
            Universities = database.GetCollection<Universities>("Universities");
        }

        /// получаем все университеты по городу в БД
        public async Task<IEnumerable<Universities>> GetUniversities(string id)
        {
            return await Universities.Find(new BsonDocument("City", new ObjectId(id))).ToListAsync();
        }

        /// добавление документа
        public async Task Create(Universities p)
        {
            await Universities.InsertOneAsync(p);
        }
        /// обновление документа
        public async Task Update(Universities p)
        {
            await Universities.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        /// удаление документа
        public async Task Remove(string id)
        {
            await Universities.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
