using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using TelegramOnlyBot.Helpers.JSON;

namespace TelegramOnlyBot.Models
{
    public class TimeTables
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Display(Name = "Группа")]
        public string Group { get; set; }

        [Display(Name = "Недели")]
        public Week[] Weeks { get; set; }

        [Display(Name = "Студенты")]
        public List<long> Students { get; set; }
    }
}
