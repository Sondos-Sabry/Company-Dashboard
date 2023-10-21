using AutoMapper;
using Demo.BLL;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
	[Authorize]
	public class EmployeeController : Controller
    {
        //private readonly EmployeeRepository employeeRepository;        
        //private readonly IDepartmentRepository departmentRepository;
        private readonly IUnitOfWork uniteOfWork;
        private readonly IMapper mapper;
        
        public EmployeeController (IUnitOfWork unitOfWork , IMapper mapper)
        {
            ///employeeRepository must not be allow null
            //this.employeeRepository = (EmployeeRepository)employeeRepository;
            //this.departmentRepository = departmentRepository;
            this.uniteOfWork = unitOfWork;
            this.mapper = mapper;
            
        }
        public async Task <IActionResult> Index( string searchInp)
        {
            ///Binding:
            ///HTTPGet => return data from action to view
            ///HTTPPost => return data from view to action(model) <summary>
            /// 
            /// Binding through view's dictionary :return data from action to view
            /// 1 viewData => Dictionry type property ..inherit from controller class not in controllerBase becouse there is not view in api
            ///        ViewData["Message"] = "Hello Employee ";
            /// 2 viewBag  => dynamic type property(object)
            ///        ViewBag.Message = "Hello Employee ";
            ///3 TempData => Dictionry type property ..inherit from controller class ...acttion create ....is used to pass data between two consecutive request
            ///ViewData is faster than ViewBag => we know type of message in compilation time
        
            var employees = Enumerable.Empty<Employee>();

            if (string.IsNullOrEmpty(searchInp))

                employees = await uniteOfWork.EmployeeRepository.GetAll();
            else
                 employees = uniteOfWork.EmployeeRepository.SearchByName(searchInp.ToLower());

            var mappedEmployee = mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
           
            return View(mappedEmployee);
        }
        [HttpGet]
        public IActionResult Create()
        {
            //ViewData["Departments"] = departmentRepository.GetAll();
            //ViewBag.Departments = departmentRepository.GetAll();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel employeeViewModel)
        {
            if (ModelState.IsValid) //server side validation
            {
                ///manual mapping
                ///var mappedEmployee = new Employee()
                ///    Name = employeeViewModel.Name,
                ///    Age = employeeViewModel.Age,
                ///    Address = employeeViewModel.Address,
                ///    Salary = employeeViewModel.Salary,
                ///    Email = employeeViewModel.Email,
                ///    PhoneNumber = employeeViewModel.PhoneNumber,
                ///    IsActive = employeeViewModel.IsActive,
                ///    HiringDate = employeeViewModel.HiringDate
                ///};

              //  employeeViewModel.ImageName = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");
                var mappedEmployee = mapper.Map<EmployeeViewModel, Employee>(employeeViewModel);
                mappedEmployee.ImageName = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");

              await uniteOfWork.EmployeeRepository.Add(mappedEmployee);
                
                int count = await uniteOfWork.Complete();

                if (count > 0) 
                    TempData["Message"] = "Employee Is Created Successfully :)";
               
                else
                    TempData["Message"] = "An Error Has Occured , Employee Not Created :(";
         
                return RedirectToAction (nameof(Index));
                
            }
            return View(employeeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();//400
            var Employee = await uniteOfWork.EmployeeRepository.Get(id.Value);

            var mappedEmployee = mapper.Map<Employee, EmployeeViewModel>(Employee);


            if (Employee == null)
                return NotFound(); //404

            return View(viewName, mappedEmployee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int? id )
        {
            //ViewBag.Departments = departmentRepository.GetAll();

            return await Details(id, "Edit");
            ///if (id is null)
            ///    return BadRequest();//400
            ///var Employee = employeeRepository.Get(id.Value);
            ///if (Employee is null)
            ///   return NotFound(); //404
            ///return View(Employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //edit only from your website 
       // FromRoute..to avoid update value of id
        public async Task<IActionResult> Edit([FromRoute] int id, EmployeeViewModel employeeViewModel)
        {
            if (id != employeeViewModel.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = await uniteOfWork.EmployeeRepository.Get(employeeViewModel.Id);

                    if (employeeViewModel.Image != null)
                    {
                        // Delete the old photo only if a new one is provided
                        if (!string.IsNullOrEmpty(employee.ImageName))
                        {
                            DocumentSettings.DeleteFile(employee.ImageName, "UserPhotos");
                        }
                        // Upload the new photo
                        employeeViewModel.ImageName = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");
                    }
                    else
                    {
                        // If no new image is provided, keep the existing one
                        employeeViewModel.ImageName = employee.ImageName;
                    }                  
                    uniteOfWork.EmployeeRepository.DetachEnitity(employee);
                    var mappedEmployee = mapper.Map<EmployeeViewModel, Employee>(employeeViewModel);
                    uniteOfWork.EmployeeRepository.Update(mappedEmployee);
                    await uniteOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    /// Log exception
                    /// friendly message
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(employeeViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null)
                return BadRequest();

            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //edit only from your website 
        //FromRoute ..to avoid update value of id 
        public async Task<IActionResult> Delete([FromRoute] int id, EmployeeViewModel employeeViewModel)
        {
            if (id != employeeViewModel.Id)
                return BadRequest();

            if (ModelState.IsValid) //server side validation
            {

                try
                {          
                    var mappedEmployee = mapper.Map<EmployeeViewModel, Employee>(employeeViewModel);
                   
                    if(mappedEmployee.ImageName != null)
                    {
                        DocumentSettings.DeleteFile(employeeViewModel.ImageName, "Images");
                    }
                                      
                        uniteOfWork.EmployeeRepository.Delete(mappedEmployee);
                         var count = await uniteOfWork.Complete();
                        //if (count > 0) 
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    // log exception
                    // friendly message

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(employeeViewModel);
        }

    }
}
