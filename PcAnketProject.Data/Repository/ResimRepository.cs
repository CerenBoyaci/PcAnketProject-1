using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using PcAnketProject.Core.Dto;
using PcAnketProject.Data.Context;

namespace PcAnketProject.Data.Repository
{
    // veri tabanında yapılacak işlemler burada 
    public class ResimRepository
    {
        private readonly DbContext _dbContext;

        public ResimRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // resmi veri tabanına ekler 
        public async Task<int> AddAsync(Resim resim)
        {
            var query = @"INSERT INTO dt_resim (DosyaAdi, DosyaYolu, YuklenmeTarihi)
                          VALUES (@DosyaAdi, @DosyaYolu, @YuklenmeTarihi);
                          SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _dbContext.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, resim);
        }

        //tüm kayıtları getirir 
        public async Task<IEnumerable<Resim>> GetAllAsync()
        {
            var query = "SELECT * FROM dt_resim";
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<Resim>(query);
        }

        //belirli bir resmi getirir
        public async Task<Resim?> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM dt_resim WHERE ID = @ID";
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Resim>(query, new { ID = id });
        }

        //belli bir resmi siler 
        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM dt_resim WHERE ID = @ID";
            using var connection = _dbContext.CreateConnection();
            var affected = await connection.ExecuteAsync(query, new { ID = id });
            return affected > 0;
        }
    }
}
