using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDBBlog.Tester;

namespace MongoMapReduce
{
    class Program
    {
        static void Main(string[] args)
        {
            MapReduceTest.test();

            //MigratingFromSqlToMongo.ttt();

            Console.WriteLine("ok,,,,,,,|");
            //Class2.test();
            // Class1.test();
        }
    }
}
