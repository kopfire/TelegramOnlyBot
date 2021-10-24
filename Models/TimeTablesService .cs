using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegramOnlyBot.Models
{
    public class TimeTablesService
    {
        IMongoCollection<TimeTables> TimeTables; /// коллекция в базе данных
        public TimeTablesService()
        {
            /// строка подключения
            string connectionString = "mongodb://localhost:27017";
            var connection = new MongoUrlBuilder(connectionString);
            /// получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            /// получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase("Telegram");
            /// обращаемся к коллекции TimeTable
            TimeTables = database.GetCollection<TimeTables>("TimeTables");
        }

        /// получаем все группы по городу в БД
        public async Task<IEnumerable<TimeTables>> GetTimeTables(string id)
        {
            return await TimeTables.Find(new BsonDocument("Speciality", new ObjectId(id))).ToListAsync();
        }

        /// получаем один документ по группе

        public async Task<TimeTables> GetTimeTable(string group)
        {
            return await TimeTables.FindAsync(new BsonDocument("Group", group)).Result.FirstOrDefaultAsync();
        }

        /// получаем один документ по id студента
        public async Task<TimeTables> GetTimeTable(long id)
        {
            var filter = Builders<TimeTables>.Filter.AnyEq(x => x.Students,  id );
            return await TimeTables.FindAsync(filter).Result.FirstOrDefaultAsync();
        }
        /// добавление документа
        public async Task Create(TimeTables p)
        {
            await TimeTables.InsertOneAsync(p);
        }

        /// добавление студента в бд
        public async Task UpdateStudents(string id, long idStudent)
        {
            var filter = Builders<TimeTables>
             .Filter.Eq(e => e.Id, id);

            var update = Builders<TimeTables>.Update
                    .Push<long>(e => e.Students, idStudent);

            await TimeTables.FindOneAndUpdateAsync(filter, update);
        }

        /// обновление документа
        public async Task Update(TimeTables p)
        {
            await TimeTables.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(p.Id)), p);
        }
        /// удаление документа
        public async Task Remove(string id)
        {
            await TimeTables.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));
        }
        /// удаление студента
        public async Task RemoveStudent(long id)
        {
            var filter = Builders<TimeTables>.Filter.Where(mu => mu.Id == GetTimeTable(id).Result.Id);
            var students = GetTimeTable(id).Result.Students;
            students = students.Where(val => val != id).ToList();
            var update = Builders<TimeTables>.Update.Set(mu => mu.Students, students);
            await TimeTables.UpdateOneAsync(filter, update);
        }
    }
}
