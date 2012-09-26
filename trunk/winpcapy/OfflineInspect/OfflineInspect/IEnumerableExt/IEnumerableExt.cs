/*
 * 
 * 用户TCP会话的BVCI分裂
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.CommonTools
{
    public static class IEnumerableExt
    {
        //记录集合的索引位置进行，集合分裂
        public static List<int> IEnumBvciSequenceCollectionIndex<T>(this IEnumerable<T> source)
        {
            //source没有记录则返回空值
            if (!source.Any()) return null;
            if (!source.Where(e => e != null).Any()) return null;
            //先去重复的再排序最后用逗号分割
            string douhao = null;
            string temp = null;
            int pos = -1;
            List<int> hs = new List<int>();
            foreach (var m in source)
            {
                pos++;
                douhao = m.ToString();
                if (temp != douhao)
                {
                    hs.Add(pos);//返回位置，每个序列的开始位置，hs的大小为分裂的集合数量
                    temp = douhao;
                }
            }
            hs.Add(pos);//记录第1个位置和最后一个位置进行区间分割
            return hs;
        }

        //按照索引位置执行集合分裂
        public static Dictionary<int, List<T>> IEnumBvciSequenceCollection<T>(this IEnumerable<T> source, int[] ArrayIndex)
        {
            Dictionary<int, List<T>> lt = new Dictionary<int, List<T>>();
            int pos = -1;
            List<T> seq = new List<T>();
            foreach (var m in source)
            {
                pos++;
                seq.Add(m);
                for (int i = 1; i < ArrayIndex.Length; i++) //第2个元素开始搜素
                {
                    if (pos == ArrayIndex[i])//到达索引位置则开始分裂
                    {
                        lt.Add(i, seq);
                        seq = new List<T>();
                    }
                }
            }
            return lt;
        }
    }
}
