using System.ComponentModel.DataAnnotations;

namespace thread_api_asp.ViewModels
{
    //null-forgiving operator
    //public string Id { get; set; } = ""!; Giá trị mặc định khi khởi tạo sẽ là ""
    public class UserVm
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
    }

    public class UserLoginVm
    {
        [Required]
        public string UserName { get; init; } = null!;
        [Required]
        public string Password { get; init; } = null!;
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