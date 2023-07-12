using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Data;
using PefectMoney.Core.Model;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase.Notify;
using PefectMoney.Shared.Helper;
using PefectMoney.Shared.Utility;
using PefectMoney.Shared.Utility.ResultUtil;

namespace PefectMoney.Core.UseCase._Shop
{
    public record ValidateBasketRequest(string status, string trackId, string success, string orderId) : IRequest<ResultOperation>
    {
    
    }
    public class ValidateBasketHandler : IRequestHandler<ValidateBasketRequest, ResultOperation>
    {
        public ValidateBasketHandler(ITelContext context,IMediator mediator,
            ILogger<ValidateBasketHandler> logger,IOptions<BotSettings> options)
        {
            Context = context;
            Mediator = mediator;
            Logger = logger;
            BotSettings = options.Value ;
        }

        public ITelContext Context { get; }
        public IMediator Mediator { get; }
        public ILogger<ValidateBasketHandler> Logger { get; }
        public BotSettings BotSettings { get; }

        public async Task<ResultOperation> Handle(ValidateBasketRequest request, CancellationToken cancellationToken)
        {
            if(request == null)
            {
                return ResultOperation.ToFailedResult("درخواست خالیست");
            }
            try
            {
                int status = int.MinValue; 
                try
                {
                     status = Convert.ToInt32(request.status);
                }
                catch (Exception)
                {
                    return ResultOperation.ToFailedResult("وضعیت ارسال شده برای سفارش نامعتبر است");
                }

                var result = await  Context.Orders.FirstOrDefaultAsync(x => x.trackId == request.trackId);
                if(result == null) {
                    return ResultOperation.ToFailedResult("چنین سبد خریدی پیدا نشد");
                }
                if(result.OrderStatus == (long)OrderStatus.OnProcess || result.OrderStatus == (long)OrderStatus.Initial)
                {
                    DateTime expireDate = result.CreationTime.AddMinutes(BotSettings.ValidBasketTime);
                    if(TimeHelper.DateTimeNow < expireDate)
                    {
                        result.OrderStatus = (int)OrderStatus.TimeOut;
                        Context.Update(result);
                        await Context.SaveChangesAsync();
                        return ResultOperation.ToFailedResult("مدت زمان سبد به پایان رسیده");

                    }
                   
                  
                    if(ZibalVerifyPaymentPlusResultStatus.Success.Id == status
                        || ZibalVerifyPaymentPlusResultStatus.PaymentConfirmed.Id == status 
                        || ZibalVerifyPaymentPlusResultStatus.AlreadySuccess.Id == status)
                    {
                        return ResultOperation.ToSuccessResult("پرداخت موفقیت آمیز بوده یا همین الان بررسی شده و موفقیت آمیز بوده و تایید شده");
                    }
                    
                }
               var res =  ZibalVerifyPaymentPlusResultStatus.FindById<ZibalVerifyPaymentPlusResultStatus>(status);
                if(res == null)
                {
                    return ResultOperation.ToFailedResult($"وضعیت سبد {result.OrderStatus.ToString()}");
                }
                return ResultOperation.ToFailedResult($"وضعیت سبد {result.OrderStatus.ToString()} --- result id = {res.Id} --- error message {res.TypeErrorCodeStatus}");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e.InnerException?.Message, e.StackTrace , e.InnerException?.StackTrace);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation.ToFailedResult();
            }
           
            
        }
    }

    public class ZibalVerifyPaymentPlusResultStatus : Enumeration<int, string>
    {
        public TypeErrorCodeStatus TypeErrorCodeStatus { get; set; }
        public static ZibalVerifyPaymentPlusResultStatus Success = new ZibalVerifyPaymentPlusResultStatus(100, "با موفقیت تایید شد.", TypeErrorCodeStatus.NoError);
        public static ZibalVerifyPaymentPlusResultStatus MerchantNotFind = new ZibalVerifyPaymentPlusResultStatus(102, " merchant یافت نشد ", TypeErrorCodeStatus.ApplicationError);
        public static ZibalVerifyPaymentPlusResultStatus MerchantUnActive = new ZibalVerifyPaymentPlusResultStatus(103, "merchant غیرفعال", TypeErrorCodeStatus.ApplicationError);
        public static ZibalVerifyPaymentPlusResultStatus MerchantUnValid = new ZibalVerifyPaymentPlusResultStatus(104, " merchant نامعتبر ", TypeErrorCodeStatus.ApplicationError);
        public static ZibalVerifyPaymentPlusResultStatus AlreadySuccess = new ZibalVerifyPaymentPlusResultStatus(201, "قبلا تایید شده", TypeErrorCodeStatus.NoError);
        public static ZibalVerifyPaymentPlusResultStatus UnPayOrFailPay = new ZibalVerifyPaymentPlusResultStatus(202, "سفارش پرداخت نشده یا ناموفق بوده است.جهت اطلاعات بیشتر جدول وضعیت‌ها را مطالعه کنید", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus UnValidTrackId = new ZibalVerifyPaymentPlusResultStatus(203, "trackId نامعتبر می‌باشد .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus PaymentPending = new ZibalVerifyPaymentPlusResultStatus(-1, "در انتظار پردخت .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus InternalError = new ZibalVerifyPaymentPlusResultStatus(-2, "خطای داخلی .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus PaymentConfirmed = new ZibalVerifyPaymentPlusResultStatus(1, "پرداخت شده - تاییدشده.", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus PaymentUnconfirmed = new ZibalVerifyPaymentPlusResultStatus(2, "پرداخت شده - تاییدنشده .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus CancelledbyUser = new ZibalVerifyPaymentPlusResultStatus(3, "لغوشده توسط کاربر.", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus InvalidCardNumber = new ZibalVerifyPaymentPlusResultStatus(4, "‌شماره کارت نامعتبر می‌باشد. .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus InsufficientAccountBalance = new ZibalVerifyPaymentPlusResultStatus(5, "‌موجودی حساب کافی نمی‌باشد..", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus IncorrectPassword = new ZibalVerifyPaymentPlusResultStatus(6, "رمز واردشده اشتباه می‌باشد.", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus ExceededMaximumRequestLimit = new ZibalVerifyPaymentPlusResultStatus(7, "‌تعداد درخواست‌ها بیش از حد مجاز می‌باشد. .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus ExceededDailyOnlinePaymentLimit = new ZibalVerifyPaymentPlusResultStatus(8, "تعداد پرداخت اینترنتی روزانه بیش از حد مجاز می‌باشد. .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus ExceededDailyOnlinePaymentAmountLimit = new ZibalVerifyPaymentPlusResultStatus(9, "مبلغ پرداخت اینترنتی روزانه بیش از حد مجاز می‌باشد..", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus InvalidCardIssuer = new ZibalVerifyPaymentPlusResultStatus(10, "صادرکننده‌ی کارت نامعتبر می‌باشد..", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus SwitchError = new ZibalVerifyPaymentPlusResultStatus(11, "‌خطای سوییچ .", TypeErrorCodeStatus.UserError);
        public static ZibalVerifyPaymentPlusResultStatus CardUnavailable = new ZibalVerifyPaymentPlusResultStatus(12, "کارت قابل دسترسی نمی‌باشد.", TypeErrorCodeStatus.UserError);
        public ZibalVerifyPaymentPlusResultStatus(int id, string name, TypeErrorCodeStatus typeErrorCodeStatus) : base(id, name)
        {
            TypeErrorCodeStatus = typeErrorCodeStatus;
        }


    }
}
