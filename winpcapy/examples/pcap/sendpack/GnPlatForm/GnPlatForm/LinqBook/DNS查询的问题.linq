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

var dns=from p in DNS_Flows
        where p.Query_Name.Trim().EndsWith(".10.in-addr.arpa")
        where p.Query_Type !="PTR"
		
		select p;
		
		dns.Dump();