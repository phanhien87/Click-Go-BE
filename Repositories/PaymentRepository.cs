using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;

namespace Click_Go.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPayment(Payment payment)
        {
             _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }
    }
}
