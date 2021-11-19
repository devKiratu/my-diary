using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Model
{
    public interface IInMemoryDb
    {
        public List<Entry> Entries { get; set; }
    }
}
