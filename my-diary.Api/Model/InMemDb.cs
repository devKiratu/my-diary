using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Model
{
    public class InMemDb : IInMemoryDb
    {
        public List<Entry> Entries { get; set; } = new List<Entry>
        {
             //new Entry {  Id="zero", LastUpdated=DateTimeOffset.UtcNow, Title="Note zero", Text="Default note"}
        };
    }
}
