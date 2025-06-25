using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcAnketProject.Core.Dto
{
    public class Resim
    {
        //veri tabanında tutulmasını istediğimiz şeyler
        public int ID { get; set; }
        public string DosyaAdi { get; set; }
        public string DosyaYolu { get; set; }
        public DateTime YuklenmeTarihi { get; set; }
    }
}
