using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Telegram.Bot;

namespace PefectMoney.Presentation.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ZibalPaymentController : ControllerBase
    {
        public ZibalPaymentController(ITelegramBotClient botClient, IMediator mediator)
        {
            BotClient = botClient;
            Mediator = mediator;
        }

        public ITelegramBotClient BotClient { get; }
        public IMediator Mediator { get; }
        //        در صورت موفقیت‌آمیز بودن تراکنش 1، در غیر این‌صورت 0 می‌باشد.
        //trackId شناسه پیگیری جلسه‌ی پرداخت
        //orderId شناسه سفارش ارسال شده در هنگام درخواست پرداخت(در صورت ارسال)
        //status وضعیت پرداخت(از طریق جدول وضعیت‌ها می‌توانید مقادیر status را مشاهده نمایید)
        //https://yourcallbackurl.com/callback?trackId=9900&success=1&status=2&orderId=1
        [HttpGet]
        public async Task Get(RequestZibalPaymentCallBack request)
        {
            
            //if(request.success == false)
            //{
            //    // Code
            //}
            //ZibalVerifyPaymentRequestDto verifyRequest = new ZibalVerifyPaymentRequestDto
            //{
            //    TrackId = request.trackId
            //};
            //var result = await Mediator.Send(verifyRequest);
            //if(result.IsSuccess)
            //{
            //    // Code
            //}
            // Code
         }

    }
    public record RequestZibalPaymentCallBack
    {
        [FromQuery]
        public string trackId { get; set; }
        [FromQuery]
        public bool success { get; set; }
        [FromQuery]
        public int status { get; set; }
        [FromQuery]
        public int orderId { get; set; }
    }
}
