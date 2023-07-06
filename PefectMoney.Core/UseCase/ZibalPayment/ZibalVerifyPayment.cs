using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Shared.Utility;
using PefectMoney.Shared.Utility.ResultUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PefectMoney.Core.UseCase.ZibalPayment
{

    public record ZibalVerifyPaymentRequestDto : IRequest<ResultOperation<OrderDto>>
    {
        
        public string paidAt { get; set; }
        public string createdAt { get; set; }
        public string cardNumber { get; set; }
        public string refNumber { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string wage { get; set; }
        public string amount { get; set; }
        public string result { get; set; }
        public string message { get; set; }
        public bool HaveError { get; set; }
        public string errorMessage { get; set; }
        public string innerErrorMessage { get; set; }
        public long orderId { get; set; }
    }
   
    public class ZibalVerifyPaymentHandler : IRequestHandler<ZibalVerifyPaymentRequestDto, ResultOperation<OrderDto>>
    {
        public ZibalVerifyPaymentHandler(IOptions<ZibalPaymentSettings> paymentSettings,
            ITelContext context,IMediator mediator,
            ILogger<ZibalVerifyPaymentHandler> logger )
        {
            PaymentSettings = paymentSettings.Value;
            Context = context;
            Mediator = mediator;
            Logger = logger;
        }

        public ZibalPaymentSettings PaymentSettings { get; }
        public ITelContext Context { get; }
        public IMediator Mediator { get; }
        public ILogger<ZibalVerifyPaymentHandler> Logger { get; }

        public async Task<ResultOperation<OrderDto>> Handle(ZibalVerifyPaymentRequestDto request, CancellationToken cancellationToken)
        {


            try
            {
                var orderEntity = await Context.Orders.FirstOrDefaultAsync(x => x.Id == request.orderId);
                if (orderEntity == null) return ResultOperation<OrderDto>.ToFailedResult("سفارش یافت نشد");
                var resultCode = int.Parse(request.status);
                    if (resultCode == ZibalVerifyPaymentResultStatus.Success.Id || resultCode == ZibalVerifyPaymentResultStatus.AlreadySuccess.Id)
                    {
                     orderEntity.OrderStatus = (int)OrderStatus.Finish;
                    Context.Update(orderEntity);
                    await Context.SaveChangesAsync();
                   var orderDto = new OrderDto
                    {
                        BotChatId = orderEntity.BotChatId,
                        Count = orderEntity.Count,
                        CreationTime = orderEntity.CreationTime,
                        Dollar = orderEntity.Dollar,
                        Id = orderEntity.Id,
                        OrderStatus = orderEntity.OrderStatus,
                        ProductId = orderEntity.ProductId,
                        Rial = orderEntity.Rial,
                        TotalDollarPrice = orderEntity.TotalDollarPrice,
                        TotalRialsPrice = orderEntity.TotalRialsPrice,
                        trackId = orderEntity.trackId,
                    };
                       // Rise Event
                        
                        return orderDto.ToSuccessResult();
                    }
                    else
                    {
                        
                        return ResultOperation<OrderDto>.ToFailedResult();
                    }
              
            }
            catch (WebException ex)
            {
                Logger.LogError(ex.Message,ex.InnerException?.Message); // print exception error
                await Mediator.Publish(new NotifyAdminRequest($"{ex.Message}---{ex.InnerException?.Message}"));
                return ResultOperation<OrderDto>.ToFailedResult();
            }
            catch (Exception ex)
            {

                Logger.LogCritical(ex.Message, ex.InnerException?.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{ex.Message}---{ex.InnerException?.Message}"));
                // Rise Event
                return ResultOperation<OrderDto>.ToFailedResult();
            }

        }



        public class ZibalVerifyPaymentResultStatus : Enumeration<int, string>
        {
            public TypeErrorCodeStatus TypeErrorCodeStatus { get; set; }
            public static ZibalVerifyPaymentResultStatus Success = new ZibalVerifyPaymentResultStatus(100, "با موفقیت تایید شد.", TypeErrorCodeStatus.NoError);
            public static ZibalVerifyPaymentResultStatus MerchantNotFind = new ZibalVerifyPaymentResultStatus(102, " merchant یافت نشد ", TypeErrorCodeStatus.ApplicationError);
            public static ZibalVerifyPaymentResultStatus MerchantUnActive = new ZibalVerifyPaymentResultStatus(103, "merchant غیرفعال", TypeErrorCodeStatus.ApplicationError);
            public static ZibalVerifyPaymentResultStatus MerchantUnValid = new ZibalVerifyPaymentResultStatus(104, " merchant نامعتبر ", TypeErrorCodeStatus.ApplicationError);
            public static ZibalVerifyPaymentResultStatus AlreadySuccess = new ZibalVerifyPaymentResultStatus(201, "قبلا تایید شده", TypeErrorCodeStatus.NoError);
            public static ZibalVerifyPaymentResultStatus UnPayOrFailPay = new ZibalVerifyPaymentResultStatus(202, "سفارش پرداخت نشده یا ناموفق بوده است.جهت اطلاعات بیشتر جدول وضعیت‌ها را مطالعه کنید", TypeErrorCodeStatus.UserError);
            public static ZibalVerifyPaymentResultStatus UnValidTrackId = new ZibalVerifyPaymentResultStatus(203, "trackId نامعتبر می‌باشد .", TypeErrorCodeStatus.UserError);
            public ZibalVerifyPaymentResultStatus(int id, string name, TypeErrorCodeStatus typeErrorCodeStatus) : base(id, name)
            {
                TypeErrorCodeStatus = typeErrorCodeStatus;
            }


        }
    }
}
