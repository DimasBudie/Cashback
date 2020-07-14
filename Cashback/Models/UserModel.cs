using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Cashback.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get;set;}
        public DateTimeOffset? CreatedAt { get; set; }
        public string Name { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}