using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.PL.Extensions
{
    public static class ApplicationServicesExtension
    {

        public static void AddApplicationServices (this IServiceCollection services)
        {
            //AddScoped...per request
            //services.AddScoped<IDepartmentRepository, DepartmentRepository>();
           // services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        }


    }
}
