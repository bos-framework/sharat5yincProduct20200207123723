using BOS.Auth.Client.ServiceExtension;
using BOS.IA.Client.ServiceExtension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BOS.StarterCode.Helpers.HttpClientFactories;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BOS.StarterCode.Services
{
    public class MultitenantService : IMultitenantService
    {
        public readonly IConfiguration _configuration;
        public readonly IHttpContextAccessor _contextAccessor;
        public MultitenantService()
        {
        }

        public MultitenantService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }
        public async Task<TokenResponse> ClientIdClientSecretToGenerateToken()
        {
            //string appPath = "https://jyoproduct20200206142215.dev.bosframework.com";
            string appPath = string.Format("{0}://{1}", "https", _contextAccessor.HttpContext.Request.Host);
            TokenRequestParameters requestParameters = new TokenRequestParameters();
            requestParameters.url = appPath;
            requestParameters.clientId = _configuration["AppCredentials:ClientId"];
            requestParameters.clientSecret = _configuration["AppCredentials:ClientSecret"];
            var baseUrl = _configuration["BOS:ServiceBaseURL"] + "" + _configuration["BOS:MultiTenancyRelativeURL"];
            TokenResponse result = await ApiClientFactory.Instance.ValidateURLClientIdClientSecret(baseUrl, requestParameters);
            return result;
        }

        public async Task<TokenResponse> GetGeneratedToken()
        {
            TokenResponse response = new TokenResponse();
            try
            {
                bool isFetch = true;
                var token = _contextAccessor.HttpContext.Session.GetString("ApplicationToken");
                if (token != null)
                {
                    response = JsonConvert.DeserializeObject<TokenResponse>(token);
                    if (response != null)
                    {
                        isFetch = false;
                    }
                }
                if (isFetch)
                {
                    response = await ClientIdClientSecretToGenerateToken();
                    if (!string.IsNullOrEmpty(response.data))
                    {
                        _contextAccessor.HttpContext.Session.SetString("ApplicationToken", JsonConvert.SerializeObject(response));
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return response;
        }
    }

    public class TokenResponse
    {
        public string data { get; set; }
        public string message { get; set; }
    }

    public class TokenParameters
    {
        public string Token { get; set; }
        public string URL { get; set; }
        public string ApplicationId { get; set; }
        public string TenantId { get; set; }
        public string AccountId { get; set; }
        public string InstanceId { get; set; }
        public string IssuedAt { get; set; }
        public string ExpiresBy { get; set; }
    }

    public class TokenRequestParameters
    {
        public string url { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
    }
}
