using DataTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sem2_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToiletSettingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ToiletSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ToiletSettings/5
        [HttpGet("{id}")]
        [HttpPost("{id}")]
        public IActionResult ToiletSettings([FromRoute] int id)
        {
            var dbType = Environment.GetEnvironmentVariable("DBTYPE");
            var dbConnection = Environment.GetEnvironmentVariable("DBCONNECTION");

            using (var db = new Database(dbType, dbConnection))
            {
                var response = new Editor(db, "ToiletsSettings", "ToiletSettingsId")
                    .Model<ToiletSettingsModel>()
                    .Field(new Field("UpdateFrequency")
                        .Validator(Validation.MinMaxNum(1000, 15000,
                            new ValidationOpts {Message = "Update frequency must be between 1000 and 15000"}
                        ))
                    )
                    .Field(new Field("FanMode"))
                    .Field(new Field("FanThreshold")
                        .Validator(Validation.MinMaxNum(40, 100,
                            new ValidationOpts {Message = "Fan threshold must be between 40 and 100"}
                        ))
                    )
                    .Field(new Field("UserThreshold")
                        .Validator(Validation.MinMaxNum(20, 100,
                            new ValidationOpts {Message = "User threshold must be between 40 and 100"}
                        ))
                    )
                    .Field(new Field("GasValueThreshold")
                        .Validator(Validation.MinMaxNum(60, 100,
                            new ValidationOpts {Message = "Gas value threshold must be between 60 and 100"}
                        ))
                    )
                    .Field(new Field("RequestThreshold")
                        .Validator(Validation.MinMaxNum(10, 100,
                            new ValidationOpts {Message = "Request threshold must be between 10 and 100"}
                        ))
                    )
                    .TryCatch(false)
                    .Where("ToiletId", id)
                    .Process(Request)
                    .Data();

                return Ok(response);
            }
        }

        // GET: api/ToiletSettings/GetToiletSettings?sensorId=VolNOOr85gwHpC5o6Me0xGRrKX%2b2VwB2SuVYC%2fOk6Bw%3d=&name=RPi
        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToiletSettings([FromQuery] string sensorId, [FromQuery] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var settings = _context.ToiletsSettings
                .Where(s => s.Toilet.SensorId == HttpUtility.UrlDecode(sensorId))
                .Select(s => new
                {
                    s.ToiletId,
                    s.UpdateFrequency,
                    s.FanMode,
                    s.FanThreshold
                }).FirstOrDefault();

            if (settings != null) return Ok(settings);
            {
                var toilet = new Toilet
                {
                    ToiletOwner = "[REDACTED]",
                    ToiletName = name,
                    SensorId = sensorId
                };
                _context.Toilets.Add(toilet);
                _context.SaveChanges();

                var toiletSettings = new ToiletSettings { ToiletId = toilet.ToiletId };
                _context.ToiletsSettings.Add(toiletSettings);
                await _context.SaveChangesAsync();

                var result = new List<ToiletSettings> { toiletSettings };
                return Ok(result.Select(s => new
                {
                    s.ToiletId,
                    s.UpdateFrequency,
                    s.FanMode,
                    s.FanThreshold
                }).ToList());
            }
        }

        // POST: api/ToiletSettings/PostToiletSettings
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PostToiletSettings([FromBody] int toiletId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toiletSettings = new ToiletSettings { ToiletId = toiletId };
            _context.ToiletsSettings.Add(toiletSettings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("ToiletSettings", new {id = toiletSettings.ToiletId}, toiletSettings);
        }
    }
}