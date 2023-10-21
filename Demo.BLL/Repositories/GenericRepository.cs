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
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        private protected readonly AppDbContext dbContext;
        public GenericRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Add(T entity)        
            => await dbContext.Set<T>().AddAsync(entity);

        //No diffrence between code in Delete and Update in Asynchronous or synchronous => مش مستهلة 
        public void Delete(T entity)
            => dbContext.Set<T>().Remove(entity);

        public void Update(T entity)
           => dbContext.Set<T>().Update(entity);

        public async Task<T> Get(int id)
           => await dbContext.Set<T>().FindAsync(id);


        public async Task<IEnumerable<T>> GetAll()
        {
            if (typeof(T) == typeof(Employee))
                return (IEnumerable<T>)await dbContext.Employees.Include(E => E.Department).AsNoTracking().ToListAsync();
            else
                return await dbContext.Set<T>().AsNoTracking().ToListAsync();
        }
    }
}
