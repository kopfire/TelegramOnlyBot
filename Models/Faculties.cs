using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TelegramOnlyBot.Models
{
    public class Faculties
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string University { get; set; }

        [Display(Name = "Факультет")]
        public string Name { get; set; }
    }
}
