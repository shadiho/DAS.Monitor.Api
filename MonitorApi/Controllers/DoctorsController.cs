using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DASInMemoryDatabase;
namespace MonitorApi.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/v1/DoctorsController")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        [HttpGet]
        [Route("GetDoctors")]
        [ProducesResponseType(200, Type = typeof(List<DoctorModel>))]
        public async Task<IActionResult> GetDoctors()
        {
            return StatusCode(201, await Task.FromResult(InMemoryDatabase.Doctors.ToList()));
        }
    }
}