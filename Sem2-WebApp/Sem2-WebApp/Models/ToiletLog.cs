using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sem2_WebApp.Models
{
    public class ToiletLog
    {
        public int ToiletLogId { get; set; }

        public int ToiletId { get; set; }

        public int CleanerId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime EndDate { get; set; }

        public int Duration { get; set; }

        public Toilet Toilet { get; set; }

        public Cleaner Cleaner { get; set; }
    }
}
