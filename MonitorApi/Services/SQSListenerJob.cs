using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MonitorApi.Services
{
    public class SQSListenerJob : IJob
    {

        public IConfiguration Configuration { get; }
        public IAppointmentStorageService AppointmentStorageService { get; }

        public IAppointmentOpLogStorageService AppointmentOpLogStorageService { get; }

        public IConflictsManager ConflictsManager { get; }

        
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var DASAppointmentsQueueName = dataMap.GetString("DASAppointmentsQueueName");
                SQSListener listener = new SQSListener(DASAppointmentsQueueName, new InMemoryAppointmentStorage(), new AppointmentOpLogStorageService(), new ConflictManager());
                listener.Listen();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Task.FromResult(0);
        }
    }
}
