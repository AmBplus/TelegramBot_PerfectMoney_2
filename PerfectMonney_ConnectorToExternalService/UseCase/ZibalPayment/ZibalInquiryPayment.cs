//using MediatR;
//using Newtonsoft.Json;
//using PefectMoney.Core.Settings;
//using PefectMoney.Shared.Utility.ResultUtil;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using static PefectMoney.Core.UseCase.ZibalPayment.ZibalVerifyPaymentHandler;

//namespace PefectMoney.Core.UseCase.ZibalPayment
//{
//    public record ZibalInquiryPaymentRequest :IRequest<ResultOperation>
//    {
//        public string TrackId { get; set; }
//    }

//    public class ZibalInquiryPaymentHandler : IRequestHandler<ZibalInquiryPaymentRequest, ResultOperation>
//    {
//        public ZibalPaymentSettings PaymentSettings { get; }

//        public ZibalInquiryPaymentHandler(ZibalPaymentSettings paymentSettings)
//        {
//            PaymentSettings = paymentSettings;
//        }

//        public async Task<ResultOperation> Handle(ZibalInquiryPaymentRequest request, CancellationToken cancellationToken)
//        {

//            try
//            {
//                string url = PaymentSettings.UrlVerifyPaymentRequest; // url
//                Zibal.verifyRequest Request = new Zibal.verifyRequest(); // define Request
//                Request.merchant = PaymentSettings.Merchant; // String
//                Request.trackId = request.TrackId; // String 
//                var httpResponse = Zibal.HttpRequestToZibal(url, JsonConvert.SerializeObject(Request));  // get Response
//                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) // make stream reader
//                {
//                    var responseText = streamReader.ReadToEnd(); // read Response
//                    Zibal.verifyResponse result = JsonConvert.DeserializeObject<Zibal.verifyResponse>(responseText); // Deserilize as response class object
//                                                                                                                     // you can access paidAt time with item.paidAt , result with item.result , message with item.message , status with item.status and amount with item.amount
//                    var resultCode = int.Parse(result.status);
//                    if (resultCode == ZibalVerifyPaymentResultStatus.Success.Id || resultCode == ZibalVerifyPaymentResultStatus.AlreadySuccess.Id)
//                    {
//                        return ResultOperation.ToSuccessResult();
//                    }
//                    else
//                    {
//                        return ResultOperation.ToFailedResult();
//                    }
//                }
//            }
//            catch (WebException ex)
//            {
//                Console.WriteLine(ex.Message); // print exception error
//                return ResultOperation.ToFailedResult();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message); // print exception error
//                return ResultOperation.ToFailedResult();
//            }


//            throw new NotImplementedException();
//        }
//    }
//}
