using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace PefectMoney.Presentation.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FinotechVerifyController : ControllerBase
    {
        public FinotechVerifyController(IMediator mediator)
        {
         
            Mediator = mediator;
        }
        public IMediator Mediator { get; }


    }

}
