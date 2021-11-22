using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TelegramOnlyBot.Models
{
    public class FacultiesService
    {
        readonly IMongoCollection<Faculties> Faculties; /// коллекция в базе данных
        public FacultiesService()
        {
            /// строка подключения
            string connectionString = "mongodb://localhost:27017";
            /// получаем клиента для взаимодействия с базой данных
            MongoClient client = new(connectionString);
            /// получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase("Telegram");
            /// обращаемся к коллекции TimeTable
            Faculties = database.GetCollection<Faculties>("Faculties");
        }

        /// получаем все факультеты по университету
        public async Task<IEnumerable<Faculties>> GetFaculties(string id)
        {
            return await Faculties.Find(new BsonDocument("University", new ObjectId(id))).ToListAsync();
        }

        /// добавление документа
        public async Task<string> Create(Faculties p)
        {
            await Faculties.InsertOneAsync(p);
            return p.Id;
        }
        /// обновление документа
        public async Task Update(Faculties p)
        {
            await Faculties.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        /// удаление документа
        public async Task Remove(string id)
        {
            await Faculties.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
    }
}
