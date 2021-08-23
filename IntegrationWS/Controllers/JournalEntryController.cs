using IntegrationWS.DTOs;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Models;
using IntegrationWS.Utils;
using IntegrationWS.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace IntegrationWS.Controllers
{
    public class JournalEntryController : ApiController
    {
        private readonly IJournalEntry _journalEntry;
        public JournalEntryController(IJournalEntry journalEntry)
        {
            _journalEntry = journalEntry;
        }
        // GET: JournalEntry
        public IHttpActionResult Index()
        {
            return Ok();
        }
        [System.Web.Http.Route("api/JournalEntry/Add")]
        [HttpPost]
        public async Task<IHttpActionResult> Add(JournalEntryHeader add)
        {
            if(!ModelState.IsValid)
                return BadRequest("Entrada de diario incorrecta");

            decimal? TotalDebit = add.Detail.Sum(x => x.DebitAmount);
            decimal? TotalCredit = add.Detail.Sum(x => x.CreditAmount);
            if(TotalCredit != TotalDebit)
                return BadRequest("Total de debito y credito no cuadran diferencia: "+decimal.Round((decimal)(TotalCredit- TotalDebit),2).ToString());

            int _CompanyCode = 1;
            string InterId = "BNRD";
            switch (add.CompanyCode)
            {
                case 1:
                    _CompanyCode = 1;
                    InterId = "BNRD";
                    break;
                case 2:
                    _CompanyCode = 9;
                    InterId = "IPRE";
                    break;
                case 4:
                    _CompanyCode = 8;
                    InterId = "ULAB";
                    break;
                default:
                    InterId = "";
                    _CompanyCode = 0;
                    break;
            }
            if (_CompanyCode == 0)
                return BadRequest("Este compañia no esta configurada en el sistema ERP");
            add.InterId = InterId;
            if (await _journalEntry.ExistsPayroll(add.Payroll, add.InterId))
                return BadRequest($"El número de Nomina: ({add.Payroll}) existe en el sistema ERP ");

            foreach (var item in add.Detail)
               if (!await _journalEntry.ExistsAccount(item.Account,add.InterId))
                   return BadRequest($"La cuenta {item.Account} no existe en el sistema ERP ");              
            
            add.JournalId = await _journalEntry.GetNumJournalEntry(add.InterId);
            var data =await _journalEntry.SendDynamics(add);
            if(data== "Procesada")
                return Ok("Procesada");
            else return BadRequest(data);
        }
    }
}