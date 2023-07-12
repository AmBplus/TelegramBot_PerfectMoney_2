using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using PefectMoney.Presentation.Filters;
using PefectMoney.Presentation.Services;
using PefectMoney.Core.Settings;
using MediatR;
using PefectMoney.Core.UseCase._Shop;
using PefectMoney.Core.UseCase.ZibalPayment;

namespace PefectMoney.Presentation.Controllers;
[Route("validatebasket")]
public class ValidateBasketController : ControllerBase
{
    [HttpPost]
    
    public async Task<IActionResult> Post([FromBody] ValidateBasketRequest request
        ,
        [FromServices] IMediator mediator  ,
        CancellationToken cancellationToken)
    {

        var result = await mediator.Send(new ValidateBasketRequest( request.status, request.trackId,request.success,request.orderId));
        var response = ResultHandler.MapToResultHandler(result);
        return Ok(response);
    }
    public class ValidateBasketControllerRequest
    {
        [FromBody]
        public string success { get; set; }
        [FromBody]
        public string status { get; set; }
        [FromBody]
        public string trackId { get; set; }
        [FromBody]
        public string orderId { get; set; }
    }
}
