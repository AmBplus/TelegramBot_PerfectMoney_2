using Microsoft.Extensions.Configuration;

using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json.Serialization;
using PefectMoney.Shared.Utility.ResultUtil;
using PefectMoney.Shared.Utility;
using Microsoft.Extensions.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using PefectMoney.Core.Settings;
using PefectMoney.Core.UseCase.Notify;

namespace PefectMoney.Core.UseCase.VerifyCard
{
    public interface IVerifyUserCard
    {
        public Task<ResultOperation> Handle(VerifyUserCardRequestDto request, CancellationToken cancellationToken);
    }
    public record VerifyUserCardRequestDto : IRequest<ResultOperation>
    {
        public string phoneNumber;
        public string cartNumber;
        public string trackId;
    }
    public class VerifyUserCardHandler : IRequestHandler<VerifyUserCardRequestDto, ResultOperation>, IVerifyUserCard
    {

        public VerifyUserCardHandler(IOptions<VerifyBankCardSettings> configuration, IMediator mediator,
            IOptions<BotSettings> botsettings, ILogger<VerifyUserCardHandler> logger)
        {
            VerifyAccountSettings = configuration.Value;
            Botsettings = botsettings.Value;
            Mediator = mediator;
            Logger = logger;
            RestClient = new RestClient(botsettings.Value.ExternalAppBaseUrl);

        }
        RestClient RestClient { get; set; }
        VerifyBankCardSettings VerifyAccountSettings { get; set; }
        public BotSettings Botsettings { get; }
        public IMediator Mediator { get; }
        public ILogger<VerifyUserCardHandler> Logger { get; }

