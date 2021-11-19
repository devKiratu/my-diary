using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Model
{
    public class Entry
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string Text { get; set; }
    }
}
