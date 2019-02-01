using System.ComponentModel.DataAnnotations;

namespace Sem2_WebApp.Models
{
    public class Review
    {
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(50)]
        public string Month { get; set; }

        [Required]
        [MaxLength(50)]
        public string Year { get; set; }
    }
}