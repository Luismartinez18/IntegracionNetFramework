using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationWS.Utils.Interfaces
{
    public interface ISobjectCRUD<TEntity> where TEntity : class
    {
        Task<string> getSobjectsAsync(string loginResult, string sobject);
        Task<string> addSobjectAsync(string loginResult, TEntity entity, string sobject);
        Task<string> getSobjectByIdAsync(string loginResult, string Id, string sobject);
        Task<string> updateSobjectByIdAsync(string loginResult, TEntity entity, string id, string sobject);
        Task<string> deleteSobjectByIdAsync(string loginResult, string Id, string sobject);
        Task<string> rawQuery(string loginResult, TEntity entity, string id, string sobject);
        Task<string> rawQuery2(string loginResult, TEntity entity, string id, string sobject);
        Task<string> rawQuery3(string loginResult, TEntity entity, string id, string sobject);
        Task<string> PricebookentryId(string loginResult, string Product2Id, string Pricebook2Id, string sobject);
        Task<string> rawQuery5(string loginResult, TEntity entity, string id, string sobject);
        Task<string> rawQueryAsset(string loginResult, TEntity entity, string id, string sobject);        
        Task<string> TeamMemberId(string loginResult, string acc, string userId, string sobject, string obj);
        Task<string> BulkApi(string loginResult, string sobject, string operation);
        Task<string> rawQuery6(string loginResult, TEntity entity, string id, string sobject);
    }
}
