using System.ComponentModel.DataAnnotations;

namespace Sem2_WebApp.Models
{
    public class Subscription
    {
        public string Id { get; set; }

        [Required]
        public string PushEndpoint { get; set; }

        [Required]
        public string PushP256Dh { get; set; }

        [Required]
        public string PushAuth { get; set; }
    }
}
