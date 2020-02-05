using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiModels;

namespace MonitorApi.Services
{
    public interface IAppointmentStorageService
    {
        Task<bool> Book(AppointmentModel model);
        Task<bool> Cancel(AppointmentModel model);

    }
}
