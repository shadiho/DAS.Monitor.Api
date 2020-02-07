using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiModels;
using DASInMemoryDatabase;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MonitorApi.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/v1/PatientsController")]
    [ApiController]
    public class PatientsController : ControllerBase
    {

        [HttpGet]
        [Route("GetPatients")]
        [ProducesResponseType(200, Type = typeof(List<PatientModel>))]
        public async Task<IActionResult> GetPatients()
        {
            return StatusCode(201, await Task.FromResult(InMemoryDatabase.Patients.ToList()));
        }
    }
}