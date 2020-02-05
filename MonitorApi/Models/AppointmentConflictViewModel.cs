using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiModels;
namespace MonitorApi.Models
{
    public class AppointmentConflictViewModel
    {
        public AppointmentsConflictModel AppointmentsConflictModel { get; set; }

        public AppointmentModel Appointment1 { get; set; }

        public DoctorModel Doctor1 { get; set; }

        public PatientModel Patient1 { get; set; }

        public AppointmentModel Appointment2 { get; set; }

        public DoctorModel Doctor2 { get; set; }

        public PatientModel Patient2 { get; set; }


    }
}
