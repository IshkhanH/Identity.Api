using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Models.User
{
    public class UserCreate
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
