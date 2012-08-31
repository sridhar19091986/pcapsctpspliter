using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
//using System.ServiceModel;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Data.Entity;
//using System.Data.Entity.Design;
//using EntityFramework.Extensions;
//using EntityFramework.Caching;
//using EntityFramework.Batch;

//using MutliInterfaceGnGi.GbFlowControl;
using System.IO;
using EFProviderWrapperToolkit;
using EFCachingProvider;
using EFCachingProvider.Caching;
using EFTracingProvider;
//using MutliInterfaceGnGi.ServerEntity.Gb;

//using EFProviderWrapperToolkit;
//using EFTracingProvider;
//using EFCachingProvider.Caching;
//using EFCachingProvider;


namespace EntityClass.ServerEntity.Gb
{
    public partial class ExtendedGuangZhou_GbEntities : Guangzhou_GbEntities
    {
        private TextWriter logOutput;

        public ExtendedGuangZhou_GbEntities()
            : this("name=Guangzhou_GbEntities")
        {
        }

        public ExtendedGuangZhou_GbEntities(string connectionString)
            : base(EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(
                    connectionString,
                    "EFTracingProvider",
                    "EFCachingProvider"
            ))
        {
        }

        #region Tracing Extensions

        private EFTracingConnection TracingConnection
        {
            get { return this.UnwrapConnection<EFTracingConnection>(); }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandExecuting
        {
            add { this.TracingConnection.CommandExecuting += value; }
            remove { this.TracingConnection.CommandExecuting -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFinished
        {
            add { this.TracingConnection.CommandFinished += value; }
            remove { this.TracingConnection.CommandFinished -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFailed
        {
            add { this.TracingConnection.CommandFailed += value; }
            remove { this.TracingConnection.CommandFailed -= value; }
        }

        private void AppendToLog(object sender, CommandExecutionEventArgs e)
        {
            if (this.logOutput != null)
            {
                this.logOutput.WriteLine(e.ToTraceString().TrimEnd());
                this.logOutput.WriteLine();
            }
        }

        public TextWriter Log
        {
            get { return this.logOutput; }
            set
            {
                if ((this.logOutput != null) != (value != null))
                {
                    if (value == null)
                    {
                        CommandExecuting -= AppendToLog;
                    }
                    else
                    {
                        CommandExecuting += AppendToLog;
                    }
                }

                this.logOutput = value;
            }
        }


        #endregion

        #region Caching Extensions

        private EFCachingConnection CachingConnection
        {
            get { return this.UnwrapConnection<EFCachingConnection>(); }
        }

        public ICache Cache
        {
            get { return CachingConnection.Cache; }
            set { CachingConnection.Cache = value; }
        }

        public CachingPolicy CachingPolicy
        {
            get { return CachingConnection.CachingPolicy; }
            set { CachingConnection.CachingPolicy = value; }
        }

        #endregion
    }

}
