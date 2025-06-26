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
    public class ResimDuzenRepository
    {
        private readonly DbContext _dbContext;

        public ResimDuzenRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddAsync(ResimDuzen duzen)
        {
            var query = @"INSERT INTO dt_resim_duzen 
                  (ResimID, Baslik, Rolu, Genislik, Yukseklik)
                  VALUES (@ResimID, @Baslik, @Rolu, @Genislik, @Yukseklik);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var connection = _dbContext.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(query, duzen);
        }


        public async Task<IEnumerable<ResimDuzen>> GetAllAsync()
        {
            var query = "SELECT * FROM dt_resim_duzen";
            using var connection = _dbContext.CreateConnection();
            return await connection.QueryAsync<ResimDuzen>(query);
        }

        public async Task<bool> UpdateAsync(ResimDuzen duzen)
        {
            var query = @"UPDATE dt_resim_duzen 
                  SET Baslik = @Baslik, 
                      Rolu = @Rolu,
                      Genislik = @Genislik,
                      Yukseklik = @Yukseklik
                  WHERE ID = @ID";

            using var connection = _dbContext.CreateConnection();
            var affected = await connection.ExecuteAsync(query, duzen);
            return affected > 0;
        }


    }
}
