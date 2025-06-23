
using System.ComponentModel.DataAnnotations;

namespace PcAnketProject.CerenUI.Models
{
    public class KullaniciGirisKontrolRequest
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı zorunludur.")]
        public string username { get; set; }

        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        public string password { get; set; }

        public string? apikey { get; set; }
    }
}
