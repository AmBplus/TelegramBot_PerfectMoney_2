
//using RestSharp;
//using System.Text.Json.Serialization;
//using Microsoft.Extensions.Configuration;
//using PefectMoney.Shared.Helper;
//using Microsoft.Extensions.Options;
//using System.Security.Cryptography;
//using static System.Formats.Asn1.AsnWriter;
//using Microsoft.Extensions.Logging;
//using PefectMoney.Core.Settings;

//namespace PefectMoney.Core.UseCase.VerifyCard
//{
//    public class VerifyCardToken : IVerifyCardToken
//    {
//        public VerifyCardToken(IOptions<VerifyBankCardSettings> configuration,ILogger<VerifyCardToken> logger)
//        {

//            VerifyAccountSettings = configuration.Value;
            
//            restClient = new RestClient(VerifyAccountSettings.Address);
//            Logger = logger;
//        }

//        private string token;
//        private string refreshToken;
//        private RestClient restClient;
//        private RestRequest restRequest;
//        private DateTime? DateExpireTokenLifeTime { get; set; }

//        private VerifyBankCardSettings VerifyAccountSettings { get; }
//        public ILogger<VerifyCardToken> Logger { get; }

//        public async Task<string> GetToken()
//        {
//            if (token == null)
//                return await GenerateTokenSetRefreshToken();
//            if (DateExpireTokenLifeTime == null)
//            {
//                return await GenerateTokenSetRefreshToken();
//            }

//            //DateTime lastGetToken = new DateTime(LastGetTokenTime.Year, LastGetTokenTime.Month, LastGetTokenTime.Day, LastGetTokenTime.Hour, LastGetTokenTime.Minute, LastGetTokenTime.Second, LastGetTokenTime.Millisecond) ;
//            if (TimeHelper.DateTimeNow < DateExpireTokenLifeTime)
//            {
//                return await GetTokenWithRefreshToken();
//            }
//            return token;

//        }


//        private async Task<string> GenerateTokenSetRefreshToken()
//        {

//            string authenticationString = GetAuthenticationString();
//            restRequest = GenerateVerifyRestRequest(authenticationString);
//            try
//            {
//                var result = await restClient.PostAsync<ResponseGetTokenDto>(restRequest);
//                if (result!.Status == FinnotechVerifyCardStatus.DONE.Name)
//                {
//                    token = result.Result.Value;
//                    refreshToken = result.Result.RefreshToken;
//                    var tokenLifeTime = TimeSpan.FromMilliseconds(result.Result.LifeTime);
//                    DateExpireTokenLifeTime = new DateTime().Date.Add(tokenLifeTime).AddMinutes(5);
//                    return token;
//                }

//                Logger.LogError(result.Error.Message);
//                throw new Exception(result.Error.Message);
//            }
//            catch (Exception)
//            {
//                throw;
//            }
            
            

//        }
//        private string GetAuthenticationString()
//        {
//            string clientId = VerifyAccountSettings.ClientId;
//            string clientSecret = VerifyAccountSettings.ClientSecret;

//            string authString = $"{clientId}:{clientSecret}";
//            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authString));
//        }
//        private RestRequest GenerateVerifyRestRequest(string authenticationString)
//        {

//            RestRequest restRequest = new RestRequest("/dev/v2/oauth2/token");
//            // Add the headers to the request object
        
//            restRequest.AddHeader("Authorization", $"Basic {authenticationString}");
//            restRequest.AddJsonBody(
//                new
//                {
//                    grant_type = "client_credentials",
//                    nid = VerifyAccountSettings.Nid,
//                    scopes = VerifyAccountSettings.Scopes

//                }
//                );
    

//            return restRequest;

//        }
//        private RestRequest GenerateVerifyRefreshTokenRestRequest(string authenticationString)
//        {

//            RestRequest restRequest = new RestRequest("/dev/v2/oauth2/token");
//            // Add the headers to the request object
            
//            restRequest.AddHeader("Authorization", $"Basic {authenticationString}");
//            //Add the body to the request object
//            restRequest.AddJsonBody(
//         new
//         {
//             grant_type = "refresh_token",
//             token_type = "CLIENT-CREDENTIAL",
//             refresh_token = GetRefreshToken().Result

//         }
//         );

     
//            return restRequest;

//        }
//        public async Task<string> GetTokenWithRefreshToken()
//        {
//            string authenticationString = GetAuthenticationString();
//            restRequest = GenerateVerifyRefreshTokenRestRequest(authenticationString);
//            try
//            {
//                var result = await restClient.PostAsync<ResponseGetTokenDto>(restRequest);
//                if (result!.Status == FinnotechVerifyCardStatus.DONE.Name)
//                {
//                    token = result.Result.Value;
//                    refreshToken = result.Result.RefreshToken;
//                    var tokenLifeTime = TimeSpan.FromMilliseconds(result.Result.LifeTime);
//                    DateExpireTokenLifeTime = new DateTime().Date.Add(tokenLifeTime).AddMinutes(5);
//                    return token;
//                }
//                throw new Exception(result.Error.Message);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        public async Task<string> GetNewToken()
//        {
//            return await GetTokenWithRefreshToken();
//        }



//        public async Task<string> GetRefreshToken()
//        {
//            if (refreshToken == null)
//            {
//                if (token == null)
//                {
//                    await GenerateTokenSetRefreshToken();
//                    return refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
//                }
//                throw new Exception("Bussiness Error");
//            }

//            return refreshToken!;
//        }
//    }
//    public class ResponseGetTokenDto
//    {
//        [JsonPropertyName("result")]

//        public ResultGetTokenResponseDto Result { get; set; }
//        [JsonPropertyName("error")]
//        public ErrorGetTokenResponseDto Error { get; set; }
//        [JsonPropertyName("status")]
//        public string Status { get; set; }
//        [JsonPropertyName("trackId")]
//        public string TrackId { get; set; }
//    }
//    public class ResultGetTokenResponseDto
//    {
//        [JsonPropertyName("scopes")]

//        public string[] Scopes { get; set; }
//        [JsonPropertyName("value")]
//        public string Value { get; set; }
//        [JsonPropertyName("lifeTime")]
//        public long LifeTime { get; set; }
//        [JsonPropertyName("creationDate")]
//        public long CreationDate { get; set; }
//        [JsonPropertyName("refreshToken")]
//        public string RefreshToken { get; set; }

//    }
//    public class ErrorGetTokenResponseDto
//    {
//        [JsonPropertyName("code")]
//        public string Code { get; set; }

//        [JsonPropertyName("message")]
//        public string Message { get; set; }

//    }
//    public interface IVerifyCardToken
//    {

//        public Task<string> GetToken();
//        public Task<string> GetRefreshToken();
//        public Task<string> GetNewToken();
//    }
//}
