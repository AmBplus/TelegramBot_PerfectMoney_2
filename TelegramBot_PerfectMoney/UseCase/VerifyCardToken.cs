
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;
using TelegramBot_PerfectMoney.Settings;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using TelegramBot_PerfectMoney.Helper;
using TelegramBot_PerfectMoney.Exceptions;

namespace TelegramBot_PerfectMoney.UseCase
{
    public class VerifyCardToken : IVerifyCardToken
    {
        public VerifyCardToken(IConfiguration configuration)
        {

            VerifyAccountSettings = configuration.GetSection(nameof(VerifyAccountSettings)).Get<VerifyAccountSettings>(); 
            restClient = new RestClient(VerifyAccountSettings.Address);
        }

        private string token ;
        private string refreshToken;
        private RestClient restClient;
        private RestRequest restRequest;
        private DateTime? DateExpireTokenLifeTime { get; set; }
        
        private VerifyAccountSettings VerifyAccountSettings { get; }

       

        public async Task<string> GetToken()
        {
            if(token == null)
            return  await GenerateTokenSetRefreshToken();
            if (DateExpireTokenLifeTime == null)
            {
                return await GenerateTokenSetRefreshToken();
            }

            //DateTime lastGetToken = new DateTime(LastGetTokenTime.Year, LastGetTokenTime.Month, LastGetTokenTime.Day, LastGetTokenTime.Hour, LastGetTokenTime.Minute, LastGetTokenTime.Second, LastGetTokenTime.Millisecond) ;
            if(TimeHelper.DateTimeNow < DateExpireTokenLifeTime)
            {
                return  await GetTokenWithRefreshToken();
            }
            return token;

        }

        
        private async Task<string> GenerateTokenSetRefreshToken()
        {

            string authenticationString = GetAuthenticationString();
            restRequest = GenerateVerifyRestRequest(authenticationString);
            var result =  await restClient.PostAsync<ResponseGetTokenDto>(restRequest);
            if(result!.Status == FinnotechVerifyCardStatus.DONE.Name )
            {
                token = result.Result.Value;
                refreshToken = result.Result.RefreshToken;
                var tokenLifeTime = TimeSpan.FromMilliseconds(result.Result.LifeTime);
                DateExpireTokenLifeTime = new DateTime().Date.Add(tokenLifeTime).AddMinutes(5);
                return token;
            }
            throw new Exception(result.Error.Message);
           
        }
        private string GetAuthenticationString()
        {
            string clientId = VerifyAccountSettings.ClientId;
            string clientSecret = VerifyAccountSettings.ClientSecret;
            
            string authString = $"{clientId}:{clientSecret}";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authString));
        }
        private RestRequest GenerateVerifyRestRequest(string authenticationString)
        {
            
            RestRequest restRequest = new RestRequest("/dev/v2/oauth2/token");
            // Add the headers to the request object
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Authorization", $"Basic {authenticationString}");
            restRequest.AddBody("grant_type", "client_credentials");
            restRequest.AddBody("nid", VerifyAccountSettings.Nid);
            restRequest.AddBody("scopes", VerifyAccountSettings.Scopes);

            return restRequest;

        }
        private RestRequest GenerateVerifyRefreshTokenRestRequest(string authenticationString)
        {

            RestRequest restRequest = new RestRequest("/dev/v2/oauth2/token");
            // Add the headers to the request object
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("Authorization", $"Basic {authenticationString}");
            //Add the body to the request object
            restRequest.AddBody("grant_type", "refresh_token");
            restRequest.AddBody("token_type", "CLIENT-CREDENTIAL");
            restRequest.AddBody("refresh_token", GetRefreshToken().Result);

            return restRequest;

        }
        public async Task<string> GetTokenWithRefreshToken()
        {
            string authenticationString = GetAuthenticationString();
            restRequest = GenerateVerifyRefreshTokenRestRequest(authenticationString);
            var result = await restClient.PostAsync<ResponseGetTokenDto>(restRequest);
            if (result!.Status == FinnotechVerifyCardStatus.DONE.Name)
            {
                token = result.Result.Value;
                refreshToken = result.Result.RefreshToken;
                var tokenLifeTime = TimeSpan.FromMilliseconds(result.Result.LifeTime);
                DateExpireTokenLifeTime = new DateTime().Date.Add(tokenLifeTime).AddMinutes(5);
                return token;
            }
            throw new Exception(result.Error.Message);
        }
        public async Task<string> GetNewToken()
        {
            return await GetTokenWithRefreshToken();
        }

        

        public async Task<string> GetRefreshToken()
        {
            if (refreshToken == null)
            {
                if(token == null)
                {
                     await GenerateTokenSetRefreshToken();
                     return refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
                }
                throw new Exception("Bussiness Error");
            }
              
            return refreshToken!;
        }
    }
    public class ResponseGetTokenDto
    {
        [JsonPropertyName("result")]
        
        public ResultGetTokenResponseDto Result { get; set; }
        [JsonPropertyName("error")]
        public ErrorGetTokenResponseDto Error { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("trackId")]
        public string TrackId { get; set; }
    }
    public class ResultGetTokenResponseDto
    {
        [JsonPropertyName("scopes")]

        public string[] Scopes { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("lifeTime")]
        public long LifeTime { get; set; }
        [JsonPropertyName("creationDate")]
        public long CreationDate { get; set; }
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

    }
    public class ErrorGetTokenResponseDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
    
        [JsonPropertyName("message")]
        public string Message { get; set; }

    }
    public interface IVerifyCardToken
    {

        public  Task<string> GetToken();
        public  Task<string> GetRefreshToken();
        public  Task<string> GetNewToken();
    }
}
