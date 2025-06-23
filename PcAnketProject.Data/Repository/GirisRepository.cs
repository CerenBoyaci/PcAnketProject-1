using PcAnketProject.Data.Context;
using System.Threading.Tasks;
using Dapper;

namespace PcAnketProject.Data.Repository
{
    public class GirisRepository
    {
        private readonly DbContext _dbContext;

        public GirisRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool IsValid, string? KullaniciID)> KullaniciDogrula(string username, string password)
        {
            var query = @"
                SELECT TOP 1 NufusID 
                FROM dt_kullanici 
                WHERE Ad = @Username AND Parola = @Password AND Durum = 1";

            using var connection = _dbContext.CreateConnection();

            var nufusId = await connection.QueryFirstOrDefaultAsync<int?>(query, new
            {
                Username = username,
                Password = password // Eğer şifre hash'liyse burada hashlenmiş haliyle karşılaştırılmalı
            });

            bool isValid = nufusId.HasValue;
            return (isValid, isValid ? nufusId.Value.ToString() : null);
        }

        public async Task<(bool IsValid, string? Ad, string? Soyad)> GetKullaniciBilgileri(int nufusId)
        {
            var query = "SELECT Ad, Soyad FROM dt_kullanici WHERE NufusID = @NufusID AND Durum = 1";

            using var connection = _dbContext.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<(string?, string?)>(query, new { NufusID = nufusId });

            return result != default ? (true, result.Item1, result.Item2) : (false, null, null);
        }

        public async Task<(int? Yetki, string AdSoyad)?> GetKullaniciYetki(int nufusId)
        {
            var query = @"
                SELECT Yetki, CONCAT(Ad, ' ', Soyad) AS AdSoyad 
                FROM dt_kullanici 
                WHERE NufusID = @NufusID AND Durum = 1";

            using var connection = _dbContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<(int?, string)?>(query, new { NufusID = nufusId });
        }
    }
}
