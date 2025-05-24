using Azure.Core;
using Click_Go.DTOs;
using Click_Go.Helper;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using System.Security.Claims;


public class PayOSService
{
    private readonly PayOS _payOS;
   private readonly IPackageRepository _packageRepository;
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public PayOSService(IOptions<PayOSOptions> options, IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager, IUserPackageRepository userPackageRepository,
        IPackageRepository packageRepository, IOrderRepository orderRepository)
    {
        var opts = options.Value;
        _payOS = new PayOS(opts.ClientId, opts.ApiKey, opts.ChecksumKey);
        _packageRepository = packageRepository;
        _userPackageRepository = userPackageRepository; 
        _userManager = userManager;
        _orderRepository = orderRepository;
    }

    public async Task<string> CreatePaymentLink(long packageId, string returnUrl, string cancelUrl,string userId)
    {
        var package = await _packageRepository.GetPackageByIdAsync(packageId);
        var amountStr = package.Price.ToString();
        var amount = int.Parse(amountStr);


        var orderCode = await GenerateUniqueOrderCodeAsync();


        var order = new Order
        {
            OrderCode = orderCode.ToString(),
            UserId = userId,
            PackageId = packageId,
            Amount = amount,
            Status = Click_Go.Enum.OrderStatus.Pending,
            CreatedDate = DateTime.UtcNow,
            CreatedUser = Guid.Parse(userId),
        };
        
        await _orderRepository.AddAsync(order);

        PaymentData paymentData = new PaymentData(orderCode, amount, package.Name, null
              , cancelUrl, returnUrl);

        CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);


        return createPayment.checkoutUrl;
       
    }


    public async Task<bool> ConfirmPayment(WebhookType request, string userId)
    {
        try
        {
            if (request.success)
            {
                var order = await _orderRepository.GetByCodeAsync(request.data.orderCode);
                if (order == null || order.Status == Click_Go.Enum.OrderStatus.Paid)
                {
                    return false;
                }

                order.Status = Click_Go.Enum.OrderStatus.Paid;
                order.transactionDateTime = request.data.transactionDateTime;
                order.TransactionId = request.data.reference;
                order.counterAccountBankName = request.data.counterAccountBankName;
                order.counterAccountNumber = request.data.counterAccountNumber;
                order.counterAccountName = request.data.counterAccountName;

                if (order.Package == null)
                {
                    throw new Exception("order.Package is null");
                }

                await _orderRepository.UpdateAsync(order);

                var userpackage = new UserPackage
                {
                    UserId = order.UserId,
                    OrderId = order.Id,
                    StartDate = DateTime.Now,
                    ExpireDate = DateTime.Now.AddDays(order.Package.DurationDays),
                    CreatedDate = DateTime.Now,
                    CreatedUser = Guid.Parse(order.UserId),
                    Status = 1
                };

                await _userPackageRepository.AddAsync(userpackage);

                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ConfirmPayment Error] {ex.Message}");
            throw new AppException("ConfirmPayment Error");
           
        }

        return false;
    }

    public async Task<long> GenerateUniqueOrderCodeAsync()
    {
        long orderCode;
        Order existedCode;

        do
        {
            var ticks = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var random = new Random().Next(100, 999);
            orderCode = long.Parse($"{ticks}{random}");

            existedCode = await _orderRepository.GetByCodeAsync(orderCode);
        } while (existedCode != null);

        return orderCode;
    }


}
