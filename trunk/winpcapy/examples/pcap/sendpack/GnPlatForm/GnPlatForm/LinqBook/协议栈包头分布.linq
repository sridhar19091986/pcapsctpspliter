<Query Kind="Statements">
  <Connection>
    <ID>92afe844-6bee-429a-8d93-c850e50afd51</ID>
    <Persist>true</Persist>
    <Server>localhost</Server>
    <Database>GuangZhou_Gn</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Reference>G:\htmlconvertsql\SqlCompact.v.2011.05.21\Soccer Score Forecast\Soccer Score Forecast\bin\Release\HtmlAgilityPack.dll</Reference>
  <Reference>D:\Program Files\Microsoft SQL Server\100\SDK\Assemblies\Microsoft.SqlServer.Types.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Data.DataSetExtensions.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.Data.dll</Reference>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

var gb=from p in Gb_IP_Fragments
       where p.Ip_flags_mf=="Not set"
	   where p.Ip2_flags_mf=="Not set"
	   where p.Tcp_need_segment==0
	   where p.Sndcp_m=="Not set"
//	   select p;
	   group p by new { p.Bssgp_direction, ip1_ip2_len=p.Ip_len-p.Ip2_len} into ttt
	   select new  
	   {
	   	 ttt.Key.Bssgp_direction,
	     ip1_ip2_len= ttt.Key.ip1_ip2_len,
		 cnt=ttt.Count(),
		 
	   };
	   
	   gb.OrderByDescending(e=>e.ip1_ip2_len).Dump();
	   
	   Gb_IP_Fragments.Where(e=>e.Ip_len-e.Ip2_len==65).Take(1).Dump();
	   
	   Gb_IP_Fragments.Where(e=>e.Ip_len-e.Ip2_len==107).Take(1).Dump();
	   