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
        public async Task<List<AppointmentModel>> CheckAndAddConflict(AppointmentModel appointment)
        {
            List<AppointmentModel> foundDoctorConflicts = new List<AppointmentModel>();
            //conflit found if two appointments with the same doctor and their date ranges intersect
            foundDoctorConflicts = InMemoryDatabase.Appointments.FindAll(
                ap =>   appointment.DoctorId == ap.DoctorId && appointment.AppointmentID != ap.AppointmentID &&
                        (
                            (appointment.FromDate >= ap.FromDate && appointment.FromDate <= appointment.ToDate) ||
                            (appointment.ToDate >= ap.FromDate && appointment.ToDate <= appointment.ToDate)
                        )).ToList();
            if (foundDoctorConflicts.Count!=0)
            {
                foreach (AppointmentModel app in foundDoctorConflicts)
                {
                    InMemoryDatabase.AppointmentsConflicts.Add(new AppointmentsConflictModel()
                    {
                        ConflictID = Guid.NewGuid().ToString(),
                        Appointment1ID = appointment.AppointmentID,
                        Appointment2ID = app.AppointmentID,
                        ConflictDateTime = DateTime.Now,
                    });
                }
            }

            List<AppointmentModel> foundPatientConflicts = new List<AppointmentModel>();
            foundPatientConflicts = new List<AppointmentModel>();
            //conflit found if two appointments for the same patiend and their date ranges intersect
            foundPatientConflicts = InMemoryDatabase.Appointments.FindAll(
                ap => appointment.PatientId == ap.PatientId && appointment.AppointmentID != ap.AppointmentID &&
                        (
                            (appointment.FromDate >= ap.FromDate && appointment.FromDate <= appointment.ToDate) ||
                            (appointment.ToDate >= ap.FromDate && appointment.ToDate <= appointment.ToDate)
                        )).ToList();
            if (foundPatientConflicts.Count!=0)
            {
                foreach (AppointmentModel app in foundPatientConflicts)
                {
                    InMemoryDatabase.AppointmentsConflicts.Add(new AppointmentsConflictModel()
                    {
                        Appointment1ID = appointment.AppointmentID,
                        Appointment2ID = app.AppointmentID
                    });
                }
            }
            foundDoctorConflicts.AddRange(foundPatientConflicts);
            //return null in case conflict is not found
            return await Task.FromResult(foundDoctorConflicts);
        }

        //this is used to remove all conflicts with an appointment when we cancel it
        public async Task<bool> RemoveConflicts(AppointmentModel appointment)
        {
            InMemoryDatabase.AppointmentsConflicts.RemoveAll(ap => ap.Appointment1ID == appointment.AppointmentID || ap.Appointment2ID == appointment.AppointmentID);
            return await Task.FromResult(true);
        }
    }
}
