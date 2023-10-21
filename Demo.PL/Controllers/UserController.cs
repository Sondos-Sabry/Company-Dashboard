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
using System.Xml.Linq;

namespace Demo.PL.Controllers
{
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly IMapper mapper;
		public UserController(UserManager<ApplicationUser>userManager , SignInManager<ApplicationUser> signInManager , IMapper mapper)
        {
            this.userManager = userManager;
			this.signInManager = signInManager;
			this.mapper = mapper;
        }
		public async Task<IActionResult> Index(string email)
		{
            if (string.IsNullOrEmpty(email))
            {
                var roles = await userManager.Users.Select(U => new UserViewModel()
                {
                    Id = U.Id,
                    FName = U.FName,
                    LName = U.LName,
                    Email = U.Email,
                    PhoneNumber = U.PhoneNumber,
                    Roles = userManager.GetRolesAsync(U).Result
                    }).ToListAsync();
                return View(roles);
            }
            else
            {
                var user = await userManager.FindByNameAsync(email);
                var mappedURole = mapper.Map<ApplicationUser, UserViewModel>(user);

                return View(new List<UserViewModel>() { mappedURole });
            }

            //if (string.IsNullOrEmpty(email))
            //{

            //	var Users = await userManager.Users.Select(U => new UserViewModel()
            //	{
            //		Id= U.Id,
            //		FName = U.FName,
            //		LName = U.LName,
            //		Email = U.Email,
            //		PhoneNumber = U.PhoneNumber,
            //		Roles = userManager.GetRolesAsync(U).Result
            //	}).ToListAsync();				
            //	return View(Users);
            //}
            //else
            //{
            //	var user = await userManager.FindByEmailAsync(email);
            //             //var mappedUser = new UserViewModel()
            //             //{
            //             //	Id = user.Id,
            //             //	FName= user.FName,
            //             //	LName= user.LName,
            //             //	Email = user.Email,
            //             //	PhoneNumber= user.PhoneNumber,
            //             //	Roles = userManager.GetRolesAsync(user).Result

            //             //};
            //             var mappedUser = mapper.Map<ApplicationUser, UserViewModel>(user);

            //             return View(new List<UserViewModel>() { mappedUser });
        
		}

        [HttpGet]
        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();//400
            var user = await userManager.FindByIdAsync(id);

            var mappedUser = mapper.Map<ApplicationUser, UserViewModel>(user);


            if (user == null)
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
        public async Task<IActionResult> Edit([FromRoute] string id, UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
                return BadRequest();
            
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await userManager.FindByIdAsync(id);
                   
                    user.FName=userViewModel.FName;
                    user.LName = userViewModel.LName;
                    user.PhoneNumber=userViewModel.PhoneNumber;
                     
                    await userManager.UpdateAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    /// Log exception
                    /// friendly message
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(userViewModel);
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
        public async Task<IActionResult> Delete([FromRoute] string id , UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
                return BadRequest();

            try
            {
                var user = await userManager.FindByIdAsync(id);

                await userManager.DeleteAsync(user);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                // log exception
                // friendly message

                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(userViewModel);
        }
           
    }
}
