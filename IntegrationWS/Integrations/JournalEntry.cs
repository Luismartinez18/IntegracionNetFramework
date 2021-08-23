using IntegrationWS.Data;
using IntegrationWS.DynamicsGPService;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Web;

namespace IntegrationWS.Integrations
{
    public class JournalEntry : IJournalEntry
    {
        public async Task<bool> ExistsAccount(string Account,string InterId)
        {
            using (var db = new BnrdDbContextToRemove()) 
              return await db.Database.SqlQuery<bool>($"exec {InterId}_sp_ValidateAccount '{Account}' ").AnyAsync();
            
        }

        public Task<bool> ExistsJournalEntry(int JournalId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsPayroll(string Reference, string InterId)
        {
            
            using (var db = new BnrdDbContextToRemove())
                return await db.Database.SqlQuery<bool>($"exec {InterId}_sp_PayrollValidity '{Reference}' ").AnyAsync();
        }

        public async Task<int> GetNumJournalEntry(string InterId)
        {
            using (var db = new BnrdDbContextToRemove())
                return await db.Database.SqlQuery<int>($"exec {InterId}_sp_SearchEntryNumber ").FirstOrDefaultAsync();
        }

        public async Task<string> InsertLineJournalEntry(JournalEntryHeader add)
        {
            int counted = 16384;
            string Error = "";
            foreach (var item in add.Detail)
            {
                using (var db = new BnrdDbContextToRemove())
                Error =  await db.Database.SqlQuery<string>($"exec {add.InterId}_SP_InsertarDetalleEntradaDeDiario '{add.BatchKey}',{add.JournalId},{counted},'{item.Account}',{item.DebitAmount},{item.CreditAmount}").FirstOrDefaultAsync();
                counted = counted + 16384;
                if (Error != "SUCCESS")
                    return Error;
            }
            return "SUCCESS";
        }
        public async Task<string> SendDynamics(JournalEntryHeader add)
        {
            try
            {
               
                CompanyKey companyKey = new CompanyKey { Id = add.CompanyCode };
                Context context = new Context() { OrganizationKey = companyKey };
                Policy policy;
                DynamicsGPClient wsDynamicsGP = new DynamicsGPClient();
    
                var GLtrans = new GLTransaction();
                List<GLTransactionLine> Details = new List<GLTransactionLine>();
                var batchKey = new BatchKey();
                batchKey.Id = add.BatchKey;
                batchKey.Source = "GL_Normal";
                var transactionKey = new GLTransactionKey();
                transactionKey.JournalId = add.JournalId;
                transactionKey.Date = add.DocDate.Value.Date;
                //transactionKey.CompanyKey = companyKey;
                GLtrans.Key = transactionKey;
                GLtrans.BatchKey = batchKey;
                GLtrans.ExchangeDate = (DateTime)Convert.ToDateTime("1900-01-01");
                GLtrans.Reference = add.Payroll;
                GLtrans.SourceDocumentKey = new SourceDocumentKey();
                GLtrans.SourceDocumentKey.Id = "DG";
                GLtrans.LedgerType = GLLedgerType.Base;
                GLtrans.OriginatingDocument = new GLOriginatingDocument();
                GLtrans.OriginatingDocument.Series = SeriesType.Financial;
                GLtrans.OriginatingDocument.PostedDate = add.DocDate.Value.Date;
                GLtrans.TransactionState = GLTransactionState.Work;
                
              ///  GLtrans.Lines = Details.ToArray();
                policy = wsDynamicsGP.GetPolicyByOperation("CreateGLTransaction", context);
                wsDynamicsGP.CreateGLTransaction(GLtrans, context, policy);
                if (wsDynamicsGP.State != CommunicationState.Faulted)
                    wsDynamicsGP.Close();
                await InsertLineJournalEntry(add);
                return "Procesada";
            }
            catch (SecurityNegotiationException se)
            {
                return se.Message.ToString();
            }
            catch (FaultException<ExceptionDetail> fe)
            {
                return fe.Detail?.Message.ToString();
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
    }
}