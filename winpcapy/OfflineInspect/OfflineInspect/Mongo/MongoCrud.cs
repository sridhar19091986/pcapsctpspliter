using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MongoDB.Driver;
using MongoDB.Linq;
using MongoDB.Driver.Linq;

namespace OfflineInspect.Mongo
{
    public class MongoCrud<T>
    {

        public string mongodb_collection;
        private string mongo_conn;
        private string mongo_db;

        public MongoCrud(string conn, string db, string col)
        {
            this.mongo_conn = conn;
            this.mongo_db = db;
            this.mongodb_collection = col;
        }

        public void BulkMongo(List<T> fcmb, bool delete_col)
        {
            var collection = GetMongoCollection(delete_col);

            collection.InsertBatch(fcmb);
        }

        public IQueryable<T> QueryMongo()
        {

            var collection = GetMongoCollection(false);

            var query = from p in collection.AsQueryable<T>()
                        select p;


            return query;
        }
        public MongoCollection GetMongoCollection(bool delete_col)
        {

            var mongoUrlBuilder = new MongoUrlBuilder(this.mongo_conn);
            var serverSettings = mongoUrlBuilder.ToServerSettings();
            if (!serverSettings.SafeMode.Enabled)
            {
                serverSettings.SafeMode = SafeMode.True;
            }

            var mongo = MongoServer.Create(serverSettings);
            var db = mongo[this.mongo_db];

            if (delete_col == true)
                db.DropCollection(this.mongodb_collection);

            var collection = db[this.mongodb_collection];

            return collection;
        }
    }
}
