using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem2_WebApp.Models;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/History
        [HttpGet]
        public IActionResult GetToiletLogs()
        {

            var result = (from t in _context.ToiletLogs
                          join s in _context.Cleaners on t.CleanerId equals s.CleanerId
                          join d in _context.Toilets on t.ToiletId equals d.ToiletId
                          select new
                          {
                              ToiletLogId = t.ToiletLogId,
                              ToiletId = t.ToiletId,
                              CleanerId = t.CleanerId,
                              Name = s.FirstName + " " + s.LastName,
                              StartDate = t.StartDate,
                              EndDate = t.EndDate,
                              Duration = t.Duration,
                              Toilet = d.ToiletName
                          });

            var data = result.OrderByDescending(l => l.EndDate).Take(5);     

            return Ok(data);
        }

        // GET: api/History/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToiletLog([FromRoute] DateTime id)
        {

            var result = (from t in _context.ToiletLogs
                          join s in _context.Cleaners on t.CleanerId equals s.CleanerId
                          join d in _context.Toilets on t.ToiletId equals d.ToiletId
                          select new
                          {
                              ToiletLogId = t.ToiletLogId,
                              ToiletId = t.ToiletId,
                              CleanerId = t.CleanerId,
                              Name = s.FirstName + " " + s.LastName,
                              StartDate = t.StartDate,
                              EndDate = t.EndDate,
                              Duration = t.Duration,
                              Toilet = d.ToiletName
                          });

            var data = result.Where(p => p.StartDate.Date >= id && p.EndDate.Date <= id).OrderByDescending(l => l.EndDate).Take(5);

            return Ok(data);
        }

        // PUT: api/History/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToiletLog([FromRoute] int id, [FromBody] ToiletLog toiletLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != toiletLog.ToiletLogId)
            {
                return BadRequest();
            }

            _context.Entry(toiletLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToiletLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/History
        [HttpPost]
        public async Task<IActionResult> PostToiletLog([FromBody] ToiletLog toiletLog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ToiletLogs.Add(toiletLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetToiletLog", new { id = toiletLog.ToiletLogId }, toiletLog);
        }

        // DELETE: api/History/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToiletLog([FromRoute] int id)
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

            _context.ToiletLogs.Remove(toiletLog);
            await _context.SaveChangesAsync();

            return Ok(toiletLog);
        }

        private bool ToiletLogExists(int id)
        {
            return _context.ToiletLogs.Any(e => e.ToiletLogId == id);
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
}