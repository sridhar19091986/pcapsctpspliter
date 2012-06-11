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
  <Reference>G:\htmlconvertsql\SqlCompact.v.2011.05.21\Soccer Score Forecast\Soccer Score Forecast\bin\Release\HtmlAgilityPack.dll</Reference>
  <Reference>D:\Program Files\Microsoft SQL Server\100\SDK\Assemblies\Microsoft.SqlServer.Types.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Data.DataSetExtensions.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.Data.dll</Reference>
  <Reference>G:\htmlconvertsql\SqlCompact.v.2011.05.21\Soccer Score Forecast\Soccer Score Forecast\bin\Release\HtmlAgilityPack.dll</Reference>
  <Reference>D:\Program Files\Microsoft SQL Server\100\SDK\Assemblies\Microsoft.SqlServer.Types.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Data.DataSetExtensions.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.Data.dll</Reference>
  <Reference>G:\htmlconvertsql\SqlCompact.v.2011.05.21\Soccer Score Forecast\Soccer Score Forecast\bin\Release\HtmlAgilityPack.dll</Reference>
  <Reference>D:\Program Files\Microsoft SQL Server\100\SDK\Assemblies\Microsoft.SqlServer.Types.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\System.Data.DataSetExtensions.dll</Reference>
  <Reference>&lt;ProgramFiles&gt;\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\Profile\Client\System.Data.dll</Reference>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

var total=Gb_XIDs.Where(e=>e.Bssgp_direction=="Down")
.Where(e=>e.Llcgprs_xid1type==5)
.Where(e=>e._1llcgprs_xid1type==5)
.Where(e=>e.Llcgprs_xid1byte1 *256+ e.Llcgprs_xid1byte2 !=1504)
.Count();
var xid=from p in Gb_XIDs
        where p.Bssgp_direction=="Down"
        where p.Llcgprs_xid1type==5 && p._1llcgprs_xid1type==5
		where p.Llcgprs_xid1byte1 *256+ p.Llcgprs_xid1byte2 !=1504
        group p by new {p.Bssgp_direction,
		N201_U_Ne=p.Llcgprs_xid1byte1 *256+ p.Llcgprs_xid1byte2,
		N201_U_Result=p._1llcgprs_xid1byte1*256+ p._1llcgprs_xid1byte2
		} into ttt
		select new 
		{
		ttt.Key.Bssgp_direction,
		ttt.Key.N201_U_Ne,
		ttt.Key.N201_U_Result,
		cnt=ttt.Count(),
		percent=1.0*ttt.Count()/total,	
		};
		
		xid.OrderByDescending(e=>e.N201_U_Result).ThenByDescending(e=>e.cnt).Dump();
		
		Gb_XIDs
		//.Where(e=>e.Bssgp_direction=="Down")
.Where(e=>e.Llcgprs_xid1type==5)
.Where(e=>e._1llcgprs_xid1type==5)
.Where(e=>e.Llcgprs_xid1byte1 *256+ e.Llcgprs_xid1byte2 ==760)
.Where(e=>e._1llcgprs_xid1byte1*256+ e._1llcgprs_xid1byte2==1520)
.Dump();
		
		