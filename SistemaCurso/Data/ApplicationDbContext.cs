using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaCurso.Areas.Users.Models;
using SistemaCurso.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaCurso.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<TUsers> TUsers { get; set; }

    }
}
