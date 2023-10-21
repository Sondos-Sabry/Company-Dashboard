using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL
{
    public class UnitOfWork : IUnitOfWork  
    {

        private readonly AppDbContext dbContext;


        public IEmployeeRepository EmployeeRepository { get ; set ; }
        public IDepartmentRepository DepartmentRepository { get; set; }

        public UnitOfWork(AppDbContext dbContext)
        {
            EmployeeRepository = new EmployeeRepository(dbContext);
            DepartmentRepository = new DepartmentRepository(dbContext);

            this.dbContext = dbContext;
        }

        public async Task<int> Complete()
            => await dbContext.SaveChangesAsync();

        public void Dispose()
            =>  dbContext.Dispose();

      
        
    }
}
