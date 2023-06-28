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

namespace PefectMoney.Core.UseCase.ZibalPayment;

// Send A Link To User To Pay
// Get A Link To Verify Payment 


public record ZibalPaymentRequest : IRequest<ResultOperation<Uri>>
{
    public string orderId { get; set; }
    public int amount { get; set; }
    //public string callbackUrl { get; set; }
    public string mobile { get; set; }
    public string[] allowedCards { get; set; }

}

public class ZiBalPaymentHandler : IRequestHandler<ZibalPaymentRequest, ResultOperation<Uri>>
{
    public ZiBalPaymentHandler(IOptions<ZibalPaymentSettings> zibalPaymentSettings)
    {
        ZibalPaymentSettings = zibalPaymentSettings.Value;
    }
    public ZibalPaymentSettings ZibalPaymentSettings { get; set; }

    //public string description { get; set; }
    public async Task<ResultOperation<Uri>> Handle(ZibalPaymentRequest request, CancellationToken cancellationToken)
    {
        Uri paymentLink;
        try
        {
            // url
            Zibal.makeRequest Request = new Zibal.makeRequest()
            {
                merchant = ZibalPaymentSettings.Merchant,// String
                orderId = request.orderId, // String
                amount = request.amount, //Integer
                callbackUrl = ZibalPaymentSettings.BaseCallbackUrl,//String
                description = "Hello Zibal !" ,// String
                mobile = request.mobile,
                allowedCards = request.allowedCards,
                
            };

            var httpResponse = Zibal.HttpRequestToZibal(ZibalPaymentSettings.UrlPaymentRequest, JsonConvert.SerializeObject(Request));  // get Response
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) // make stream reader
            {
                var responseText = streamReader.ReadToEnd(); // read Response
                Zibal.makeRequest_response response = JsonConvert.DeserializeObject<Zibal.makeRequest_response>(responseText); // Deserilize as response class object
                int responseCode = int.Parse(response.result);
                if (responseCode == ZibalResultResponseCode.Success.Id)
                {
                    paymentLink = new Uri($"{ZibalPaymentSettings.UrlPaymentRequest}/{response.trackId}");
                    return paymentLink.ToSuccessResult();
                    //
                }

                var error = ZibalResultResponseCode.FindById<ZibalResultResponseCode>(responseCode);
                if (error == null)
                {
                    throw new Exception("Error Not Find");
                }

                if (error.TypeErrorCodeStatus == TypeErrorCodeStatus.UserError)
                {
                    return ResultOperation<Uri>.ToFailedResult(error.Name);
                }
                Console.WriteLine(error.Name); // print exception error
                return ResultOperation<Uri>.ToFailedResult("مشکلی در برنامه پیش آمده به ادمین اطلاع دهید");

            }
        }
        catch (WebException ex)
        {
            Console.WriteLine(ex.Message); // print exception error
            return ResultOperation<Uri>.ToFailedResult(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); // print exception error
            return ResultOperation<Uri>.ToFailedResult(ex.Message);
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

//class Verify
//{

//    static void Main(string[] args)
//    {
//        try
//        {
//            string url = "https://gateway.zibal.ir/v1/verify"; // url
//            Zibal.verifyRequest Request = new Zibal.verifyRequest(); // define Request
//            Request.merchant = "zibal"; // String
//            Request.trackId = "TRACK ID IN MAKEREQUEST() RESPONSE PARAMETERS"; // String 
//            var httpResponse = Zibal.HttpRequestToZibal(url, JsonConvert.SerializeObject(Request));  // get Response
//            using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) // make stream reader
//            {
//                var responseText = streamReader.ReadToEnd(); // read Response
//                Zibal.verifyResponse item = JsonConvert.DeserializeObject<Zibal.verifyResponse>(responseText); // Deserilize as response class object
//                                                                                                               // you can access paidAt time with item.paidAt , result with item.result , message with item.message , status with item.status and amount with item.amount
//            }
//        }
//        catch (WebException ex)
//        {
//            Console.WriteLine(ex.Message); // print exception error
//        }
//    }
//}


//class Request
//{
//    static void Main(string[] args)
//    {
//        try
//        {
//            string url = "https://gateway.zibal.ir/v1/request"; // url
//            Zibal.makeRequest Request = new Zibal.makeRequest(); // define Request
//            Request.merchant = "zibal"; // String
//            Request.orderId = "1000"; // String
//            Request.amount = 1000; //Integer
//            Request.callbackUrl = "http://callback.com/api"; //String
//            Request.description = "Hello Zibal !"; // String
//            var httpResponse = Zibal.HttpRequestToZibal(url, JsonConvert.SerializeObject(Request));  // get Response
//            using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) // make stream reader
//            {
//                var responseText = streamReader.ReadToEnd(); // read Response
//                Zibal.makeRequest_response item = JsonConvert.DeserializeObject<Zibal.makeRequest_response>(responseText); // Deserilize as response class object
//                                                                                                                           // you can access track id with item.trackId , result with item.result and message with item.message
//                                                                                                                           // in asp.net you can use Response.Redirect("https://gateway.zibal.ir/start/item.trackId"); for start gateway and redirect to third-party gateway page                                                                                                          // also you can use Response.Redirect("https://gateway.zibal.ir/start/item.trackId/direct"); for start gateway page directly
//            }
//        }
//        catch (WebException ex)
//        {
//            Console.WriteLine(ex.Message); // print exception error
//        }
//    }
//}


