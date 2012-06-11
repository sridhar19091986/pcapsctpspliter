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

var total=Gb_XIDs.Count();
var xid=from p in Gb_XIDs

        group p by new {p.Bssgp_direction,p.Llcgprs_xid1type} into ttt
		
		select new 
		
		{
		ttt.Key.Bssgp_direction,
		ttt.Key.Llcgprs_xid1type,
		
		cnt=ttt.Count(),
		percent=1.0*ttt.Count()/total,
		
		};
		
		xid.OrderByDescending(e=>e.cnt).Dump();
		
			Gb_XIDs
			.Where(e=>e.Llcgprs_xid1type==11)
			.Take(10).Dump();
			
			
		Gb_XIDs
		.Where(e=>e.Llcgprs_xid1type==5)
		.Where(e=>e.Bssgp_direction=="Down")
		.Where(e=>e._1llcgprs_xid1type==5)
		.Where(e=>e._1llcgprs_xid1byte2+e._1llcgprs_xid1byte1*256==1520)
		.Where(e=>e.Llcgprs_xid1byte2+e.Llcgprs_xid1byte1*256==1520)
		.Take(10).Dump();