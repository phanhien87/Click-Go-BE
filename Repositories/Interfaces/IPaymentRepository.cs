using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task AddPayment(Payment payment);   
    }
}
