namespace PcAnketProject.CerenUI.Models
{
    public class KullaniciYetkiResponse
    {
        public bool success { get; set; }  // API isteğinin başarılı olup olmadığını gösterir
        public int nufusID { get; set; }   // Kullanıcının Nüfus ID'si
        public int yetki { get; set; }     // Kullanıcının yetki seviyesi (0: Öğrenci, 1: Komisyon, 2: Yönetici)
        public string? Message { get; set; } // Hata mesajı veya bilgilendirme metni (isteğe bağlı)
    }

}
