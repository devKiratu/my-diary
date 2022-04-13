using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace my_diary.Api.Model
{
    public class SignupDto : LoginDto
    {
        [Required]
        public string UserName { get; set; }

    }
}
