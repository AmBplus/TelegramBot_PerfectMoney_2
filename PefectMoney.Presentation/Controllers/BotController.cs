using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using PefectMoney.Presentation.Filters;
using PefectMoney.Presentation.Services;
using PefectMoney.Core.Settings;

namespace PefectMoney.Presentation.Controllers;

public class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromServices] UpdateHandlers handleUpdateService,
        
        CancellationToken cancellationToken)
    {
      


        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
