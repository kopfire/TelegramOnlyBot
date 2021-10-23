using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TelegramOnlyBot.Models
{
    public class Cities
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string Country { get; set; }

        [Display(Name = "Город")]
        public string Name { get; set; }
    }
}
