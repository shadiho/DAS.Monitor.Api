using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreApiModels;
using MonitorApi.Models;
using DASInMemoryDatabase;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;

namespace MonitorApi.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/v1/AppointmentConflictsController")]
    [ApiController]
    public class AppointmentConflictsController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        public AppointmentConflictsController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //this method returns the conflicts happened and discovered more than configurable minutes
        [HttpGet]
        [Route("GetConflicts")]
        [ProducesResponseType(200, Type = typeof(List<AppointmentConflictViewModel>))]
        public async Task<IActionResult> GetConflicts()
        {
            List<AppointmentConflictViewModel> outViewModels;
            Int32 threshold = 0 - Configuration.GetValue<Int32>("ConflictsThreshold");
            var conflictViewModels = from conf in InMemoryDatabase.AppointmentsConflicts
                                     join App1 in InMemoryDatabase.Appointments on conf.Appointment1ID equals App1.AppointmentID
                                     join App2 in InMemoryDatabase.Appointments on conf.Appointment2ID equals App2.AppointmentID
                                     join doc1 in InMemoryDatabase.Doctors on App1.DoctorId equals doc1.DoctorId
                                     join doc2 in InMemoryDatabase.Doctors on App2.DoctorId equals doc2.DoctorId
                                     join pat1 in InMemoryDatabase.Patients on App1.PatientId equals pat1.PatientId
                                     join pat2 in InMemoryDatabase.Patients on App2.PatientId equals pat2.PatientId
                                     where conf.ConflictDateTime <= DateTime.Now.AddMinutes(threshold)
                                     //select  new AppointmentConflictViewModel{ Appointment1 = App1, Appointment2 = App2, 
                                     //                                          AppointmentsConflictModel = conf,Doctor1 = doc1,
                                     //                                          Doctor2 = doc2, Patient1 = pat1, Patient2 = pat2} ;
                                     select new AppointmentConflictViewModel
                                     {
                                         Appointment1ID = App1.AppointmentID,
                                         Appointment1CreationDateTime = App1.CreationDateTime,
                                         Appointment1DoctorID = App1.DoctorId,
                                         Appointment1PatientID = App1.PatientId,
                                         Appointment1DoctorName = doc1.DoctorName,
                                         Appointment1PatientName = pat1.PatientName,
                                         Appointment1FromDate = App1.FromDate,
                                         Appointment1ToDate = App1.ToDate,
                                         Appointment2ID = App2.AppointmentID,
                                         Appointment2CreationDateTime = App2.CreationDateTime,
                                         Appointment2DoctorID = App2.DoctorId,
                                         Appointment2PatientID = App2.PatientId,
                                         Appointment2DoctorName = doc2.DoctorName,
                                         Appointment2PatientName = pat2.PatientName,
                                         Appointment2FromDate = App2.FromDate,
                                         Appointment2ToDate = App2.ToDate,
                                     };


            outViewModels = conflictViewModels.ToList();

            return StatusCode(201, await Task.FromResult(outViewModels));
        }
    }
}