using Harmonic.Controllers.Living;
using Harmonic.Hosting;
using Harmonic.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer.Extensions
{
    public class RtmpStartup : IStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {

        }
    }
    public class RtmpServerManager
    {
        private RtmpServer server = null;
        private Task serverTask = null;
        private CancellationTokenSource tokenSource = null;
        private RtmpServerOptions options = null;
        private PublisherSessionService publisherSessionService = null;

        public bool ServiceHostStarted()
        {
            return serverTask != null;
        }

        public void Start() 
        {
            server = new RtmpServerBuilder()
                    .UseStartup<RtmpStartup>()
                    .UseHarmonic(config =>
                    {
                        options = config;
                    })
                    .UseWebSocket(c =>
                    {
                        c.BindEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8080);
                    })
                    .Build();
            tokenSource = new CancellationTokenSource();
            serverTask = server.StartAsync(tokenSource.Token);
        }
        public async Task Stop() 
        {
            using (serverTask)
            using (tokenSource)
            {
                try
                {
                    tokenSource.Cancel();
                    await serverTask;
                }
                catch(Exception e)
                {

                }
                server = null;
                serverTask = null;
                tokenSource = null;
                options = null;
                publisherSessionService = null;
            }
        }

        public LivingStream GetLivingStream(string streamName)
        {
            if(publisherSessionService == null && serverTask != null)
            {
                var type = options.GetType();
                var proInfo = type.GetProperty("ScopedServiceProvider", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var serviceProvider = (IServiceProvider)proInfo.GetValue(options);
                publisherSessionService = serviceProvider.GetService<PublisherSessionService>();
            }

            var _ = publisherSessionService ?? throw new InvalidOperationException("无法获取直播流");
            return publisherSessionService.FindPublisher(streamName);
        }
    }
}
