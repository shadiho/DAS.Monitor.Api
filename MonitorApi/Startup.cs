using DASInMemoryDatabase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitorApi.Services;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace MonitorApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            InMemoryDatabase.Initialize();
        }

        public IConfiguration Configuration { get; }
        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAppointmentStorageService, InMemoryAppointmentStorage>();
            services.AddTransient<IAppointmentOpLogStorageService, AppointmentOpLogStorageService>();
            services.AddTransient<IConflictsManager, ConflictManager>();


            //services.AddSingleton(provider => GetScheduler().Result);
            services.AddControllers();

           
            // var hostBuilder = new WebHostBuilder().ConfigureServices(services =>
            //services.AddHostedService<SQSListener>());
            //hostBuilder.UseIIS();

        }

        private async Task<IScheduler> GetScheduler()
        {
            var properties = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", "QuartzWithCore" },
                { "quartz.threadPool.type" , "Quartz.Simpl.SimpleThreadPool, Quartz" },
                { "quartz.threadPool.threadCount" , "3" },
                { "quartz.jobStore.type" , "Quartz.Simpl.RAMJobStore, Quartz" },
                { "quartz.serializer.type", "json" },
            };
            var schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = await schedulerFactory.GetScheduler();
            await scheduler.Start();
            return scheduler;
        }

        public async Task ConfigureSQSListenerJob()
        {
            ITrigger trigger = TriggerBuilder.Create()
             .WithIdentity($"SQS Listener-{DateTime.Now}")
             //.StartNow()
             .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(2)))
             .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())             
             .WithPriority(1)
             .Build();

            IDictionary<string, object> map = new Dictionary<string, object>()
            {
                {"DASAppointmentsQueueName", Configuration.GetValue<string>("DASAppointmentsQueueName") },
            };

            IJobDetail job = JobBuilder.Create<SQSListenerJob>()
                        .WithIdentity("SQS Listener")
                        .SetJobData(new JobDataMap(map))
                        .Build();
            var scheduler = GetScheduler().Result;
            await scheduler.ScheduleJob(job, trigger);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            ConfigureSQSListenerJob();


        }

       
    }
}
