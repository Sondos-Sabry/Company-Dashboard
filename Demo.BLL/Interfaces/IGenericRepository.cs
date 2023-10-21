using Demo.DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Interfaces
{
    public interface IGenericRepository<T> where T : BaseModel // T is domain model only (special primary constarin)
    {
        //befor return type add "Task" => function is Asynchronous => MultiThreading
        // void => delete it add "Task" only 
        Task<IEnumerable<T>> GetAll();
        Task<T> Get(int id);
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);

    }
}
