using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Model
{
    public class User : SignupDto
    {
        [Key, Required]
        public string Id { get; set; }
        public ICollection<Entry> Entries { get; set; }
    }
}
