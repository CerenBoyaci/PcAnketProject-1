using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcAnketProject.Core.Dto
{
    public class ResimDuzen
    {
        public int ID { get; set; }
        public int ResimID { get; set; }
        public string? Baslik { get; set; }
        public string? Rolu { get; set; }
        public int? Genislik { get; set; }
        public int? Yukseklik { get; set; }

    }
}
