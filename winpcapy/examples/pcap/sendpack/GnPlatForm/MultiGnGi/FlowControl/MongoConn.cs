using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Configuration;

namespace MultiGnGi
{
    public class MongoConn
    {

        public static MongoCollection GetMongoCollection(string mongo_conn, string mongodb_collection_name, bool delete_col)
        {
            var connectionString = mongo_conn;

            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var serverSettings = mongoUrlBuilder.ToServerSettings();
            if (!serverSettings.SafeMode.Enabled)
            {
                serverSettings.SafeMode = SafeMode.True;
            }

            var mongo = MongoServer.Create(serverSettings);
            var db = mongo[ConfigurationManager.AppSettings[mongodb_collection_name].ToString()];

            if (delete_col == true)
                db.DropCollection(mongodb_collection_name);

            var collection = db[mongodb_collection_name];

            return collection;
        }
    }
}
