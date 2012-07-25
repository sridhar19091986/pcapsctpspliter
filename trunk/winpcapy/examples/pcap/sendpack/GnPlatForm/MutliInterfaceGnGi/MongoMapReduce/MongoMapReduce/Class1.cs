using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB;

namespace MongoDBBlog.Tester
{
    class Class1
    {

        public static void test()
        {
            Mongo db = new Mongo();
            db.Connect();

            var test = db.GetDatabase("test");
            var things = test["things"];

            for (int i = 1; i <= 10; i++)
            {
                Document record = new Document();
                record["_id"] = i;
                record["userid"] = i;
                record["ip"] = "10.0.7." + i;
                record["username"] = "用户" + i;
                record["nickname"] = "用户" + i;
                record["password"] = "";
                record["groupid"] = i;//下面将就该字段使用MAPREDUCE方式进行分组统计
                record["olimg"] = "";
                record["adminid"] = 0;
                record["invisible"] = 0;
                record["action"] = 0;
                record["lastactivity"] = 1;
                record["lastposttime"] = DateTime.Now.ToString();
                record["lastpostpmtime"] = DateTime.Now.ToString();
                record["lastsearchtime"] = DateTime.Now.ToString();
                record["lastupdatetime"] = "1212313221231231213321";
                record["forumid"] = 0;
                record["forumname"] = "";
                record["titleid"] = 0;
                record["title"] = "";
                record["verifycode"] = "";
                record["newpms"] = 0;
                record["newnotices"] = 0;
                things.Insert(record);

            }

            //db.Disconnect();
            string mapfunction = "function() {  if(this.groupid==5) {emit({groupid : 5}, 1);} }";
            string reducefunction = "function(key, current ){" +
                                            "   var count = 0;" +
                                            "   for(var i in current) {" +
                                            "       count+=current[i];" +
                                            "   }" +
                                            "   return count;" +
                                          "};";
            MapReduce mr = things.MapReduce();

            string outstr;

            mr.Map(new Code(mapfunction)).Reduce(new Code(reducefunction));

            //mr.Execute();

            foreach (Document doc in mr.Documents)
            {
                int groupCount = Convert.ToInt32(doc["value"]);

                Console.WriteLine(groupCount);
            }

            mr.Dispose();




        }
    }
}
