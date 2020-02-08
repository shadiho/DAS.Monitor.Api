using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiModels;
namespace MonitorApi.Models
{
    public class AppointmentConflictViewModel
    {
        public string Appointment1ID { get; set; }
        public DateTime Appointment1CreationDateTime { get; set; }

        public string Appointment1DoctorID { get; set; }

        public string Appointment1PatientID { get; set; }

        public string Appointment1DoctorName { get; set; }

        public string Appointment1PatientName { get; set; }

        public DateTime Appointment1FromDate { get; set; }

        public DateTime Appointment1ToDate { get; set; }

        public string Appointment2ID { get; set; }
        public DateTime Appointment2CreationDateTime { get; set; }

        public string Appointment2DoctorID { get; set; }

        public string Appointment2PatientID { get; set; }

        public string Appointment2DoctorName { get; set; }

        public string Appointment2PatientName { get; set; }

        public DateTime Appointment2FromDate { get; set; }

        public DateTime Appointment2ToDate { get; set; }

      


    }
}
