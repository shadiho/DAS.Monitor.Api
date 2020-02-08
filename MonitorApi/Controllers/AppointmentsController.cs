using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreApiModels;
using DASInMemoryDatabase;
using Microsoft.AspNetCore.Cors;

namespace MonitorApi.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/v1/AppointmentsController")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {

        [HttpGet]
        [Route("GetAppointments")]
        [ProducesResponseType(200, Type = typeof(List<AppointmentModel>))]
        public async Task<IActionResult> GetAppointments(string doctorID, string patientID, DateTime? fromDate, DateTime? toDate)
        {
            List<AppointmentModel> appointments = InMemoryDatabase.Appointments;
            if (!string.IsNullOrEmpty(doctorID))
            {
                appointments = appointments.Where((a, i) => a.DoctorId == doctorID).ToList();
            }
            if (!string.IsNullOrEmpty(patientID))
            {
                appointments = appointments.Where((a, i) => a.PatientId == patientID).ToList();
            }
            if (fromDate != null)
            {
                appointments = appointments.Where((a, i) => a.CreationDateTime >= fromDate).ToList();
            }
            if (toDate != null)
            {
                appointments = appointments.Where((a, i) => a.CreationDateTime <= toDate).ToList();
            }
            if (string.IsNullOrEmpty(doctorID) && string.IsNullOrEmpty(patientID))
            {
                appointments = appointments.ToList();
            }

            List<AppointmentsOpLogModel> outAppointments=new List<AppointmentsOpLogModel>();
            foreach (var appointment in appointments)
            {
                var outAppointment = new AppointmentsOpLogModel();
                outAppointment.AppointmentID = appointment.AppointmentID;
                outAppointment.CreationDateTime = appointment.CreationDateTime;
                outAppointment.DoctorName = InMemoryDatabase.Doctors.Where(d => d.DoctorId == appointment.DoctorId).FirstOrDefault().DoctorName;
                outAppointment.PatientName = InMemoryDatabase.Patients.Where(p => p.PatientId == appointment.PatientId).FirstOrDefault().PatientName;
                outAppointment.Operation = "";
                outAppointment.FromDate = appointment.FromDate;
                outAppointment.ToDate = appointment.ToDate;
                outAppointment.PatientID = appointment.PatientId;
                outAppointment.DoctorID = appointment.DoctorId;
                outAppointments.Add(outAppointment);
            }

            return StatusCode(201, await Task.FromResult(outAppointments));
        }


        [HttpDelete]
        [Route("ResetApp")]
        [ProducesResponseType(200)]
        [EnableCors("AllowAll")]
        public async Task<IActionResult> ResetApp()
        {
            InMemoryDatabase.Appointments.Clear();
            InMemoryDatabase.AppointmentsConflicts.Clear();
            InMemoryDatabase.AppointmentsOpLog.Clear();
            return await Task.FromResult(new OkResult());
        }

        [HttpGet]
        [Route("DeleteAll")]
        public async Task<IActionResult> DeleteAll()
        {
            InMemoryDatabase.Appointments.Clear();
            InMemoryDatabase.AppointmentsConflicts.Clear();
            InMemoryDatabase.AppointmentsOpLog.Clear();
            return await Task.FromResult(new OkResult());
        }
    }

   
}