        public async Task<ResultOperation> Handle(VerifyUserCardRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
               
                RestRequest restRequest = GenerateVerifyRestRequest(request.phoneNumber, request.cartNumber, request.trackId);

                var result = await RestClient.PostAsync<VerifyUserCardResponseDto>(restRequest, cancellationToken);

                if (result?.Result?.IsValid == true)
                {
                    return ResultOperation.ToSuccessResult();
                }

                if (result?.Status == "FAILED")
                {
                    // if Token Unvalid Generate Token
                    if (result.Error.Code == "UNAUTHORIZED")
                    {

                        return ResultOperation.ToFailedResult("خطای سیستمی بعدا امتحان نمایید");
                    }
                }

                var matchToVerifyUserCardResponseErrorCode = VerifyUserCardResponseErrorCode.VerifyMatchToError(result?.ResponseCode);
                if (matchToVerifyUserCardResponseErrorCode != null)
                {
                    if (matchToVerifyUserCardResponseErrorCode.Status == TypeErrorCodeStatus.UserError)
                    {

                        Logger.LogError(matchToVerifyUserCardResponseErrorCode.Name);
                        
                        return ResultOperation.ToFailedResult(matchToVerifyUserCardResponseErrorCode.Name);
                    }
                    else if (matchToVerifyUserCardResponseErrorCode.Status == TypeErrorCodeStatus.DestinationSystemError)
                    {
                        Logger.LogError(matchToVerifyUserCardResponseErrorCode.Name);
                        await Mediator.Publish(new NotifyAdminRequest($"{matchToVerifyUserCardResponseErrorCode.Name}"));
                        return ResultOperation.ToFailedResult("در سیستم احراز هویت مشکلی پیش آمده صبور باشید و بعد امتحان نمایید");
                    }
                    Logger.LogError(matchToVerifyUserCardResponseErrorCode.Name);
                    await Mediator.Publish(new NotifyAdminRequest($"{matchToVerifyUserCardResponseErrorCode.Name}"));
                    return ResultOperation.ToFailedResult("مشکل سیستمی به وجود آمده به ادمین اطلاع دهید");
                }
                Logger.LogError($"{result.Error}--{result.Status}{result.ResponseCode}");
                await Mediator.Publish(new NotifyAdminRequest($"{result.Error}--{result.Status}{result.ResponseCode}"));
                return ResultOperation.ToFailedResult("خطای سیستمی به وجود آمده به ادمین اطلاع دهید");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message,e.InnerException?.Message);
                await Mediator.Publish(new NotifyAdminRequest($"{e.Message}---{e.InnerException?.Message}"));
                return ResultOperation.ToFailedResult("خطای سیستمی به وجود آمده به ادمین اطلاع دهید");
            }
        }

        private RestRequest GenerateVerifyRestRequest(string phoneNumber, string cartNumber, string trackId)
        {
            RestRequest restRequest = new RestRequest($"{VerifyAccountSettings.VerifyCardlink}");
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddJsonBody(new
            {
                mobile = phoneNumber,
                cart = cartNumber,
                token = Botsettings.TokenExternalApp,
                trackId,
            });

            return restRequest;

        }


    }
    public class VerifyUserCardResponseErrorCode : Enumeration<string, string>
    {
        public TypeErrorCodeStatus Status { get; set; }
        public static VerifyUserCardResponseErrorCode UnValidNationalCode = new("FN-KCFH-40000530009", "کد ملی معتبر نیست", TypeErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidNationalCode1 = new("FN-KCFH-40000130009", "کد ملی معتبر نیست", TypeErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidNationalCode2 = new("FN-KCFH-40000630009", "کد ملی معتبر نیست", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static VerifyUserCardResponseErrorCode UnValidNationalCodeAndMobileNumber = new("FN-KCFH-40000130058", "شماره موبایل یا کد ملی نا معتبر است", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static VerifyUserCardResponseErrorCode NationalCodeIsNecessary = new("FN-KCFH-400006300127", "پارامتر کد ملی الزامی است", TypeErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode NationalCodeIsNecessary2 = new("FN-KCFH-40001030060", "کد ملی الزامی است ", TypeErrorCodeStatus.ApplicationError);
        public static VerifyUserCardResponseErrorCode NationalCodeIsNecessary3 = new("FN-KCFH-40000230060", "کد ملی الزامی است", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode MobileNumberIsNecessary = new("FN-KCFH-40000230041", "موبایل اجباری است", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode UnValidMobileNumber = new("FN-KCFH-400005300578", "شماره موبایل  نا معتبر است", TypeErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidMobileNumber1 = new("FN-KCFH-40000530057", "شماره موبایل  نا معتبر است", TypeErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidMobileNumber2 = new("FN-KCFH-40000130057", "شماره موبایل  نا معتبر است", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode NationalForDeathPerson = new("FN-KCFH-40000830046", "کد ملی ارسال شده برای شخص فوت شده است", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode NetworkError = new("FN-KCAA-50000860000", "خطای داخل شبکه", TypeErrorCodeStatus.DestinationSystemError);
        public static VerifyUserCardResponseErrorCode NetworkError1 = new("FN-KCAA-50000960000", "خطای داخل شبکه", TypeErrorCodeStatus.DestinationSystemError);
        public static VerifyUserCardResponseErrorCode NetworkError2 = new("FN-KCHR-50001060000", "خطای داخل شبکه", TypeErrorCodeStatus.DestinationSystemError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode AuthenticationRequestNotFound = new("FN-KCFH-40400030038", "درخواست احراز هویت یافت نشد", TypeErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode AuthenticationRequestNotFound2 = new("FN-KCFH-40400730038", "درخواست احراز هویت یافت نشد", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------


        public static VerifyUserCardResponseErrorCode UserInBankBlackList = new("FN-KCKZ-40000430037", "کاربر در لیست سیاه بانک است ", TypeErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UserInBankBlackList2 = new("FN-KCKZ-20000530037", "کاربر در لیست سیاه بانک است ", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode ValidationError = new("FN-KCFH-40000530039", "Validation Error", TypeErrorCodeStatus.ApplicationError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode NationalCodeAndMobileNumberDoNotMatch = new("FN-KCFH-200001000005", "کد ملی و شماره موبایل منطبق نیست ", TypeErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public VerifyUserCardResponseErrorCode(string id, string messsage, TypeErrorCodeStatus status) : base(id, messsage)
        {
            Status = status;
        }


        public static VerifyUserCardResponseErrorCode? VerifyMatchToError(string errorId)
        {
            foreach (var item in GetAll<VerifyUserCardResponseErrorCode>())
            {
                if (item.Id == errorId)
                {
                    return item;
                }
            }
            return null;

        }
    }


    public class VerifyUserCardResponseDto
    {
        [JsonPropertyName("responseCode")]
        public string ResponseCode { get; set; }

        [JsonPropertyName("trackId")]
        public string TrackId { get; set; }

        [JsonPropertyName("result")]
        public VerifyUserCardResponseResultDto Result { get; set; }

        [JsonPropertyName("error")]
        public VerifyUserCardResponseErrorDto Error { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

    }
    public class VerifyUserCardResponseResultDto
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }
    }
    public class VerifyUserCardResponseErrorDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("message")]
        public string message { get; set; }
    }

  
}
