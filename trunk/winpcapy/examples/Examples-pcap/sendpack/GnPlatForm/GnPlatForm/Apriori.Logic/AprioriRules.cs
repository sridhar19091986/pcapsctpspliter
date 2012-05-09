/*
 * 
 * http://en.wikipedia.org/wiki/Apriori_algorithm
 * 
 * http://www.codeproject.com/Articles/70371/Apriori-Algorithm
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using GnPlatForm.AnalysisOut;
using codeding.Apriori.DataStructures;
using System.IO;
using System.Text;


namespace GnPlatForm.BusinessLogic
{
    public class AprioriRules
    {
        public const double supportThreshold = 0;
        public const double confidenceThreshold = 0;

        public Itemset uniqueItems;
        public AprioriRules(AprioriSqlServer ass)
        {
            //事务集合
            ItemsetCollection db = new ItemsetCollection();
            foreach (var m in ass.m_dicTransactions)
            {

                Itemset iset = new Itemset();
                iset.AddRange(m.Distinct());
                db.Add(iset);

            }

            //最大集合
            //Console.WriteLine();
            WriteToFile(string.Format("uniqueItemsCount:{0}", db.Count()));
            uniqueItems= db.GetUniqueItems();

            //Console.WriteLine();
            WriteToFile(string.Format("uniqueItems:{0}", uniqueItems));

            //test apriori
            ItemsetCollection L = AprioriMining.DoApriori(db, supportThreshold);
            //Console.WriteLine();
            WriteToFile(string.Format("DoApriori:{0}", L.Count));
            foreach (Itemset i in L.OrderByDescending(e => e.Support))
            {
                //Console.WriteLine();
                WriteToFile(string.Format("L:{0}", i));
            }

            //test mining
            List<AssociationRule> allRules = AprioriMining.Mine(db, L, confidenceThreshold);
            //Console.WriteLine();
            WriteToFile(string.Format("Mine:{0}", allRules.Count));
            foreach (AssociationRule rule in allRules.OrderByDescending(e => e.Support))
            {
                //Console.WriteLine( rule);
                WriteToFile(string.Format("rule:{0}", rule));
            }
            //Console.WriteLine("ok");
        }
        public static void StartLog()  //启动时删除旧文件
        {
            if (File.Exists(@"c:\apriori.txt")) File.Delete(@"c:\apriori.txt");
        }
        private  void WriteToFile(string strs)  //这里增加些文件的方法
        {
            using (var writer = new StreamWriter(@"c:\apriori.txt", true, Encoding.UTF8))
            {
                writer.WriteLine(strs);
                Console.WriteLine(strs);
            }
        }
    }
}


/*
 *             //example for binary-to-decimal
            for (int i = 0; i < 40; i++)
            {
                string binary = Bit.DecimalToBinary(i, 8);
                Console.WriteLine(i + ": " + binary + "," + Bit.GetOnCount(i, 8) + "<br/>");
            }

            //example for getbit
            int bit = Bit.GetBit(5, 40);
            Console.WriteLine("," + bit + "<br/>");

            //example for subsets
            Itemset i1 = new Itemset() { "1", "2", "3", "4", "5" };
            Itemset i2 = new Itemset() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            ItemsetCollection subsets = Bit.FindSubsets(i1, 0);

            Console.WriteLine(subsets.Count + " subsets<br/>");
            foreach (Itemset subset in subsets)
            {
                Console.WriteLine(subset.ToString() + "<br/>");
            }
 * */
