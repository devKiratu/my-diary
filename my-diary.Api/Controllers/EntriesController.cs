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
        private readonly IInMemoryDb db;

        public EntriesController(IInMemoryDb db)
        {
            this.db = db;
        }

        [HttpPost]
        public IActionResult CreateEntry([FromBody] Entry entry)
        {
            if (!IsValidEntry(entry))
            {
                return BadRequest("Invalid Entry. An Entry should have a title and content");
            }
            db.Entries.Add(entry);
            return Ok(entry);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute] string Id)
        {
            var entry = db.Entries.FirstOrDefault(e => e.Id == Id);
            if (entry == null)
            {
                return BadRequest($"Entry of Id {Id} does not exist");
            }

            return Ok(entry);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (db.Entries.Count == 0)
            {
                return NotFound("No content exists yet");
            }

            return Ok(db.Entries);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEntry([FromRoute] string id, [FromBody] Entry entry)
        {
            var oldEntry = db.Entries.FirstOrDefault(e => e.Id == id);
            if (oldEntry == null)
            {
                return BadRequest($"Entry of Id {id} does not exist");
            } 
          
            if (!IsValidEntry(entry))
            {
                return BadRequest("Invalid Entry");
            }
            entry.LastUpdated = DateTimeOffset.UtcNow;
            db.Entries.Remove(oldEntry);
            db.Entries.Add(entry);

            return Ok("Successfully Updated");

        }

        private bool IsValidEntry(Entry entry)
        {
            return !string.IsNullOrWhiteSpace(entry.Id) || !string.IsNullOrWhiteSpace(entry.Title) || !string.IsNullOrWhiteSpace(entry.Text);
        }
    }
}
