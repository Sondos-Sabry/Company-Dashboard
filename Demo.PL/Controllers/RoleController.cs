using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        public RoleController( RoleManager<IdentityRole> roleManager ,IMapper mapper)
        {
            this.roleManager = roleManager;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                var roles = await roleManager.Roles.Select(R => new RoleViewModel()
                {
                    Id = R.Id,
                    RoleName = R.Name,
                }).ToListAsync();

                return View(roles);
            }
            else
            {
                var role = await roleManager.FindByNameAsync(name);
                var mappedURole = mapper.Map< IdentityRole, RoleViewModel>(role);

                return View(new List<RoleViewModel>() { mappedURole });
            }

            }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid) 
            {
                var mappedRole = mapper.Map< RoleViewModel , IdentityRole >(roleViewModel);        
                await roleManager.CreateAsync(mappedRole);

                return RedirectToAction(nameof(Index));

            }
            return View(roleViewModel);
        }

        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();//400
            var role = await roleManager.FindByIdAsync(id);

            var mappedUser = mapper.Map<IdentityRole ,RoleViewModel>(role);


            if (role == null)
                return NotFound(); //404

            return View(viewName, mappedUser);
        }

        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            return await Details(id, "Edit");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id,RoleViewModel roleViewModel )
        {
            if (id != roleViewModel.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await roleManager.FindByIdAsync(id);

                    role.Name = roleViewModel.RoleName;
                   
                    await roleManager.UpdateAsync(role);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    /// Log exception
                    /// friendly message
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(roleViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id is null)
                return BadRequest();

            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] string id, RoleViewModel roleViewModel)
        {
            if (id != roleViewModel.Id)
                return BadRequest();

            try
            {
                var user = await roleManager.FindByIdAsync(id);

                await roleManager.DeleteAsync(user);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                // log exception
                // friendly message

                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(roleViewModel);
        }



    }
}
