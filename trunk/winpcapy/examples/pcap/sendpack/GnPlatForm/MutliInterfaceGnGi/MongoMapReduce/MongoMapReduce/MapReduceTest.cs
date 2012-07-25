using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using MongoDB;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Linq;

namespace MongoDBBlog.Tester
{

    class MapReduceTest
    {


        public static void test()
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
            var collection = db["Gi_Get2x_Multi"];

            string map = @"
    function() {
        var abc = this;
        emit(abc.URI_Main, { count: 1, totalMinutes: 0,totalCount:0 });
}


    }"
                ;

            string reduce = @"    
    
    function(key, values) {
        var result = {count: 0, totalMinutes: 0,totalCount:0 };

        values.forEach(function(value){               
            result.count += value.count;
            result.totalMinutes += value.totalMinutes ;

  
        });




        return result;
    }"
                ;
           
           //db.RunCommand
            var abbsum = db["Gi_Get2x_Multi"].FindAll().Count();


            string cmd = @"
                db.Gi_Get2x_Multi.aggregate([ 
 { $group: {_id:null, 
            total:{$sum:'$Get2x'} } }
])
";
            //CommandResult cr = new CommandResult();
            var comd = db.RunCommandAs(typeof(CommandResult), cmd);
        
   

            string finalize = @"

    function(key, value){
      
value.average =value.count / " +     comd.Response.AsBsonArray[0].ToString()+ @";
value.totalCount=" + abbsum.ToString() + @"
      
      return value;

    }"
                ;

            //
            //var query=QueryBuilde
            IMongoSortBy abc = SortBy.Descending("value.count");
            IMongoQuery query=Query.EQ("value.count",1);
            //MapReduceOutput outname=

           // var abc = "{ \"sort\" : { \"value.count\" : 1} }";

        
            //MapReduceOutput out=new 

            Console.WriteLine(abc.ToString());
            var options = new MapReduceOptionsBuilder();
            //options.SetScope(
            //options.SetQuery(query);//
           // options.SetSortOrder(abc);
            //options.SetLimit(2);
            //options.SetKeepTemp(true);
            options.SetFinalize(finalize);
            options.SetOutput("tempppp");
        
            var results = collection.MapReduce(map, reduce, options);

            Console.WriteLine(results.Command);
            //results.Command.ToString();

            // var resultss = results.GetResults().OrderByDescending(e => e.Elements.Select["count"]).Take(100);

            var resultss = db["tempppp"].FindAll().SetSortOrder(abc).SetLimit(10);
            //var resultss = results.GetResults().OrderByDescending(p => "value.count").Take(10);

            foreach (var result in resultss )
            {
                Console.WriteLine(result);
            }


        }

    }
}