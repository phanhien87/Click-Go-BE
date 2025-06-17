using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<List<Voucher>> GetallByPostIdAsync(long id);
       
    }
}
