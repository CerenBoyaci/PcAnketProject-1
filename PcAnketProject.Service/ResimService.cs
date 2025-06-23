using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PcAnketProject.Core.Dto;
using PcAnketProject.Data.Repository;

// controller dan gelen veri burada işlenir sonra repository ye gönderilir
namespace PcAnketProject.Service
{
    public class ResimService
    {
        private readonly ResimRepository _resimRepository;


        // dışardan repository alıyoruz ve bunu _repository alanına atıyoruz böylece service repository deki kodlara ulaşabilir
        public ResimService(ResimRepository resimRepository)
        {
            _resimRepository = resimRepository;
        }

        // repository ye göndererek veri tabanına ekler 
        public async Task<int> AddAsync(Resim resim)
        {
            return await _resimRepository.AddAsync(resim);
        }

        // tüm resimler gelir 
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
