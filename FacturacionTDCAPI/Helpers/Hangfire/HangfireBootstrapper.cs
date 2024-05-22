using Hangfire;
using Hangfire.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace FacturacionTDCAPI.Helpers.Hangfire
{
    public class HangfireBootstrapper : IRegisteredObject
    {
        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

        private readonly object _lockObject = new object();
        private bool _started;

        private BackgroundJobServer _backgroundJobServer;

        private HangfireBootstrapper()
        {
        }

        public void Start()
        {
            lock (_lockObject)
            {
                if (_started) return;
                _started = true;

                HostingEnvironment.RegisterObject(this);

                var options = new SqlServerStorageOptions
                {
                };

                GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireConnection", options);

                _backgroundJobServer = new BackgroundJobServer();

            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                _backgroundJobServer?.Dispose();

                HostingEnvironment.UnregisterObject(this);
            }
        }

        void IRegisteredObject.Stop(bool immediate)
        {
            Stop();
        }
    }
}