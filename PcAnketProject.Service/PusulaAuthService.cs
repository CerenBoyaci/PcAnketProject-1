using PcAnketProject.Data.Repository;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PcAnketProject.Service
{
    public class PusulaAuthService
    {
        private readonly GirisRepository _girisRepository;

        public PusulaAuthService(GirisRepository girisRepository)
        {
            _girisRepository = girisRepository;
        }

        public async Task<(bool IsValid, string NufusID)> KullaniciDogrula(string kullaniciAdi, string parola)
        {
            // Eğer veritabanında şifreler hash'li değilse doğrudan parola kullanılabilir.
            // Ancak eğer MD5 ile saklanıyorsa şu şekilde hash'lenmeli:
            // string hashedPassword = MD5eDonustur(parola);
            // return await _girisRepository.KullaniciDogrula(kullaniciAdi, hashedPassword);

            return await _girisRepository.KullaniciDogrula(kullaniciAdi, parola);
        }

        public async Task<(string NufusID, string? Ad, string? Soyad)> KullaniciBilgi(int nufusId)
        {
            var (isValid, ad, soyad) = await _girisRepository.GetKullaniciBilgileri(nufusId);
            return (nufusId.ToString(), ad, soyad);
        }

        public async Task<(int? Yetki, string AdSoyad)?> GetKullaniciYetki(int nufusId)
        {
            return await _girisRepository.GetKullaniciYetki(nufusId);
        }

        public static string MD5eDonustur(string metin)
        {
            using var md5 = MD5.Create();
            return Sifrele(metin, md5);
        }

        private static string Sifrele(string metin, HashAlgorithm alg)
        {
            byte[] byteDegeri = Encoding.UTF8.GetBytes(metin);
            byte[] sifreliByte = alg.ComputeHash(byteDegeri);
            return Convert.ToBase64String(sifreliByte);
        }

        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
