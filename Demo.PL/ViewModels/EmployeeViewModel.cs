using Demo.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.Http;

namespace Demo.PL.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name Is Required !!")]
        [MaxLength(50, ErrorMessage = "Max Length Of Name Must Be 50")]
        [MinLength(5, ErrorMessage = "Min Length Of Name Must Be 5")]
        public string Name { get; set; }

        [Range(22, 30)]
        public int? Age { get; set; }

        [RegularExpression(@"^[0-9]{1,3}-[a-zA-Z]{5,10}-[a-zA-Z]{4,10}-[a-zA-Z]{5,10}$"
                  , ErrorMessage = "Address Must Be Like 123-Street-City-Country")]
        public string Address { get; set; }

        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Hiring Date")]
        public DateTime HiringDate { get; set; }

        public IFormFile Image { get; set; } // uploaded image from user save in server
        public string ImageName { get; set; } // get name of uploaded image and save it in ImageName => 

        //navigation property (one)
        //[InverseProperty(nameof(Models.Department.Employees))]
        public Department Department { get; set; }
        //[ForeignKey("Department")]
        public int? DepartmentId { get; set; } //forgini key

    }
}
