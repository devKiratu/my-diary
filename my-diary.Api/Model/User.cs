using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Model
{
    public class User
    {
        [Key, Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public ICollection<Entry> Entries { get; set; }
    }
}
