using Demo.BLL.Interfaces;
using Demo.DAL.Data;
using Demo.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
    public class DepartmentRepository :GenericRepository<Department> , IDepartmentRepository
    {
        //private readonly AppDbContext dbContext;

        public DepartmentRepository(AppDbContext dbContext) :base( dbContext)
        {
            //dbContext = dbContext;
        }

    }
}
