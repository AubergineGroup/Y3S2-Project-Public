using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Vibrant.InfluxDB.Client;
using PubnubApi;
using Sem2_WebApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sem2_WebApp.Controllers
{
    public class TempController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TempController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class SensorInfo
        {
            [InfluxTimestamp]
            public DateTime Timestamp { get; set; }
            [InfluxField("mean_GasValue")]
            public double mean_GasValue { get; set; }

            [InfluxField("mean_TotalUsers")]
            public double mean_TotalUsers { get; set; }

            [InfluxField("mean_CurrentUsers")]
            public double mean_CurrentUsers { get; set; }

            [InfluxField("mean_Requests")]
            public double mean_Requests { get; set; }

            [InfluxField("result")]
            public double result { get; set; }
        }
        
        [HttpPost]
        // Returns mean sensor values based on <datefrom>, <dateto> and <interval> inputs
        // <datefrom>, <dateto> : RFC3339 Date Format
        // <interval> : 15m - 15 minutes, 1d - 1 day etc.
        public async Task<dynamic> GetSensorHistory(string tablename, string datefrom, string dateto, string interval)
        {
            var client = new InfluxClient(new Uri("[REDACTED]"), "[REDACTED]", "[REDACTED]");
            var resultSet = await client.ReadAsync<SensorInfo>("SensorValues", String.Format("SELECT mean(*) FROM \"{0}\" WHERE time > '{1}' AND time < '{2}' GROUP BY time({3})", tablename, datefrom, dateto, interval));
            if (resultSet.Results[0].Series.Count == 0)
            {
                return "{}";
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(resultSet.Results[0].Series[0]);
        }

        [HttpPost]
        // Returns informationa about a toilet for the day
        public async Task<dynamic> GetSensorDailyStats(string toiletid, string datefrom, string dateto)
        {
            var client = new InfluxClient(new Uri("[REDACTED]"), "[REDACTED]", "[REDACTED]");

            // Get average users using a toilet
            var mean_users_result = await client.ReadAsync<SensorInfo>("SensorValues", String.Format("SELECT mean(CurrentUsers) AS \"result\" FROM \"{0}\" WHERE time > '{1}' AND time < '{2}'", toiletid, datefrom, dateto));
            
            // Get average gas
            var mean_gas_result = await client.ReadAsync<SensorInfo>("SensorValues", String.Format("SELECT mean(GasValue) AS \"result\" FROM \"{0}\" WHERE time > '{1}' AND time < '{2}'", toiletid, datefrom, dateto));

            // Get max gas
            var max_gas_result = await client.ReadAsync<SensorInfo>("SensorValues", String.Format("SELECT max(GasValue) AS \"result\" FROM \"{0}\" WHERE time > '{1}' AND time < '{2}'", toiletid, datefrom, dateto));

            // Get total requests
            var mean_request_result = await client.ReadAsync<SensorInfo>("SensorValues", String.Format("SELECT mean(Requests) AS \"result\" FROM \"{0}\" WHERE time > '{1}' AND time < '{2}'", toiletid, datefrom, dateto));

            var data = new
            {
                max_gas = max_gas_result.Results[0].Series[0].Rows[0].result,
                mean_gas = mean_gas_result.Results[0].Series[0].Rows[0].result,
                mean_requests = mean_request_result.Results[0].Series[0].Rows[0].result,
                mean_users = mean_users_result.Results[0].Series[0].Rows[0].result
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        [HttpPost]
        // Publishes a <message> to the specified PubNub <channel>
        public dynamic PublishPornHubMessage(string channel = "", string message = "")
        {
            Pubnub pubnub = new Pubnub(new PNConfiguration
            {
                PublishKey = "[REDACTED]",
                SubscribeKey = "[REDACTED]",
                Uuid = "Web_api",
                Secure = true
            });
            pubnub.Publish()
                    .Channel(channel)
                    .Message(message)
                    .Async(new PNPublishResultExt(
                        (result, status) => { }
                    ));
            return null;
        }
        
        [HttpGet]
        // Returns toilet name and information for <toiletid> 
        // TODO: Rename toiletid to toiletname
        public async Task<dynamic> GetSensorInformation(string toiletid = "")
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toilet = await _context.Toilets
                .Where(t => t.ToiletName == toiletid)
                .Select(t => t).ToListAsync();

            return Ok(toilet);
        }

        [HttpGet]
        // Returns sensor thresholds and settings for <toiletid>
        public async Task<dynamic> GetToiletSettings(int toiletid = 0)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toilet = await _context.ToiletsSettings
                .Where(t => t.ToiletId == toiletid)
                .Select(t => t).ToListAsync();

            return Ok(toilet);
        }
        
        [HttpGet]
        // Returns all toilets
        public async Task<dynamic> GetAllToilets()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toilets = await _context.Toilets
                .Select(t => t).ToListAsync();

            return Ok(toilets);
        }

        [HttpGet]
        // Returns all cleaners
        public async Task<dynamic> GetAllCleaners()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cleaners = await _context.Cleaners
                .Select(t => t).ToListAsync();

            return Ok(cleaners);
        }

        [HttpGet]
        // Returns all cleaners
        public async Task<dynamic> GetCleanerByid(int cleanerid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cleaner = await _context.Cleaners
                .Where(t => t.CleanerId == cleanerid)
                .Select(t => t).ToListAsync();

            return Ok(cleaner);
        }

        [HttpGet]
        // Returns all cleaners
        public async Task<dynamic> GetToiletById(int toiletid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toilet = await _context.Toilets
                .Where(t => t.ToiletId == toiletid)
                .Select(t => t).ToListAsync();

            return Ok(toilet);
        }

        [HttpPost]
        // Returns all cleaners
        public async Task<dynamic> GetUpcomingToiletSchedules(int toiletid, bool getAll = false)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (getAll)
            {
                var schedules = await _context.Schedules
                    .Where(t => t.ToiletId == toiletid && t.StartDate > DateTime.Now)
                    .OrderBy(t => t.StartDate)
                    .Join(_context.Cleaners, c => c.CleanerId, cm => cm.CleanerId, (c, cm) => new {
                        cleaner_name = cm.FirstName + " " + cm.LastName,
                        start_date = c.StartDate
                    })
                    .Select(t => t)
                    .Take(5).ToListAsync();
                return Ok(schedules);
            }
            else
            {
                var schedules = await _context.Schedules
                    .Where(t => t.ToiletId == toiletid && t.StartDate > DateTime.Now)
                    .OrderBy(t => t.StartDate)
                    .Join(_context.Cleaners, c => c.CleanerId, cm => cm.CleanerId, (c, cm) => new {
                        cleaner_name = cm.FirstName + " " + cm.LastName,
                        start_date = c.StartDate
                    })
                    .Select(t => t)
                    .ToListAsync();
                return Ok(schedules);
            }
        }
    }
}