using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Password Is Required!!")]
        [MinLength(5, ErrorMessage = "Minimum Password Length is 5")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password Is Required!!")]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm Password Does Not Match Password!! ")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
	
	}
}
