using System.Net.WebSockets;
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

        public async Task<List<AllVouncherDto>> GetAllVoucherByPostIdAsync(long id)
        {
            var voucherList = await _voucherRepository.GetallByPostIdAsync(id);

            return voucherList.Select(voucher => new AllVouncherDto
            {
                idVoucher = voucher.Id,
                Code = voucher.Code,
                DiscountAmount = voucher.DiscountAmount,
                DiscountPercentage = voucher.DiscountPercentage,
                Description = voucher.Description,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                IsActive = voucher.IsActive,
                UsageLimit = voucher.UsageLimit,
                UsedCount = voucher.UsedCount
            }).ToList();
        }

        public async Task<long> UpdateUsedCountAsync(long idvoucher, bool action)
        {
            return await _voucherRepository.UpdateUsedCountAsync(idvoucher, action);
        }
    }
}
