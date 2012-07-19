using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Linq;
using MongoDB.Driver.Linq;

namespace MultiGnGi
{
    /*
     * http://stackoverflow.com/questions/6376436/mongodb-drop-every-database
     * you can create a javascript loop that do the job and then execute it in the mongoconsole.

var dbs = db.getMongo().getDBNames()
for(var i in dbs){
    db = db.getMongo().getDB( dbs[i] );
    print( "dropping db " + db.getName() );
    db.dropDatabase();
}
save it to dropall.js and then execute:

mongo dropall.js
*/
    public class FlowControlOneMs
    {
        public object _id;
        
        public int? BeginFrameNum; 

        //public int lac;
        //public int ci;

  

        public int fcontrol_cnt;
        public int packet_cnt;
        public double bucket_size_avg;
        public double bucket_size_min;
        public double bucket_size_max;
        public double leak_rate_avg;
        public double leak_rate_min;
        public double leak_rate_max;
        public string first_delay;
        public string leak_rate;
        public string bucket_size;
        public string down_total_len;
        public string down_packet_rate;
        public string fcm_time;

        public void BulkMongo(List<FlowControlOneMs> fcom)
        {

            var connectionString = "mongodb://localhost/?safe=true"; // TODO: make this configurable

            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var serverSettings = mongoUrlBuilder.ToServerSettings();
            if (!serverSettings.SafeMode.Enabled)
            {
                serverSettings.SafeMode = SafeMode.True;
            }

            var mongo = MongoServer.Create(serverSettings);
            var db = mongo["guangzhou_gb"];

            db.DropCollection("FlowControlOneMs");
            var collection = db["FlowControlOneMs"];



            collection.InsertBatch(fcom);
        }

        public IQueryable<FlowControlOneMs> QueryMongo()
        {

            var connectionString = "mongodb://localhost/?safe=true"; // TODO: make this configurable

            var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
            var serverSettings = mongoUrlBuilder.ToServerSettings();
            if (!serverSettings.SafeMode.Enabled)
            {
                serverSettings.SafeMode = SafeMode.True;
            }

            var mongo = MongoServer.Create(serverSettings);

            var db = mongo["guangzhou_gb"];
            var collection = db.GetCollection("FlowControlOneMs");



            var query = from p in collection.AsQueryable<FlowControlOneMs>()
                        select p;


            return query;
        }
    }
}
