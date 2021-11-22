using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TelegramOnlyBot.Models
{
    public class SpecialtiesService
    {
        readonly IMongoCollection<Specialties> Specialties; /// коллекция в базе данных
        public SpecialtiesService()
        {
            /// строка подключения
            string connectionString = "mongodb://localhost:27017";
            /// получаем клиента для взаимодействия с базой данных
            MongoClient client = new(connectionString);
            /// получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase("Telegram");
            /// обращаемся к коллекции TimeTable
            Specialties = database.GetCollection<Specialties>("Specialties");
        }

        /// получаем все города по стране в БД
        public async Task<IEnumerable<Specialties>> GetSpecialties(string id)
        {
            return await Specialties.Find(new BsonDocument("Facylty", new ObjectId(id))).ToListAsync();
        }

        /// добавление документа
        public async Task<string> Create(Specialties p)
        {
            await Specialties.InsertOneAsync(p);
            return p.Id;
        }
        /// обновление документа
        public async Task Update(Specialties p)
        {
            await Specialties.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        /// удаление документа
        public async Task Remove(string id)
        {
            await Specialties.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
