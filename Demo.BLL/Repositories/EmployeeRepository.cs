using Demo.BLL.Interfaces;
using Demo.DAL.Data;
using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        //private readonly AppDbContext dbContext;

        public EmployeeRepository(AppDbContext dbContext) :base(dbContext)
        {
            //this.dbContext = dbContext;
        }

        //No Asynchronous in case IQueryable => filtering data in database 
        public IQueryable<Employee> GetEmployeesByAddress(string address)
        {
            return dbContext.Employees.Where(e=>e.Address.ToLower().Contains( address.ToLower()));
        }
        public IQueryable<Employee> SearchByName(string name)
        => dbContext.Employees.Where(E => E.Name.ToLower().Contains( name));
        public void DetachEnitity(Employee employee)
        {
            var entry = dbContext.Entry(employee);
            entry.State = EntityState.Detached;
        }
  
    }
}
