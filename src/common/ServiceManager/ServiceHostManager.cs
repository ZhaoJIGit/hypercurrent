using JieNor.Megi.Common.Utility;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.GZipEncoder;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;

namespace JieNor.Megi.Common.ServiceManager
{
	public class ServiceHostManager
	{
		private static EndpointIdentity identity;

		public static string ClientCertName;

		private static string ServiceCertValue;

		private static string GZipEncoderTransfer;

		private static bool UseGZipEncoderTransfer => GZipEncoderTransfer == "1" && true;

		static ServiceHostManager()
		{
			ClientCertName = ConfigurationManager.AppSettings["ClientCertName"];
			ServiceCertValue = ConfigurationManager.AppSettings["ClientCertValue"];
			GZipEncoderTransfer = ConfigurationManager.AppSettings["GZipEncoderTransfer"];
			identity = GetIdentity();
		}

		public static T GetSysService<T>()
		{
			return GetService<T>(ServerHelper.SysServiceUrl);
		}

		public static T GetWebApiService<T>()
		{
			return GetService<T>(ServerHelper.WebApiServiceUrl);
		}

		public static T GetMongoService<T>()
		{
			return GetService<T>(ServerHelper.MongoServiceUrl);
		}

		public static T GetService<T>(ServiceType serviceType)
		{
			Type type = typeof(T);
			EndpointAddress address = GetAddressByContract(type, GetServiceAddress(serviceType));
			return UseGZipEncoderTransfer ? GetService<T>(GetCustomBinding(), address) : GetService<T>(GetBinding(), address);
		}

		public static T GetService<T>(string serviceurl)
		{
			Type type = typeof(T);
			EndpointAddress address = GetAddressByContract(type, serviceurl);
			return GetService<T>(address);
		}

		public static string GetServiceAddress(ServiceType serviceType)
		{
			switch (serviceType)
			{
			case ServiceType.WebApi:
				return ServerHelper.WebApiServiceUrl;
			case ServiceType.Sys:
				return ServerHelper.SysServiceUrl;
			default:
				return ServerHelper.WebApiServiceUrl;
			}
		}

		public static T GetService<T>(EndpointAddress address)
		{
			return UseGZipEncoderTransfer ? GetService<T>(GetCustomBinding(), address) : GetService<T>(GetBinding(), address);
		}

		public static WSHttpBinding GetBinding()
		{
			WSHttpBinding binding = new WSHttpBinding();
			binding.MaxBufferPoolSize = 2147483647L;
			binding.MaxReceivedMessageSize = 2147483647L;
			binding.SendTimeout = TimeSpan.FromHours(2.0);
			binding.ReceiveTimeout = TimeSpan.FromHours(2.0);
			binding.ReaderQuotas.MaxArrayLength = 2147483647;
			binding.ReaderQuotas.MaxBytesPerRead = 2147483647;
			binding.ReaderQuotas.MaxDepth = 2147483647;
			binding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
			binding.ReaderQuotas.MaxStringContentLength = 2147483647;
			binding.UseDefaultWebProxy = false;
			binding.Security.Mode = SecurityMode.Message;
			binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
			return binding;
		}

		public static CustomBinding GetCustomBinding()
		{
			CustomBinding binding = new CustomBinding();
			GZipMessageEncodingBindingElement gzip = new GZipMessageEncodingBindingElement();
			binding.Elements.Add(gzip);
			HttpTransportBindingElement transPort = new HttpTransportBindingElement();
			transPort.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
			transPort.ManualAddressing = false;
			transPort.MaxReceivedMessageSize = 2147483647L;
			transPort.MaxBufferPoolSize = 2147483647L;
			transPort.MaxBufferSize = 2147483647;
			transPort.AuthenticationScheme = AuthenticationSchemes.Anonymous;
			transPort.BypassProxyOnLocal = false;
			transPort.Realm = string.Empty;
			transPort.UseDefaultWebProxy = false;
			transPort.RequestInitializationTimeout = TimeSpan.FromHours(1.0);
			binding.SendTimeout = TimeSpan.FromHours(1.0);
			binding.ReceiveTimeout = TimeSpan.FromHours(1.0);
			binding.Elements.Add(transPort);
			return binding;
		}

		public static T GetService<T>(Binding binding, EndpointAddress address)
		{
			ChannelFactory<T> factory = GetServiceFactory<T>(binding, address);
			((Collection<IEndpointBehavior>)factory.Endpoint.EndpointBehaviors).Add((IEndpointBehavior)new ClientBehavior());
			return factory.CreateChannel();
		}

		public static ChannelFactory<T> GetServiceFactory<T>(Binding binding, EndpointAddress address)
		{
			ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
			foreach (OperationDescription item in (Collection<OperationDescription>)factory.Endpoint.Contract.Operations)
			{
				item.Behaviors.Find<DataContractSerializerOperationBehavior>().MaxItemsInObjectGraph = 2147483647;
			}
			ClientCredentials behaviors = factory.Endpoint.Behaviors.Find<ClientCredentials>();
			behaviors.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
			behaviors.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, ClientCertName);
			return factory;
		}

		public static EndpointAddress GetAddressByContract(Type type, string serviceurl)
		{
			string folderName = type.FullName.Split('.')[type.FullName.Split('.').Length - 2];
			string sname = $"/{folderName}/{type.Name.Substring(1)}Service.svc".ToLower();
			return new EndpointAddress(new Uri(serviceurl + sname), identity);
		}

		public static EndpointIdentity GetIdentity()
		{
			X509Certificate2Collection supportingCertificates = new X509Certificate2Collection();
			supportingCertificates.Import(Convert.FromBase64String(ConfigurationManager.AppSettings["ClientCertValue"]));
			X509Certificate2 primaryCertificate = supportingCertificates[0];
			return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate);
		}

		public static object GetWcfClient(Type contractType, ServiceType type = ServiceType.Sys)
		{
			try
			{
				WSHttpBinding binding = GetBinding();
				string serviceUrl = GetServiceAddress(type);
				EndpointAddress address = GetAddressByContract(contractType, serviceUrl);
				Type channelFactoryGenericType = typeof(ChannelFactory<>).MakeGenericType(contractType);
				MethodInfo method = channelFactoryGenericType.GetMethod("CreateChannel", new Type[2]
				{
					typeof(Binding),
					typeof(EndpointAddress)
				});
				//object channel = null;
				return method.Invoke(null, new object[2]
				{
					binding,
					address
				});
			}
			catch (Exception)
			{
			}
			return null;
		}
	}
}
