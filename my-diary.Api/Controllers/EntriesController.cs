using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using my_diary.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly IInMemoryDb db;

        public EntriesController(IInMemoryDb db)
        {
            this.db = db;
        }

        [HttpPost]
        public ActionResult<Entry> CreateEntry([FromBody] Entry entry)
        {
            db.Entries.Add(entry);
            return entry;
        }

        [HttpGet("{id}")]
        public ActionResult<Entry> GetOne([FromRoute] string Id)
        {
            var entry = db.Entries.FirstOrDefault(e => e.Id == Id);
            if (entry == null)
            {
                return BadRequest($"Entry of Id {Id} does not exist");
            }

            return entry;
        }

        [HttpGet]
        public ActionResult<List<Entry>> GetAll()
        {
            if (db.Entries.Count == 0)
            {
                return NotFound("No content exists yet");
            }

            return db.Entries;
        }

        [HttpPut("{id}")]
        public ActionResult UpdateEntry([FromRoute] string id, [FromBody] Entry entry)
        {
            var oldEntry = db.Entries.FirstOrDefault(e => e.Id == id);
            if (entry == null)
            {
                return BadRequest($"Entry of Id {id} does not exist");
            }

            db.Entries.Remove(oldEntry);
            db.Entries.Add(entry);

            return NoContent();
        }
    }
}
