using IntegrationWS.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.ServiceModel;
using IntegrationWS.Utils;
using IntegrationWS.Models;
using IntegrationWS.Data;
using System.Web.Script.Serialization;
using IntegrationWS.ModelsNotMapped;
using IntegrationWS.Integrations;

namespace IntegrationWS.Controllers
{
    [RoutePrefix("api/PrintLabels")]
    public class PrintLabelsController : ApiController
    {
        // GET: PrintLabels
        [HttpPost]
        [Authorize]
        public IHttpActionResult PrintMaintPrevent([FromBody] PrintMaintenancePlan vm)
        {
            string Urel = "https://bionuclear.lightning.force.com/lightning/r/Product2/01t1C000005g8voQAA/view";
            string ZPLString = @"CT~~CD,~CC^~CT~
                                            ^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5~SD22^JUS^LRN^CI0^XZ
                                            ^XA
                                            ^MMT
                                            ^PW812
                                            ^LL0406
                                            ^LS0
                                            ^FO0,32^GFA,03584,03584,00056,:Z64:eJzt0rGK3DAQBuBxHKImSIG0wn6FS6dw5vxKG65dYi9bbOetA2nzDmkCWlxsquQFUlgo4FZLGhVCykj2GdInRUB/afyBZv4ByMnJycn5T0JsfXNE+trV5WH0zWM4Ggilh5ZZz0lHtG3BleFsW8Y7Mr0v7OJcffNEhujG5E4WwtED/hko7dgv2x19dN2Zd8wE4hZX8eJKpD7I6PqHnaAc9NgXlwH0QHl1N6nrCxAUbuePvHozDi8Xx9DRCd39cU6uqkDpvpDRVbx5pdT8DF0xoWtej9+eL4784dqd4BQmdIaCPnEuQMlZRifZDy7Kw/d+dbVCN0f3aXT3q3PgmZlXpy0IdpM0uavZnOe7UMh34xd05sl1ZxPKz8kpvzgX3ei2d7ra+EJ26Pq3q+uBDeBXNz2kd9J9dHK/7UW2di5k0B+SW/dSDjCji3v5SdNehn3cy1e+9SCFwPlwN2MvzFMPI7qDWXqgqYehST3wrXd0+pKcR0dS70VgVl9M6v3kcD7bDU3sna33YtDtuFIX+RjdTpQGB/PgmFXKpDvDL+hamu6MTP/28HNycnJy/l5+Aws8G0k=:1305
                                            ^FO0,64^GFA,00896,00896,00028,:Z64:eJxjYBgFgwrwMIOpBLjAASLlGEiVYzOccSxZgvcwe9vHNvmEBz/uMD8+XNzAf/gAXI7/Mf/xn232iR/+/GF//vj/AfnjD+ByPMUyzyTbCpMleg7w6RVLHLA4VwCUY7bmeWO8gcdYIk2yLQGo6AAPkH3A4IwBSK6a9405XO5HDYpcNu8aM7jczz/IcoxerBoiC2ByjD0ocufYLWQenIG6pbPnwDmgWw5D3AKRO/+Y/5jkP/tEoJnHnz+WOAzxA8MZZgOeA+eBfpf4Jp/w482Bw48PSxzmO4wUaPQBAJJub4o=:BBFC
                                            ^FO0,0^GFA,03328,03328,00052,:Z64:eJzt1EtuwyAQANBBSGVTlRuEa3RlXyuLSqY34ygcgSULy9MZiE2MsVtXqroxSlD8eZ4wjAfgGn8zbr8w3WX2jBzPGz2dNwZ/Y+xp0yeDiPC2nJPpc2QcgCDjbjDgRF90yoKyBtF2wLNEjPzdGm84miUTtAXNP3xHl9ErxJFMSdZQTIpWmXRI97dNMPJhYjJ8qoN0IRus/psHQSaAdncQ2YwgyfBsHAzSQ78xEA2dl/4OcE8m0GM6oIVHTTtC14wtxi7GZfPB5jMAkKE5vmSjF2OejG8bqI3mpR3Hmc2yHjXBbhxYGVXqJMKStzrO2lRDeMn7sxOHd+/ZUBVqR0ZRHbTi0O1RVYay1ofZNOKw0WsjaEeH+E0cszYSPeB4sJ6GURhEMnt54xrt13lT/GZMx/vDeVobNMW096c2nJL9OP4Hpl1v5+K0TVmP2OZaZBM2Zs6bTEZl4+hloZ2mLOe5Mnl/Sg9hwwWWeohxjx7SqoPSq56NN2HYmrne+O5kLBv5aI5exzQ36tonY2+pa3PfSYfUQZ2OPNfvAuWEujNwwl+pn4xs3rl70lInSgzdz0+chINrXOMa/zC+AOW0G/k=:A6EF
                                            ^FO448,96^GFA,02048,02048,00016,:Z64:eJztlDGO3SAQQLGIxBYrkXKLaH2NLb5CjrK5QcoUX4ulXMx7E45A6QJ5MsPM4OGfIEUovnnmM/MYMM4VNzXftJf6bwTBhTsrbMzh5H/BzhybsARcD+Eq4fiZgd+7XIQlocTJcEq6TRhseuI+4IUT8ER/Di5Gj7haXg1/6etjgYj8TO+BBYi/9TwsQHW4dY+L792bBSJqNRFk3pdmBAKUcBiBADUe7hLAvFyZwEx5q7sE6FHcJfBT8nYBrMg7wFUx7FTx7ALEWlgSON1yJC18JPYt6sZ0gXAG3UhPYgHcdRBQLOq2d4GCQcvgjGNj27vAhu+u8RVozjYYt25R2d7ai4dm+LgFleut3qIJ71xJwYR3bo/e6Dm3xU8bHjPu58z1PrE73mZuX2e+zfiSHobjzB9xnxj8xAv8scvBiu8T4+psObD4ZRJYYZ8EMmyTABbXCiy034YDFtubhP0kGE7EzY7XqQKeij8EAixU/CGAcrTXQyBDzTRJGU8XzVUBPAntlTpVg8PZ+TqdQN+pCuDphSfqfNd0crTiAwdOmI4sF4ZwWYXlftqCnB1hXp7TCnygITML3AdzBX4PZoFfuELhHvZ93HS+a/242GlFtDQP/MoPvRm15IMzP/VmHKxnPXGcoFu5itcDr7r1gTupCHvu5KHHHfMld5350/rf/un2FyaxVws=:783C
                                            ^FT27,203^A0N,15,14^FH\^FDRepresentante Asignado:^FS
                            ^FO25,248^GB253,0,2^FS
                            ^FO300,306^GB255,0,2^FS
                            ^FO299,368^GB200,0,2^FS
                            ^FT300,267^A0N,18,21^FH\^FDFecha Ingreso a Taller^FS
                            ^FT25,266^A0N,18,21^FH\^FDFecha Ultimo Mantenimiento^FS
                            ^FT25,327^A0N,18,21^FH\^FDFabricante / Marca^FS
                            ^FT25,386^A0N,18,21^FH\^FDModelo^FS
                            ^FT300,325^A0N,18,21^FH\^FDNumero de Serie^FS
                            ^FT300,386^A0N,18,21^FH\^FDCliente^FS
                            ^FO25,307^GB253,0,2^FS
                            ^FO299,248^GB257,0,2^FS
                            ^FO25,367^GB253,0,2^FS
                            ^FT605,221^BQN,2,5
                            ^FH\^FDLA," + Urel.Replace("01t1C000005g8voQAA",vm.aId) + @"^FS
                            ^FT25,185^A0N,25,31^FH\^FD" + vm.VAR_ID_RST_Name_RST + @"^FS
                            ^FT25,245^A0N,24,26^FH\^FD" + vm.VAR_Date_Last_Mnt.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                            ^FT25,303^A0N,24,26^FH\^FD" + vm.Fabricante + @"^FS
                            ^FT299,245^A0N,24,26^FH\^FD" + vm.VAR_Date_Ingre.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                            ^FT25,364^A0N,24,26^FH\^FD" + vm.ActiveName + @"^FS
                            ^FT299,363^A0N,24,26^FH\^FD" + vm.AccountName + @"^FS
                            ^FT300,303^A0N,24,26^FH\^FD" + vm.Serie + @"^FS
                            ^FT22,146^A0N,37,45^FH\^FD" + vm.EQUIPO_EN_TALLER + @"^FS
                            ^FT666,229^A0N,14,16^FH\^FD" + vm.Serie + @"^FS
                            ^FO26,187^GB253,0,2^FS
                            ^FT578,364^A0N,18,26^FB187,1,0,C^FH\^FD" + vm.Propiedad + @"^FS
                            ^PQ1,0,1,Y^XZ"; // device-dependent string, need a FormFeed?
            try
            {
                string ipAddress = "192.168.4.250";
                int port = 9100;
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(ipAddress, port);

                // Write ZPL String to connection
                System.IO.StreamWriter writer =
                new System.IO.StreamWriter(client.GetStream());
                writer.Write(ZPLString);
                writer.Flush();

                // Close Connection
                writer.Close();
                //RawPrinterHelper.SendStringToPrinter("ZDesigner LP 2824", s);
                return Content(HttpStatusCode.OK, "Impresión realizada con exito");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message.ToString());
            }
         
        }

    }
}