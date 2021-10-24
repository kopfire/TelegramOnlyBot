using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TelegramOnlyBot.Models
{
    public class CountriesService
    {
        IMongoCollection<Countries> Countries; /// коллекция в базе данных
        public CountriesService()
        {
            /// строка подключения
            string connectionString = "mongodb://localhost:27017";
            var connection = new MongoUrlBuilder(connectionString);
            /// получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            /// получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase("Telegram");
            /// обращаемся к коллекции TimeTable
            Countries = database.GetCollection<Countries>("Countries");
        }

        /// получаем все страны в БД
        public async Task<IEnumerable<Countries>> GetCounties()
        {
            var builder = new FilterDefinitionBuilder<Countries>();
            var filter = builder.Empty;
            return await Countries.Find(filter).ToListAsync();
        }

        /// добавление документа
        public async Task Create(Countries p)
        {
            await Countries.InsertOneAsync(p);
        }
        /// обновление документа
        public async Task Update(Countries p)
        {
            await Countries.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        /// удаление документа
        public async Task Remove(string id)
        {
            await Countries.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
