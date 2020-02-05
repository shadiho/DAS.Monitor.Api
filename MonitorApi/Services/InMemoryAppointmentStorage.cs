using DASInMemoryDatabase;
using CoreApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorApi.Services
{
    public class InMemoryAppointmentStorage : IAppointmentStorageService
    {
        public async Task<bool> Book(AppointmentModel model)
        {
            //we need to check if the database has this item before since there is a chance in SQS that we receive the message twice
            AppointmentModel itemTobeAdded = InMemoryDatabase.Appointments.SingleOrDefault(m => m.AppointmentID == model.AppointmentID);
            if (itemTobeAdded == null)
            {
                InMemoryDatabase.Appointments.Add(model);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> Cancel(AppointmentModel model)
        {
            AppointmentModel itemToBeRemoved = InMemoryDatabase.Appointments.SingleOrDefault(m => m.AppointmentID == model.AppointmentID);
            if (itemToBeRemoved  == null)
                return await Task.FromResult(false);
            InMemoryDatabase.Appointments.Remove(itemToBeRemoved);
            return await Task.FromResult(true);
        }

    }
}
