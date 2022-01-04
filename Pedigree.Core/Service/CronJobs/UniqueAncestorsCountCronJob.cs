using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class UniqueAncestorsCountCronJob : CronJobService
    {
        private readonly ILogger<UniqueAncestorsCountCronJob> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public UniqueAncestorsCountCronJob(IScheduleConfig<UniqueAncestorsCountCronJob> config, ILogger<UniqueAncestorsCountCronJob> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UniqueAncestorsCountCronJob starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} UniqueAncestorsCountCronJob is working.");

            var horseService = _scope.ServiceProvider.GetRequiredService<IHorseService>();
            await horseService.DoCalculateUniqueAncestorsCount();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} UniqueAncestorsCountCronJob completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UniqueAncestorsCountCronJob is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
