using System.ComponentModel.DataAnnotations;

namespace thread_api_asp.ViewModels
{
    public class TokenVm
    {
        [Required]
        public string? AccessToken { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}