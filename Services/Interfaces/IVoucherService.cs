using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<Voucher> CreateVoucherAsync(VoucherProcessDto dto);
        Task<Voucher?> UpdateVoucherAsync(long id, VoucherProcessDto dto);
        Task<Voucher?> GetVoucherByIdAsync(long id);
        Task<Voucher?> GetVoucherByCodeAsync(string code);
        Task<List<AllVouncherDto>> GetAllVoucherByPostIdAsync(long id);
        Task UpdateUsedCountAsync(long idvoucher, bool action);
    }
}
