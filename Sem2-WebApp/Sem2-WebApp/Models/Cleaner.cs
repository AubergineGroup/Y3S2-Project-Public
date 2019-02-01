using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sem2_WebApp.Models
{
    public class Cleaner
    {
        public int CleanerId { get; set; }

        [Required]
        [MaxLength(36)]
        public string CleanerOwner { get; set; }

        [Required]
        [MaxLength(64)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(64)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(254)]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        public string PhoneNumber { get; set; }

        [Required]
        [MaxLength(18)]
        public string Rfid { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public int? CleanedCount { get; set; }

        public List<ToiletLog> ToiletLogs { get; set; }
    }

    public class CleanerModel
    {
        public int CleanerId { get; set; }

        public string CleanerOwner { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Rfid { get; set; }

        public string Status { get; set; }

        public int CleanedCount { get; set; }
    }
}
