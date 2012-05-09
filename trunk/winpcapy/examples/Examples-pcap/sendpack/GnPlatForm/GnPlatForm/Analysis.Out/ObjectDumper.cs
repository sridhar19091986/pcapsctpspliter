// Copyright © Microsoft Corporation.  All Rights Reserved. 
// This code released under the terms of the  
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) 
// 
//Copyright (C) Microsoft Corporation.  All rights reserved. 


/*
 * 
 * 增加写入文件的功能和在平台显示的功能，2012.4.10
 * 
 * 用逗号代替tab， 2012.4.11
 * 
 * 完成指标计算的显示和保存
 * 
 * */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GnPlatForm.BusinessLogic
{
    public class MicroObjectDumper
    {
        public static void StartLog()  //启动时删除旧文件
        {
            if (File.Exists(@"c:\abc.txt")) File.Delete(@"c:\abc.txt");
        }
        private  static void WriteToFile(string strs)  //这里增加些文件的方法
        {
            using (var writer = new StreamWriter(@"c:\abc.txt",true ,Encoding.UTF8))
            {
                writer.Write(strs);
            }
        }

        public static void Write(object o)
        {
            Write(o, 0);
        }

        public static void Write(object o, int depth)
        {
            MicroObjectDumper dumper = new MicroObjectDumper(depth);
            dumper.WriteObject(null, o);
        }

        TextWriter writer;
        int pos;
        int level;
        int depth;

        private MicroObjectDumper(int depth)
        {
            this.writer = Console.Out;
            this.depth = depth;
        }

        private void Write(string s)
        {
            if (s != null)
            {
                writer.Write(s); WriteToFile(s);//这里增加写入文件
                pos += s.Length; 
            }
        }

        private void WriteIndent()
        {
            //for (int i = 0; i < level; i++) writer.Write("  ");
        }

        private void WriteLine()
        {
            writer.WriteLine(); WriteToFile("\r\n");//这里增加写换行
            pos = 0;
     
        }

        private void WriteTab()
        {
            Write(",");
            //while (pos % 8 != 0) Write(" ");
        }

        private void WriteObject(string prefix, object o)
        {

            if (o == null || o is ValueType || o is string)
            {
                WriteIndent();
                Write(prefix);
                WriteValue(o);
                WriteLine();
            }
            else if (o is IEnumerable)
            {
                foreach (object element in (IEnumerable)o)
                {
                    if (element is IEnumerable && !(element is string))
                    {
                        WriteIndent();
                        Write(prefix);
                        Write("...");
                        WriteLine();
                        if (level < depth)
                        {
                            level++;
                            WriteObject(prefix, element);
                            level--;
                        }
                    }
                    else
                    {
                        WriteObject(prefix, element);
                    }
                }
            }
            else
            {
                MemberInfo[] members = o.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                WriteIndent();
                Write(prefix);
                bool propWritten = false;
                foreach (MemberInfo m in members)
                {
                    FieldInfo f = m as FieldInfo;
                    PropertyInfo p = m as PropertyInfo;
                    if (f != null || p != null)
                    {
                        if (propWritten)
                        {
                            WriteTab();
                        }
                        else
                        {
                            propWritten = true;
                        }

                        Write(m.Name);
                        Write("=");

                        Type t = f != null ? f.FieldType : p.PropertyType;
                        if (t.IsValueType || t == typeof(string))
                        {
                            WriteValue(f != null ? f.GetValue(o) : p.GetValue(o, null));
                        }
                        else
                        {
                            if (typeof(IEnumerable).IsAssignableFrom(t))
                            {
                                Write("...");
                            }
                            else
                            {
                                Write("{ }");
                            }
                        }
                    }
                }
                if (propWritten) WriteLine();
                if (level < depth)
                {
                    foreach (MemberInfo m in members)
                    {
                        FieldInfo f = m as FieldInfo;
                        PropertyInfo p = m as PropertyInfo;
                        if (f != null || p != null)
                        {
                            Type t = f != null ? f.FieldType : p.PropertyType;
                            if (!(t.IsValueType || t == typeof(string)))
                            {
                                object value = f != null ? f.GetValue(o) : p.GetValue(o, null);
                                if (value != null)
                                {
                                    level++;
                                    WriteObject(m.Name + ": ", value);
                                    level--;
                                }
                            }
                        }
                    }
                }
            }
     
        }

        private void WriteValue(object o)
        {
            if (o == null)
            {
                Write("null");
            }
            else if (o is DateTime)
            {
                Write(((DateTime)o).ToShortDateString());
            }
            else if (o is ValueType || o is string)
            {
                Write(o.ToString());
            }
            else if (o is IEnumerable)
            {
                Write("...");
            }
            else
            {
                Write("{ }");
            }
        }
    }
}