
using Click_Go.DTOs;
using Click_Go.Models;


namespace Click_Go.Repositories.Interfaces
{
    public interface IVoucherRepository
    {

        Task<List<Voucher>> GetallByPostIdAsync(long id);
        Task<Voucher> CreateAsync(VoucherProcessDto dto);
        Task<Voucher?> UpdateAsync(long id, VoucherProcessDto dto);
        Task<Voucher?> GetByIdAsync(long id);
        Task<Voucher?> GetByCodeAsync(string code);
        Task<long> UpdateUsedCountAsync(long idVoucher, bool action);

    }
}
