using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

/*

改进第1点，如果部署时，只要注释  app.config 的 localhost

改进第2点，new client 时，重新配置时间。


http://www.cnblogs.com/LingzhiSun/archive/2011/03/22/EF41_Find.html
EF4.1为我们提供了一个新的API： DbSet<>.Find()。它可以帮助我们通过主键来查找对应的实体。
并且如果相应的实体已经被ObjectContext缓存，EF会在缓存中直接返回对应的实体，而不会执行数据库访问。


和之前直接用Where或First的调用不同，Find函数对于刚刚新增的实体也能找到：

 

using (var context = new MyContext())
{
    context.People.Add(new People { PersonID = 100 });
    var newPerson = context.People.Find(100);
}


 * http://www.cnblogs.com/LingzhiSun/archive/2011/03/25/EF41_Local.html
今天为大家带来DbSet.Local属性的使用与实现。
 *
 * 和上次介绍的Find函数首先查找context中缓存的实体类似，DbSet的Local属性也是返回context中缓存并且被跟踪的实体。
 * 不同点在于，Local属性不会返回状态为EntityState.Deleted的实体，且即使缓存中什么实体都没有，
 * 也不会对数据库进行访问。这样的设计也正符合Local（本地）之意。

 

看一个例子：



using (var db = new MyDbContext())
{
     // 此处调用EF4.1的新扩展方法DbSet<>.Load()从数据库导入对应的实体到缓存中
     db.People.Load();
     db.People.Add(new Person { Name = "Michael" });
     db.People.Remove(db.People.Find(1));
     foreach (var p in db.People.Local)
     {
         // 这里调用了EF4.1的新方法Entry来得到实体的DbEntryEntry
         Console.WriteLine("Found {0}: {1} with state {2}", p.PersonID, p.Name, db.Entry(p).State);
     }
}  
这里的输出结果类似于：

Found 0: Michael with state Added 
Found 2: Jennie with state Unchanged 
Found 3: Bob with state Unchanged  
Found 4: Mike with state Unchanged

http://www.cnblogs.com/LingzhiSun/archive/2011/04/13/EF41_WokingWithProperties.html#2384862
using (var context = new MyDbContext())
{
    // 有关Find方法，请看EF4.1系列博文一
    var person = context.People.Find(1);

    // 得到Person.Name属性的当前值
    string currentName = context.Entry(person).Property(p => p.Name).CurrentValue;

    // 设置Person.Name属性的当前值
    context.Entry(person).Property(p => p.Name).CurrentValue = "Michael";

    // 通过string值"Name"来获得DbPropertyEntry，而非通过Lambda Expression
    object currentName2 = context.Entry(person).Property("Name").CurrentValue;
}
 * 
 http://www.cnblogs.com/LingzhiSun/archive/2011/04/27/EF_Trick4.html
 * 
 * 在EF3.5 SP1和EF 4中，我们可以这样来进行NoTracking查询：

using (var context = new MyObjectContext())
{
    context.People.MergeOption = System.Data.Objects.MergeOption.NoTracking;
    var list = context.People.Where(p => PersonID > 100).ToList();
}
http://www.cnblogs.com/LingzhiSun/archive/2011/05/05/EF_Trick5.html
在EF 4.1中，我们可以直接调用DbQuery<>的ToString()方法得到所生成的SQL。

using (var context = new MyDbContext())
{
    var people = from p in context.People
                 where p.PersonID > 100
                 select p;

    string sql = people.ToString();
}
所生成的SQL是：

SELECT 
[Extent1].[PersonID] AS [PersonID], 
[Extent1].[Name] AS [Name]
FROM [dbo].[People] AS [Extent1]
WHERE [Extent1].[PersonID] > 100

http://www.mhzg.net/a/20122/201221310280533.html
C#代码直接调用WCF服务时的协议设置代码


private WSHttpBinding GetWSHttpBinding() 
        { 
            //wsHttpBinding 
            WSHttpBinding ws = new WSHttpBinding(SecurityMode.None); 
            ws.CloseTimeout = new TimeSpan(0, 1, 0); 
            ws.OpenTimeout = new TimeSpan(0, 1, 0); 
            ws.ReceiveTimeout = new TimeSpan(0, 20, 0); 
            ws.SendTimeout = new TimeSpan(0, 1, 0); 
            ws.BypassProxyOnLocal = false; 
            ws.TransactionFlow = false; 
            ws.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard; 
            ws.MaxBufferPoolSize = 524288; 
            ws.MaxReceivedMessageSize = 2147483647; 
            ws.MessageEncoding = WSMessageEncoding.Text; 
            ws.TextEncoding = Encoding.UTF8; 
            ws.UseDefaultWebProxy = true; 
            ws.AllowCookies = false; 
            ws.ReaderQuotas.MaxDepth = 6553600; 
            ws.ReaderQuotas.MaxStringContentLength = 2147483647; 
            ws.ReaderQuotas.MaxArrayLength = 6553600; 
            ws.ReaderQuotas.MaxBytesPerRead = 6553600; 
            ws.ReaderQuotas.MaxNameTableCharCount = 6553600; 
            ws.ReliableSession.Ordered = true; 
            ws.ReliableSession.InactivityTimeout = new TimeSpan(0, 20, 0); 
            ws.ReliableSession.Enabled = true; 
            ws.Security.Mode = SecurityMode.None; 
            ws.Security.Transport.ClientCredentialType = HttpClientCredentialType.None; 
            ws.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None; 
            ws.Security.Transport.Realm = ""; 
            ws.Security.Message.ClientCredentialType = MessageCredentialType.Windows; 
            ws.Security.Message.NegotiateServiceCredential = true; 
            ws.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default; 
            ws.Security.Message.EstablishSecurityContext = false; 


            return ws; 
        } 


**/