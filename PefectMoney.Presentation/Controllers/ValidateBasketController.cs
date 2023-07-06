using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using PefectMoney.Presentation.Filters;
using PefectMoney.Presentation.Services;
using PefectMoney.Core.Settings;
using MediatR;

namespace PefectMoney.Presentation.Controllers;

public class ValidateBasketController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromServices] IMediator mediator  ,
        
        CancellationToken cancellationToken)
    {
      


       
        return Ok();
    }
}
