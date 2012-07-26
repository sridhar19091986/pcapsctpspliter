/*
JS
function do_Print() {
    control.setPage(this);
    control.scriptPrint(); 
}
function report_back(report){
    alert("Report:"+report);
}

C#
public void setPage(mshtml.HTMLWindow2Class JSFile) {
            window = JSFile;
}
public void scriptPrint(){
            window.execScript("report_back('Printing complete!')", "JScript");
}

It works perfectly and without ASP.NET!

thanks for the help anyway.



mongo --host="127.0.0.1:27017"   guangzhou_gb theScript.js  


mongo --host="127.0.0.1:27017"   guangzhou_gb G:\qtpcap\mongodb-mongo-r2.2.0-rc0-52-g2724a08\mongodb-mongo-2724a08\jstests\mr2.js  


mongo --host="127.0.0.1:27017"   guangzhou_gb G:\qtpcap\mongodb-mongo-r2.2.0-rc0-52-g2724a08\mongodb-mongo-2724a08\jstests\mr2.js  


                    lac_cell = ttt.Key,
                            //lac_cell = ttt.Key.lac_cell,
                            //bvci=ttt.Key.bvci,
                            fcb_cnt = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Count(),
                            packet_cnt = ttt.Count(),
                            tlli_cnt = ttt.Select(e => e.tlli).Distinct().Count(),

**/

/*
看来mapreduce

mongo --host="192.168.4.209:27017"   guangzhou_gb_bvc G:\pcapsctpspliter\winpcapy\examples\pcap\sendpack\GnPlatForm\MutliInterfaceGnGi\MongoMapReduce\js\JScript1.js 



bssgp_R_default_ms = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_R_default_ms);
bssgp_ms_bucket_size = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_ms_bucket_size),
//bssgp_bvc_bucket_size = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_bvc_bucket_size),
//bssgp_bucket_leak_rate = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_bucket_leak_rate),
bssgp_bucket_full_ratio = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_bucket_full_ratio),
bssgp_bmax_default_ms = ttt.Where(e => e.Flow_Control_MsgType == fc_msg).Average(e => e.bssgp_bmax_default_ms),

*/


t = db.FlowControlOneBvc;

ttt = db.FlowControlMapBvc;
ttt.drop();


//var msfc_msg = "BSSGP.FLOW-CONTROL-MS";
/*
/* 
{
  "_id" : 24,
  "Flow_Control_time" : ISODate("2012-07-22T17:01:03.87Z"),
  "lac_cell" : "10299-33785",
  "Flow_Control_MsgType" : "TCP.Ack,Push",
  "ip_len" : 328,
  "tlli" : "4288614433",
  "bssgp_direction" : "Down",
  "bssgp_bmax_default_ms" : 0.0,
  "bssgp_bucket_full_ratio" : null,
  "bssgp_bucket_leak_rate" : 0.0,
  "bssgp_bvc_bucket_size" : 0.0,
  "bssgp_ms_bucket_size" : 0.0,
  "bssgp_R_default_ms" : 0.0,
  "bvci" : "62"
}
**/



m = function () {

    emit(this.lac_cell, {
        fcb_cnt: 1,
        packet_cnt: 1,
        bvci: this.bvci,
        tlli: this.tlli,
        Flow_Control_MsgType: this.Flow_Control_MsgType.toString(),
        bssgp_R_default_ms: this.bssgp_R_default_ms.toString(),
        bssgp_ms_bucket_size: this.bssgp_ms_bucket_size,
        bssgp_bvc_bucket_size: this.bssgp_bvc_bucket_size,
        bssgp_bucket_leak_rate: this.bssgp_bucket_leak_rate,
        bssgp_bucket_full_ratio: this.bssgp_bucket_full_ratio,
        bssgp_bmax_default_ms: this.bssgp_bmax_default_ms,
        Flow_Control_time: this.Flow_Control_time.toString()
      

    });
}

r = function (key, values) {

    var fc_msg = "BSSGP.FLOW-CONTROL-BVC";

    var result = { fcb_cnt: 0, packet_cnt: 0, tlli: "", bvci: "", bssgp_R_default_ms: "", Flow_Control_time: "" };

    values.forEach(function (doc) {

        if (doc.Flow_Control_MsgType == fc_msg) {

            result.fcb_cnt += doc.fcb_cnt;

            result.bssgp_R_default_ms += doc.bssgp_R_default_ms + ",";

            result.Flow_Control_time += doc.Flow_Control_time + ",";

        }

        result.bvci += doc.bvci + ",";

        result.tlli_cnt += doc.tlli_cnt + ",";

        result.packet_cnt += doc.packet_cnt;

    });

    return result;
}

f = function finalizef(key, values) {

    values.average = 0;
    return values;

}


res = t.mapReduce(m, r, { out: "FlowControlMapBvc" });

//res = t.mapReduce(m, r, { finalize: f, out: "FlowControlMapBvc" });


printjson(res)




