using IntegrationWS.Integrations;
using IntegrationWS.Integrations.Interfaces;
using IntegrationWS.Utils;
using IntegrationWS.Utils.Interfaces;
using System.Web.Http;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace IntegrationWS
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IAuthToSalesforce, AuthToSalesforce>();
            container.RegisterType<IStringCipher, StringCipher>();
            container.RegisterType<IResponseAfterAuth, ResponseAfterAuth>();
            container.RegisterType(typeof(ISobjectCRUD<>), typeof(SobjectCRUD<>), new TransientLifetimeManager());
            container.RegisterType<IDynamicsTransfers, DynamicsTransfers>();
            container.RegisterType<ITransferenciasProductos, TransferenciasProductos>();
            container.RegisterType<IProductos, Productos>();
            container.RegisterType<IUbicaciones, Ubicaciones>();
            container.RegisterType<IArticulosProductos, ArticulosProductos>();
            container.RegisterType<ICuentas, Cuentas>();
            container.RegisterType<IOportunidades, Oportunidades>();
            container.RegisterType<IContratos, Contratos>();
            container.RegisterType<IReadGpTables, ReadGpTables>();
            container.RegisterType<IAuthRepository, AuthRepository>();
            container.RegisterType<IEntradaDelCatalogoDePrecios, EntradaDelCatalogoDePrecios>();
            container.RegisterType<IListaDePrecios, ListaDePrecios>();
            container.RegisterType<IProductoDeOportunidad, ProductoDeOportunidad>();
            container.RegisterType<IProductoDeContrato, ProductoDeContrato>();
            container.RegisterType<IProductoDePedido, ProductoDePedido>();
            container.RegisterType<IActivos, Activos>();
            container.RegisterType<IExcepcionesFEFO, ExcepcionesFEFO>();
            container.RegisterType<IExcepcionesFEFODetalle, ExcepcionesFEFODetalle>();
            container.RegisterType<IDocumentoAbierto, DocumentoAbierto>();
            container.RegisterType<IProductoConLoteUtils, ProductoConLoteUtils>();
            container.RegisterType<IPedidos, PedidoServices>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}