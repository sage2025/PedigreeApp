using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pedigree.Core.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service
{
    public class StallionRatingCronJob : CronJobService
    {
        private readonly ILogger<StallionRatingCronJob> _logger;
        private readonly IServiceScopeFactory _serviceFactory;
        private IServiceScope _scope;
        public StallionRatingCronJob(IScheduleConfig<StallionRatingCronJob> config, ILogger<StallionRatingCronJob> logger, IServiceScopeFactory serviceFactory)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StallionRatingCronJob starts.");

            _scope = _serviceFactory.CreateScope();
            
            return base.StartAsync(cancellationToken);
        }

        public async override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} StallionRatingCronJob is working.");

            var stallionRatingService = _scope.ServiceProvider.GetRequiredService<IStallionRatingService>();
            await stallionRatingService.CalculateStallionRatings();
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} StallionRatingCronJob completed.");
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StallionRatingCronJob is stopping.");

            return base.StopAsync(cancellationToken);
        }
    }
}
