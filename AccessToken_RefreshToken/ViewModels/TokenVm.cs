using System.ComponentModel.DataAnnotations;

namespace thread_api_asp.ViewModels
{
    public class TokenVm
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
    
    public class TokenLoginVm
    {
        [Required]
        public string? AccessToken { get; set; }
        [Required]
        public string? RefreshToken { get; set; }
    }
}