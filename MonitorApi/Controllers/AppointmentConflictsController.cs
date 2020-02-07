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

namespace MonitorApi.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/v1/AppointmentConflictsController")]
    [ApiController]
    public class AppointmentConflictsController : ControllerBase
    {
        //this method returns the conflicts happened and discovered more than 5 minutes
        [HttpGet]
        [Route("GetConflicts")]
        [ProducesResponseType(200, Type = typeof(List<AppointmentConflictViewModel>))]
        public async Task<IActionResult> GetConflicts()
        {
            var conflictViewModels = from conf in InMemoryDatabase.AppointmentsConflicts
                       join App1 in InMemoryDatabase.Appointments on conf.Appointment1ID equals App1.AppointmentID
                       join App2 in InMemoryDatabase.Appointments on conf.Appointment1ID equals App2.AppointmentID
                       join doc1 in InMemoryDatabase.Doctors on App1.DoctorId equals doc1.DoctorId
                       join doc2 in InMemoryDatabase.Doctors on App2.DoctorId equals doc2.DoctorId
                       join pat1 in InMemoryDatabase.Patients on App1.PatientId equals pat1.PatientId
                       join pat2 in InMemoryDatabase.Patients on App2.PatientId equals pat2.PatientId
                       where conf.ConflictDateTime < DateTime.Now.AddMinutes(-2)
                       select  new AppointmentConflictViewModel{ Appointment1 = App1, Appointment2 = App2, 
                                                                 AppointmentsConflictModel = conf,Doctor1 = doc1,
                                                                 Doctor2 = doc2, Patient1 = pat1, Patient2 = pat2} ;
            return StatusCode(201, await Task.FromResult(conflictViewModels));
        }
    }
}