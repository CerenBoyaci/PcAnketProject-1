using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcAnketProject.Core.Dto;
using PcAnketProject.Data.Repository;


namespace PcAnketProject.Service
{
    public class ResimService
    {
        private readonly ResimRepository _resimRepository;

        public ResimService(ResimRepository resimRepository)
        {
            _resimRepository = resimRepository;
        }

        public async Task<int> AddAsync(Resim resim)
        {
            return await _resimRepository.AddAsync(resim);
        }

        public async Task<IEnumerable<Resim>> GetAllAsync()
        {
            return await _resimRepository.GetAllAsync();
        }

        public async Task<Resim?> GetByIdAsync(int id)
        {
            return await _resimRepository.GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _resimRepository.DeleteAsync(id);
        }
    }
}
