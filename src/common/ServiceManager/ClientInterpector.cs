using JieNor.Megi.Common.Context;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace JieNor.Megi.Common.ServiceManager
{
	public class ClientInterpector : IClientMessageInspector
	{
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			MessageHeader MAccesToken = MessageHeader.CreateHeader(ContextHelper.MAccessTokenCookie, ContextHelper.MegiChinaNamespace, ContextHelper.MAccessToken, false, "");
			MessageHeader MLocaleID = MessageHeader.CreateHeader(ContextHelper.MLocaleIDCookie, ContextHelper.MegiChinaNamespace, ContextHelper.MLocaleID, false, "");
			MessageHeader AppSource = MessageHeader.CreateHeader(ContextHelper.MAppSourceName, ContextHelper.MegiChinaNamespace, ContextHelper.MAppSource, false, "");
			request.Headers.Add(MAccesToken);
			request.Headers.Add(MLocaleID);
			request.Headers.Add(AppSource);
			return null;
		}
	}
}
