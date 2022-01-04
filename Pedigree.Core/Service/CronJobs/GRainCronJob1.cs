using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class GRainCronJob1 : CronJobService
    {
        private readonly ILogger<GRainCronJob1> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public GRainCronJob1(IScheduleConfig<GRainCronJob1> config, ILogger<GRainCronJob1> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GRainCronJob1 starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} GRainCronJob1 is working.");

            var horseService = _scope.ServiceProvider.GetRequiredService<IHorseService>();
            await horseService.DoCalculateGRain();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} GRainCronJob1 completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GRainCronJob1 is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
