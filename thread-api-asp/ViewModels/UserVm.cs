using System.ComponentModel.DataAnnotations;

namespace thread_api_asp.ViewModels
{
    //null-forgiving operator
    //public string Id { get; set; } = ""!; Giá trị mặc định khi khởi tạo sẽ là ""
    public class UserVm
    {
        public string? Id { get; set; } 
        public string? Username { get; set; } 
    }

    public class UserLoginVm
    {
        [Required]
        public string? UserName { get; init; }
        [Required]
        public string? Password { get; init; }
    }

    public class UserInsertVm
    {
        [Required]
        public required string UserName { get; init; }
        [Required]
        public required string Password { get; init; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu không khớp")]
        public string? ConfirmPassword { get; init; }
    }
}