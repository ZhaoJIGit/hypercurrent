using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.MSG;
using JieNor.Megi.ServiceContract.REG;
using JieNor.Megi.ServiceContract.SEC;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Mvc;

namespace JieNor.Megi.My.Web.AutoManager
{
	public class AutofacWCF
	{
		public static void BuilderWcfService()
		{
			ContainerBuilder containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterControllers(BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray());
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<IBASMyHome>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<IBASOrganisation>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECAccount>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<IMSGMessage>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECUserLoginLog>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECUserAccount>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECPermission>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<IBASOrganisation>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECUser>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECSendLinkInfo>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISECUserLoginLog>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<IBDAttachment>()).UseWcfSafeRelease();
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<IREGGlobalization>()).UseWcfSafeRelease();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(containerBuilder.Build(ContainerBuildOptions.None)));
		}
	}
}
