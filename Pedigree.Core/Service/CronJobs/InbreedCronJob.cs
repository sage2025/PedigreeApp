using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class InbreedCronJob : CronJobService
    {
        private readonly ILogger<InbreedCronJob> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public InbreedCronJob(IScheduleConfig<InbreedCronJob> config, ILogger<InbreedCronJob> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("InbreedCronJob starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} InbreedCronJob is working.");

            var inbreedService = _scope.ServiceProvider.GetRequiredService<IInbreedService>();
            await inbreedService.RemoveUnnecessaryInbreeds();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} InbreedCronJob completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("InbreedCronJob is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
