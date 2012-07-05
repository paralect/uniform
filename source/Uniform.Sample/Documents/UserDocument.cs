using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Uniform.Attributes;

namespace Uniform.Documents
{
    [Collection("users")]
    public class UserDocument
    {
        [BsonId]
        public String UserId { get; set; }
        public String UserName { get; set; }
        public String About { get; set; } 
    }

/*    public class UserDocumentByUserName
    {
        public void DoIt(List<UserDocument> data)
        {
            var z = from user in data
                select new { user.UserId, user.UserName };
        }
    }*/
}