using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcAnketProject.Core.Dto;
using PcAnketProject.Data.Repository;
using System.Threading.Tasks;


namespace PcAnketProject.Service
{
    public class ResimDuzenService
    {
        private readonly ResimDuzenRepository _repository;

        public ResimDuzenService(ResimDuzenRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddAsync(ResimDuzen duzen)
        {
            return await _repository.AddAsync(duzen);
        }

        public async Task<IEnumerable<ResimDuzen>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<bool> UpdateAsync(ResimDuzen duzen)
        {
            return await _repository.UpdateAsync(duzen);
        }
    }
}
