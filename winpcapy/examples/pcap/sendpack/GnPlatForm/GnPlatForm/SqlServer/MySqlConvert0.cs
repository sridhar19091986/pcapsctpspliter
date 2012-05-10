/*
 * 
 * 正则表达式处理uri的问题？整数转换成ip地址
 * 
 * 注意正则表达式的编写方法
 * 
 * http://blog.csdn.net/java2000_net/article/details/2895897
 * 
 * http://www.williamlong.info/archives/433.html
 * 
 * 
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

namespace GnPlatForm.SqlServer
{
    public partial class MySqlConvert0
    {
        /*
         *  Console.WriteLine( Event_Type_MySql.getHostFromUri("wap.cmread.com:80"));
            Console.WriteLine(Event_Type_MySql.getHostFromUri("album30.z.qq.com.cn"));
            Console.WriteLine(Event_Type_MySql.getHostFromUri("album30.z.qq.com"));
            Console.WriteLine( Event_Type_MySql.getHostFromUri("wap.cmread.com:80"));
         * */

        //提取uri中的手机型号
        public static string getURI_MS_Type(string uri)
        {
            string host = null;
            if (uri == null || uri == "") return host;
            string pattern = @"[a-zA-Z0-9+.,-]+(__)";
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(uri).Groups[0].ToString();
            if (host == string.Empty || host == null) return host;
            return host.Replace("__", "");
        }

        //提取uri中的微博版本
        public static string getURI_Weibo_Ver(string uri)
        {
            string host = null;
            if (uri == null || uri == "") return host;
            string pattern = @"(__)[a-zA-Z0-9+.,-]+\d+";
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(uri).Groups[0].ToString();
            if (host == string.Empty || host == null) return host;
            return host.Replace("__", "");
        }

        //提取uri中的操作系统
        public static string getURI_OS(string uri)
        {
            string host = null;
            if (uri == null || uri == "") return host;
            string pattern = @"(__)[a-zA-Z0-9+.,-]+[a-zA-Z0-9+.,-]$";
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(uri).Groups[0].ToString();
            if (host == string.Empty || host == null) return host;
            return host.Replace("__", "");
        }

        //提取uri中的ua
        public static string getURI_UA(string uri)
        {
            string host = null;
            if (uri == null || uri == "") return host;
            //这个表达式很关键，需要开始匹配和结束匹配，中间的匹配也很重要
            string pattern = @"(ua|moblieType|phoneModel|mod)=[\w\-\,\+\.]+[&\s\w]";
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(uri).Groups[0].ToString();
            host = host
                .Replace("ua=", "")
                .Replace("mod", "")
                .Replace("moblieType", "")
                .Replace("phoneModel", "")
                .Replace("&", "").Trim();
            if (host == string.Empty || host == null) return host;
            //Console.WriteLine(host);
            return host;
        }

        //提取uri-main的内容
        public static string getUriMain(string uri)
        {
            string host = null;
            if (uri == null || uri == "") return host;
            ///w 匹配包括下划线的任何单词字符。等价于’[A-Za-z0-9_]’
            //string pattern = @"\w+\.(com.cn|com|net.cn|net|org.cn|org|gov.cn|gov|cn|mobi|me|info|name|biz|cc|tv|asia|hk|网络|公司|中国)";
            //* 匹配前面的子表达式零次或多次。* 等价于 {0,}。
            //+ 匹配前面的子表达式一次或多次。+ 等价于 {1,}。 
            //? 匹配前面的子表达式零次或一次。? 等价于 {0,1}。 
            string pattern = @"[^\.[*]{1,}\.(com.cn|com|net.cn|net|org.cn|org|gov.cn|gov|cn|mobi|me|info|name|biz|cc|tv|asia|hk|网络|公司|中国)";
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(uri).Groups[0].ToString();
            return host;
        }

        //提取uri-main的业务类型
        public static string getUriMainHeader(string uri)
        {
            string pattern = null;
            string host = null;
            Regex rg;
            if (uri == null || uri == "") return host;
            host = getUriMain(uri);
            if (host == null || host == string.Empty)
            {
                pattern = @"\d+\.\d+\.\d+\.\d+";//匹配
                rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                host = rg.Match(uri).Groups[0].ToString();
                if (host != null || host != string.Empty)
                {
                    return host;
                }
            }
            pattern = @"\w+\.";//匹配
            rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(uri).Groups[0].ToString().Trim('.');//提取第1个匹配的，去掉最后的1个.
            return host;
        }

        public static string getIpAddress(int longip)
        {
            string ipstr = new IPAddress(BitConverter.GetBytes((longip))).ToString();
            return ipstr;
        }

        //提取content—type的内容
        public static string getContentTypeHeader(string content)
        {
            string host = null;
            if (content == null || content == "") return host;
            string pattern = @"\w+[\/][\w+\-]+[\w]\b";
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(content).Groups[0].ToString();
            return host;
        }


        public static int getURI_Control_N(string uri)
        {
            int host = 0;
            if (uri == null || uri == "") return host;
            //string pattern = @"(\r|\n)*";
            //Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = uri.Length;
            return host;
        }

        //提取uri中的IMEI
        public static string getURI_IMEI(string uri)
        {
            string host = null;
            if (uri == null || uri == "") return host;
            string pattern = @"imei=\d{8}";//15位？
            Regex rg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            host = rg.Match(uri).Groups[0].ToString();
            host = host.Replace("imei=", "");
            if (host == string.Empty || host == null) return host;
            return host;
        }

        /*
         *
            匹配首尾空白字符的正则表达式：^\s*|\s*$
            评注：可以用来删除行首行尾的空白字符(包括空格、制表符、换页符等等)，非常有用的表达式

            匹配Email地址的正则表达式：\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*
            评注：表单验证时很实用

            匹配网址URL的正则表达式：[a-zA-z]+://[^\s]*
            评注：网上流传的版本功能很有限，上面这个基本可以满足需求

            匹配帐号是否合法(字母开头，允许5-16字节，允许字母数字下划线)：^[a-zA-Z][a-zA-Z0-9_]{4,15}$
            评注：表单验证时很实用

            匹配国内电话号码：\d{3}-\d{8}|\d{4}-\d{7}
            评注：匹配形式如 0511-4405222 或 021-87888822

            匹配腾讯QQ号：[1-9][0-9]{4,}
            评注：腾讯QQ号从10000开始

            匹配中国邮政编码：[1-9]\d{5}(?!\d)
            评注：中国邮政编码为6位数字

            匹配身份证：\d{15}|\d{18}
            评注：中国的身份证为15位或18位

            匹配ip地址：\d+\.\d+\.\d+\.\d+
            评注：提取ip地址时有用
         * 
         * 
         **/
    }
}

