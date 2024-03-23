using deviceInterfacev2.ExtraFiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerService.Models
{
    public class SchedulerService : IHostedService, IDisposable
    {
        private int executionCount = 0;

        private System.Threading.Timer _timerNotification;
        public IConfiguration _iconfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;


        public SchedulerService(IServiceScopeFactory serviceScopeFactory, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration iconfiguration)
        {

            _serviceScopeFactory = serviceScopeFactory;
            _env = env;
            _iconfiguration = iconfiguration;
     
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timerNotification = new Timer(RunJob, null, TimeSpan.Zero,
              TimeSpan.FromMinutes(60)); /*Set Interval time here*/


            return Task.CompletedTask;
        }

        private void RunJob(object state)
        {

            using (var scrope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    //  var store = scrope.ServiceProvider.GetService<IStoreRepo>(); /* You can access any interface or service like this here*/
                    //store.GetAll(); /* You can access any interface or service method like this here*/

                    /*
                     Place your code here which you want to schedule on regular intervals
                     */
                    MqttConnector mqttc = new MqttConnector();
                    mqttc.MqttConnectionLoad();

                }


                catch (Exception ex)
                {

                }

                Interlocked.Increment(ref executionCount);

            }

        }




        public Task StopAsync(CancellationToken stoppingToken)
        {

            _timerNotification?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerNotification?.Dispose();

        }
    }
}