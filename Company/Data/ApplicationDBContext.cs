using Company.Model;
using Microsoft.EntityFrameworkCore;

namespace Company.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext>options): base(options) {
            
        }
        public DbSet<User>Users { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<CertificateModel> Certificates { get; set; }
  
    }
}
