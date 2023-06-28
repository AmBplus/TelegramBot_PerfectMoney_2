using MediatR;
using Newtonsoft.Json;
using PefectMoney.Core.Settings;
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

    public record ZibalVerifyPaymentRequestDto : IRequest<ResultOperation>
    {
        public string TrackId { get; set; }
    }
    public class ZibalVerifyPaymentHandler : IRequestHandler<ZibalVerifyPaymentRequestDto, ResultOperation>
    {
        public ZibalVerifyPaymentHandler(ZibalPaymentSettings paymentSettings)
        {
            PaymentSettings = paymentSettings;
        }

        public ZibalPaymentSettings PaymentSettings { get; }

        public async Task<ResultOperation> Handle(ZibalVerifyPaymentRequestDto request, CancellationToken cancellationToken)
        {

            
                try
                {
                    string url = PaymentSettings.UrlVerifyPaymentRequest; // url
                    Zibal.verifyRequest Request = new Zibal.verifyRequest(); // define Request
                    Request.merchant = PaymentSettings.Merchant; // String
                    Request.trackId = request.TrackId; // String 
                    var httpResponse = Zibal.HttpRequestToZibal(url, JsonConvert.SerializeObject(Request));  // get Response
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) // make stream reader
                    {
                        var responseText = streamReader.ReadToEnd(); // read Response
                        Zibal.verifyResponse result = JsonConvert.DeserializeObject<Zibal.verifyResponse>(responseText); // Deserilize as response class object
                                                                                                                         // you can access paidAt time with item.paidAt , result with item.result , message with item.message , status with item.status and amount with item.amount
                    var resultCode = int.Parse(result.status);
                    if(resultCode == ZibalVerifyPaymentResultStatus.Success.Id || resultCode == ZibalVerifyPaymentResultStatus.AlreadySuccess.Id)
                         {
                        return ResultOperation.ToSuccessResult();
                    }
                    else
                    {
                        return ResultOperation.ToFailedResult();
                    }
                    }
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.Message); // print exception error
                    return ResultOperation.ToFailedResult();
                }
                catch (Exception ex) {
                Console.WriteLine(ex.Message); // print exception error
                return ResultOperation.ToFailedResult();
                }

        }



        public class ZibalVerifyPaymentResultStatus    : Enumeration<int, string>
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
