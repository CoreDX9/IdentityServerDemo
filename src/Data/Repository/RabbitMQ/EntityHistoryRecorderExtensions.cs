using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Repository.RabbitMQ
{
    public class EntityHistoryRecorderOptions : IOptions<EntityHistoryRecorderOptions>
    {
        public EntityHistoryRecorderOptions Value => this;

        public string HostName { get; set; }
        public bool AutomaticRecoveryEnabled { get; set; }
        public double NetworkRecoveryIntervalSeconds { get; set; }
        public bool TopologyRecoveryEnabled { get; set; }
    }

    public static class EntityHistoryRecorderExtensions
    {
        public static IServiceCollection AddEntityHistoryRecorder(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<EntityHistoryRecorderOptions>(configuration.GetSection("EntityHistoryRecorder"));
            return services.AddSingleton<IEntityHistoryRecorder, EntityHistoryRecorder>();
        }

        public static IServiceCollection AddEntityHistoryRecorder(this IServiceCollection services,
            Action<EntityHistoryRecorderOptions> setupAction)
        {
            services.Configure(setupAction);
            return services.AddSingleton<IEntityHistoryRecorder, EntityHistoryRecorder>();
        }
    }
}
