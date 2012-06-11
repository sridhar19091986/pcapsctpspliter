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

namespace GbPlatForm
{
    #region Contexts
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    public partial class Guangzhou_GbEntities : ObjectContext
    {
        #region Constructors
    
        /// <summary>
        /// Initializes a new Guangzhou_GbEntities object using the connection string found in the 'Guangzhou_GbEntities' section of the application configuration file.
        /// </summary>
        public Guangzhou_GbEntities() : base("name=Guangzhou_GbEntities", "Guangzhou_GbEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new Guangzhou_GbEntities object.
        /// </summary>
        public Guangzhou_GbEntities(string connectionString) : base(connectionString, "Guangzhou_GbEntities")
        {
            this.ContextOptions.LazyLoadingEnabled = true;
            OnContextCreated();
        }
    
        /// <summary>
        /// Initialize a new Guangzhou_GbEntities object.
        /// </summary>
        public Guangzhou_GbEntities(EntityConnection connection) : base(connection, "Guangzhou_GbEntities")
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
        public ObjectSet<myTcpSession> myTcpSession
        {
            get
            {
                if ((_myTcpSession == null))
                {
                    _myTcpSession = base.CreateObjectSet<myTcpSession>("myTcpSession");
                }
                return _myTcpSession;
            }
        }
        private ObjectSet<myTcpSession> _myTcpSession;

        #endregion
        #region AddTo Methods
    
        /// <summary>
        /// Deprecated Method for adding a new object to the myTcpSession EntitySet. Consider using the .Add method of the associated ObjectSet&lt;T&gt; property instead.
        /// </summary>
        public void AddTomyTcpSession(myTcpSession myTcpSession)
        {
            base.AddObject("myTcpSession", myTcpSession);
        }

        #endregion
    }
    

    #endregion
    
    #region Entities
    
    /// <summary>
    /// No Metadata Documentation available.
    /// </summary>
    [EdmEntityTypeAttribute(NamespaceName="Guangzhou_GbModel", Name="myTcpSession")]
    [Serializable()]
    [DataContractAttribute(IsReference=true)]
    public partial class myTcpSession : EntityObject
    {
        #region Factory Method
    
        /// <summary>
        /// Create a new myTcpSession object.
        /// </summary>
        /// <param name="session_id">Initial value of the session_id property.</param>
        /// <param name="direction">Initial value of the direction property.</param>
        public static myTcpSession CreatemyTcpSession(global::System.Int32 session_id, global::System.String direction)
        {
            myTcpSession myTcpSession = new myTcpSession();
            myTcpSession.session_id = session_id;
            myTcpSession.direction = direction;
            return myTcpSession;
        }

        #endregion
        #region Primitive Properties
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.Int32 session_id
        {
            get
            {
                return _session_id;
            }
            set
            {
                if (_session_id != value)
                {
                    Onsession_idChanging(value);
                    ReportPropertyChanging("session_id");
                    _session_id = StructuralObject.SetValidValue(value);
                    ReportPropertyChanged("session_id");
                    Onsession_idChanged();
                }
            }
        }
        private global::System.Int32 _session_id;
        partial void Onsession_idChanging(global::System.Int32 value);
        partial void Onsession_idChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [DataMemberAttribute()]
        public global::System.String direction
        {
            get
            {
                return _direction;
            }
            set
            {
                if (_direction != value)
                {
                    OndirectionChanging(value);
                    ReportPropertyChanging("direction");
                    _direction = StructuralObject.SetValidValue(value, false);
                    ReportPropertyChanged("direction");
                    OndirectionChanged();
                }
            }
        }
        private global::System.String _direction;
        partial void OndirectionChanging(global::System.String value);
        partial void OndirectionChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String imsi
        {
            get
            {
                return _imsi;
            }
            set
            {
                OnimsiChanging(value);
                ReportPropertyChanging("imsi");
                _imsi = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("imsi");
                OnimsiChanged();
            }
        }
        private global::System.String _imsi;
        partial void OnimsiChanging(global::System.String value);
        partial void OnimsiChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> lac
        {
            get
            {
                return _lac;
            }
            set
            {
                OnlacChanging(value);
                ReportPropertyChanging("lac");
                _lac = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("lac");
                OnlacChanged();
            }
        }
        private Nullable<global::System.Int32> _lac;
        partial void OnlacChanging(Nullable<global::System.Int32> value);
        partial void OnlacChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ci
        {
            get
            {
                return _ci;
            }
            set
            {
                OnciChanging(value);
                ReportPropertyChanging("ci");
                _ci = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ci");
                OnciChanged();
            }
        }
        private Nullable<global::System.Int32> _ci;
        partial void OnciChanging(Nullable<global::System.Int32> value);
        partial void OnciChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public global::System.String bsc_ip
        {
            get
            {
                return _bsc_ip;
            }
            set
            {
                Onbsc_ipChanging(value);
                ReportPropertyChanging("bsc_ip");
                _bsc_ip = StructuralObject.SetValidValue(value, true);
                ReportPropertyChanged("bsc_ip");
                Onbsc_ipChanged();
            }
        }
        private global::System.String _bsc_ip;
        partial void Onbsc_ipChanging(global::System.String value);
        partial void Onbsc_ipChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> bsc_bvci
        {
            get
            {
                return _bsc_bvci;
            }
            set
            {
                Onbsc_bvciChanging(value);
                ReportPropertyChanging("bsc_bvci");
                _bsc_bvci = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("bsc_bvci");
                Onbsc_bvciChanged();
            }
        }
        private Nullable<global::System.Int32> _bsc_bvci;
        partial void Onbsc_bvciChanging(Nullable<global::System.Int32> value);
        partial void Onbsc_bvciChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Double> duration
        {
            get
            {
                return _duration;
            }
            set
            {
                OndurationChanging(value);
                ReportPropertyChanging("duration");
                _duration = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("duration");
                OndurationChanged();
            }
        }
        private Nullable<global::System.Double> _duration;
        partial void OndurationChanging(Nullable<global::System.Double> value);
        partial void OndurationChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> ip_total
        {
            get
            {
                return _ip_total;
            }
            set
            {
                Onip_totalChanging(value);
                ReportPropertyChanging("ip_total");
                _ip_total = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ip_total");
                Onip_totalChanged();
            }
        }
        private Nullable<global::System.Decimal> _ip_total;
        partial void Onip_totalChanging(Nullable<global::System.Decimal> value);
        partial void Onip_totalChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Double> ip_rate
        {
            get
            {
                return _ip_rate;
            }
            set
            {
                Onip_rateChanging(value);
                ReportPropertyChanging("ip_rate");
                _ip_rate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ip_rate");
                Onip_rateChanged();
            }
        }
        private Nullable<global::System.Double> _ip_rate;
        partial void Onip_rateChanging(Nullable<global::System.Double> value);
        partial void Onip_rateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> ip2_total
        {
            get
            {
                return _ip2_total;
            }
            set
            {
                Onip2_totalChanging(value);
                ReportPropertyChanging("ip2_total");
                _ip2_total = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ip2_total");
                Onip2_totalChanged();
            }
        }
        private Nullable<global::System.Decimal> _ip2_total;
        partial void Onip2_totalChanging(Nullable<global::System.Decimal> value);
        partial void Onip2_totalChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Double> ip2_rate
        {
            get
            {
                return _ip2_rate;
            }
            set
            {
                Onip2_rateChanging(value);
                ReportPropertyChanging("ip2_rate");
                _ip2_rate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ip2_rate");
                Onip2_rateChanged();
            }
        }
        private Nullable<global::System.Double> _ip2_rate;
        partial void Onip2_rateChanging(Nullable<global::System.Double> value);
        partial void Onip2_rateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> seq_total
        {
            get
            {
                return _seq_total;
            }
            set
            {
                Onseq_totalChanging(value);
                ReportPropertyChanging("seq_total");
                _seq_total = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("seq_total");
                Onseq_totalChanged();
            }
        }
        private Nullable<global::System.Decimal> _seq_total;
        partial void Onseq_totalChanging(Nullable<global::System.Decimal> value);
        partial void Onseq_totalChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> ip2_min_len
        {
            get
            {
                return _ip2_min_len;
            }
            set
            {
                Onip2_min_lenChanging(value);
                ReportPropertyChanging("ip2_min_len");
                _ip2_min_len = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("ip2_min_len");
                Onip2_min_lenChanged();
            }
        }
        private Nullable<global::System.Int32> _ip2_min_len;
        partial void Onip2_min_lenChanging(Nullable<global::System.Int32> value);
        partial void Onip2_min_lenChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> seq_max
        {
            get
            {
                return _seq_max;
            }
            set
            {
                Onseq_maxChanging(value);
                ReportPropertyChanging("seq_max");
                _seq_max = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("seq_max");
                Onseq_maxChanged();
            }
        }
        private Nullable<global::System.Decimal> _seq_max;
        partial void Onseq_maxChanging(Nullable<global::System.Decimal> value);
        partial void Onseq_maxChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> seq_nxt
        {
            get
            {
                return _seq_nxt;
            }
            set
            {
                Onseq_nxtChanging(value);
                ReportPropertyChanging("seq_nxt");
                _seq_nxt = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("seq_nxt");
                Onseq_nxtChanged();
            }
        }
        private Nullable<global::System.Decimal> _seq_nxt;
        partial void Onseq_nxtChanging(Nullable<global::System.Decimal> value);
        partial void Onseq_nxtChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> seq_min
        {
            get
            {
                return _seq_min;
            }
            set
            {
                Onseq_minChanging(value);
                ReportPropertyChanging("seq_min");
                _seq_min = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("seq_min");
                Onseq_minChanged();
            }
        }
        private Nullable<global::System.Decimal> _seq_min;
        partial void Onseq_minChanging(Nullable<global::System.Decimal> value);
        partial void Onseq_minChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Double> seq_rate
        {
            get
            {
                return _seq_rate;
            }
            set
            {
                Onseq_rateChanging(value);
                ReportPropertyChanging("seq_rate");
                _seq_rate = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("seq_rate");
                Onseq_rateChanged();
            }
        }
        private Nullable<global::System.Double> _seq_rate;
        partial void Onseq_rateChanging(Nullable<global::System.Double> value);
        partial void Onseq_rateChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> headersize
        {
            get
            {
                return _headersize;
            }
            set
            {
                OnheadersizeChanging(value);
                ReportPropertyChanging("headersize");
                _headersize = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("headersize");
                OnheadersizeChanged();
            }
        }
        private Nullable<global::System.Decimal> _headersize;
        partial void OnheadersizeChanging(Nullable<global::System.Decimal> value);
        partial void OnheadersizeChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Decimal> seq_ip2
        {
            get
            {
                return _seq_ip2;
            }
            set
            {
                Onseq_ip2Changing(value);
                ReportPropertyChanging("seq_ip2");
                _seq_ip2 = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("seq_ip2");
                Onseq_ip2Changed();
            }
        }
        private Nullable<global::System.Decimal> _seq_ip2;
        partial void Onseq_ip2Changing(Nullable<global::System.Decimal> value);
        partial void Onseq_ip2Changed();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> packet_count
        {
            get
            {
                return _packet_count;
            }
            set
            {
                Onpacket_countChanging(value);
                ReportPropertyChanging("packet_count");
                _packet_count = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("packet_count");
                Onpacket_countChanged();
            }
        }
        private Nullable<global::System.Int32> _packet_count;
        partial void Onpacket_countChanging(Nullable<global::System.Int32> value);
        partial void Onpacket_countChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> packet_count_repeat
        {
            get
            {
                return _packet_count_repeat;
            }
            set
            {
                Onpacket_count_repeatChanging(value);
                ReportPropertyChanging("packet_count_repeat");
                _packet_count_repeat = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("packet_count_repeat");
                Onpacket_count_repeatChanged();
            }
        }
        private Nullable<global::System.Int32> _packet_count_repeat;
        partial void Onpacket_count_repeatChanging(Nullable<global::System.Int32> value);
        partial void Onpacket_count_repeatChanged();
    
        /// <summary>
        /// No Metadata Documentation available.
        /// </summary>
        [EdmScalarPropertyAttribute(EntityKeyProperty=false, IsNullable=true)]
        [DataMemberAttribute()]
        public Nullable<global::System.Int32> packet_discard_total
        {
            get
            {
                return _packet_discard_total;
            }
            set
            {
                Onpacket_discard_totalChanging(value);
                ReportPropertyChanging("packet_discard_total");
                _packet_discard_total = StructuralObject.SetValidValue(value);
                ReportPropertyChanged("packet_discard_total");
                Onpacket_discard_totalChanged();
            }
        }
        private Nullable<global::System.Int32> _packet_discard_total;
        partial void Onpacket_discard_totalChanging(Nullable<global::System.Int32> value);
        partial void Onpacket_discard_totalChanged();

        #endregion
    
    }

    #endregion
    
}
