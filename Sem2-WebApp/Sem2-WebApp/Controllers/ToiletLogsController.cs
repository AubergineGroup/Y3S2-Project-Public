using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sem2_WebApp.Models;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToiletLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ToiletLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ToiletLogs
        [HttpGet]
        public IActionResult GetToiletLogs()
        {
            var toiletLogs = _context.ToiletLogs.Select(l => new
            {
                l.ToiletLogId,
                l.ToiletId,
                l.Cleaner.FirstName,
                l.Cleaner.LastName,
                StartDate = l.StartDate.ToLocalTime(),
                EndDate = l.EndDate.ToLocalTime(),
                l.Duration
            });
            return Ok(toiletLogs);
        }

        // GET: api/ToiletLogs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToiletLog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toiletLog = await _context.ToiletLogs.FindAsync(id);

            if (toiletLog == null)
            {
                return NotFound();
            }

            return Ok(toiletLog);
        }

        // POST: api/ToiletLogs
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> PostToiletLog([FromBody] CustomModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toiletLog = new ToiletLog
            {
                ToiletId = model.ToiletId,
                CleanerId = _context.Cleaners.Where(c => c.Rfid == model.Rfid).Select(c => c.CleanerId).FirstOrDefault(),
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Duration = model.Duration
            };
            _context.ToiletLogs.Add(toiletLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetToiletLog", new { id = toiletLog.ToiletLogId }, toiletLog);
        }
    }

    public class CustomModel
    {
        public int ToiletId { get; set; }

        public string Rfid { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Duration { get; set; }
    }
}
