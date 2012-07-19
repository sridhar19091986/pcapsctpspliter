using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiGnGi
{
    public static class IEnumerableExtensions
    {
        public static string GetHeader(this string source, string postion)
        {
            int pos = source.IndexOf(postion);
            return source.Substring(0, pos);
        }

        public static string AggregateSum<T>(this IEnumerable<T> source,
                                   Func<T, int> select_sum, Func<T, string> select_msg, string msg_flag)
        {
            double interTotal = 0;
            string aggregatesum = null;
            foreach (var item in source)
            {
                double nextItem = select_sum(item) / 1000.0;
                string nextMessge = select_msg(item);
                if (nextMessge != msg_flag)  //如果是不是fc消息，则ip.len++
                {
                    interTotal += nextItem;
                }
                if (nextMessge == msg_flag) //如果是fc消息，则，用逗号隔开
                {
                    aggregatesum += interTotal.ToString("f1") + ",";
                    interTotal = 0;
                }
            }
            //最后消息，需要再增加1次。
            //aggregatesum += interTotal.ToString();
            //return aggregatesum.Trim(','); //去掉首位的逗号
            return aggregatesum;
        }

        public static string AggregatePacketRate<T>(this IEnumerable<T> source,
            Func<T, int> select_sum,
                        Func<T, DateTime> select_time,
            Func<T, string> select_msg,

            string msg_flag)
        {
            double interTotal = 0;
            string aggregatesum = null;
            Stack<DateTime> sk = new Stack<DateTime>();
            Queue<DateTime> que = new Queue<DateTime>();
            DateTime sk_max = DateTime.Now.AddDays(-1000);
            DateTime sk_min;
            double msec = 1;  //初始值，为fc发送频率
            foreach (var item in source)
            {
                double nextItem = select_sum(item) / 1000.0;
                string nextMessge = select_msg(item);
                DateTime dt = select_time(item);
                if (nextMessge != msg_flag)  //如果是不是fc消息，则ip.len++
                {
                    interTotal += nextItem;
                    sk.Push(dt);
                    que.Enqueue(dt);
                }
                if (nextMessge == msg_flag) //如果是fc消息，则，用逗号隔开
                {
                    sk.Push(dt);
                    que.Enqueue(dt);
                    if (sk.Count > 0 && que.Count > 0)
                    {
                        sk_max = sk.Peek();
                        sk_min = que.Peek();
                        TimeSpan ts = sk_max - sk_min;
                        msec = ts.TotalMilliseconds / 1000.0;
                        if (sk_max == sk_min) msec = 1;
                    }

                    double pr = 8 * interTotal / msec;
                    aggregatesum += pr.ToString("f1") + ",";

                    interTotal = 0;
                    msec = 1;
                    que.Clear();
                    sk.Clear();
                    que.Enqueue(sk_max);

                }
            }
            //最后消息，需要再增加1次。
            //aggregatesum += interTotal.ToString();
            //return aggregatesum.Trim(','); //去掉首位的逗号
            return aggregatesum;
        }

        public static string AggregatePacketTime<T>(this IEnumerable<T> source,
                     Func<T, DateTime> select_time,
         Func<T, string> select_msg,

         string msg_flag)
        {
            string aggregatesum = null;
            Stack<DateTime> sk = new Stack<DateTime>();
            Queue<DateTime> que = new Queue<DateTime>();
            DateTime sk_max = DateTime.Now.AddDays(-1000);
            DateTime sk_min;
            double msec = 1;  //初始值，为fc发送频率
            foreach (var item in source)
            {
                string nextMessge = select_msg(item);
                DateTime dt = select_time(item);
                if (nextMessge != msg_flag)  //如果是不是fc消息，则ip.len++
                {
                    sk.Push(dt);
                    que.Enqueue(dt);
                }
                if (nextMessge == msg_flag) //如果是fc消息，则，用逗号隔开
                {
                    sk.Push(dt);
                    que.Enqueue(dt);
                    if (sk.Count > 0 && que.Count > 0)
                    {
                        sk_max = sk.Peek();
                        sk_min = que.Peek();
                        TimeSpan ts = sk_max - sk_min;
                        msec = ts.TotalMilliseconds / 1000.0;
                        if (sk_max == sk_min) msec = 1;
                    }

                    aggregatesum += msec.ToString("f1") + ",";

                    msec = 1;
                    que.Clear();
                    sk.Clear();
                    que.Enqueue(sk_max);
                }
            }
            //最后消息，需要再增加1次。
            //aggregatesum += interTotal.ToString();
            //return aggregatesum.Trim(','); //去掉首位的逗号
            return aggregatesum;
        }
    }
}
