using CoreApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorApi.Services
{
    public interface IConflictsManager
    {
        public Task<List<AppointmentModel>> CheckAndAddConflict(AppointmentModel appointment);

        Task<bool> RemoveConflicts(AppointmentModel appointment);
    }
}
