using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using CoreApiModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace MonitorApi.Services
{
    public class SQSListener
    {
        public string DASAppointmentsQueueName { get; set; }
        public IAppointmentStorageService AppointmentStorageService { get;  }

        public IAppointmentOpLogStorageService AppointmentOpLogStorageService { get;  }

        public IConflictsManager ConflictsManager { get;  }
        public SQSListener(string dASAppointmentsQueueName, IAppointmentStorageService appointmentStorageService, 
            IAppointmentOpLogStorageService appointmentOpLogStorageService,IConflictsManager conflictsManager)
        {
            DASAppointmentsQueueName = dASAppointmentsQueueName;
            AppointmentStorageService = appointmentStorageService;
            AppointmentOpLogStorageService = appointmentOpLogStorageService;
            ConflictsManager = conflictsManager;
        }
            
      
        public async Task Listen()
        {
            IAmazonSQS sqs = new AmazonSQSClient(RegionEndpoint.USEast2);
            //var queueUrl = sqs.GetQueueUrlAsync(Configuration.GetValue<string>("DASAppointmentsQueueName")).Result.QueueUrl;
            var queueUrl = sqs.GetQueueUrlAsync(DASAppointmentsQueueName).Result.QueueUrl;

            //while (!stoppingToken.IsCancellationRequested)
            //{
            try
                {
                    var receiveMessageRequest = new ReceiveMessageRequest 
                    { 
                        QueueUrl = queueUrl
                    };
                    var receiveMessageResponse = sqs.ReceiveMessageAsync(receiveMessageRequest).Result;
                    foreach (var message in receiveMessageResponse.Messages)
                    {
                        var messageObj = JsonConvert.DeserializeObject<SQSMessageModel>(message.Body);
                        if (messageObj.Op == "bookAppointment")
                        {
                            if (AppointmentStorageService.Book(messageObj.Appointment).Result)
                            {
                                await AppointmentOpLogStorageService.AddLogEntry(messageObj.Appointment, "book");
                                await ConflictsManager.CheckAndAddConflict(messageObj.Appointment);
                            }
                        }
                        else
                        {
                            if (messageObj.Op == "cancelAppointment")
                            {
                                if (AppointmentStorageService.Cancel(messageObj.Appointment).Result)
                                {
                                    await AppointmentOpLogStorageService.AddLogEntry(messageObj.Appointment, "cancel");
                                    await ConflictsManager.RemoveConflicts(messageObj.Appointment);
                                }
                            }
                        }
                        var messageReceiveHandle = receiveMessageResponse.Messages.FirstOrDefault()?.ReceiptHandle;
                        var deleteRequest = new DeleteMessageRequest
                        {
                            QueueUrl = queueUrl,
                            ReceiptHandle = messageReceiveHandle
                        };
                        await sqs.DeleteMessageAsync(deleteRequest);
                    }
                }
                catch(Exception ex)
                {
                    //await Task.Delay(TimeSpan.FromSeconds(3),stoppingToken);
                }
            //}
        }

        
    }
}
