using IntegrationWS.Data;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Utils.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class Activos : IActivos
    {
        private readonly IResponseAfterAuth _responseAfterAuth;
        private readonly IAuthToSalesforce _authToSalesforce;
        private readonly ISobjectCRUD<Asset1> _sobjectCRUD;
        private readonly ISobjectCRUD<AssetToUpdate> _sobjectCRUD2;
        private readonly string sobject;

        public Activos( IAuthToSalesforce authToSalesforce,
                        ISobjectCRUD<Asset1> sobjectCRUD,
                        IResponseAfterAuth responseAfterAuth,
                        ISobjectCRUD<AssetToUpdate> sobjectCRUD2
                        )
        {
            _responseAfterAuth = responseAfterAuth;
            _authToSalesforce = authToSalesforce;
            _sobjectCRUD = sobjectCRUD;
            _sobjectCRUD2 = sobjectCRUD2;
            sobject = "Asset";
        }

        public async Task<string> create(string Id, string loginResult, string authToken, string serviceURL)
        {
            Asset1 asset = getOne(Id);

            if(asset == null)
            {
                return "No existe";
            }

            var result = await _sobjectCRUD.addSobjectAsync(loginResult, asset, sobject);

            if (result.Contains("DUPLICATE"))
            {

                JArray jsonArray = JArray.Parse(result);
                result = jsonArray[0].ToString();
                JObject obj3 = JObject.Parse(result);
                result = (string)obj3["message"];

                var salesforceId = result.Substring(result.IndexOf("Id.:") + 4, 16).Trim();

                result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, asset, salesforceId, sobject);                

                Asset asset1 = new Asset();
                asset1.DynamicsId = Id;
                asset1.SalesforceId = salesforceId;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    db.Asset.Add(asset1);
                    db.SaveChanges();
                }

                if (result != "Ok")
                {
                    return result;
                }

                return "actualizado";
            }
            else if (result.Contains("errorCode"))
            {
                return result;
            }

            return result;
        }

        public async Task<string> delete(string loginResult, string Id, string DynamicsId)
        {
            var result = await _sobjectCRUD.deleteSobjectByIdAsync(loginResult, Id, sobject);

            if (result != "Ok")
            {
                return result;
            }

            return "Ok";
        }

        public async Task<string> update(string Id, string loginResult, string authToken, string serviceURL)
        {
            Asset1 asset = getOne(Id);

            if (asset == null)
            {
                return "No existe";
            }

            string salesforceID = string.Empty;

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                salesforceID = db.Asset.Where(x => x.DynamicsId == Id).Select(x => x.SalesforceId).FirstOrDefault();
            }
            var result = await _sobjectCRUD.updateSobjectByIdAsync(loginResult, asset, salesforceID, sobject);

            if (result != "Ok")
            {
                return result;
            }

            return "Ok";
        }

        public Asset1 getOne(string Id)
        {
            Asset1 asset = new Asset1();

            using (DevelopmentDbContext db = new DevelopmentDbContext())
            {
                asset = db.Database.SqlQuery<Asset1>($"EXEC SP_GPSalesforce_Asset_V2 '{Id}'").FirstOrDefault();
            }

            return asset;
        }
    }
}