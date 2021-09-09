using System;
using System.Collections.Generic;

#nullable disable

namespace Web.Domain
{
    public partial class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid StampCode { get; set; }
    }
}
