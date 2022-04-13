using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_diary.Api.Authorization;
using my_diary.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly MainDbContext _db;

        public EntriesController(MainDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpPost]
        public IActionResult CreateEntry([FromBody] EntryDto entryDto)
        {
            if (!IsValidEntry(entryDto))
            {
                return BadRequest("Invalid Entry. An Entry should have a title and content");
            }

            var userId = HttpContext.Items["User"].ToString();

            var entry = new Entry
            {
                Id = Guid.NewGuid().ToString(),
                Title = entryDto.Title,
                Text = entryDto.Text,
                Created = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow
            };

            _db.Users.Where(u => u.Id == userId).First().Entries.Add(entry);
            _db.SaveChanges();
            
            return Ok(entry);
        }

        [HttpGet("{Id}")]
        public IActionResult GetOne([FromRoute] string Id)
        {
            var userId = HttpContext.Items["User"].ToString();
            var user = _db.Users.Where(u => u.Id == userId).First();

            var entry = user.Entries.FirstOrDefault(e => e.Id == Id);
            if (entry == null)
            {
                return BadRequest($"Entry of Id {Id} does not exist");
            }

            return Ok(entry);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = HttpContext.Items["User"].ToString();
            var user = _db.Users.Where(u => u.Id == userId).First();

            if (!user.Entries.Any())
            {
                return NotFound("You do not have entries yet. ");
            }

            var entries = user.Entries.ToList();

            return Ok(entries);
        }

        [HttpPut("{Id}")]
        public IActionResult UpdateEntry([FromRoute] string Id, [FromBody] EntryDto entryDto)
        {
            if (!IsValidEntry(entryDto))
            {
                return BadRequest("Invalid Entry. An Entry should have a title and content");
            }

            var userId = HttpContext.Items["User"].ToString();
            var user = _db.Users.Where(u => u.Id == userId).First();

            var oldEntry = user.Entries.FirstOrDefault(e => e.Id == Id);
            if (oldEntry == null)
            {
                return BadRequest($"Entry of Id {Id} does not exist");
            } 
          
            oldEntry.LastUpdated = DateTimeOffset.UtcNow;
            oldEntry.Title = entryDto.Title;
            oldEntry.Text = entryDto.Text;
            _db.SaveChanges();

            return Ok("Successfully Updated");

        }

        private static bool IsValidEntry(EntryDto entryDto)
        {
            return !string.IsNullOrWhiteSpace(entryDto.Title) || !string.IsNullOrWhiteSpace(entryDto.Text);
        }
    }
}
