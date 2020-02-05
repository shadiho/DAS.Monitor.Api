using CoreApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorApi.Services
{
    public interface IConflictsManager
    {
        Task<AppointmentModel> CheckAndAddConflict(AppointmentModel appointment);

        Task<bool> RemoveConflicts(AppointmentModel appointment);
    }
}
