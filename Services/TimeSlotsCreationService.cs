using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using uul_api.Models;
using uul_api.Data;
using Microsoft.Extensions.DependencyInjection;
using uul_api.Operations;
using Microsoft.Extensions.Logging;

namespace uul_api.Services {
    public class TimeSlotsCreationService : IHostedService {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TimeSlotsCreationService> _logger;
        private Timer _timer;

        public TimeSlotsCreationService(IServiceScopeFactory scopeFactory, ILogger<TimeSlotsCreationService> logger) {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken) {
            // timer repeates call to CreateTimeSlots every 24 hours.
            TimeSpan interval = TimeSpan.FromHours(24);//.FromMinutes(1);//
                                                        //calculate time to run the first time & delay to set the timer
                                                        //DateTime.Today gives time of midnight 00.00
            var nextRunTime = DateOperations.Today().AddDays(1).AddHours(7); // DateTime.Now.AddMinutes(2);//
            var curTime = DateOperations.Now();// DateTime.Now;
            var firstInterval = nextRunTime.Subtract(curTime);

            void action() {
                var t1 = Task.Delay(firstInterval, cancellationToken);
                t1.Wait(cancellationToken);
                //create at expected time
                _logger.LogInformation("First service run");
                CreateTimeSlots(null);
                //now schedule it to be called every 24 hours for future
                // timer repeates call to CreateTimeSlots every 24 hours.
                _timer = new Timer(
                    CreateTimeSlots,
                    null,
                    TimeSpan.Zero,
                    interval
                );
            }

            // no need to await this call here because this task is scheduled to run much much later.
            Task.Run(action, cancellationToken);
			return Task.CompletedTask;
		}

        Task IHostedService.StopAsync(CancellationToken cancellationToken) {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
        
        private void CreateTimeSlots(object state) {
            var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UULContext>();
            var newSlots = TimeSlotsFactory.CreateTodayTimeSlots(dbContext, 11);
            newSlots.Wait();
            dbContext.TimeSlots.AddRange(newSlots.Result);
            int rows = dbContext.SaveChanges();
            _logger.LogInformation("Creation func affected " + rows + " rows, run at " + DateOperations.Now().ToString());
            scope.Dispose();
        }
    }
}
