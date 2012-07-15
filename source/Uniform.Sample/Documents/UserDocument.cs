using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using ServiceStack.DataAnnotations;

namespace Uniform.Sample.Documents
{
    [Document(SampleDatabases.Mongodb, SampleCollections.Users)]
    [Alias("users")]
    public class UserDocument
    {
        [DocumentId, BsonId]
        public String UserId { get; set; }
        public String UserName { get; set; }
        public String About { get; set; }

        [StringLength(6000)]
        public String Aga { get; set; }

        private static Random _random = new Random();

        public UserDocument()
        {
            Aga =
                "Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях. Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях. Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях. Янка Купала родился 7 июля 1882 года в деревне Вязынка (ныне Молодечненского района Минской области Беларуси). Родители были обедневшие шляхтичи, арендовавшие земли в помещичьих угодьях. ";



        }
    }
}