using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceFlow.Persistence.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int UserId { get; set; }
    }
}
