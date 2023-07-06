using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using PefectMoney.Presentation.Filters;
using PefectMoney.Presentation.Services;
using PefectMoney.Core.Settings;
using MediatR;
using PefectMoney.Core.UseCase._Shop;
using PefectMoney.Core.UseCase.ZibalPayment;

namespace PefectMoney.Presentation.Controllers;

public class ValidateBasketController : ControllerBase
{
    [HttpPost]
    
    public async Task<IActionResult> Post(
        [FromBody] string trackId,
        [FromServices] IMediator mediator  ,
        
        CancellationToken cancellationToken)
    {

        var result = mediator.Send(new ValidateBasketRequest(trackId));
        
        return Ok(result);
    }
}
