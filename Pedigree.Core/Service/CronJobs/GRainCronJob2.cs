using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class GRainCronJob2 : CronJobService
    {
        private readonly ILogger<GRainCronJob2> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public GRainCronJob2(IScheduleConfig<GRainCronJob2> config, ILogger<GRainCronJob2> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GRainCronJob2 starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} GRainCronJob2 is working.");

            var horseService = _scope.ServiceProvider.GetRequiredService<IHorseService>();
            await horseService.DoCalculateGRain();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} GRainCronJob2 completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GRainCronJob2 is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
