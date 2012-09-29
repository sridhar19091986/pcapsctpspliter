﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Data.EntityClient;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;

[assembly: EdmSchemaAttribute()]

namespace EntitySqlTable.SqlServer.ip249.llc_data
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class foshan_llc_dataEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new foshan_llc_dataEntities object using the connection string found in the 'foshan_llc_dataEntities' section of the application configuration file.
        /// </summary>
        public foshan_llc_dataEntities() : base("name=foshan_llc_dataEntities", "foshan_llc_dataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new foshan_llc_dataEntities object.
        /// </summary>
        public foshan_llc_dataEntities(string connectionString) : base(connectionString, "foshan_llc_dataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new foshan_llc_dataEntities object.
        /// </summary>
        public foshan_llc_dataEntities(EntityConnection connection) : base(connection, "foshan_llc_dataEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        #endregion
    
        #region Partial Methods
    
        partial void OnContextCreated();
    
        #endregion
    
        #region ObjectSet Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        public ObjectSet<Gb_LLC_ReTransmission> Gb_LLC_ReTransmission
        {
            get
            {
                if ((_Gb_LLC_ReTransmission == null))
                {
                    _Gb_LLC_ReTransmission = base.CreateObjectSet<Gb_LLC_ReTransmission>("Gb_LLC_ReTransmission");
                }
                return _Gb_LLC_ReTransmission;
            }
        }
        private ObjectSet<Gb_LLC_ReTransmission> _Gb_LLC_ReTransmission;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the Gb_LLC_ReTransmission EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddToGb_LLC_ReTransmission(Gb_LLC_ReTransmission gb_LLC_ReTransmission)
        {
            base.AddObject("Gb_LLC_ReTransmission", gb_LLC_ReTransmission);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="foshan_llc_dataModel", Name="Gb_LLC_ReTransmission")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class Gb_LLC_ReTransmission : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new Gb_LLC_ReTransmission object.
        /// </summary>
        /// <param name="fileNum">Initial value of the FileNum property.</param>
        /// <param name="packetNum">Initial value of the PacketNum property.</param>
        public static Gb_LLC_ReTransmission CreateGb_LLC_ReTransmission(global::System.Int32 fileNum, global::System.Int32 packetNum)
        {
            Gb_LLC_ReTransmission gb_LLC_ReTransmission = new Gb_LLC_ReTransmission();
            gb_LLC_ReTransmission.FileNum = fileNum;
            gb_LLC_ReTransmission.PacketNum = packetNum;
            return gb_LLC_ReTransmission;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 FileNum
        {
            get
            {
                return _FileNum;
            }
            set
            {
                if (_FileNum != value)
                {
                    OnFileNumChanging(value);
                    ReportPropertyChanging("FileNum");
                    _FileNum = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("FileNum");
                    OnFileNumChanged();
                }
            }
        }
        private global::System.Int32 _FileNum;
        partial void OnFileNumChanging(global::System.Int32 value);
        partial void OnFileNumChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 PacketNum
        {
            get
            {
                return _PacketNum;
            }
            set
            {
                if (_PacketNum != value)
                {
                    OnPacketNumChanging(value);
                    ReportPropertyChanging("PacketNum");
                    _PacketNum = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("PacketNum");
                    OnPacketNumChanged();
                }
            }
        }
        private global::System.Int32 _PacketNum;
        partial void OnPacketNumChanging(global::System.Int32 value);
        partial void OnPacketNumChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> BeginFileNum
        {
            get
            {
                return _BeginFileNum;
            }
            set
            {
                OnBeginFileNumChanging(value);
                ReportPropertyChanging("BeginFileNum");
                _BeginFileNum = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("BeginFileNum");
                OnBeginFileNumChanged();
            }
        }
        private Nullable<global::System.Int32> _BeginFileNum;
        partial void OnBeginFileNumChanging(Nullable<global::System.Int32> value);
        partial void OnBeginFileNumChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> BeginFrameNum
        {
            get
            {
                return _BeginFrameNum;
            }
            set
            {
                OnBeginFrameNumChanging(value);
                ReportPropertyChanging("BeginFrameNum");
                _BeginFrameNum = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("BeginFrameNum");
                OnBeginFrameNumChanged();
            }
        }
        private Nullable<global::System.Int32> _BeginFrameNum;
        partial void OnBeginFrameNumChanging(Nullable<global::System.Int32> value);
        partial void OnBeginFrameNumChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.DateTime> PacketTime
        {
            get
            {
                return _PacketTime;
            }
            set
            {
                OnPacketTimeChanging(value);
                ReportPropertyChanging("PacketTime");
                _PacketTime = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("PacketTime");
                OnPacketTimeChanged();
            }
        }
        private Nullable<global::System.DateTime> _PacketTime;
        partial void OnPacketTimeChanging(Nullable<global::System.DateTime> value);
        partial void OnPacketTimeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> PacketTime_ms_
        {
            get
            {
                return _PacketTime_ms_;
            }
            set
            {
                OnPacketTime_ms_Changing(value);
                ReportPropertyChanging("PacketTime_ms_");
                _PacketTime_ms_ = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("PacketTime_ms_");
                OnPacketTime_ms_Changed();
            }
        }
        private Nullable<global::System.Int32> _PacketTime_ms_;
        partial void OnPacketTime_ms_Changing(Nullable<global::System.Int32> value);
        partial void OnPacketTime_ms_Changed();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String DumpFor
        {
            get
            {
                return _DumpFor;
            }
            set
            {
                OnDumpForChanging(value);
                ReportPropertyChanging("DumpFor");
                _DumpFor = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("DumpFor");
                OnDumpForChanged();
            }
        }
        private global::System.String _DumpFor;
        partial void OnDumpForChanging(global::System.String value);
        partial void OnDumpForChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> LLC
        {
            get
            {
                return _LLC;
            }
            set
            {
                OnLLCChanging(value);
                ReportPropertyChanging("LLC");
                _LLC = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("LLC");
                OnLLCChanged();
            }
        }
        private Nullable<global::System.Int32> _LLC;
        partial void OnLLCChanging(Nullable<global::System.Int32> value);
        partial void OnLLCChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String LLC_time
        {
            get
            {
                return _LLC_time;
            }
            set
            {
                OnLLC_timeChanging(value);
                ReportPropertyChanging("LLC_time");
                _LLC_time = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("LLC_time");
                OnLLC_timeChanged();
            }
        }
        private global::System.String _LLC_time;
        partial void OnLLC_timeChanging(global::System.String value);
        partial void OnLLC_timeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String LLC_MsgType
        {
            get
            {
                return _LLC_MsgType;
            }
            set
            {
                OnLLC_MsgTypeChanging(value);
                ReportPropertyChanging("LLC_MsgType");
                _LLC_MsgType = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("LLC_MsgType");
                OnLLC_MsgTypeChanged();
            }
        }
        private global::System.String _LLC_MsgType;
        partial void OnLLC_MsgTypeChanging(global::System.String value);
        partial void OnLLC_MsgTypeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> bssgp_ci
        {
            get
            {
                return _bssgp_ci;
            }
            set
            {
                Onbssgp_ciChanging(value);
                ReportPropertyChanging("bssgp_ci");
                _bssgp_ci = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("bssgp_ci");
                Onbssgp_ciChanged();
            }
        }
        private Nullable<global::System.Int32> _bssgp_ci;
        partial void Onbssgp_ciChanging(Nullable<global::System.Int32> value);
        partial void Onbssgp_ciChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String bssgp_direction
        {
            get
            {
                return _bssgp_direction;
            }
            set
            {
                Onbssgp_directionChanging(value);
                ReportPropertyChanging("bssgp_direction");
                _bssgp_direction = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("bssgp_direction");
                Onbssgp_directionChanged();
            }
        }
        private global::System.String _bssgp_direction;
        partial void Onbssgp_directionChanging(global::System.String value);
        partial void Onbssgp_directionChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String bssgp_imsi
        {
            get
            {
                return _bssgp_imsi;
            }
            set
            {
                Onbssgp_imsiChanging(value);
                ReportPropertyChanging("bssgp_imsi");
                _bssgp_imsi = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("bssgp_imsi");
                Onbssgp_imsiChanged();
            }
        }
        private global::System.String _bssgp_imsi;
        partial void Onbssgp_imsiChanging(global::System.String value);
        partial void Onbssgp_imsiChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> bssgp_lac
        {
            get
            {
                return _bssgp_lac;
            }
            set
            {
                Onbssgp_lacChanging(value);
                ReportPropertyChanging("bssgp_lac");
                _bssgp_lac = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("bssgp_lac");
                Onbssgp_lacChanged();
            }
        }
        private Nullable<global::System.Int32> _bssgp_lac;
        partial void Onbssgp_lacChanging(Nullable<global::System.Int32> value);
        partial void Onbssgp_lacChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String bssgp_tlli
        {
            get
            {
                return _bssgp_tlli;
            }
            set
            {
                Onbssgp_tlliChanging(value);
                ReportPropertyChanging("bssgp_tlli");
                _bssgp_tlli = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("bssgp_tlli");
                Onbssgp_tlliChanged();
            }
        }
        private global::System.String _bssgp_tlli;
        partial void Onbssgp_tlliChanging(global::System.String value);
        partial void Onbssgp_tlliChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip_dst_host
        {
            get
            {
                return _ip_dst_host;
            }
            set
            {
                Onip_dst_hostChanging(value);
                ReportPropertyChanging("ip_dst_host");
                _ip_dst_host = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip_dst_host");
                Onip_dst_hostChanged();
            }
        }
        private global::System.String _ip_dst_host;
        partial void Onip_dst_hostChanging(global::System.String value);
        partial void Onip_dst_hostChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip_flags_mf
        {
            get
            {
                return _ip_flags_mf;
            }
            set
            {
                Onip_flags_mfChanging(value);
                ReportPropertyChanging("ip_flags_mf");
                _ip_flags_mf = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip_flags_mf");
                Onip_flags_mfChanged();
            }
        }
        private global::System.String _ip_flags_mf;
        partial void Onip_flags_mfChanging(global::System.String value);
        partial void Onip_flags_mfChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ip_len
        {
            get
            {
                return _ip_len;
            }
            set
            {
                Onip_lenChanging(value);
                ReportPropertyChanging("ip_len");
                _ip_len = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ip_len");
                Onip_lenChanged();
            }
        }
        private Nullable<global::System.Int32> _ip_len;
        partial void Onip_lenChanging(Nullable<global::System.Int32> value);
        partial void Onip_lenChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip_src_host
        {
            get
            {
                return _ip_src_host;
            }
            set
            {
                Onip_src_hostChanging(value);
                ReportPropertyChanging("ip_src_host");
                _ip_src_host = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip_src_host");
                Onip_src_hostChanged();
            }
        }
        private global::System.String _ip_src_host;
        partial void Onip_src_hostChanging(global::System.String value);
        partial void Onip_src_hostChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip_ttl
        {
            get
            {
                return _ip_ttl;
            }
            set
            {
                Onip_ttlChanging(value);
                ReportPropertyChanging("ip_ttl");
                _ip_ttl = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip_ttl");
                Onip_ttlChanged();
            }
        }
        private global::System.String _ip_ttl;
        partial void Onip_ttlChanging(global::System.String value);
        partial void Onip_ttlChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip2_dst_host
        {
            get
            {
                return _ip2_dst_host;
            }
            set
            {
                Onip2_dst_hostChanging(value);
                ReportPropertyChanging("ip2_dst_host");
                _ip2_dst_host = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip2_dst_host");
                Onip2_dst_hostChanged();
            }
        }
        private global::System.String _ip2_dst_host;
        partial void Onip2_dst_hostChanging(global::System.String value);
        partial void Onip2_dst_hostChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip2_flags_mf
        {
            get
            {
                return _ip2_flags_mf;
            }
            set
            {
                Onip2_flags_mfChanging(value);
                ReportPropertyChanging("ip2_flags_mf");
                _ip2_flags_mf = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip2_flags_mf");
                Onip2_flags_mfChanged();
            }
        }
        private global::System.String _ip2_flags_mf;
        partial void Onip2_flags_mfChanging(global::System.String value);
        partial void Onip2_flags_mfChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ip2_len
        {
            get
            {
                return _ip2_len;
            }
            set
            {
                Onip2_lenChanging(value);
                ReportPropertyChanging("ip2_len");
                _ip2_len = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ip2_len");
                Onip2_lenChanged();
            }
        }
        private Nullable<global::System.Int32> _ip2_len;
        partial void Onip2_lenChanging(Nullable<global::System.Int32> value);
        partial void Onip2_lenChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip2_src_host
        {
            get
            {
                return _ip2_src_host;
            }
            set
            {
                Onip2_src_hostChanging(value);
                ReportPropertyChanging("ip2_src_host");
                _ip2_src_host = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip2_src_host");
                Onip2_src_hostChanged();
            }
        }
        private global::System.String _ip2_src_host;
        partial void Onip2_src_hostChanging(global::System.String value);
        partial void Onip2_src_hostChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String ip2_ttl
        {
            get
            {
                return _ip2_ttl;
            }
            set
            {
                Onip2_ttlChanging(value);
                ReportPropertyChanging("ip2_ttl");
                _ip2_ttl = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("ip2_ttl");
                Onip2_ttlChanged();
            }
        }
        private global::System.String _ip2_ttl;
        partial void Onip2_ttlChanging(global::System.String value);
        partial void Onip2_ttlChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> llcgprs_nu
        {
            get
            {
                return _llcgprs_nu;
            }
            set
            {
                Onllcgprs_nuChanging(value);
                ReportPropertyChanging("llcgprs_nu");
                _llcgprs_nu = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("llcgprs_nu");
                Onllcgprs_nuChanged();
            }
        }
        private Nullable<global::System.Int32> _llcgprs_nu;
        partial void Onllcgprs_nuChanging(Nullable<global::System.Int32> value);
        partial void Onllcgprs_nuChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> nsip_bvci
        {
            get
            {
                return _nsip_bvci;
            }
            set
            {
                Onnsip_bvciChanging(value);
                ReportPropertyChanging("nsip_bvci");
                _nsip_bvci = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("nsip_bvci");
                Onnsip_bvciChanged();
            }
        }
        private Nullable<global::System.Int32> _nsip_bvci;
        partial void Onnsip_bvciChanging(Nullable<global::System.Int32> value);
        partial void Onnsip_bvciChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String sndcp_m
        {
            get
            {
                return _sndcp_m;
            }
            set
            {
                Onsndcp_mChanging(value);
                ReportPropertyChanging("sndcp_m");
                _sndcp_m = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("sndcp_m");
                Onsndcp_mChanged();
            }
        }
        private global::System.String _sndcp_m;
        partial void Onsndcp_mChanging(global::System.String value);
        partial void Onsndcp_mChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String tcp_ack
        {
            get
            {
                return _tcp_ack;
            }
            set
            {
                Ontcp_ackChanging(value);
                ReportPropertyChanging("tcp_ack");
                _tcp_ack = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("tcp_ack");
                Ontcp_ackChanged();
            }
        }
        private global::System.String _tcp_ack;
        partial void Ontcp_ackChanging(global::System.String value);
        partial void Ontcp_ackChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> tcp_dstport
        {
            get
            {
                return _tcp_dstport;
            }
            set
            {
                Ontcp_dstportChanging(value);
                ReportPropertyChanging("tcp_dstport");
                _tcp_dstport = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("tcp_dstport");
                Ontcp_dstportChanged();
            }
        }
        private Nullable<global::System.Int32> _tcp_dstport;
        partial void Ontcp_dstportChanging(Nullable<global::System.Int32> value);
        partial void Ontcp_dstportChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String tcp_flags_cwr
        {
            get
            {
                return _tcp_flags_cwr;
            }
            set
            {
                Ontcp_flags_cwrChanging(value);
                ReportPropertyChanging("tcp_flags_cwr");
                _tcp_flags_cwr = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("tcp_flags_cwr");
                Ontcp_flags_cwrChanged();
            }
        }
        private global::System.String _tcp_flags_cwr;
        partial void Ontcp_flags_cwrChanging(global::System.String value);
        partial void Ontcp_flags_cwrChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> tcp_hdr_len
        {
            get
            {
                return _tcp_hdr_len;
            }
            set
            {
                Ontcp_hdr_lenChanging(value);
                ReportPropertyChanging("tcp_hdr_len");
                _tcp_hdr_len = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("tcp_hdr_len");
                Ontcp_hdr_lenChanged();
            }
        }
        private Nullable<global::System.Int32> _tcp_hdr_len;
        partial void Ontcp_hdr_lenChanging(Nullable<global::System.Int32> value);
        partial void Ontcp_hdr_lenChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String tcp_need_segment
        {
            get
            {
                return _tcp_need_segment;
            }
            set
            {
                Ontcp_need_segmentChanging(value);
                ReportPropertyChanging("tcp_need_segment");
                _tcp_need_segment = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("tcp_need_segment");
                Ontcp_need_segmentChanged();
            }
        }
        private global::System.String _tcp_need_segment;
        partial void Ontcp_need_segmentChanging(global::System.String value);
        partial void Ontcp_need_segmentChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String tcp_nxtseq
        {
            get
            {
                return _tcp_nxtseq;
            }
            set
            {
                Ontcp_nxtseqChanging(value);
                ReportPropertyChanging("tcp_nxtseq");
                _tcp_nxtseq = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("tcp_nxtseq");
                Ontcp_nxtseqChanged();
            }
        }
        private global::System.String _tcp_nxtseq;
        partial void Ontcp_nxtseqChanging(global::System.String value);
        partial void Ontcp_nxtseqChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> tcp_options_sack_se_num
        {
            get
            {
                return _tcp_options_sack_se_num;
            }
            set
            {
                Ontcp_options_sack_se_numChanging(value);
                ReportPropertyChanging("tcp_options_sack_se_num");
                _tcp_options_sack_se_num = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("tcp_options_sack_se_num");
                Ontcp_options_sack_se_numChanged();
            }
        }
        private Nullable<global::System.Int32> _tcp_options_sack_se_num;
        partial void Ontcp_options_sack_se_numChanging(Nullable<global::System.Int32> value);
        partial void Ontcp_options_sack_se_numChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String tcp_seq
        {
            get
            {
                return _tcp_seq;
            }
            set
            {
                Ontcp_seqChanging(value);
                ReportPropertyChanging("tcp_seq");
                _tcp_seq = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("tcp_seq");
                Ontcp_seqChanged();
            }
        }
        private global::System.String _tcp_seq;
        partial void Ontcp_seqChanging(global::System.String value);
        partial void Ontcp_seqChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> tcp_srcport
        {
            get
            {
                return _tcp_srcport;
            }
            set
            {
                Ontcp_srcportChanging(value);
                ReportPropertyChanging("tcp_srcport");
                _tcp_srcport = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("tcp_srcport");
                Ontcp_srcportChanged();
            }
        }
        private Nullable<global::System.Int32> _tcp_srcport;
        partial void Ontcp_srcportChanging(Nullable<global::System.Int32> value);
        partial void Ontcp_srcportChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String tcp_window_size
        {
            get
            {
                return _tcp_window_size;
            }
            set
            {
                Ontcp_window_sizeChanging(value);
                ReportPropertyChanging("tcp_window_size");
                _tcp_window_size = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("tcp_window_size");
                Ontcp_window_sizeChanged();
            }
        }
        private global::System.String _tcp_window_size;
        partial void Ontcp_window_sizeChanging(global::System.String value);
        partial void Ontcp_window_sizeChanged();

        #endregion
    
    }

    #endregion
    
}