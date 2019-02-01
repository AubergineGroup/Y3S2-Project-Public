using DataTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sem2_WebApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Sem2_WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToiletsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ToiletsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Toilets
        [HttpGet]
        [HttpPost]
        public IActionResult Toilets()
        {
            var toiletOwner = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var dbType = Environment.GetEnvironmentVariable("DBTYPE");
            var dbConnection = Environment.GetEnvironmentVariable("DBCONNECTION");

            using (var db = new Database(dbType, dbConnection))
            {
                var response = new Editor(db, "Toilets", "ToiletId")
                    .Model<ToiletModel>()
                    .Field(new Field("ToiletOwner")
                        .SetValue(toiletOwner)
                    )
                    .Field(new Field("ToiletName")
                        .Validator(Validation.Required(
                            new ValidationOpts {Message = "Toilet name is required"}
                        ))
                        .Validator(Validation.MaxLen(64,
                            new ValidationOpts {Message = "Toilet name must be 64 characters or less"}
                        ))
                        .Validator(Validation.Unique(
                            new ValidationOpts {Message = "A toilet with that name already exists"}
                        ))
                    )
                    .Field(new Field("ToiletGender"))
                    .Field(new Field("Comments")
                        .Validator(Validation.Xss(
                            new ValidationOpts
                                {Message = "Comments can't contain any of the following characters: &, <, >, \", \'"}
                        ))
                        .SetFormatter(Format.NullEmpty())
                    )
                    .TryCatch(false)
                    .Where("ToiletOwner", toiletOwner)
                    .Process(Request)
                    .Data();

                return Ok(response);
            }
        }

        // GET: api/Toilets/511b4ec9-b1d7-408c-937e-be71c8233c1b
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToilets([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toilet = await _context.Toilets
                .Where(t => t.ToiletOwner == id)
                .Select(t => t.ToiletName).ToListAsync();

            return Ok(toilet);
        }

        // POST: api/Toilets/SendSms
        [HttpPost]
        [Route("[action]")]
        public async Task SendSms([FromBody] dynamic data)
        {
            const string accountSid = "[REDACTED]";
            const string authToken = "[REDACTED]";

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
                body: $"You have been assigned to {data.toilet}. Please proceed to the location immediately.",
                from: new Twilio.Types.PhoneNumber("+[REDACTED]"),
                to: new Twilio.Types.PhoneNumber($"+{data.phoneNumber}")
            );
        }

        // PATCH: api/Toilets
        [HttpPatch]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateLastCleaned([FromBody] dynamic toiletObject)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var toilet = new Toilet {ToiletId = toiletObject.id, LastCleaned = toiletObject.lastCleaned};
                _context.Toilets.Attach(toilet);
                _context.Entry(toilet).Property(t => t.LastCleaned).IsModified = true;
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