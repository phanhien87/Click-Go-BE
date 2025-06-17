using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;

namespace Click_Go.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;

        public VoucherService(IVoucherRepository voucherRepository)
        {
            _voucherRepository = voucherRepository;
        }

        public async Task<Voucher> CreateVoucherAsync(VoucherProcessDto dto)
        {
            return await _voucherRepository.CreateAsync(dto);
        }

        public async Task<Voucher?> UpdateVoucherAsync(long id, VoucherProcessDto dto)
        {
            return await _voucherRepository.UpdateAsync(id, dto);
        }

        public async Task<Voucher?> GetVoucherByIdAsync(long id)
        {
            return await _voucherRepository.GetByIdAsync(id);
        }
        
        public async Task<Voucher?> GetVoucherByCodeAsync(string code)
        {
            return await _voucherRepository.GetByCodeAsync(code);
        }
    }
}
