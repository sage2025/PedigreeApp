using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class HorseCronJob : CronJobService
    {
        private readonly ILogger<HorseCronJob> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public HorseCronJob(IScheduleConfig<HorseCronJob> config, ILogger<HorseCronJob> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HorseCronJob starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} HorseCronJob is working.");

            var horseService = _scope.ServiceProvider.GetRequiredService<IHorseService>();
            await horseService.DoCalculateCOIsForAllHorses();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} HorseCronJob completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HorseCronJob is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
