using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sem2_WebApp.Models;
using WebPush;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebPushController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly ApplicationDbContext _context;

        public WebPushController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task Send([FromBody] CustomModel data)
        {
            var subscription = await _context.Subscriptions.SingleOrDefaultAsync(s => s.Id == data.Id);

            string vapidPublicKey = _configuration.GetSection("VapidKeys")["PublicKey"];
            string vapidPrivateKey = _configuration.GetSection("VapidKeys")["PrivateKey"];

            var pushSubscription =
                new PushSubscription(subscription.PushEndpoint, subscription.PushP256Dh, subscription.PushAuth);
            var vapidDetails = new VapidDetails("mailto:admin@aubergine.site", vapidPublicKey, vapidPrivateKey);

            var webPushClient = new WebPushClient();
            var numberOfThresholds = data.Thresholds.Count;
            var payload =
                $"{data.ToiletName}'s {GetThresholdsResponse(numberOfThresholds, data.Thresholds)} {(numberOfThresholds > 1 ? "thresholds" : "threshold")} {(numberOfThresholds > 1 ? "have" : "has")} been exceeded.";
            webPushClient.SendNotification(pushSubscription, payload, vapidDetails);
        }

        private static string GetThresholdsResponse(int numberOfThresholds, IReadOnlyList<string> thresholds)
        {
            if (numberOfThresholds > 1)
            {
                var needsOxfordComma = numberOfThresholds > 2;
                var lastThresholdConjunction = (needsOxfordComma ? "," : "") + " and ";
                var lastThreshold = lastThresholdConjunction + thresholds[thresholds.Count - 1];
                return (string.Join(", ", thresholds.SkipLast(1)) + lastThreshold).ToLower();
            }

            return string.Join("", thresholds).ToLower();
        }

        public class CustomModel
        {
            public string Id { get; set; }

            public string ToiletName { get; set; }

            public List<string> Thresholds { get; set; }
        }
    }
}