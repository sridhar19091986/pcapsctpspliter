using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using OfflineInspect.ReTransmission.Table;

namespace OfflineInspect.ReTransmission
{
    class TcpDataContextSave
    {

        //private string sqlconn = null;
        public void saveTcpRetransStaticsDocumentSet(string sqlconn)
        {
            //sqlconn = db.Database.Connection.ConnectionString;
            Console.WriteLine(sqlconn);
            //using (TransactionScope scope = new TransactionScope())
            //{
                TcpDbContext context = null;
                try
                {
                    context = new TcpDbContext(sqlconn);
                    context.Configuration.AutoDetectChangesEnabled = false;

                    int count = 0;
                    TcpRetransStatics trs = new TcpRetransStatics();
                    foreach (var tcp in trs.mongo_TcpRetransStatics.QueryMongo())
                    {
                        ++count;
                        context = AddToContext(context, tcp, count, 100, true, sqlconn);
                    }

                    context.SaveChanges();
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

                //scope.Complete();
            //}

            Console.WriteLine("TcpRetransStatics->TcpDbContext->ok");
            //step = 0;
            //foreach (var tcp in db.getTcpRetransStaticsDocumentSet())
            //{
            //    step++;
            //    tcp.trsdID = tcp._id;
            //    db.Set<TcpRetransStaticsDocument>().Add(tcp);
            //    if (step == 5000)
            //    {
            //        db.SaveChanges();
            //        step = 0;
            //    }
            //}
            //db.SaveChanges();

        }
        private TcpDbContext AddToContext(TcpDbContext context,
          TcpRetransStaticsDocument entity, int count, int commitCount, bool recreateContext, string sqlconn)
        {
            context.Set<TcpRetransStaticsDocument>().Add(entity);

            if (count % commitCount == 0)
            {
                Console.WriteLine(count);
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new TcpDbContext(sqlconn);
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }
    }
}
