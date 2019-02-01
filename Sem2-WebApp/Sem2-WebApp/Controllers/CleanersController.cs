using DataTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem2_WebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CleanersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CleanersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cleaners
        [HttpGet]
        [HttpPost]
        public IActionResult Cleaners()
        {
            var cleanerOwner = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var dbType = Environment.GetEnvironmentVariable("DBTYPE");
            var dbConnection = Environment.GetEnvironmentVariable("DBCONNECTION");

            using (var db = new Database(dbType, dbConnection))
            {
                var response = new Editor(db, "Cleaners", "CleanerId")
                    .Model<CleanerModel>()
                    .Field(new Field("CleanerOwner")
                        .SetValue(cleanerOwner)
                    )
                    .Field(new Field("FirstName")
                        .Validator(Validation.Required(
                            new ValidationOpts {Message = "First name is required"}
                        ))
                        .Validator(Validation.MaxLen(64,
                            new ValidationOpts {Message = "First name must be 64 characters or less"}
                        ))
                    )
                    .Field(new Field("LastName")
                        .Validator(Validation.Required(
                            new ValidationOpts {Message = "Last name is required"}
                        ))
                        .Validator(Validation.MaxLen(64,
                            new ValidationOpts {Message = "Last name must be 64 characters or less"}
                        ))
                    )
                    .Field(new Field("Email")
                        .Validator(Validation.Required(
                            new ValidationOpts {Message = "Email is required"}
                        ))
                        .Validator(Validation.Email(
                            new ValidationOpts {Message = "Enter a valid email"}
                        ))
                        .Validator(Validation.MaxLen(254,
                            new ValidationOpts {Message = "Email is too long"}
                        ))
                        .Validator(Validation.Unique(
                            new ValidationOpts {Message = "That email is already being used"}
                        ))
                    )
                    .Field(new Field("PhoneNumber")
                        .Validator(Validation.Required(
                            new ValidationOpts {Message = "Phone number is required"}
                        ))
                        .Validator(Validation.MaxLen(32,
                            new ValidationOpts {Message = "Enter a valid number"}
                        ))
                        .Validator(Validation.Unique(
                            new ValidationOpts {Message = "That number is already being used"}
                        ))
                    )
                    .Field(new Field("Rfid")
                        .Validator(Validation.MinMaxLen(12, 12,
                            new ValidationOpts {Message = "RFID must be 12 characters or less"}
                        ))
                        .SetFormatter(Format.IfEmpty("No card registered"))
                    )
                    .Field(new Field("Status"))
                    .TryCatch(false)
                    .Where("CleanerOwner", cleanerOwner)
                    .Process(Request)
                    .Data();

                return Ok(response);
            }
        }

        // GET: api/Cleaners/511b4ec9-b1d7-408c-937e-be71c8233c1b
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAvailableCleaners([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cleaner = await _context.Cleaners
                .Where(c => c.CleanerOwner == id && c.Status == "Available")
                .ToListAsync();

            return Ok(cleaner);
        }

        // PATCH: api/Cleaners/1
        [HttpPatch]
        [Route("[action]/{id}")]
        public async Task<IActionResult> UpdateCleanerStatus([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var cleaner = new Cleaner { CleanerId = id, Status = "Busy" };
                _context.Cleaners.Attach(cleaner);
                _context.Entry(cleaner).Property(c => c.Status).IsModified = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // PATCH: api/Cleaners/UpdateCleanerStatus/070075DCE648
        [HttpPatch]
        [Route("[action]/{rfid}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCleanerStatus([FromRoute] string rfid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int id = _context.Cleaners.Where(c => c.Rfid == rfid).Select(c => c.CleanerId).FirstOrDefault();
                var cleaner = new Cleaner {CleanerId = id, Status = "Busy"};
                _context.Cleaners.Attach(cleaner);
                _context.Entry(cleaner).Property(c => c.Status).IsModified = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // PATCH: api/Cleaners/070075DCE648
        [HttpPatch("{rfid}")]
        [AllowAnonymous]
        public async Task<IActionResult> PatchCleaner([FromRoute] string rfid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var args = _context.Cleaners
                    .Where(c => c.Rfid == rfid)
                    .Select(c => new {c.CleanerId, c.CleanedCount})
                    .FirstOrDefault();
                var cleaner = new Cleaner
                {
                    CleanerId = args.CleanerId,
                    Status = "Available",
                    CleanedCount = args.CleanedCount + 1
                };
                _context.Cleaners.Attach(cleaner);
                _context.Entry(cleaner).Property(c => c.Status).IsModified = true;
                _context.Entry(cleaner).Property(c => c.CleanedCount).IsModified = true;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }
}