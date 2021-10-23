using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TelegramOnlyBot.Models
{
    public class Universities
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string City { get; set; }

        [Display(Name = "Университет")]
        public string Name { get; set; }
    }
}
