using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sem2_WebApp.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        public int CleanerId { get; set; }

        public int ToiletId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EndDate { get; set; }

        public Cleaner Cleaner { get; set; }

        public Toilet Toilet { get;set; }
    }
}
