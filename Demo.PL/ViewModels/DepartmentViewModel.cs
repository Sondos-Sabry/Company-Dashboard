using Demo.DAL.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;

namespace Demo.PL.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Code Is Required !!")]
        [MaxLength(50)]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name Is Required !!")]
        public string Name { get; set; }
        [Display(Name = "Date Of Creation")]
        public DateTime DateOfCreation { get; set; }
        //[InverseProperty(nameof(Employee.Department))]
        //navigation Property => (many) 
        public ICollection<Employee> Employees { get; set; } = new HashSet<Employee>(); //to avoid null refrence exception


    }
}
