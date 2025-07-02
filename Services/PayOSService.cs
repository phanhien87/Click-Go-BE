using System.Security.Claims;
using Azure.Core;
using Click_Go.DTOs;
using Click_Go.Helper;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;


public class PayOSService
{
    private readonly PayOS _payOS;
   private readonly IPackageRepository _packageRepository;
    private readonly IUserPackageRepository _userPackageRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IPostService _postService;
    public PayOSService(IOptions<PayOSOptions> options, IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager, IUserPackageRepository userPackageRepository,
        IPackageRepository packageRepository, IOrderRepository orderRepository, IEmailService emailService,
        IPostService postService)
    {
        var opts = options.Value;
        _payOS = new PayOS(opts.ClientId, opts.ApiKey, opts.ChecksumKey);
        _packageRepository = packageRepository;
        _userPackageRepository = userPackageRepository; 
        _userManager = userManager;
        _orderRepository = orderRepository;
        _emailService = emailService;
        _postService = postService;
    }

    public async Task<PaymentResult> CreatePaymentLink(long packageId, string returnUrl, string cancelUrl,string userId)
    {
        var userpackage = await _userPackageRepository.CheckPackageByUserId(userId);
        if (userpackage != null) return new PaymentResult
        {
            Success = false,
            Message = "Bạn đang dùng 1 gói rồi !"
        }
        ;
       
        PaymentData paymentData;

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
            paymentData = new PaymentData(orderCode, amount, package.Name, null
             , cancelUrl, returnUrl);
       

       

        CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);


        return new PaymentResult
        {
            Success = true,
            CheckoutUrl = createPayment.checkoutUrl,
        };
    }


    public async Task<bool> ConfirmPayment(WebhookType request)
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
                // Gửi email cho người dùng
                var user = await _userManager.FindByIdAsync(order.UserId);
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    string subject = "Xác nhận thanh toán thành công";
                    string body = $@"
                    <p>Xin chào {user.FullName},</p>
                    <p>Chúng tôi đã nhận được thanh toán của bạn cho gói <strong>{order.Package.Name}</strong>.</p>
                    <p>Giao dịch của bạn đã được xác nhận vào lúc {order.transactionDateTime:dd/MM/yyyy HH:mm}.</p>
                    <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
                    <p>Trân trọng,<br/>Đội ngũ hỗ trợ - 0963009178 - hienpmhe180216@fpt.edu.vn</p>";

                    await _emailService.SendEmailAsync(user.Email, subject, body);
                }

                //Unlock Post if post is existed
                await _postService.LockPostAsync(order.UserId, 1);

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
