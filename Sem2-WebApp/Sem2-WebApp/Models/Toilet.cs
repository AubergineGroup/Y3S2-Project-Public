using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sem2_WebApp.Models
{
    public class Toilet
    {
        public int ToiletId { get; set; }

        [Required]
        [MaxLength(36)]
        public string ToiletOwner { get; set; }

        [Required]
        [MaxLength(64)]
        public string ToiletName { get; set; }

        [Required]
        [MaxLength(6)]
        public string ToiletGender { get; set; }

        [Column(TypeName = "smalldatetime")]
        public DateTime? LastCleaned { get; set; }

        [MaxLength(100)]
        public string Comments { get; set; }

        [Required]
        public string SensorId { get; set; }

        public List<ToiletSettings> ToiletsSettings { get; set; }

        public List<SensorValues> SensorsValues { get; set; }

        public List<ToiletLog> ToiletLogs { get; set; }
    }

    public class ToiletModel
    {
        public int ToiletId { get; set; }

        public string ToiletOwner { get; set; }

        public string ToiletName { get; set; }

        public string ToiletGender { get; set; }

        public DateTime LastCleaned { get; set; }

        public string Comments { get; set; }
    }
}
