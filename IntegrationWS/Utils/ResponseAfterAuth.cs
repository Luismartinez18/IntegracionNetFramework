using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace IntegrationWS.Utils
{
    public class ResponseAfterAuth : IResponseAfterAuth
    {
        public SfResponseModel convertResponse(string loginResult)
        {
            SfResponseModel sfResponse = new SfResponseModel();
            JObject obj = JObject.Parse(loginResult);
            sfResponse.authToken = (string)obj["access_token"];
            sfResponse.serviceURL = (string)obj["instance_url"];
            sfResponse.error = (string)obj["error"];
            sfResponse.error_description = (string)obj["error_description"];
            sfResponse.version = ConfigurationManager.AppSettings["VersionAPI"];

            return sfResponse;
        }
    }
}