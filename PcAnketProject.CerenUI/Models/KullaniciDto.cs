namespace PcAnketProject.CerenUI.Models
{
    public class KullaniciDto
    {
        public int ID { get; set; }
        public int NufusID { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Tc { get; set; }
        public string Parola { get; set; }
        public bool Durum { get; set; }
        public DateTime Tarih { get; set; }
        public int Yetki { get; set; }
    }
}
