namespace PcAnketProject.CerenUI.Models
{
    public class ResimVeDuzenDto
    {
        public int ResimID { get; set; }
        public string DosyaYolu { get; set; }

        public int? DuzenID { get; set; }
        public string? Baslik { get; set; }
        public string? Rolu { get; set; }
        public int? Genislik { get; set; }
        public int? Yukseklik { get; set; }
    }
}

