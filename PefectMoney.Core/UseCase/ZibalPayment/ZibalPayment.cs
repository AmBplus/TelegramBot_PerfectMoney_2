using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json; // Solution Explorer->Right Click on Project Name -> Click on Manage Nuget Packages -> Search for newtonsoft -> Click on install button 
using System.Net;
using MediatR;
using PefectMoney.Shared.Utility.ResultUtil;
using Microsoft.Extensions.Options;
using PefectMoney.Core.Settings;
using PefectMoney.Shared.Utility;
using static System.Net.WebRequestMethods;
using RestSharp;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.UseCase.Notify;

namespace PefectMoney.Core.UseCase.ZibalPayment;

// Send A Link To User To Pay
// Get A Link To Verify Payment 
public record ZiBalPaymentResponse
{
    public Uri Uri { get; set; }
    public string trackId { get; set; }
    public string result { get; set; }
    public string message { get; set; }

}

public record ZibalPaymentRequest : IRequest<ResultOperation<ZiBalPaymentHandlerResponse>>
{
    public string orderId { get; set; }
    
    public int amount { get; set; }
    //public string callbackUrl { get; set; }
    public string mobile { get; set; }
    public string[] allowedCards { get; set; }

}
public record GetLinkPaymentRequest
{

    public string token { get; set; }

    public string orderId { get; set; }

    public int amount { get; set; }
    //public string callbackUrl { get; set; }


    public string mobile { get; set; }

    public string description { get; set; }

    public string[] allowedCards { get; set; }



}
public record ZiBalPaymentHandlerResponse { 
    public string trackId { get; set; }
    public Uri Uri { get; set; }
}

public class ZiBalPaymentHandler : IRequestHandler<ZibalPaymentRequest, ResultOperation<ZiBalPaymentHandlerResponse>>
{
    RestRequest RestRequest { get; set; }
    RestSharp.RestClient RestClient { get; set; }

    public ZiBalPaymentHandler(IOptions<ZibalPaymentSettings> zibalPaymentSettings,
        IOptions<BotSettings> options,IMediator mediator,
        ILogger<ZiBalPaymentHandler> logger)
    {
        ZibalPaymentSettings = zibalPaymentSettings.Value;
        BotSettings = options.Value;
        Mediator = mediator;
        Logger = logger;
    }
    public ZibalPaymentSettings ZibalPaymentSettings { get; set; }
    public BotSettings BotSettings { get; }
    public IMediator Mediator { get; }
    public ILogger<ZiBalPaymentHandler> Logger { get; }

    //public string description { get; set; }
    public async Task<ResultOperation<ZiBalPaymentHandlerResponse>> Handle(ZibalPaymentRequest request
        , CancellationToken cancellationToken)
    {
        Uri paymentLink;
        try
        {

            var getLinkPaymentRequest = new GetLinkPaymentRequest()
            {
                allowedCards = request.allowedCards,
                amount = request.amount,
                description = "پرداخت",
                mobile = request.mobile,
                orderId = request.orderId,
                token = BotSettings.TokenExternalApp,

            };
            // url
            RestClient = new RestSharp.RestClient(BotSettings.ExternalAppBaseUrl);
            RestRequest = new RestRequest(ZibalPaymentSettings.UrlPaymentRequest);
            RestRequest.AddBody(getLinkPaymentRequest);
            

            var httpResponse = await RestClient.PostAsync<ResultOperation<ZiBalPaymentResponse>>(RestRequest);  // get Response
                if(!httpResponse.IsSuccess)
            {
                return ResultOperation<ZiBalPaymentHandlerResponse>.ToFailedResult("عدم دریافت لینک پرداخت");
            }
                 int responseCode = int.Parse(httpResponse.Data.result);
                if (responseCode == ZibalResultResponseCode.Success.Id)
                {
                    return new ZiBalPaymentHandlerResponse
                    {
                        Uri = httpResponse.Data.Uri,
                        trackId = httpResponse.Data.trackId
                    }.ToSuccessResult();
       
                }

                var error = ZibalResultResponseCode.FindById<ZibalResultResponseCode>(responseCode);
                if (error == null)
                {
                await Mediator.Publish(new NotifyAdminRequest($"ZiBalPaymentHandlerResponse -- {responseCode} == {error?.Name} == Error Not Find"));
                    return ResultOperation<ZiBalPaymentHandlerResponse>.ToFailedResult("مشکلی در برنامه پیش آمده در اصرع وقت مشکل حل خواهد شد  ، شکیبا باشید.");
            }

                if (error?.TypeErrorCodeStatus == TypeErrorCodeStatus.UserError)
                {
               
                return ResultOperation<ZiBalPaymentHandlerResponse>.ToFailedResult(error.Name);
                }
                 await Mediator.Publish(new NotifyAdminRequest($"{error.Name}"));
                 Logger.LogError(error?.Name); // print exception error
                 await Mediator.Publish(new NotifyAdminRequest($"{error?.Name}"));
            return ResultOperation<ZiBalPaymentHandlerResponse>.ToFailedResult("مشکلی در برنامه پیش آمده به ادمین اطلاع دهید");

            
        }
        catch (WebException ex)
        {
            Logger.LogError(ex.Message, ex.InnerException?.Message); // print exception error
            await Mediator.Publish(new NotifyAdminRequest($"{ex.Message}---{ex.InnerException?.Message}"));
            return ResultOperation<ZiBalPaymentHandlerResponse>.ToFailedResult(ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message,ex.InnerException?.Message); // print exception error
            await Mediator.Publish(new NotifyAdminRequest($"{ex.Message}---{ex.InnerException?.Message}"));
            return ResultOperation<ZiBalPaymentHandlerResponse>.ToFailedResult(ex.Message);
        }
    }
}
public class ZibalResultResponseCode : Enumeration<int, string>
{
    public TypeErrorCodeStatus TypeErrorCodeStatus { get; set; }
    public static ZibalResultResponseCode Success = new ZibalResultResponseCode(100, "با موفقیت تایید شد.", TypeErrorCodeStatus.NoError);
    public static ZibalResultResponseCode MerchantNotFind = new ZibalResultResponseCode(102, " merchant یافت نشد ", TypeErrorCodeStatus.ApplicationError);
    public static ZibalResultResponseCode MerchantUnActive = new ZibalResultResponseCode(103, "merchant غیرفعال", TypeErrorCodeStatus.ApplicationError);
    public static ZibalResultResponseCode MerchantUnValid = new ZibalResultResponseCode(104, " merchant نامعتبر ", TypeErrorCodeStatus.ApplicationError);
    public static ZibalResultResponseCode LowAmount = new ZibalResultResponseCode(105, "amount بایستی بزرگتر از 1,000 ریال باشد", TypeErrorCodeStatus.UserError);
    public static ZibalResultResponseCode CallbackUrlUnValid = new ZibalResultResponseCode(106, "callbackUrlنامعتبر می‌باشد. (شروع با http و یا https)", TypeErrorCodeStatus.ApplicationError);
    public static ZibalResultResponseCode ExceededAmount = new ZibalResultResponseCode(113, "مبلغ تراکنش از سقف میزان تراکنش بیشتر است.", TypeErrorCodeStatus.UserError);
    public ZibalResultResponseCode(int id, string name, TypeErrorCodeStatus typeErrorCodeStatus) : base(id, name)
    {
        TypeErrorCodeStatus = typeErrorCodeStatus;
    }


}

