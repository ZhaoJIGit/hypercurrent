using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.SEC;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;

namespace JieNor.Megi.Login.Web.AutoManager
{
	public class AutofacWCF
	{
		public static void BuilderWcfService()
		{
			ContainerBuilder containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterControllers(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray());
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECUserAccount>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECPermission>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<IBASOrganisation>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECUser>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECSendLinkInfo>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECUserLoginLog>()).UseWcfSafeRelease();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(containerBuilder.Build(ContainerBuildOptions.None)));
		}
	}
}
