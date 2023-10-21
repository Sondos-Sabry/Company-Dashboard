using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.Settings;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class AuthController : Controller
    {
        // Create object from "UserManager" depend on create object from "IUserStore" => Allow DI from IUserStore to UserStore
        // To add user => i want to call UserManager
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IMailSettings mailSettings;
		private readonly ISmsMessage smsMessage;

		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager , IMailSettings mailSettings , ISmsMessage smsMessage)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mailSettings = mailSettings;
			this.smsMessage = smsMessage;
		}

        #region Sign Up
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {

            // ensure all data is true
            // mapped from viewmodel to model
            if (ModelState.IsValid) //server side validation
            {
                var user = await userManager.FindByEmailAsync(signUpViewModel.UserName);
                if (user is null)
                {
                    user = new ApplicationUser()
                    {
                        UserName = signUpViewModel.Email.Split('@')[0], //sondossabry@gamil.com => sondossabry is user name 
                        Email = signUpViewModel.Email,
                        IsAgree = signUpViewModel.IsAgree,
                        FName = signUpViewModel.FName,
                        LName = signUpViewModel.LName,
                    };

                    var result = await userManager.CreateAsync(user, signUpViewModel.Password);//add user in store (Repository) with password
                    if (result.Succeeded)
                        return RedirectToAction(nameof(SignIn));

                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);

                }

                ModelState.AddModelError(string.Empty, "UserName is aleady exist!");
            }
            return View(signUpViewModel);
        }

        #endregion

        #region  SignIn
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        // G enerate token 
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(signInViewModel.Email);
                if (user is not null)
                {
                    var flag = await userManager.CheckPasswordAsync(user, signInViewModel.Password);
                    if (flag)
                    {
                        //token in cookie storage
                        var result = await signInManager.PasswordSignInAsync(user, signInViewModel.Password, signInViewModel.RememberMe, false);
                        if (result.Succeeded)
                            return RedirectToAction(nameof(HomeController.Index), "Home");
                    }

                }
                ModelState.AddModelError(string.Empty, "Invalid Login");
            }
            return View(signInViewModel);
        }

//login by google
		public IActionResult GoogleLogin()
		{
            var property = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponce")
            };

			return Challenge(property , GoogleDefaults.AuthenticationScheme);
		}
		//login on google AuthenticationScheme
		public async Task<IActionResult> GoogleResponce()
		{
        
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });


            return RedirectToAction("Index" , "Home");
          }

		#endregion

		#region SignOut
		//remove token 
		public async new Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        #endregion

        #region ForgetPassword

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            var user = await userManager.FindByEmailAsync(forgetPasswordViewModel.Email);

            if (ModelState.IsValid)
            {
                // Generate URL =>https//localhost:44340/Auth/ResetPassword?email=sondos@gmail.com&token...
                var token = userManager.GeneratePasswordResetTokenAsync(user);// token=> Unique for this user for one time 


                if (user is not null)
                {
                    var ResetPasswordUrl = Url.Action("ResetPassword", "Auth", new { email = forgetPasswordViewModel.Email, token = token });
                        var email = new Email()
                        {
                            Subject = "Reset Your Password",
                            To = forgetPasswordViewModel.Email,
                            Body = ResetPasswordUrl

                        };

                        mailSettings.SendEmail(email);
                        return RedirectToAction(nameof(CheckYourInbox));
                    }
                
                ModelState.AddModelError(string.Empty, "Invalid Email");
            }

            return View(forgetPasswordViewModel);
        }


        //using  sms
		[HttpPost]
		public async Task<IActionResult> SendSms(ForgetPasswordViewModel forgetPasswordViewModel)
		{
			var user = await userManager.FindByEmailAsync(forgetPasswordViewModel.Email);

			if (ModelState.IsValid)
			{
				// Generate URL =>https//localhost:44340/Auth/ResetPassword?email=sondos@gmail.com&token...
				var token = userManager.GeneratePasswordResetTokenAsync(user);// token=> Unique for this user for one time 


				if (user is not null)
				{
					var ResetPasswordUrl = Url.Action("ResetPassword", "Auth", new { email = forgetPasswordViewModel.Email, token = token });

                    var sms = new SmsMessage()
                    {
                        PhoneNumber = user.PhoneNumber,
                        Body = ResetPasswordUrl
					};
					
                    smsMessage.Send(sms);
                    return Ok("Check Your Phone :)");

					return RedirectToAction(nameof(CheckYourInbox));
				}
				ModelState.AddModelError(string.Empty, "Invalid Email");
			}

			return View(forgetPasswordViewModel);
		}



		public IActionResult CheckYourInbox()
        {
            return View();
        }

        #endregion

        #region ResetPassword

        public IActionResult ResetPassword( string email ,string token )
        {
            TempData["email"] = email;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
		public async Task<IActionResult> ResetPassword( ResetPasswordViewModel resetPasswordViewModel)
		{
            if (ModelState.IsValid)
            {
                string email = TempData["email"] as string;
                string token = TempData["token"] as string;

                var user = await userManager.FindByEmailAsync(email);
                var result = await userManager.ResetPasswordAsync( user ,token , resetPasswordViewModel.NewPassword );

                if (result.Succeeded)
                    return RedirectToAction(nameof(SignIn));

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
           
			return View(resetPasswordViewModel);
		}

		#endregion
	}
}
