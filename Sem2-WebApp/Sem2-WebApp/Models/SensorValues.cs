using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sem2_WebApp.Models
{
    public class SensorValues
    {
        public int SensorValuesId { get; set; }

        public int ToiletId { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime Timestamp { get; set; }

        public int CurrentUsers { get; set; }

        public int TotalUsers { get; set; }

        public int GasValue { get; set; }

        public int Requests { get; set; }

        public Toilet Toilet { get; set; }
    }
}
