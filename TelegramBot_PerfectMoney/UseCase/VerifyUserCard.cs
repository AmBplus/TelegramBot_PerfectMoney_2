﻿using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;
using TelegramBot_PerfectMoney.Settings;
using TelegramBot_PerfectMoney.Utility;
using TelegramBot_PerfectMoney.Utility.ResultUtil;

namespace TelegramBot_PerfectMoney.UseCase
{
    public interface IVerifyUserCard
    {
        public Task<ResultOperation> Verify(string phoneNumber, string cartNumber, string trackId, CancellationToken cancellationToken = default);
    }
    public class VerifyUserCard : IVerifyUserCard
    {
        
        public VerifyUserCard(IConfiguration configuration, IVerifyCardToken verifyCartToken)
        {
            VerifyAccountSettings = configuration.GetSection(nameof(VerifyAccountSettings)).Get<VerifyAccountSettings>();
            VerifyCartToken = verifyCartToken;
            RestClient = new RestClient(VerifyAccountSettings.Address);
            
        }
        RestClient RestClient { get; set; }
        VerifyAccountSettings VerifyAccountSettings { get; set; }
       
        public IVerifyCardToken VerifyCartToken { get; }

        public async Task<ResultOperation> Verify(string phoneNumber, string cartNumber,string trackId, CancellationToken  cancellationToken = default)
        {
            var token = await VerifyCartToken.GetToken();
            RestRequest restRequest = GenerateVerifyRestRequest(phoneNumber, cartNumber, trackId, token);

            var result =  await  RestClient.PostAsync<VerifyUserCardResponseDto>(restRequest,cancellationToken);

            if(result.Result.IsValid)
            {
                return  ResultOperation.ToSuccessResult();
            }

            if(result.Status == "FAILED")
            {
                // if Token Unvalid Generate Token
               if(result.Error.Code == "UNAUTHORIZED")
                {
                    var newToken = await VerifyCartToken.GetNewToken();
                    restRequest = GenerateVerifyRestRequest(phoneNumber, cartNumber, trackId, newToken);
                    result = await RestClient.PostAsync<VerifyUserCardResponseDto>(restRequest);
                    if (result.Result.IsValid)
                    {
                        return ResultOperation.ToSuccessResult();
                    }
                }
            }
            
            var matchToVerifyUserCardResponseErrorCode = VerifyUserCardResponseErrorCode.VerifyMatchToError(result.ResponseCode);
           if (matchToVerifyUserCardResponseErrorCode != null)
            {
                if(matchToVerifyUserCardResponseErrorCode.Status == VerifyUserCardResponseErrorCodeStatus.UserError)
                {
                    return ResultOperation.ToFailedResult(matchToVerifyUserCardResponseErrorCode.Name);
                }
                if (matchToVerifyUserCardResponseErrorCode.Status == VerifyUserCardResponseErrorCodeStatus.DestinationSystemError)
                {
                    return ResultOperation.ToFailedResult("در سیستم احراز هویت مشکلی پیش آمده صبور باشید و بعد امتحان نمایید");
                }

                return ResultOperation.ToFailedResult("مشکل سیستمی به وجود آمده به ادمین اطلاع دهید");
            }
            return ResultOperation.ToFailedResult("خطای سیستمی به وجود آمده به ادمین اطلاع دهید");
        }

