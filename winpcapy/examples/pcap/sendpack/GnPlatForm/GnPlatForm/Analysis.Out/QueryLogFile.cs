
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Collections;

namespace GnPlatForm.Analysis.Out
{
    public static class aaaaaa
    {
        public static string ToTraceString(this IQueryable t)
        {

            // try to cast to ObjectQuery<T>

            ObjectQuery oqt = t as ObjectQuery;

            if (oqt != null)

                return oqt.ToTraceString().ToString();

            return "";

        }
    }
    public class QueryLogFile
    {



        private QueryType _querytype;
        private string _linqQuery;
        private string _ESQL;
        private string _StoreSQL;

        public enum QueryType
        {
            LINQ = 1,
            ESQL = 2

        }


        public QueryLogFile()
        {
        }
        public void QueryLog(IQueryable linqQuery)
        {
            _querytype = QueryType.LINQ;
            _linqQuery = linqQuery.Expression.ToString();
            _StoreSQL = linqQuery.ToTraceString();
            CreateLogEntry();
        }

        public QueryLogFile(ObjectQuery objQuery)
        {
            _querytype = QueryType.ESQL;
            _ESQL = objQuery.CommandText;
            _StoreSQL = objQuery.ToTraceString();
            if (_ESQL != string.Empty) //if this was cast from LINQ, don\'t log  here
                CreateLogEntry();
        }


        private void CreateLogEntry()
        {
            //open xml log file

            XElement logs = LoadFile(); //XElement.Load(@\"logs\\LogFile.xml\");
            XElement newlog = new XElement("log", new XAttribute("Type", _querytype));
            if (_querytype == QueryType.LINQ)
                newlog.Add(new XElement("Expression", _linqQuery));
            else
                newlog.Add(new XElement("Expression", _ESQL));

            newlog.Add(new XElement("StoreSQL", _StoreSQL));
            logs.Add(newlog);
            logs.Save("LogFile.xml");

        }
        private XElement LoadFile()
        {
            if (System.IO.File.Exists("LogFile.xml"))
                return XElement.Load("LogFile.xml");
            else
            {
                var file = new XElement("QueryLog");
                return file;
            }
        }
    }
}
