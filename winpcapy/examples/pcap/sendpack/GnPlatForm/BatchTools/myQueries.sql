go

 go
alter TABLE [Gb_XID]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_XID]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_XID] add  PRIMARY KEY (PacketNum,FileNum);

 go
alter TABLE [Gb_GMMSM_XID]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_GMMSM_XID]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_GMMSM_XID] add  PRIMARY KEY (PacketNum,FileNum);


--alter table [mpos_gb_gz]  ADD PRIMARY KEY([Session_ID])
go
alter TABLE [Gb_auth_imeisv]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_auth_imeisv]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_auth_imeisv] add  PRIMARY KEY (PacketNum,FileNum);

 go
alter TABLE [Gb_PDP_XID]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_PDP_XID]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_PDP_XID] add  PRIMARY KEY (PacketNum,FileNum);

go
alter TABLE [Gb_Cell_Repeat]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_Cell_Repeat]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_Cell_Repeat] add  PRIMARY KEY (PacketNum,FileNum);



go
alter TABLE [Gb_DNS_Syn]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_DNS_Syn]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_DNS_Syn] add  PRIMARY KEY (PacketNum,FileNum);


go
alter TABLE [Gb_PDP_FIN]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_PDP_FIN]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_PDP_FIN] add  PRIMARY KEY (PacketNum,FileNum);

go
alter TABLE [Gb_Cell_Syn]   alter  COLUMN [PacketNum] int  not null
go
alter TABLE [Gb_Cell_Syn]  alter  COLUMN [FileNum] int  not null
go
alter table [Gb_Cell_Syn] add  PRIMARY KEY (PacketNum,FileNum);

--