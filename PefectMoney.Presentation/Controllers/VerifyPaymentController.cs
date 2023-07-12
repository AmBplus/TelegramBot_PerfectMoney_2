using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using PefectMoney.Presentation.Filters;
using PefectMoney.Presentation.Services;
using PefectMoney.Core.Settings;
using PefectMoney.Shared.Utility.ResultUtil;
using PefectMoney.Core.UseCase.ZibalPayment;
using MediatR;
using PefectMoney.Core.UseCase.PerfectMoney;

namespace PefectMoney.Presentation.Controllers;
[Route("verifypayment")]
public class VerifyPaymentController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(
       
        ResultHandler<ZibalVerifyPaymentRequest> request,IMediator mediator,
        CancellationToken cancellationToken)
    {

        if(request.IsSuccess)
        {
            var requestToVerify = request.Data.MapToZibalVerifyPaymentResponseDto();
            var result = await mediator.Send(requestToVerify);
            if(result.IsSuccess)
            {
                
            await mediator.Send(new PerfectMoneyVoicherPayRequest() { Order = result.Data });
            }
        }
        return Ok();
    }
}

public record ZibalVerifyPaymentRequest
{
    [FromBody]
    public string paidAt { get; set; }
    [FromBody]
    public string createdAt { get; set; }
    [FromBody]
    public string cardNumber { get; set; }
    [FromBody]
    public string refNumber { get; set; }
    [FromBody]
    public string status { get; set; }
    [FromBody]
    public string description { get; set; }
    [FromBody]
    public string wage { get; set; }
    [FromBody]
    public string amount { get; set; }
    [FromBody]
    public string result { get; set; }
    [FromBody]
    public string message { get; set; }
    [FromBody]
    public bool HaveError { get; set; }
    [FromBody]
    public string errorMessage { get; set; }
    [FromBody]
    public string innerErrorMessage { get; set; }
    [FromBody]
    public long orderId { get; set; }

    public ZibalVerifyPaymentRequestDto MapToZibalVerifyPaymentResponseDto()
    {
        return new ZibalVerifyPaymentRequestDto
        {
            result = result,
            HaveError = HaveError,
            errorMessage = errorMessage,
            amount = amount,
            cardNumber = cardNumber,
            refNumber = refNumber,
            createdAt = createdAt,
            description = description,
            innerErrorMessage = innerErrorMessage,
            message = message,
            orderId = orderId,
            paidAt = paidAt,
            status = status,
            wage = wage
        };
    }
}