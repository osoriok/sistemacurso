using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaCurso.Areas.Users.Models
{
    public class TUsers
    {
        public int id { get; set; }
        public String name { get; set; }
        public String lastname { get; set; }
        public String nid { get; set; }
        public String email { get; set; }
        public String phonenumber { get; set; }
        public String iduser { get; set; }
        public byte[] image { get; set; }

    }
}
