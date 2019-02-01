using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Sem2_WebApp.Models;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class ApiResponse
        {
            public string id { get; set; }
            public string text { get; set; }
            public string start_date { get; set; }
            public string end_date { get; set; }
        }

        [HttpGet]
        public async Task<dynamic> Get()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Request.Headers.TryGetValue("id", out StringValues idValues);
            Request.Headers.TryGetValue("type", out StringValues typeValues);
            int id = int.Parse(idValues.FirstOrDefault());
            string type = typeValues.FirstOrDefault();

            switch (type)
            {
                case "cleaner":
                    var cleanerschedule = await _context.Schedules
                        .Where(t => t.CleanerId == id)
                        .Join(_context.Toilets, c => c.ToiletId, cm => cm.ToiletId, (c, cm) => new {
                            text = cm.ToiletName,
                            id = c.ScheduleId,
                            start_date = c.StartDate,
                            end_date = c.EndDate
                        })
                        .Select(t => t).ToListAsync();
                    return Ok(cleanerschedule);
                case "toilet":
                    var toiletschedule = await _context.Schedules
                        .Where(t => t.ToiletId == id)
                        .Join(_context.Cleaners, c => c.CleanerId, cm => cm.CleanerId, (c, cm) => new {
                            text = cm.FirstName + " " + cm.LastName,
                            id = c.ScheduleId,
                            start_date = c.StartDate,
                            end_date = c.EndDate
                        })
                        .Select(t => t).ToListAsync();
                    return Ok(toiletschedule);
            }
            return null;
        }
        
        [HttpPost]
        public async Task<dynamic> Post()
        {
            StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);

            Request.Headers.TryGetValue("id", out StringValues idValues);
            int cleanerid = int.Parse(idValues.FirstOrDefault());

            string responseString = reader.ReadToEnd();
            var dict = HttpUtility.ParseQueryString(responseString);
            ApiResponse respObj = JsonConvert.DeserializeObject<ApiResponse>(JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v])));

            var schedule = new Schedule
            {
                CleanerId = cleanerid,
                ToiletId = 1,
                StartDate = DateTime.Parse(respObj.start_date),
                EndDate = DateTime.Parse(respObj.end_date)
            };
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                action = "inserted",
                tid = schedule.ScheduleId
            });
        }

        [HttpPut("{id}")]
        public async Task<dynamic> Put(int id)
        {
            StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8);

            Request.Headers.TryGetValue("id", out StringValues idValues);
            int cleanerid = int.Parse(idValues.FirstOrDefault());

            string responseString = reader.ReadToEnd();
            var dict = HttpUtility.ParseQueryString(responseString);
            ApiResponse respObj = JsonConvert.DeserializeObject<ApiResponse>(JsonConvert.SerializeObject(dict.Cast<string>().ToDictionary(k => k, v => dict[v])));
            
            Schedule schedule = _context.Schedules.SingleOrDefault(co => co.ScheduleId == id);
            if (schedule != null)
            {
                schedule.StartDate = DateTime.Parse(respObj.start_date);
                schedule.EndDate = DateTime.Parse(respObj.end_date);
                _context.SaveChanges();
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                action = "updated",
                tid = schedule.ScheduleId
            });
        }
        
        [HttpDelete("{id}")]
        public dynamic Delete(int id)
        {
            Schedule schedule = new Schedule() { ScheduleId = id };
            _context.Schedules.Attach(schedule);
            _context.Schedules.Remove(schedule);
            _context.SaveChanges();

            return Ok(new
            {
                action = "deleted"
            });
        }
    }
}
