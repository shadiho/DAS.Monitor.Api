using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoreApiModels;
using DASInMemoryDatabase;
namespace MonitorApi.Controllers
{
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

            return StatusCode(201, await Task.FromResult(appointments));
        }
    }
}