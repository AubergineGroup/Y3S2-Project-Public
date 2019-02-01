using System;
using System.ComponentModel.DataAnnotations;

namespace Sem2_WebApp.Models
{
    public class ToiletSettings
    {
        public int ToiletSettingsId { get; set; }

        public int ToiletId { get; set; }

        public int UpdateFrequency { get; set; }

        [Required]
        [MaxLength(4)]
        public string FanMode { get; set; }

        public int FanThreshold { get; set; }

        public int UserThreshold { get; set; }

        public int GasValueThreshold { get; set; }

        public int RequestThreshold { get; set; }

        public Toilet Toilet { get; set; }
    }

    public class ToiletSettingsModel
    {
        public int ToiletSettingsId { get; set; }

        public int ToiletId { get; set; }

        public int UpdateFrequency { get; set; }
        
        public string FanMode { get; set; }

        public int FanThreshold { get; set; }

        public int UserThreshold { get; set; }

        public int GasValueThreshold { get; set; }

        public int RequestThreshold { get; set; }
    }
}
