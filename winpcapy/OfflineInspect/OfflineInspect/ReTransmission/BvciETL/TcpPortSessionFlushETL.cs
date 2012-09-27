using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OfflineInspect.ReTransmission.BvciETL
{
    public class TcpPortSessionFlushETLDocument
    {
        /*
         * 增加flush维表，和事实表（会话表）是多对一的关系。
         * 即1个TCP会话过程对应多个小区消息，驻留时间，传输包大小等。
         * 
         * 
         * 分裂其他粒度的问题，如何设计成通用的模块，以适应代码的最小改动。
         * 
         * */
    }
    public class TcpPortSessionFlushETL
    {
    }
}
