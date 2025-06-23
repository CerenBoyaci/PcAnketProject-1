namespace PcAnketProject.CerenUI.Models
{
    public class InventoryPersonelViewModel
    {
        public int NufusId { get; set; }
        public string Tc { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Unvan { get; set; }
        public string Birim { get; set; }
        public string Donanimlar { get; set; }
    }

    public class DonanimDurumViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; }
    }
    public class ActiveDonanimViewModel
    {
        public string DonanimAd { get; set; }
        public string DonanimDurum { get; set; }
        public string? IslemNo { get; set; }
    }

    public class DonanimListViewModel
    {
        public int Id { get; set; }
        public string Ad { get; set; }
    }
    public class DonanimDurumUpdateViewModel
    {
        public string DonanimAd { get; set; }
        public string IslemNo { get; set; }
        public int YeniDurumId { get; set; }
    }


}
