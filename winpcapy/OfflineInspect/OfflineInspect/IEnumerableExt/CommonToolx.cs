using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.CommonTools
{
    public class CommonToolx
    {
 /*
 * http://eason132.iteye.com/blog/234821
 * 生成数字型唯一ID代码： 
 * 
 * **/
        public long GenerateId()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

    }
}
