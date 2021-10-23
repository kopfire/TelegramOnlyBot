using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TelegramOnlyBot.Models
{
    public class CitiesService
    {
        IMongoCollection<Cities> Cities; /// коллекция в базе данных
        public CitiesService()
        {
            /// строка подключения
            string connectionString = "mongodb://localhost:27017";
            var connection = new MongoUrlBuilder(connectionString);
            /// получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            /// получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase("Telegram");
            /// обращаемся к коллекции TimeTable
            Cities = database.GetCollection<Cities>("Cities");
        }

        /// получаем все города по стране в БД
        public async Task<IEnumerable<Cities>> GetCities(string id)
        {
            return await Cities.Find(new BsonDocument("Country", new ObjectId(id))).ToListAsync();
        }

        /// добавление документа
        public async Task Create(Cities p)
        {
            await Cities.InsertOneAsync(p);
        }
        /// обновление документа
        public async Task Update(Cities p)
        {
            await Cities.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        /// удаление документа
        public async Task Remove(string id)
        {
            await Cities.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