        private RestRequest GenerateVerifyRestRequest(string phoneNumber, string cartNumber, string trackId, string token)
        {
            RestRequest restRequest = new RestRequest($"/kyc/v2/clients/{VerifyAccountSettings.ClientId}/mobileCardVerification?trackId={trackId}");
            restRequest.Authenticator = new JwtAuthenticator(token);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddBody("mobile", phoneNumber);
            restRequest.AddBody("cart", cartNumber);
            return restRequest;

        }
    }
    public enum VerifyUserCardResponseErrorCodeStatus
    {
        UserError ,
        DestinationSystemError,
        ApplicationError
        
    }
    public class VerifyUserCardResponseErrorCode : Enumeration<string,string>
    {
        public VerifyUserCardResponseErrorCodeStatus Status { get; set; }
        public static VerifyUserCardResponseErrorCode UnValidNationalCode = new("FN-KCFH-40000530009", "کد ملی معتبر نیست",VerifyUserCardResponseErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidNationalCode1 = new("FN-KCFH-40000130009", "کد ملی معتبر نیست", VerifyUserCardResponseErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidNationalCode2 = new("FN-KCFH-40000630009", "کد ملی معتبر نیست", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static VerifyUserCardResponseErrorCode UnValidNationalCodeAndMobileNumber = new("FN-KCFH-40000130058", "شماره موبایل یا کد ملی نا معتبر است", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static VerifyUserCardResponseErrorCode NationalCodeIsNecessary = new("FN-KCFH-400006300127", "پارامتر کد ملی الزامی است", VerifyUserCardResponseErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode NationalCodeIsNecessary2 = new("FN-KCFH-40001030060", "کد ملی الزامی است ", VerifyUserCardResponseErrorCodeStatus.ApplicationError);
        public static VerifyUserCardResponseErrorCode NationalCodeIsNecessary3 = new("FN-KCFH-40000230060", "کد ملی الزامی است", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode MobileNumberIsNecessary = new("FN-KCFH-40000230041", "موبایل اجباری است", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode UnValidMobileNumber = new("FN-KCFH-400005300578", "شماره موبایل  نا معتبر است", VerifyUserCardResponseErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidMobileNumber1 = new("FN-KCFH-40000530057", "شماره موبایل  نا معتبر است", VerifyUserCardResponseErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UnValidMobileNumber2 = new("FN-KCFH-40000130057", "شماره موبایل  نا معتبر است", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode NationalForDeathPerson = new("FN-KCFH-40000830046", "کد ملی ارسال شده برای شخص فوت شده است", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode NetworkError = new ( "FN-KCAA-50000860000", "خطای داخل شبکه", VerifyUserCardResponseErrorCodeStatus.DestinationSystemError);
        public static VerifyUserCardResponseErrorCode NetworkError1 = new("FN-KCAA-50000960000", "خطای داخل شبکه",VerifyUserCardResponseErrorCodeStatus.DestinationSystemError);
        public static VerifyUserCardResponseErrorCode NetworkError2 = new("FN-KCHR-50001060000", "خطای داخل شبکه", VerifyUserCardResponseErrorCodeStatus.DestinationSystemError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode AuthenticationRequestNotFound = new ("FN-KCFH-40400030038", "درخواست احراز هویت یافت نشد", VerifyUserCardResponseErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode AuthenticationRequestNotFound2 = new("FN-KCFH-40400730038", "درخواست احراز هویت یافت نشد", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------


        public static VerifyUserCardResponseErrorCode UserInBankBlackList = new("FN-KCKZ-40000430037", "کاربر در لیست سیاه بانک است ", VerifyUserCardResponseErrorCodeStatus.UserError);
        public static VerifyUserCardResponseErrorCode UserInBankBlackList2 = new("FN-KCKZ-20000530037", "کاربر در لیست سیاه بانک است ", VerifyUserCardResponseErrorCodeStatus.UserError) ;
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode ValidationError = new ("FN-KCFH-40000530039", "Validation Error", VerifyUserCardResponseErrorCodeStatus.ApplicationError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static VerifyUserCardResponseErrorCode NationalCodeAndMobileNumberDoNotMatch = new("FN-KCFH-200001000005", "کد ملی و شماره موبایل منطبق نیست ", VerifyUserCardResponseErrorCodeStatus.UserError);
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public VerifyUserCardResponseErrorCode(string id, string messsage, VerifyUserCardResponseErrorCodeStatus status) : base(id, messsage)
        {
            Status = status;
        }


        public static VerifyUserCardResponseErrorCode? VerifyMatchToError(string errorId)
        {
            foreach (var item in GetAll<VerifyUserCardResponseErrorCode>())
            {
                if(item.Id == errorId)
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
    public  class VerifyUserCardResponseResultDto
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