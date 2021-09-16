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
        string Url = "https://bionuclear.lightning.force.com/lightning/r/Product2/01t1C000005g8voQAA/view";
        // GET: PrintLabels
        [HttpPost]
        [Authorize]
        [Route("PrintMaintPrevent")]
   
        public IHttpActionResult PrintMaintPrevent([FromBody] PrintMaintenancePlan vm)
        {

            //string ZPLString = @"CT~~CD,~CC^~CT~
            //                        ^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5~SD22^JUS^LRN^CI0^XZ
            //                        ^XA
            //                        ^MMT
            //                        ^PW812
            //                        ^LL0406
            //                        ^LS0
            //                        ^FO0,32^GFA,03584,03584,00056,:Z64:eJzt0rGK3DAQBuBxHKImSIG0wn6FS6dw5vxKG65dYi9bbOetA2nzDmkCWlxsquQFUlgo4FZLGhVCykj2GdKlSRP0l8YfaOYfgJycnJz/JsTWN0ekr11dHkbfPIajgVB6aJn1nHRE2xZcGc62Zbwj0/vCLs7VN09kiG5M7mQhHD3gn4HSjv2y3dFH1515x0wgbnEVL65E6oOMrn/YCcpBj31xGUAPlFd3k7q+AEHhdv7Iqzfj8HJxDB2d0N0f5+SqCpTuCxldxZtXSs3P0BUTuub1+O354sgfrt0JTmFCZyjoE+cClJxldJL94KI8fO9XVyt0c3SfRne/OgeemXl12oJgN0mTu5rNeb4LhXw3fkFnnlx3NqH8nJzyi3PRjW57p6uNL2SHrn+7uh7YAH5100N6J91HJ/fbXmRr50IG/SG5dS/lADO6uJefNO1l2Me9fOVbD1IInA93M/bCPPUwojuYpQeaehia1APfekenL8l5dCT1XgRm9cWk3k8O57Pd0MTe2XovBt2OK3WRj9HtRGlwMA+OWaVMujP8gq6l6c7I9G8PPycnJycn52/zG+uVG0k=:B7F3
            //                        ^FO0,64^GFA,00896,00896,00028,:Z64:eJxjYBgFDDzMYCoBLnCASDkGUuXYDGccS5bgPcze9rFNPuHBjzvMjw8XN/AfPgCX43/Mf/xnm33ihz9/2J8//n9A/vgDuBxPscwzybbCZImeA3x6xRIHLM4VAOWYrXneGG/gMZZIk2xLACo6wANkHzA4YwCSq+Z9Yw6X+1GDIpfNu8YMLvfzD7IcoxerhsgCmBxjD4rcOXYLmQdnoG7p7DlwDuiWwxC3QOTOP+Y/JvnPPhFo5vHnjyUOQ/zAcIbZgOfAeaDfJb7JJ/x4c+Dw48MSh/kOIwXayAMAlO1vig==:8DD8
            //                        ^FO0,0^GFA,03328,03328,00052,:Z64:eJzt1EtuwyAQANBBSGVTlRuEa3RlXyuLSqY34ygcgSULy9MZiE2MsVN3V8koQfHnecIwHoBrvB63P5juMntGjueNns4bg38x9rTpk0FE+FjOyfQ5Mg5AkHE3GHCiLzplQVmDaDvgWSJG/m6NNxzNkgnaguYfvqPL6BXiSKYkaygmRatMOqT72yYY+TAxGT7VQbqQDVb/zYMgE0C7O4hsRpBkeDYOBumh3xiIhs5Lfwe4JxPoMR3QwqOmHaFrxhZjF+Oy+WLzHQDI0BzfstGLMU/Gtw3URvPSjuPMZlmPmmA3DqyMKnUSYclbHWdtqiG85P3ZicO792yoCrUjo6gOWnHo9qgqQ1nrw2wacdjotRG0o0N8EcesjUQPOB6sp2EUBpHMXt64Rvt13hS/GdPx/nCe1gZNMe39qQ2nZD+O/4Vp19u5OG1T1iO2uRbZhI2Z8yaTUdk4ellopynLea5M3p/SQ9hwgaUeYtyjh7TqoPSqZ+NNGLZmrje+OxnLRj6ao9cxzY269snYW+ra3HfSIXVQpyPP9btAOaHuDJzwd+onI5tP7p601IkSQ/fzEyfh4BrXuMY/GT+Ubhv5:8B18
            //                        ^FT24,397^A0N,16,19^FH\^FDRepresentante de Servicios:^FS^FO25,187^GB253,0,2^FS^FO300,249^GB255,0,2^FS^FO299,309^GB200,0,2^FS
            //                        ^FT300,206^A0N,18,21^FH\^FDFecha Proximo Mantenimiento^FS
            //                        ^FT25,206^A0N,18,21^FH\^FDFecha Ultimo Mantenimiento^FS
            //                        ^FT25,270^A0N,18,21^FH\^FDFabricante / Marca^FS
            //                        ^FT25,327^A0N,18,21^FH\^FDModelo^FS
            //                        ^FT300,268^A0N,18,21^FH\^FDNumero de Serie^FS
            //                        ^FT300,327^A0N,18,21^FH\^FDNumero de Activo^FS
            //                        ^FO25,249^GB253,0,2^FS^FO299,187^GB257,0,2^FS^FO25,309^GB253,0,2^FS^FT599,231^BQN,2,5
            //                        ^FH\^FDLA," + Url.Replace("01t1C000005g8voQAA", vm.aId) + @"^FS
            //                        ^ BY2,3,44^FT506,334^BCN,,Y,N^FD>954335063333552415447^FS
            //                        ^FT267,397^A0N,16,16^FH\^FD"+ vm.VAR_ID_RST_Name_RST + @"^FS
            //                        ^FT25,183^A0N,23,28^FH\^FD"+ vm.VAR_FECHA_ULT_MANT.Value.Date.ToString("dd/MM/yyyy") + @"^FS
            //                        ^FT25,244^A0N,23,28^FH\^FD"+ vm.Fabricante + @"^FS
            //                        ^FT299,184^A0N,23,28^FH\^FD"+ vm.VAR_FECHA_PROX_MANT.Value.Date.ToString("dd/MM/yyyy") + @"^FS
            //                        ^FT25,304^A0N,23,28^FH\^FD"+ vm.VAR_MODELO + @"^FS
            //                        ^FT299,302^A0N,23,28^FH\^FD"+vm.VAR_ACTIVO + @"^FS
            //                        ^FT300,244^A0N,23,28^FH\^FD"+ vm.VAR_SERIE + @"^FS
            //                        ^FT24,138^A0N,34,40^FH\^FDMANTENIMIENTO PREVENTIVO^FS
            //                        ^FT649,240^A0N,16,19^FH\^FD" + vm.VAR_SERIE + @"^FS
            //                        ^PQ1,0,1,Y^XZ
            //                        "; // device-dependent string, need a FormFeed?
            string ZPLString = @"CT~~CD,~CC^~CT~
                                ^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5~SD22^JUS^LRN^CI0^XZ
                                ^XA
                                ^MMT
                                ^PW812
                                ^LL0406
                                ^LS0
                                ^FO0,32^GFA,03584,03584,00056,:Z64:eJzt0rGK3DAQBuBxHKImSIG0wn6FS6dw5vxKG65dYi9bbOetA2nzDmkCWlxsquQFUlgo4FZLGhVCykj2GdKlSRP0l8YfaOYfgJycnJz/JsTWN0ekr11dHkbfPIajgVB6aJn1nHRE2xZcGc62Zbwj0/vCLs7VN09kiG5M7mQhHD3gn4HSjv2y3dFH1515x0wgbnEVL65E6oOMrn/YCcpBj31xGUAPlFd3k7q+AEHhdv7Iqzfj8HJxDB2d0N0f5+SqCpTuCxldxZtXSs3P0BUTuub1+O354sgfrt0JTmFCZyjoE+cClJxldJL94KI8fO9XVyt0c3SfRne/OgeemXl12oJgN0mTu5rNeb4LhXw3fkFnnlx3NqH8nJzyi3PRjW57p6uNL2SHrn+7uh7YAH5100N6J91HJ/fbXmRr50IG/SG5dS/lADO6uJefNO1l2Me9fOVbD1IInA93M/bCPPUwojuYpQeaehia1APfekenL8l5dCT1XgRm9cWk3k8O57Pd0MTe2XovBt2OK3WRj9HtRGlwMA+OWaVMujP8gq6l6c7I9G8PPycnJycn52/zG+uVG0k=:B7F3
                                ^FO0,64^GFA,00896,00896,00028,:Z64:eJxjYBgFDDzMYCoBLnCASDkGUuXYDGccS5bgPcze9rFNPuHBjzvMjw8XN/AfPgCX43/Mf/xnm33ihz9/2J8//n9A/vgDuBxPscwzybbCZImeA3x6xRIHLM4VAOWYrXneGG/gMZZIk2xLACo6wANkHzA4YwCSq+Z9Yw6X+1GDIpfNu8YMLvfzD7IcoxerhsgCmBxjD4rcOXYLmQdnoG7p7DlwDuiWwxC3QOTOP+Y/JvnPPhFo5vHnjyUOQ/zAcIbZgOfAeaDfJb7JJ/x4c+Dw48MSh/kOIwXayAMAlO1vig==:8DD8
                                ^FO0,0^GFA,03328,03328,00052,:Z64:eJztlLFu2zAQhk+SDQVpUXZo0M0yMvYJOhi1H6Ka+woeM6gQCy8COuQVOhpa+goc+hpFNXozhw5EYJu9O8oSJTtBbKBdasI6kxJ/ffp5RwL8b+3qDM3LMzSjMzTJORz5jzhnaB7hRLvTOfFTmkc4wsqTOYk9nTO1CqO1O/kKrsslvCtLGUoI5TV2RjLECGX5Da+lx3EaqwR8slvIsRdLiFVi7SyBGIeBtRqvjcepnKYSFOUUJ5CGOtUIKEb4FXR1vw3fYnVCUeW1hjsJ8BDn07MeZ1d+N+IL0XCSERKEwoEZEdoIx2kXmP0EGgA5v/H1GQyyWFFnMERONgi0+AH0LOl+G6DGiKiCqHoPkLEfg7dGQJGw+Ex4HOk48yRQEFRZrVFzugUUcZYIKogbjqsD5iyIgyu6YT/MYRrNWvicmJaDOHceh/zoPefugOPquvWDMyZ7P2Pnp+aEjSaYO06ge34015vnp2lX7gwz4br2g5rJ3g/UnPXaLjzNG4DhDCGR1Yd+kPM5TQ3WgfU5uGpTHKKmavLT+IGmDmzUaoJcwVQfcOr8gKs30eXg64GKXIf2SH7wfNtzPD+B1aTpr1vrx1U31pvPeYhoYxzPD/p5uL0df3D5aavAvkXNsXpr8gM9Dn0qcUxTb35+lpwfOn16HNqAT9Ybcfw6iGtOZ/9MuvuH/UQdTdz6qXxOu396nIh2OvkpJPpZFV9dflbFvTJFYRK5oVgU97385Jif7c9fmJ/t1irmUCffYmZmHHHYq4Pc1OcbnUecHz5kuGrMtK6dDkdBovl8U5x0SRwe0mkYa46HdS1WzMHzmiKt2/5AVYJqrXrR5fD+wb80/QgDGGJ8Dfi7SVMeBJoiPcXcPbeNnz3z0i7t0v5K+wPDshbs:1561
                                ^FT24,397^A0N,16,19^FH\^FDRepresentante de Servicios:^FS
                                ^FO25,187^GB253,0,2^FS
                                ^FO300,249^GB255,0,2^FS
                                ^FO416,309^GB200,0,2^FS
                                ^FT300,206^A0N,18,21^FH\^FDFecha Pr\A2ximo Mantenimiento^FS
                                ^FT25,206^A0N,18,21^FH\^FDFecha Ultimo Mantenimiento^FS
                                ^FT25,270^A0N,18,21^FH\^FDFabricante / Marca^FS
                                ^FT25,327^A0N,18,21^FH\^FDModelo^FS
                                ^FT300,268^A0N,18,21^FH\^FDN\A3mero de Serie^FS
                                ^FT417,327^A0N,18,21^FH\^FDN\A3mero de Activo^FS
                                ^FO25,249^GB253,0,2^FS
                                ^FO299,187^GB257,0,2^FS
                                ^FO25,309^GB253,0,2^FS
                                ^FT599,231^BQN,2,5
                                ^FH\^FDLA," + Url.Replace("01t1C000005g8voQAA", vm.aId) + @"^FS
                                ^FT333,369^BQN,2,3
                                ^FH\^FDLA,N/A (Prop. Cliente)^FS
                                ^FT267,397^A0N,16,16^FH\^FD" + vm.VAR_ID_RST_Name_RST + @"^FS
                                ^FT25,185^A0N,24,26^FH\^FD" + vm.VAR_FECHA_ULT_MANT.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                                ^FT25,246^A0N,24,26^FH\^FD" + vm.Fabricante + @"^FS
                                ^FT299,186^A0N,24,26^FH\^FD" + vm.VAR_FECHA_PROX_MANT.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                                ^FT25,306^A0N,24,26^FH\^FD" + vm.VAR_MODELO + @"^FS
                                ^FT416,304^A0N,24,26^FH\^FD" + vm.VAR_ACTIVO + @"^FS
                                ^FT300,246^A0N,24,26^FH\^FD" + vm.VAR_SERIE + @"^FS
                                ^FT24,138^A0N,34,40^FH\^FDMANTENIMIENTO PREVENTIVO^FS
                                ^FT640,240^A0N,16,19^FH\^FD" + vm.VAR_SERIE + @"^FS
                                ^PQ1,0,1,Y^XZ";
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
        [Route("PrintEquipmentinWorkshop")]
        public IHttpActionResult PrintEquipmentinWorkshop([FromBody] EquipmentinWorkshop vm)
        {
   
            string ZPLString = @"CT~~CD,~CC^~CT~
                                ^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5~SD22^JUS^LRN^CI0^XZ
                                ^XA
                                ^MMT
                                ^PW812
                                ^LL0406
                                ^LS0
                                ^FO0,352^GFA,03200,03200,00100,:Z64:eJztzUERAAAEADBN9G+nASl4uK3AIgCAB7JX1dUBPDdQ+71j:D1B3
                                ^FO0,32^GFA,03584,03584,00056,:Z64:eJzt0rGK3DAQBuBxHKImSIG0wn6FS6dw5vxKG65dYi9bbOetA2nzDmkCWlxsquQFUlgo4FZLGhVCykj2GdInRUB/afyBZv4ByMnJycn5T0JsfXNE+trV5WH0zWM4Ggilh5ZZz0lHtG3BleFsW8Y7Mr0v7OJcffNEhujG5E4WwtED/hko7dgv2x19dN2Zd8wE4hZX8eJKpD7I6PqHnaAc9NgXlwH0QHl1N6nrCxAUbuePvHozDi8Xx9DRCd39cU6uqkDpvpDRVbx5pdT8DF0xoWtej9+eL4784dqd4BQmdIaCPnEuQMlZRifZDy7Kw/d+dbVCN0f3aXT3q3PgmZlXpy0IdpM0uavZnOe7UMh34xd05sl1ZxPKz8kpvzgX3ei2d7ra+EJ26Pq3q+uBDeBXNz2kd9J9dHK/7UW2di5k0B+SW/dSDjCji3v5SdNehn3cy1e+9SCFwPlwN2MvzFMPI7qDWXqgqYehST3wrXd0+pKcR0dS70VgVl9M6v3kcD7bDU3sna33YtDtuFIX+RjdTpQGB/PgmFXKpDvDL+hamu6MTP/28HNycnJy/l5+Aws8G0k=:1305
                                ^FO0,64^GFA,00896,00896,00028,:Z64:eJxjYBgFgwrwMIOpBLjAASLlGEiVYzOccSxZgvcwe9vHNvmEBz/uMD8+XNzAf/gAXI7/Mf/xn232iR/+/GF//vj/AfnjD+ByPMUyzyTbCpMleg7w6RVLHLA4VwCUY7bmeWO8gcdYIk2yLQGo6AAPkH3A4IwBSK6a9405XO5HDYpcNu8aM7jczz/IcoxerBoiC2ByjD0ocufYLWQenIG6pbPnwDmgWw5D3AKRO/+Y/5jkP/tEoJnHnz+WOAzxA8MZZgOeA+eBfpf4Jp/w482Bw48PSxzmO4wUaPQBAJJub4o=:BBFC
                                ^FO544,288^GFA,01024,01024,00032,:Z64:eJxjYBgFo4CmQP4/TvCBGPlRQEMAALawNes=:7903
                                ^FO0,0^GFA,03328,03328,00052,:Z64:eJztlDuO2zAQhkeSDS02QZgii3SWsWVOkMKIfYiozhVcbuFADNwISLFXSGmoyRVY5BpBVLozixTEwjYzM9SDkr0Pu0hlwhqTEn99+jlDAlza8+3qDM3rMzSjMzTJORz5nzhnaB7hRPvTOfFTmkc4wsqTOYk9nTO1CqO1e/kGrosVfCgKGUoI5TV2RjLECEXxA6+Vx3EaqwR8sTvIsBdLiFVi7SyBGIeBtRqvrccpnaYUFOUUJ5CGOuUIKEb4FXR1vw3fYnVCUWWVhjsJ8BDn07MeZ1/8NOIb0XCSERKEwoEZEdoIx2kXmP0EGgA5f/H1CxgsYkWdwRA5i0GgxS+gZ0n32wA1RkQlROVHgAX7MXhrBBQJi8+Ex5GOM08CBUG5qDRqTreAIs4SQQlxw3F1wJwlcXBFt+yHOUyjWUufE9NyEOfO45AfXXPuDjiurls/OGNS+xk7PxUnbDTB3HEC3fOjud48P027cmeYCTeVH9RMaj9QcTYbu/Q07wCGM4REVh/6Qc7XNDVYB9bn4KpNcYiasslP4weaOrBRqwkyBVN9wKnyA67eRJeDrwcqch3aI/nB863meH4Cq0nTX7fWj6turDef8xDRxjieH/TzcHs7/uTy01aBfY+aY/XW5Ad6HPpU4pim3vz8rDg/dPr0OLQBn6w34vh1EFeczv6ZdPcP+4k6mrj1U/qcdv/0OBHtdPKTS/Szzr+7/Kzze2Xy3CRySzHP73v5yTA/u99/MD+7nVXMoU62w8zMOOKwVweZqc43Oo84P3zIcNWYaVU7HY6CRPP5pjjpkjg8pNMw1hwP61qsmYPnNUVat/pAVYJqrXzV5fD+wb80/QwDGGJ8C/i7SVMeBJoiPcXcvbSNXzzz0i7t0p5t/wAbuhbs:57C1
                                ^FO448,64^GFA,02048,02048,00016,:Z64:eJztlLGOHCEMQM2hiO5ImeIkrr4USZliteTDIrHS/sj+ScifjK5KOUWKUTRax2CbwZGuTxEKZt96Bh7GAPDPtZe/uOiPd733eBH+XlsfsApj7yMuIC8Cx1fmsEt8Y47MCfkJid/LeGfOq8RRPBZlFsDBLFCqjC8BeS0gCzhhim8zU3yf2SMLtHX6/j8LtHjo47JAi8c+Lws46hMvoAt46rMIrj2+cipFwFN/nwQc7m6fBNq8d5gEUERVAGUhKlBkoSLwJaPJ0IeRKM7QKeKRgV8A34ImmgTwBuh1I7qAQ1dkI6GJeayfbsKFxNwogyaw0aC/BycSC7rNwCUyykBTN30PNH3G28G5zXEgnB9B5Xp7im4antYer9PwAJ99lerh9uyXeTpqa7oY3rLl06PBZmBatPgQ7efB2+nidTGc6mo4L5vhsp4M43ae0eFmBCi5RiDhYgQyVj8LFKqESeChVcYk0CthEoiNJ4Ge/EPgBVryh4ArX1vlDIGIPzNNPgQi7rHFVIAugHijpwpQXfdzpwJUjdc29lknR/zRx1FZxNfOLEBlzXstAlT2fC5EIOxJzg33cfsozAKpOmEWIFepLRZIbXfgEGi1ynEWeBpxFiDOHGeB9+Pmgx5/HnHQDGVbHJAl4C+SIeFQLUd5ys04WG/wyOM6PVpBPPVoefYIerScrGuUhmToDT1tBf63t9ofPuNJWw==:8652
                                ^FT27,203^A0N,15,14^FH\^FDRepresentante Asignado:^FS
                                ^FO25,248^GB253,0,2^FS
                                ^FO300,306^GB255,0,2^FS
                                ^FT300,267^A0N,18,21^FH\^FDFecha Ingreso a Taller^FS
                                ^FT25,266^A0N,18,21^FH\^FDFecha Ultimo Mantenimiento^FS
                                ^FT25,327^A0N,18,21^FH\^FDFabricante / Marca^FS
                                ^FT571,325^A0N,18,21^FH\^FDModelo^FS
                                ^FT300,325^A0N,18,21^FH\^FDN\A3mero de Serie^FS
                                ^FT29,390^A0N,18,21^FH\^FDCliente^FS
                                ^FO25,307^GB253,0,2^FS
                                ^FO299,248^GB257,0,2^FS
                                ^FT600,229^BQN,2,5
                                ^FH\^FDLA," + Url.Replace("01t1C000005g8voQAA", vm.aId) + @"^FS
                                ^FT25,185^A0N,25,31^FH\^FD" + vm.OwnerName + @"^FS
                                ^FT25,245^A0N,24,26^FH\^FD" + vm.VAR_FECHA_PROX_MANT.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                                ^FT25,303^A0N,24,26^FH\^FD" + vm.Fabricante + @"^FS
                                ^FT299,245^A0N,24,26^FH\^FD" + vm.VAR_Date_Ingre.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                                ^FT571,303^A0N,24,26^FH\^FD" + vm.VAR_ACTIVO + @"^FS
                                ^FT27,364^A0N,17,19^FH\^FD" + vm.AccountName + @"^FS
                                ^FT300,303^A0N,24,26^FH\^FD" + vm.VAR_SERIE + @"^FS
                                ^FT22,143^A0N,37,45^FH\^FDEQUIPO EN TALLER^FS
                                ^FT650,244^A0N,17,19^FH\^FDvar_ID ERP^FS
                                ^FO26,187^GB253,0,2^FS
                                ^FT645,391^A0N,14,21^FB143,1,0,C^FH\^FD" + vm.OwnerName + @"^FS
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
        [Route("PrintTechnicalEvaluation")]
        public IHttpActionResult PrintTechnicalEvaluation([FromBody] TechnicalEvaluation vm)
        {
            string ZPLString = @"CT~~CD,~CC^~CT~
                                ^XA~TA000~JSN^LT0^MNW^MTT^PON^PMN^LH0,0^JMA^PR5,5~SD22^JUS^LRN^CI0^XZ
                                ^XA
                                ^MMT
                                ^PW812
                                ^LL0406
                                ^LS0
                                ^FO0,32^GFA,03584,03584,00056,:Z64:eJzt0rGK3DAQBuBxHKImSIG0wn6FS6dw5vxKG65dYi9bbOetA2nzDmkCWlxsquQFUlgo4FZLGhVCykj2GdKlSRP0l8YfaOYfgJycnJz/JsTWN0ekr11dHkbfPIajgVB6aJn1nHRE2xZcGc62Zbwj0/vCLs7VN09kiG5M7mQhHD3gn4HSjv2y3dFH1515x0wgbnEVL65E6oOMrn/YCcpBj31xGUAPlFd3k7q+AEHhdv7Iqzfj8HJxDB2d0N0f5+SqCpTuCxldxZtXSs3P0BUTuub1+O354sgfrt0JTmFCZyjoE+cClJxldJL94KI8fO9XVyt0c3SfRne/OgeemXl12oJgN0mTu5rNeb4LhXw3fkFnnlx3NqH8nJzyi3PRjW57p6uNL2SHrn+7uh7YAH5100N6J91HJ/fbXmRr50IG/SG5dS/lADO6uJefNO1l2Me9fOVbD1IInA93M/bCPPUwojuYpQeaehia1APfekenL8l5dCT1XgRm9cWk3k8O57Pd0MTe2XovBt2OK3WRj9HtRGlwMA+OWaVMujP8gq6l6c7I9G8PPycnJycn52/zG+uVG0k=:B7F3
                                ^FO0,64^GFA,00896,00896,00028,:Z64:eJxjYBgFDDzMYCoBLnCASDkGUuXYDGccS5bgPcze9rFNPuHBjzvMjw8XN/AfPgCX43/Mf/xnm33ihz9/2J8//n9A/vgDuBxPscwzybbCZImeA3x6xRIHLM4VAOWYrXneGG/gMZZIk2xLACo6wANkHzA4YwCSq+Z9Yw6X+1GDIpfNu8YMLvfzD7IcoxerhsgCmBxjD4rcOXYLmQdnoG7p7DlwDuiWwxC3QOTOP+Y/JvnPPhFo5vHnjyUOQ/zAcIbZgOfAeaDfJb7JJ/x4c+Dw48MSh/kOIwXayAMAlO1vig==:8DD8
                                ^FO0,0^GFA,03328,03328,00052,:Z64:eJzt1EtuwyAQANBBSGVTlRuEa3RlXyuLSqY34ygcgSULy9MZiE2MsVN3V8koQfHnecIwHoBrvB63P5juMntGjueNns4bg38x9rTpk0FE+FjOyfQ5Mg5AkHE3GHCiLzplQVmDaDvgWSJG/m6NNxzNkgnaguYfvqPL6BXiSKYkaygmRatMOqT72yYY+TAxGT7VQbqQDVb/zYMgE0C7O4hsRpBkeDYOBumh3xiIhs5Lfwe4JxPoMR3QwqOmHaFrxhZjF+Oy+WLzHQDI0BzfstGLMU/Gtw3URvPSjuPMZlmPmmA3DqyMKnUSYclbHWdtqiG85P3ZicO792yoCrUjo6gOWnHo9qgqQ1nrw2wacdjotRG0o0N8EcesjUQPOB6sp2EUBpHMXt64Rvt13hS/GdPx/nCe1gZNMe39qQ2nZD+O/4Vp19u5OG1T1iO2uRbZhI2Z8yaTUdk4ellopynLea5M3p/SQ9hwgaUeYtyjh7TqoPSqZ+NNGLZmrje+OxnLRj6ao9cxzY269snYW+ra3HfSIXVQpyPP9btAOaHuDJzwd+onI5tP7p601IkSQ/fzEyfh4BrXuMY/GT+Ubhv5:8B18
                                ^BY2,3,44^FT509,313^BCN,,Y,N^FD>954335063333552415447^FS
                                ^FT24,397^A0N,16,19^FH\^FDEvaluacion Tecnica Por:^FS
                                ^FO27,170^GB253,0,2^FS
                                ^FO27,346^GB253,0,2^FS
                                ^FO301,292^GB200,0,2^FS
                                ^FT302,251^A0N,18,21^FH\^FDFecha Proximo Mantenimiento^FS
                                ^FT27,189^A0N,18,21^FH\^FDFecha Evaluacion Ingenieria^FS
                                ^FT27,253^A0N,18,21^FH\^FDFabricante / Marca^FS
                                ^FT27,310^A0N,18,21^FH\^FDModelo^FS
                                ^FT27,365^A0N,18,21^FH\^FDNumero de Serie^FS
                                ^FT302,310^A0N,18,21^FH\^FDNumero de Activo^FS
                                ^FO301,171^GB268,0,2^FS
                                ^FT302,190^A0N,18,21^FH\^FDFecha Evaluacion Aplicaciones^FS
                                ^FO27,232^GB253,0,2^FS
                                ^FO301,232^GB268,0,2^FS
                                ^FO27,292^GB253,0,2^FS
                                ^FT609,219^BQN,2,5
                                ^FH\^FDLA," + Url.Replace("01t1C000005g8voQAA", vm.aId) + @"^FS
                                ^FT229,397^A0N,16,16^FH\^FD" + vm.Nombre_RST_Ingenier + @"\A1a^FS
                                ^FT518,397^A0N,16,16^FH\^FD" + vm.Nombre_RST_Aplicaciones + @"^FS
                                ^FT27,166^A0N,24,26^FH\^FD" + vm.Fecha_Evalucion_Ingenieria.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                                ^FT301,169^A0N,24,26^FH\^FD" + vm.Fecha_Evaluacion_Aplicacion.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                                ^FT27,229^A0N,24,26^FH\^FD" + vm.Fabricante + @"^FS
                                ^FT301,231^A0N,24,26^FH\^FD" + vm.Fecha_Proximo_Mantenimiento.Value.Date.ToString("dd/MM/yyyy") + @"^FS
                                ^FT27,289^A0N,24,26^FH\^FD" + vm.VAR_MODELO + @"^FS
                                ^FT301,287^A0N,24,26^FH\^FD" + vm.VAR_ACTIVO + @"^FS
                                ^FT27,343^A0N,24,26^FH\^FD" + vm.VAR_SERIE + @"^FS
                                ^FT24,132^A0N,38,45^FH\^FDEVALUACION TECNICA^FS
                                ^FT654,224^A0N,14,19^FH\^FD[var_ID ERP]^FS
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