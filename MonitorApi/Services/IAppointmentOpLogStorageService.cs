using CoreApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorApi.Services
{
    public interface IAppointmentOpLogStorageService
    {
        Task<bool> AddLogEntry(AppointmentModel appointment, string op);

        public Task<List<AppointmentsOpLogModel>> GetAppointmentsOpLogViewModels(DateTime fromDate);

    }
}
