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
            this._db = db;
        }

        [HttpPost]
        public IActionResult CreateEntry([FromBody] Entry entry)
        {
            if (!IsValidEntry(entry))
            {
                return BadRequest("Invalid Entry. An Entry should have a title and content");
            }
           var e =  _db.Entries.Add(entry);
            _db.SaveChanges();
            
            return Ok(e.Entity);
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

            return Ok(_db.Entries.ToList());
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEntry([FromRoute] string id, [FromBody] Entry entry)
        {
            var oldEntry = _db.Entries.FirstOrDefault(e => e.Id == id);
            if (oldEntry == null)
            {
                return BadRequest($"Entry of Id {id} does not exist");
            } 
          
            if (!IsValidEntry(entry))
            {
                return BadRequest("Invalid Entry");
            }
            oldEntry.LastUpdated = DateTimeOffset.UtcNow;
            oldEntry.Title = entry.Title;
            oldEntry.Text = entry.Text;
            _db.SaveChanges();


            return Ok("Successfully Updated");

        }

        private static bool IsValidEntry(Entry entry)
        {
            return !string.IsNullOrWhiteSpace(entry.Id) || !string.IsNullOrWhiteSpace(entry.Title) || !string.IsNullOrWhiteSpace(entry.Text);
        }
    }
}
