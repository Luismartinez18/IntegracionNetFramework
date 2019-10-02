using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Utils
{
    public class AuthToSalesforce : IAuthToSalesforce
    {
        private string authToken;
        private string serviceURL;
        private readonly string sfdcConsumerKey;
        private readonly string sfdcConsumerSecret;
        private string error;
        private string error_description;
        private readonly string sfdcUserName;
        private readonly string password;
        private readonly string token;
        private readonly string version;
        private readonly string loginPassword;
        private readonly IStringCipher _stringCipher;

        public AuthToSalesforce(IStringCipher stringCipher)
        {
            //Esto es recomendable sacarlo a un archivo de configuración o a una variable de ambiente del IIS
            string SecretInformation = "sdadKNJA0.KJAJHAJ@@@###kjhKHADJS";

            _stringCipher = stringCipher;
            authToken = string.Empty;
            serviceURL = string.Empty;
            sfdcConsumerKey = _stringCipher.Decrypt(ConfigurationManager.AppSettings["sfdcConsumerKey"], SecretInformation);
            sfdcConsumerSecret = _stringCipher.Decrypt(ConfigurationManager.AppSettings["sfdcConsumerSecret"], SecretInformation);
            error = string.Empty;
            error_description = string.Empty;
            sfdcUserName = _stringCipher.Decrypt(ConfigurationManager.AppSettings["SalesforceUsername"], SecretInformation);
            password = _stringCipher.Decrypt(ConfigurationManager.AppSettings["SalesforcePassword"], SecretInformation);
            token = _stringCipher.Decrypt(ConfigurationManager.AppSettings["token"], SecretInformation);
            version = ConfigurationManager.AppSettings["VersionAPI"].ToString();
            loginPassword = password + token;
        }

        public async Task<string> Login()
        {
            var dictionaryForLogin = new Dictionary<string, string>
            {
                {"grant_type","password"},
                {"client_id",sfdcConsumerKey},
                {"client_secret",sfdcConsumerSecret},
                {"username",sfdcUserName},
                {"password",loginPassword}
            };

            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent httpContent = new FormUrlEncodedContent(dictionaryForLogin);
                HttpResponseMessage message = await httpClient.PostAsync(ConfigurationManager.AppSettings["UrlForLogin"].ToString(), httpContent);
                string responseString = await message.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(responseString);
                authToken = (string)obj["access_token"];
                serviceURL = (string)obj["instance_url"];
                error = (string)obj["error"];
                error_description = (string)obj["error_description"];

                if (string.IsNullOrEmpty(authToken))
                    return "Invalid login attempt to salesforce.";

                return responseString;
            }
        }

        public async Task<string> Logout(string token, string serviceURL)
        {
            var dictionaryForLogout = new Dictionary<string, string>
            {
                {"token", token}
            };

            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent httpContent = new FormUrlEncodedContent(dictionaryForLogout);
                HttpResponseMessage message = await httpClient.PostAsync($"{serviceURL}/services/oauth2/revoke", httpContent);
                string responseString = await message.Content.ReadAsStringAsync();
                return responseString;
            }
        }
    }
}