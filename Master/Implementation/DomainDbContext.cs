using System;
using System.Data.Entity;
using Master.Models;

namespace Master.Implementation
{
    public class DomainDbContext:DbContext
    {
        public DbSet<PatientCase> PatientCases { get; set; }
       // public DbSet<ClassModel> ClassModels { get; set; }
 
    }
}
