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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPaymentRepository _paymentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public PayOSService(IOptions<PayOSOptions> options, IHttpContextAccessor httpContextAccessor, IPaymentRepository paymentRepository, UserManager<ApplicationUser> userManager)
    {
        var opts = options.Value;
        _payOS = new PayOS(opts.ClientId, opts.ApiKey, opts.ChecksumKey);
        _httpContextAccessor = httpContextAccessor;
        _paymentRepository = paymentRepository;
        _userManager = userManager;
    }

    public async Task<CreatePaymentLinkDto> CreatePaymentLink(int amount, string description, string returnUrl,
        string cancelUrl, int? level)
    {
        if (amount < 1000)
            throw new AppException("Số tiền phải >= 1,000 VND");

        // Tạo số nguyên dương ngẫu nhiên dài đủ 13-15 chữ số
        var ticks = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // ~13 chữ số
        var random = new Random().Next(100, 999); // Thêm vài chữ số ngẫu nhiên
        var orderCode = long.Parse($"{ticks}{random}");

       
        
        PaymentData paymentData = new PaymentData(orderCode, amount, description, null
              , cancelUrl, returnUrl);

        CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);


        return new CreatePaymentLinkDto
        {
            url = createPayment.checkoutUrl,
            orderCode = orderCode,
            amount = amount,
            description = description,
            level = level,
        };
    }

    public async Task<Boolean> ConfirmPayment(CreatePaymentLinkDto request,string userId)
    {

        var amount = request.amount;
        var description = request.description;
        var orderCodeStr =request.orderCode;
        var level = request.level;
        var user = await _userManager.FindByIdAsync(userId);
        user.Level = level;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            // Xử lý lỗi cập nhật
            throw new Exception("Cập nhật user thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var payment = new Payment
        {
            UserId = userId,
            Amount = amount,
            Description = description,
            ReferenceId = orderCodeStr.ToString(),
            CreatedDate = DateTime.Now,
            CreatedUser = Guid.Parse(userId),
            Status = 1
        };
       await _paymentRepository.AddPayment(payment);
        return true;
    }



}
