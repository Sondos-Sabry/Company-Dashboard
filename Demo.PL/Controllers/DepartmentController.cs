using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        //inheritance : DepartmentController is a Controller
        //composition : DepartmentController has a DepartmentRepository

        //private readonly DepartmentRepository departmentRepository;
        private readonly IUnitOfWork unitOfWork;
        
        private readonly IMapper mapper;

        public DepartmentController(IUnitOfWork unitOfWork  , IMapper mapper)
        {
            //departmentRepository must not be allow null
            //this.departmentRepository = (DepartmentRepository)departmentRepository;

            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

       


        public async Task<IActionResult> Index()
        {
             var departments = Enumerable.Empty<Department>();

             departments = await unitOfWork.DepartmentRepository.GetAll();   
          
             var mappedDepartment = mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(departments);

       
            return View(mappedDepartment); //return model display on view
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel departmentViewModel)
        {
           if(ModelState.IsValid) //server side validation
           {
               var mappedDepartment = mapper.Map<DepartmentViewModel , Department>(departmentViewModel);
               
               await unitOfWork.DepartmentRepository.Add(mappedDepartment);
              
                int count = await unitOfWork.Complete();
             
               if(count > 0)
                    TempData["Message"] = "Department Is Created Successfully :)";
               else
                    TempData["Message"] = "An Error Has Occured , Department Not Created :(";
                
                return RedirectToAction(nameof(Index));
           }
           return View(departmentViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id , string viewName="Details"  )
        {
            if (id is null)
                return BadRequest();//400
            
            var department =await unitOfWork.DepartmentRepository.Get(id.Value);
           
            var mappedDepartment = mapper.Map<Department, DepartmentViewModel>(department);
          
            if(department == null)
                return NotFound(); //404
          
            return View(viewName , mappedDepartment);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            return await Details(id , "Edit");
            ///if (id is null)
            ///    return BadRequest();//400
            ///var department = departmentRepository.Get(id.Value);
            ///if (department is null)
            ///   return NotFound(); //404
            ///return View(department);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //edit only from your website 
        //FromRoute ..to avoid update value of id 
        public async Task<IActionResult> Edit([FromRoute]int id , DepartmentViewModel departmentViewModel)
        {
            if(id != departmentViewModel.Id)
                return BadRequest();

            if (ModelState.IsValid) //server side validation
            {
                var mappedDepartment = mapper.Map<DepartmentViewModel, Department>(departmentViewModel);

                try
                {
                    unitOfWork.DepartmentRepository.Update(mappedDepartment);
                    await unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    // log exception
                    // friendly message

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(departmentViewModel);
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
        public async Task<IActionResult> Delete([FromRoute] int id, DepartmentViewModel departmentViewModel)
        {
            if (id != departmentViewModel.Id)
                return BadRequest();

            if (ModelState.IsValid) //server side validation
            {
                var mappedDepartment = mapper.Map<DepartmentViewModel, Department>(departmentViewModel);

                try
                {
                    unitOfWork.DepartmentRepository.Delete(mappedDepartment);
                    await unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    // log exception
                    // friendly message

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(departmentViewModel);
        }

    }
}
