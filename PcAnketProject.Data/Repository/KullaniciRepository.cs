using Dapper;
using PcAnketProject.Core.Dto;
using PcAnketProject.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcAnketProject.Data.Repository
{
  public  class KullaniciRepository
    {
        private readonly DbContext _dbContext;
        public KullaniciRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Kullanici>> GetAllAsync()
        {
            // veri tabanı bağlantısı 
            using var connection = _dbContext.CreateConnection();

            var query = @"SELECT * FROM dt_kullanici";
            
            //asenkron olunca await kullanılıyormuş 
            // asenkron paralel çalışma sırasında biri diğerini bozmasın diye (sanırım)
            return await connection.QueryAsync<Kullanici>(query);

        }

        public async Task<Kullanici?> GetByIdAsync(int id)
        {
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Kullanici>(
                "SELECT * FROM dt_kullanici WHERE ID = @Id", new { Id = id });
        }

        public async Task<int> AddAsync(Kullanici kullanici)
        {
            using var connection = _dbContext.CreateConnection();
            var query = @"INSERT INTO dt_kullanici 
                          (NufusID, Ad, Soyad, Tc, Parola, Durum, Tarih, Yetki)
                          VALUES (@NufusID, @Ad, @Soyad, @Tc, @Parola, @Durum, @Tarih, @Yetki);
                          SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.ExecuteScalarAsync<int>(query, kullanici);
            // ExecuteScalarAsync metodun temel işlevini belirtir. komutun ilk satırın il sütunundaki değeri döndürmeyi bekliyor
            // <int>: Bu, generik tip parametresidir. ExecuteScalarAsync metodunun döndüreceği tek skaler değerin int (tam sayı) tipinde olmasını beklediğini belirtir 
        }


        // bool olmasının sebebi güncelleme başarılı mı değil mi öğrenmek
        public async Task<bool> UpdateAsync(Kullanici kullanici)
        {
            using var connection = _dbContext.CreateConnection();
            var query = @"UPDATE dt_kullanici 
                          SET NufusID = @NufusID,
                              Ad = @Ad,
                              Soyad = @Soyad,
                              Tc = @Tc,
                              Parola = @Parola,
                              Durum = @Durum,
                              Tarih = @Tarih,
                              Yetki = @Yetki
                          WHERE ID = @ID";

            // affected kaç veri etkilendi bunun sayısını tutar
            //execute da ise sql de komutları çalıştırma işlemidir 
            var affected = await connection.ExecuteAsync(query, kullanici);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _dbContext.CreateConnection();
            var affected = await connection.ExecuteAsync("DELETE FROM dt_kullanici WHERE ID = @Id", new { Id = id });
            return affected > 0;
        }
    }
}
