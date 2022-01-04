using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class ProbOrigCronJob : CronJobService
    {
        private readonly ILogger<ProbOrigCronJob> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public ProbOrigCronJob(IScheduleConfig<ProbOrigCronJob> config, ILogger<ProbOrigCronJob> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProbOrigCronJob starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ProbOrigCronJob is working.");

            var horseService = _scope.ServiceProvider.GetRequiredService<IHorseService>();
            await horseService.DoCalculateProbOrig();
            await horseService.DoUpdatePedigree();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} ProbOrigCronJob completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProbOrigCronJob is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
