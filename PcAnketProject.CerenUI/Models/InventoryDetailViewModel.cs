namespace PcAnketProject.CerenUI.Models
{
    public class InventoryDetailViewModel
    {
        public int NufusId { get; set; }
        public string Tc { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Unvan { get; set; }
        public string Birim { get; set; }
        public string DonanimAd { get; set; }
        public string IslemNo { get; set; }
        public string? Aciklama { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public string? YetkiliAdSoyad { get; set; }
        public string DonanimDurum { get; set; }
        public bool Aktif { get; set; }
    }


}
