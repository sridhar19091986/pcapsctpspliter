echo 按照格式解码和过滤-过滤m3ua协议
cd   D:\Program Files\Wireshark\
tshark -V   -R "m3ua"  -r  D:\CMCC_SZ\merge_cap.pcap > D:\CMCC_SZ\merge_cap

m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP


tshark -V -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r D:\merge_cap.pcap > D:\merge_cap

tshark -R "m3ua.protocol_data_si==SCCP || m3ua.protocol_data_si==ISUP" -r mm.pcap -w mmm.pcap

tshark -V -r ezsniff.pcap -n frame.num=1970 -d tcp.port==14000,oicq


echo 按照协议过滤的批处理-过滤m3ua协议
cd   D:\Program Files\Wireshark\
tshark -R "m3ua"  -r D:\DX188\2010-01-15-mgw-iu-cs0.pcap -w D:\DX188\0pcap
echo C#-----------------文件合并
cd   D:\Program Files\Wireshark\
mergecap -v -T ether -w  D:\CMCC_SZ\merge_cap.pcap   D:\CMCC_SZ\iu-ho-2_00003_20100118113532  D:\CMCC_SZ\3_00001_20100118114117 d:\cmcc_sz\3_00002_20100118114710

tshark -r 1.pcap -R "gsm_a.bssmap_msgtype==Paging" -E occurrence=a -E separator=, > aa.csv

tshark -r 1.pcap -R "sctp.data_payload_proto_id==m3ua" -E occurrence=a -E separator=, > aa.csv

tshark  -T pdml -r "j:\1.pcap"  | perl -ane ' <at> flist=qw(m3ua.protocol_data_opc m3ua.protocol_data_dpc h248.transactionId);\
foreach $f ( <at> flist) {\
 if(/field name=\"$f\".*show=\"(.*?)\".*/){print "$f:$1,";}}'

Tshark -r f:\t.pcap -X lua_script:trace_stats.lua -q

tshark -r f:\t.pcap  -t ad -V -n frame.number==11 


tshark -r f:\t.pcap  -t ad -V -n frame.number==11 -T pdml > test.xml
tshark -r f:\t.pcap  -x -n frame.number==11
tshark -r f:\t.pcap -T fields -e frame.number -e ip.src -e tcp.srcport -e ip.ttl -e ip.id
tshark -r f:\t.pcap  -q -z http,stat, -z http,tree 
tshark -r f:\t.pcap -q -z io,stat,10
-X <eXtension option>： eXtension 选项使用extension_key:值形式，extension_key:可以是：lua_script:lua_script_filename,它告诉Wireshark载入指定的脚本，默认脚本是Lua scripts。
-r <infile>：指定要读取显示的文件名。
-n： 禁止所有地址名字解析（默认为允许所有） 
-N： 启用某一层的地址名字解析。
-d <layer_type>==<selector>,<decode_as_protocol> ：以某种协议解码，比如：tcp.port==888


