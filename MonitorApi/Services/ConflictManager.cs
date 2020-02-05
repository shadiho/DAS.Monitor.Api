using CoreApiModels;
using DASInMemoryDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorApi.Services
{
    public class ConflictManager : IConflictsManager
    {
        public async Task<AppointmentModel> CheckAndAddConflict(AppointmentModel appointment)
        {
            AppointmentModel foundAppointment = null;
            //conflit found if two appointments with the same doctor and their date ranges intersect
            foundAppointment = InMemoryDatabase.Appointments.Find(
                ap =>   appointment.DoctorId == ap.DoctorId &&
                        (
                            (appointment.FromDate > ap.FromDate && appointment.FromDate < appointment.ToDate) ||
                            (appointment.ToDate > ap.FromDate && appointment.ToDate < appointment.ToDate)
                        ));
            if (foundAppointment != null)
            {
                InMemoryDatabase.AppointmentsConflicts.Add(new AppointmentsConflictModel()
                {
                    ConflictID = Guid.NewGuid().ToString(),
                    Appointment1ID = appointment.AppointmentID,
                    Appointment2ID = foundAppointment.AppointmentID,
                    ConflictDateTime = DateTime.Now,
                });
                return await Task.FromResult(foundAppointment);
            }

            //conflit found if two appointments for the same patiend and their date ranges intersect
            foundAppointment = InMemoryDatabase.Appointments.Find(
                ap => appointment.PatientId == ap.PatientId &&
                        (
                            (appointment.FromDate > ap.FromDate && appointment.FromDate < appointment.ToDate) ||
                            (appointment.ToDate > ap.FromDate && appointment.ToDate < appointment.ToDate)
                        ));
            if (foundAppointment != null)
            {
                InMemoryDatabase.AppointmentsConflicts.Add(new AppointmentsConflictModel()
                {
                    Appointment1ID = appointment.AppointmentID,
                    Appointment2ID = foundAppointment.AppointmentID
                });
                return await Task.FromResult(foundAppointment);
            }
            //return null in case conflict is not found
            return await Task.FromResult(foundAppointment);
        }

        //this is used to remove all conflicts with an appointment when we cancel it
        public async Task<bool> RemoveConflicts(AppointmentModel appointment)
        {
            InMemoryDatabase.AppointmentsConflicts.RemoveAll(ap => ap.Appointment1ID == appointment.AppointmentID || ap.Appointment2ID == appointment.AppointmentID);
            return await Task.FromResult(true);
        }
    }
}
