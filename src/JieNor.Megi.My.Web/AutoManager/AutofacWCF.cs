using System;
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
using JieNor.Megi.ServiceContract.SYS;
using PaySharp.Alipay;
using PaySharp.Core;

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
			containerBuilder.Register((IComponentContext c) => ServiceHostManager.GetSysService<ISYSOrder>()).UseWcfSafeRelease();
			RegisterPaySharp(containerBuilder);
			DependencyResolver.SetResolver(new AutofacDependencyResolver(containerBuilder.Build(ContainerBuildOptions.None)));
		}

		static void RegisterPaySharp(ContainerBuilder containerBuilder)
		{
			containerBuilder.Register<IGateways>( a =>
             {
                 var gateways = new Gateways();
                 //gateways.RegisterAlipay();

                 var alipayMerchant = new Merchant
                 {
                     AppId = "2021001153626667",
                     NotifyUrl = "https://my.hypercu.cn/fw/notify",
                     ReturnUrl = "https://my.hypercu.cn/FW/FWHome/return", 
                     AlipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmjOuiVig3ckMrIzBkpqyBOpAPJwinF7JAotjbS7dcBGZK7qHZHVk88+VdXOxtv672BsyWPfor2Ug7Wp5Rm+izugmx8ZLBDgTwSz4r+jfQL3KT9Gwc+umdi5zhUlYMDVyKwNGjhgep7Y4pvgqKdCqLwAO/wuwihWRDyI1AYlgjid6WmWbWbTuQMON9qs/j1xZWlfntdXxRuPuveI8HRebucz8lS1db5DyVNI0sUcxzKnaUAuSEnVB/RyPoicNLQ+aMIydSKNxMmYk8USiiDBMIq1FZcRh8EzU2OSb1tWv+9ohLmMTD3+R47Gq6HlKhydQmx6zgZJueWoz0Dh9MSyRcQIDAQAB",
                     Privatekey = "MIIEowIBAAKCAQEAk+ll5NXroNZ890Iore2MMYWyF6hTKWCRkucARfbSIq8lrN4Nf3GE1e6y5ODrDcKR6rbEhK4xGe/sy+Ehp0yMmM2OHKDKV5NyLAKRRzxMIi8lfdQpD+rjEXIKxXKbv0ZyHlV6O7fpSU6F2jSjY83twjyji+TTSmQ/h6NAFcJ9PZ/LM73VEFUZ/+Wq0O+1MDalm4zYG5rlr6gND85uSe5B65O0w6gvTV/9k5IDz9cUiPPUllEPhY+gVMop21fkoAPiyKdItQFf5Aj55xTPhes8t3/R3hsQdX1yRohaieItks6fas4KNMlNtGySeOjd4xr0x39HFFhFRKmUwZdeqUoxqQIDAQABAoIBACIf5vI0qHgjBBHHobr+4ylJvxzWKNmS/gL2aIm7uB9oaTjIwjR07ECvIXbMrW4vRZrxL8WsqodhYzBmcG04q/dU9USFlClAS0b8EKhusOHyHqQ5HoAdoOWVegazulsW1Z1J+zlr5NaU1yzQmCGekQwPEWxlDppLJp0UfOSISny81munscKR4MjX6OoVXuBpk6NeqFEIwfIZsyTh9HO7DLVG/bVLj5gzcBckWey3UNYcKfY7Gy5aCgEEdYtwMutQfp1J0HxW3rnb5atPkMXq1/FL30v5pn33lqdDG/LlmJieCNngEAp0PSwUgxHc+SmtRDAewL/JNhgKlLotZtVGqUECgYEA36jiAiC++XhwtxriJXaEgt2DXbtiRvClcy6Nssr4G7ADhD5LJO47XfSWgwlhjcbymQig6hQd1EnupbrcmbguycIdVcHs34cRrcbH8bM29VCX8BpsfEICRjANgJtjvsiEvpT6jdHbbJE6b7xCBmfbcG/cI6AkFwcUAE+RXNOl8gcCgYEAqUyVbxIzMSSzxVqEmwFXUNjRNDfWxmJvr04RcBbxzroCur4Ya4+VLWSp8bjkovmXCfA1rQDuz6XWh2EhHGMIq+P97oLNm/uIgenCxil3XN5O0EtAR/MKIQAZ0J6NaJb58CSBxvoExtrWUK6QcPgGH1dRCo5P+9UBc9teCEpxEs8CgYBqLvmt3PIN6lpI9CBtfMkIgDX/6BiOaW2DM5TeT3JfYr8op5JxZBEXWCmk6G1CLDO859XNi+Nlh/we/ooBCOpdqyTWNA1LSgrgn8EJRPZQnQSlX0Yl8Ai4XdPrPNqsFvK+sGgLsJgmAQTnS09lTyVlShYPa9X2gMvEIgJ3OZxNPQKBgAthwLIk8RgetY9RFJUvM+WVjgsKrf/MYmHQx89XW45gZwqS+SVSE99nYx2DJqvmR8c9RP6Kj4OaTJ/xYb580fpsa4f7d0NDV9wUESotY3702yuZw4qSxl2Nmi6yWiNr8wW4DpmH+YY89CecdM2DfzXgMuyYwLvXiC930gyQpKV1AoGBAKnv/e5R3I6j3JAzDDcF2v7z/V1XFuxciKsRlbvKQWGV0F3f/QEkaC+6FBjJBk06C1+h4JnbHL+USKbFu0862lX+ZYPP4Pn2zwMqDSsf/05rcJ5OZ8+fVhi3It4aUphmNCud3n4PAPg2ln7FY/rDsXEzzaDNuIPRt1y9HhEKc6Ap"
                 };

                 gateways.Add(new AlipayGateway(alipayMerchant)
                 {
                     //GatewayUrl = "https://openapi.alipaydev.com"
                     GatewayUrl = "https://openapi.alipay.com"
                 });
               

                 return gateways;
             }).InstancePerRequest();
		}
	}
}
