using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_diary.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly MainDbContext _db;

        public EntriesController(MainDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult CreateEntry([FromBody] EntryDto entryDto)
        {
            if (!IsValidEntry(entryDto))
            {
                return BadRequest("Invalid Entry. An Entry should have a title and content");
            }
            var entry = new Entry
            {
                Id = Guid.NewGuid().ToString(),
                Title = entryDto.Title,
                Text = entryDto.Text,
                Created = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow
            };
            _db.Entries.Add(entry);
            _db.SaveChanges();
            
            return Ok(entry);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] string Id)
        {
            var entry = _db.Entries.FirstOrDefault(e => e.Id == Id);
            if (entry == null)
            {
                return BadRequest($"Entry of Id {Id} does not exist");
            }

            return Ok(entry);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (!_db.Entries.Any())
            {
                return NotFound("No content exists yet");
            }

            var entries = _db.Entries.ToList();

            return Ok(entries);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEntry([FromRoute] string id, [FromBody] EntryDto entryDto)
        {
            if (!IsValidEntry(entryDto))
            {
                return BadRequest("Invalid Entry. An Entry should have a title and content");
            }

            var oldEntry = _db.Entries.FirstOrDefault(e => e.Id == id);
            if (oldEntry == null)
            {
                return BadRequest($"Entry of Id {id} does not exist");
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
