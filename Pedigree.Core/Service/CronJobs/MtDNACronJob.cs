using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class MtDNACronJob : CronJobService
    {
        private readonly ILogger<MtDNACronJob> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public MtDNACronJob(IScheduleConfig<MtDNACronJob> config, ILogger<MtDNACronJob> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MtDNACronJob starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} MtDNACronJob is working.");

            var horseService = _scope.ServiceProvider.GetRequiredService<IHorseService>();
            await horseService.SetMtDNAForFounders();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} MtDNACronJob completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MtDNACronJob is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
