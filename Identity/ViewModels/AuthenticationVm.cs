using System.ComponentModel.DataAnnotations;

namespace Identity.ViewModels
{
    public class SignUpVm
    {
        [Required]
        public string UserName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;
     }
     
     public class SignInVm
     {
         [Required]
         public string UserName { get; set; } = null!;
         [Required]
         public string Password { get; set; } = null!;
     }
}