using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Utils
{
    public class SobjectCRUD<TEntity> : ISobjectCRUD<TEntity> where TEntity : class
    {
        private readonly IResponseAfterAuth _responseAfterAuth;

        public SobjectCRUD(IResponseAfterAuth responseAfterAuth)
        {
            _responseAfterAuth = responseAfterAuth;
        }

        public async Task<string> addSobjectAsync(string loginResult, TEntity entity, string sobject)
        {
            string result = string.Empty;
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);
            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/sobjects/{sobject}";

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(entity);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, restCallURL);
                request.Headers.Add("Authorization", "Bearer " + convertResponse.authToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);

                result = await response.Content.ReadAsStringAsync();

                return result;
            }
        }

        public async Task<string> deleteSobjectByIdAsync(string loginResult, string Id, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;
            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/sobjects/{sobject}/{Id}";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    result = "Ok";
                }

                return result;
            }
        }

        public async Task<string> getSobjectByIdAsync(string loginResult, string Id, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;
            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/sobjects/{sobject}/{Id}";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }

                return result;
            }
        }

        public async Task<string> getSobjectsAsync(string loginResult, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;
            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/sobjects/{sobject}";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }

                return result;
            }
        }

        public async Task<string> updateSobjectByIdAsync(string loginResult, TEntity entity, string id, string sobject)
        {           
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);
            var json = JsonConvert.SerializeObject(entity);
            string result = string.Empty;
            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/sobjects/{sobject}/{id}";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), restCallURL);
                request.Headers.Add("Authorization", "Bearer " + convertResponse.authToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);

                result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    result = "Ok";
                }

                return result;
            }
        }

        public async Task<string> rawQuery(string loginResult, TEntity entity, string id, string sobject)
        {
            if (id.Contains("+"))
            {
                id = id.Replace("+", "%2B");
            }

            if (id.Contains("#"))
            {
                id = id.Replace("#", "%23");
            }


            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;

            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+id+from+{sobject}+WHERE+Id_External__c+LIKE+'{id.Trim()}'";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    if(sobject == "Asset" && !result.Contains("Id"))
                    {
                        return "Serie Duplicada";
                    }

                    JObject obj2 = JObject.Parse(result);
                    var records =  obj2["records"];
                    var totalSize = obj2["totalSize"];

                    if(totalSize.Value<string>() == "0")
                    {
                        return "";
                    }
                    result = records[0].Value<string>("Id");
                }

                return result;
            }
        }

        public async Task<string> rawQueryAsset(string loginResult, TEntity entity, string id, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;

            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+id+from+{sobject}+WHERE+Id+LIKE+{id.Trim()}";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    JObject obj2 = JObject.Parse(result);
                    var records = obj2["records"];
                    result = records[0].Value<string>("Id");
                }

                return result;
            }
        }

        public async Task<string> rawQuery2(string loginResult, TEntity entity, string id, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;

            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+id+from+{sobject}+WHERE+Factura_Dynamics__c+LIKE+'{id.Trim()}'";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    JObject obj2 = JObject.Parse(result);
                    var records = obj2["records"];
                    result = records[0].Value<string>("Id");
                }

                return result;
            }
        }

        //Pricebook
        public async Task<string> rawQuery3(string loginResult, TEntity entity, string id, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;

            if (id.Contains("+"))
            {
                id = id.Replace("+", "%2B");
            }

            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+id+from+{sobject}+WHERE+PRCSHID__c+LIKE+'{id.Trim()}'";
            

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    JObject obj2 = JObject.Parse(result);
                    var records = obj2["records"];
                    result = records[0].Value<string>("Id");
                }

                return result;
            }
        }

        //Pricebookentry
        public async Task<string> PricebookentryId(string loginResult, string Product2Id, string Pricebook2Id, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;

            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+id+from+Pricebookentry+WHERE+Product2Id+%3D+'{Product2Id.Trim()}'+AND+Pricebook2Id+%3D+'{Pricebook2Id}'";


            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    JObject obj2 = JObject.Parse(result);
                    var records = obj2["records"];
                    result = records[0].Value<string>("Id");
                }

                return result;
            }
        }

        public async Task<string> rawQuery5(string loginResult, TEntity entity, string id, string sobject)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;
            
            string restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+Lista_de_precios__c+from+Account+WHERE+Id+LIKE+'{id.Trim()}'";


            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    JObject obj2 = JObject.Parse(result);
                    var records = obj2["records"];
                    result = records[0].Value<string>("Id");
                }

                return result;
            }
        }

        public async Task<string> TeamMemberId(string loginResult, string acc, string userId, string sobject, string obj)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);

            string result = string.Empty;
            string restCallURL = string.Empty;

            if (obj == "opp")
            {
                restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+Id+from+OpportunityTeamMember+WHERE+OpportunityId+%3D+'{acc.Trim()}'+AND+userId+%3D+'{userId}'";
            }
            else if(obj == "acc")
            {
                restCallURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/query/?q=SELECT+Id+from+AccountTeamMember+WHERE+AccountId+%3D+'{acc.Trim()}'+AND+userId+%3D+'{userId}'";
            }           


            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, restCallURL);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + convertResponse.authToken);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                    JObject obj2 = JObject.Parse(result);
                    var records = obj2["records"];
                    result = records[0].Value<string>("Id");
                }

                return result;
            }
        }

        public async Task<string> BulkApi(string loginResult, string sobject, string operation)
        {
            var convertResponse = _responseAfterAuth.convertResponse(loginResult);
            string result = string.Empty;

            //Create a Bulk Job
            var bulkJboURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/jobs/ingest";

            BulkApi bulkApi = new BulkApi();
            bulkApi.@object = "Prueba__c";
            bulkApi.contentType = "CSV";
            bulkApi.operation = "insert";
            bulkApi.lineEnding = "CRLF";

            using(var  client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", convertResponse.authToken);
                using (var response = await client.PostAsJsonAsync(bulkJboURL, bulkApi))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    JObject obj2 = JObject.Parse(content);
                    result = obj2.Value<string>("id");
                }
            }        

            //Enviar la data
            var sendDataBulkUrl = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/jobs/ingest/{result}/batches";

            List<Prueba> pruebas = new List<Prueba>()
            {
                new Prueba
                {
                    Nombre__c = "Wilfredo",
                    Apellido__c = "Burgos Cedeño",
                    Edad__c = 28
                },
                new Prueba
                {
                    Nombre__c = "William",
                    Apellido__c = "Burgos Cedeño",
                    Edad__c = 30
                },
                new Prueba
                {
                    Nombre__c = "Wendy",
                    Apellido__c = "Burgos Cedeño",
                    Edad__c = 2
                }
            };

            string csv = CsvSerializer.SerializeToCsv(pruebas);

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, sendDataBulkUrl);
                request.Headers.Add("Authorization", "Bearer " + convertResponse.authToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
                //request.Headers.Add("content-type", "text/csv");
                request.Content = new StringContent(csv, Encoding.Default,"text/csv");
                request.Content.Headers.ContentType.CharSet = null;



                HttpResponseMessage response = await client.SendAsync(request);

                var result1 = await response.Content.ReadAsStringAsync();
            }            

            //Cerrar Job e insertar la data
            var cerrarJobURL = $"{convertResponse.serviceURL}/services/data/{convertResponse.version.ToLower()}/jobs/ingest/{result}";

            var cerrarJob = "{\"state\" : \"UploadComplete\"}";

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), cerrarJobURL);
                request.Headers.Add("Authorization", "Bearer " + convertResponse.authToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = new StringContent(cerrarJob, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);

                result = await response.Content.ReadAsStringAsync();
            }

            return result;
        }
    }
}