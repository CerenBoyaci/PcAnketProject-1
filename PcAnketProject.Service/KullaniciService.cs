using PcAnketProject.Core.Dto;
using PcAnketProject.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcAnketProject.Service
{
    public class KullaniciService
    {
        private readonly KullaniciRepository _kullaniciRepository;

        public KullaniciService(KullaniciRepository kullaniciRepository)
        {
            _kullaniciRepository = kullaniciRepository;
        }

        public async Task<IEnumerable<Kullanici>> GetAllAsync()
        {
            return await _kullaniciRepository.GetAllAsync();
        }

        public async Task<Kullanici?> GetByIdAsync(int id)
        {
            return await _kullaniciRepository.GetByIdAsync(id);
        }

        public async Task<int> AddAsync(Kullanici kullanici)
        {
            return await _kullaniciRepository.AddAsync(kullanici);
        }

        public async Task<bool> UpdateAsync(Kullanici kullanici)
        {
            return await _kullaniciRepository.UpdateAsync(kullanici);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _kullaniciRepository.DeleteAsync(id);
        }
    }
}
