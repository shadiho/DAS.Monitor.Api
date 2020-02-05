using CoreApiModels;
using DASInMemoryDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MonitorApi.Services
{
    public class AppointmentOpLogStorageService : IAppointmentOpLogStorageService
    {
        public async Task<bool> AddLogEntry(AppointmentModel appointment, string op)
        {
            string doctorName = InMemoryDatabase.Doctors.Select((d,i) => d.DoctorName).FirstOrDefault();
            string patientName = InMemoryDatabase.Patients.Select((p,i) => p.PatientName).FirstOrDefault();
            InMemoryDatabase.AppointmentsOpLog.Add(new AppointmentsOpLogModel()
            {
                LogID = Guid.NewGuid().ToString(),
                AppointmentID = appointment.AppointmentID,
                DoctorName = doctorName,
                PatientName = patientName,
                FromDate = appointment.FromDate,
                ToDate = appointment.ToDate,
                CreationDateTime = appointment.CreationDateTime,
                Operation = op,
                LogDateTime = DateTime.Now
            }) ;
          
            return await Task.FromResult(true);
        }

        public async Task<List<AppointmentsOpLogModel>> GetAppointmentsOpLogViewModels(DateTime fromDate)
        {
            var ops = from op in InMemoryDatabase.AppointmentsOpLog
                      where fromDate < op.LogDateTime
                      select op;
            return await Task.FromResult(ops.ToList());
        }
    }
}
