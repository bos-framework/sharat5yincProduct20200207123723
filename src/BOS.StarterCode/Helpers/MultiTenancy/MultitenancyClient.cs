using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using BOS.StarterCode.Helpers.MultiTenancy;
using System;
using BOS.StarterCode.Services;
using System.Threading.Tasks;

namespace BOS.StarterCode.Helpers.HttpClientFactories
{
    public partial class ApiClient
    {
        public List<WhiteLabel> GetApplicationConfiguration(string baseurl)
        {
            List<WhiteLabel> response = new List<WhiteLabel>();
            if (!string.IsNullOrEmpty(baseurl))
            {
                Uri appUri = new Uri(baseurl);
                string appPath = string.Format("{0}://{1}", appUri.Scheme, appUri.Host);
                string queryString = "?$filter=URL eq '" + appPath + "'&api-version=1.0";
                var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "WhiteLabels"), queryString);
                try
                {
                    var jsonResult =  GetSync<JObject>(requestUrl);
                    if (jsonResult["value"] != null && jsonResult["value"].Count() > 0)
                    {
                        response = JsonConvert.DeserializeObject<List<WhiteLabel>>(jsonResult["value"].ToString());
                    }
                }
                catch (Exception) { }
            }
            return response;
        }

        public  async Task<TokenResponse> ValidateURLClientIdClientSecret(string baseurl, TokenRequestParameters requestParameters)
        {
            TokenResponse result = new TokenResponse();
            if (!string.IsNullOrEmpty(baseurl))
            {
                var requestUrl = CreateRequestUri(string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "Applications/ValidateURLClientIdClientSecret"));
                try
                {
                    var jsonResult =await GetTokenAsync<TokenRequestParameters>(requestUrl, requestParameters);
                    TokenParameters response = JsonConvert.DeserializeObject<TokenParameters>(jsonResult);
                    if (response != null && response.Token!=null)
                    {
                        result.data = response.Token;
                        //result.data = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI1ZWNiZDlmMi04NGNhLTRlMTgtYjBhYS1kYTA5NjhhOWIyZDUiLCJzdWIiOiJCT1NNdWx0aXRlbmFuY3kiLCJpYXQiOjE1ODEwNTM0NjEsImFjY291bnQiOiI3YjU3ZjQ4Ni04YzAwLTQwY2MtYTEzYy0wN2IyYWNhNmI1MmEiLCJwcm9qZWN0IjoiOTgzZTdlYzgtMjJlMS00NWNlLTk2OTAtNjlmM2RkNjIyOTE0IiwidGVuYW50IjoiMDcyNTM4ZDUtNjA0Ny00N2E4LTkzZDMtMjJjMTU0Y2NhMTAxIn0.3vUnNT5t7375VSdJQYmxTl_pUsaShGtto9ItFuhk0b0";
                        result.message = "Token Generated";
                    }
                    else if(jsonResult.Contains("passed is incorrect."))
                    {
                        result.message = "ClientId or ClientScrete is incorrect";
                    }
                    else if (jsonResult.Contains("does not exist."))
                    {
                        result.message = "Website Url does not exists in the system";
                    }
                }
                catch (Exception ex) {
                }
            }
            return result;
        }
    }
}
