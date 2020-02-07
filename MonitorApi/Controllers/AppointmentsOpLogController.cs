using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CoreApiModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitorApi.Services;


namespace MonitorApi.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/v1/AppointmentsOpLog")]
    [ApiController]
    public class AppointmentsOpLogController : ControllerBase
    {
        private IAppointmentOpLogStorageService _appointmentOpLogStorageService { get; }
        public AppointmentsOpLogController(IAppointmentOpLogStorageService appointmentOpLogStorageService)
        {
            _appointmentOpLogStorageService = appointmentOpLogStorageService;
        }

        [HttpGet]
        [Route("GetAppointmentsOpLogs")]
        [ProducesResponseType(200, Type = typeof(List<AppointmentsOpLogModel>))]
        public async Task<IActionResult> GetAppointmentsOpLogs(DateTime fromDateTime)
        {
            
            return StatusCode(201, await _appointmentOpLogStorageService.GetAppointmentsOpLogViewModels(fromDateTime));
        }

        


       
    }
}