using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class BPRCronJob : CronJobService
    {
        private readonly ILogger<BPRCronJob> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public BPRCronJob(IScheduleConfig<BPRCronJob> config, ILogger<BPRCronJob> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BPRCronJob starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} BPRCronJob is working.");

            var service = _scope.ServiceProvider.GetRequiredService<IHorseService>();
            await service.CalculateBPRs();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} BPRCronJob completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BPRCronJob is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
