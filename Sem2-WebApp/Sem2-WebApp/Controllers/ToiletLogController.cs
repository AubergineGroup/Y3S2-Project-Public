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
    public class ToiletLogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ToiletLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ToiletLog
        [HttpGet]
        public IActionResult GetToiletLogs()
        {
            var result = (from t in _context.ToiletLogs
                          join s in _context.Cleaners on t.CleanerId equals s.CleanerId
                          join d in _context.Toilets on t.ToiletId equals d.ToiletId
                          select new { t,s,d });



            return Ok(result);

        }

        // GET: api/ToiletLog/5
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

        // PUT: api/ToiletLog/5
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

        // POST: api/ToiletLog
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

        // DELETE: api/ToiletLog/5
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
    }
}