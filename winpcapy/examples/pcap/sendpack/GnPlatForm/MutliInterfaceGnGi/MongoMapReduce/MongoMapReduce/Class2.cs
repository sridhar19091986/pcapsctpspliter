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
    class Movie
    {
        public string Title;
        public string Category;
        public int Minutes;
    }
    class Class2
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
            var db = mongo["csharpdriverunittests"];
            var collection = db["movies"];


            var movies = new List<Movie>
    {
        new Movie { Title="The Perfect Developer", 
                    Category="SciFi", Minutes=118 },
        new Movie { Title="Lost In Frankfurt am Main", 
                    Category="Horror", Minutes=122 }, 
        new Movie { Title="The Infinite Standup", 
                    Category="Horror", Minutes=341 } 
    };
            collection.InsertBatch(movies);

            string map = @"
    function() {
        var movie = this;
        emit(movie.Category, { count: 1, totalMinutes: movie.Minutes });
    }";

            string reduce = @"        
    function(key, values) {
        var result = {count: 0, totalMinutes: 0 };

        values.forEach(function(value){               
            result.count += value.count;
            result.totalMinutes += value.totalMinutes;
        });

        return result;
    }";

            string finalize = @"
    function(key, value){
      
      value.average = value.totalMinutes / value.count;
      return value;

    }";
            //var linq = from p in collection[""]
            //           select t;

            var options = new MapReduceOptionsBuilder();
            options.SetFinalize(finalize);
            options.SetOutput(MapReduceOutput.Inline);
            var results = collection.MapReduce(map, reduce, options);

            foreach (var result in results.GetResults())
            {
                Console.WriteLine(result.ElementAt(0).Name); 
            }

            
        }

    }
}