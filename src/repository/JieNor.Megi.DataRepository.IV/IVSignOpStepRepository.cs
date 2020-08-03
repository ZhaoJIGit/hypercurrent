using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVSignOpStepRepository
	{
		public class StateObject
		{
			public Socket workSocket = null;

			public const int BufferSize = 1024;

			public byte[] buffer = new byte[1024];

			public StringBuilder sb = new StringBuilder();
		}

		private string verifyValue = null;

		private MContext ctx = null;

		private const int verifyPort = 8010;

		private const int sslVerfPort = 7071;

		private static string verifyIp = ConfigurationManager.AppSettings["BankClientMachineIP"];

		public static string reqUrl = "http://" + verifyIp + ":" + Convert.ToString(7071);

		private static AutoResetEvent connectDone = new AutoResetEvent(false);

		private static AutoResetEvent sendDone = new AutoResetEvent(false);

		private static AutoResetEvent receiveDone = new AutoResetEvent(false);

		private static string response = string.Empty;

		private string errMessage = string.Empty;

		public static readonly Encoding gbkEncoding = Encoding.GetEncoding("gb2312");

		public IVSignOpStepRepository(string verifyValue, MContext ctx)
		{
			this.verifyValue = verifyValue;
			this.ctx = ctx;
		}

		public string sign(out string errMsg)
		{
			errMsg = string.Empty;
			string s = "<?xml version=\"1.0\" encoding=\"gb2312\"?><msg><msg_head><msg_type>0</msg_type><msg_id>1005</msg_id><msg_sn>0</msg_sn><version>1</version></msg_head><msg_body><origin_data_len>" + gbkEncoding.GetBytes(verifyValue).Length + "</origin_data_len><origin_data>" + verifyValue + "</origin_data></msg_body></msg>";
			IPAddress address = IPAddress.Parse(verifyIp);
			IPEndPoint remoteEP = new IPEndPoint(address, 8010);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				socket.BeginConnect(remoteEP, EndConnect, socket);
				connectDone.WaitOne();
				if (!string.IsNullOrWhiteSpace(errMessage))
				{
					errMsg = errMessage;
					DestroySocket(socket);
					return response.Replace("\n", "");
				}
				byte[] bytes = gbkEncoding.GetBytes(s);
				socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, EndSend, socket);
				sendDone.WaitOne();
				StateObject stateObject = new StateObject();
				stateObject.workSocket = socket;
				socket.BeginReceive(stateObject.buffer, 0, 1024, SocketFlags.None, EndReceive, stateObject);
				receiveDone.WaitOne();
				DestroySocket(socket);
				return response.Replace("\n", "");
			}
			catch (Exception ex)
			{
				errMsg = ex.Message;
				DestroySocket(socket);
				return string.Empty;
			}
		}

		public static string getNodeValue(string returnStr, string tagName)
		{
			string text = "<" + tagName + ">";
			string value = "</" + tagName + ">";
			int num = returnStr.IndexOf(text) + text.Length;
			int num2 = returnStr.IndexOf(value, num);
			return returnStr.Substring(num, num2 - num);
		}

		private void EndConnect(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;
			if (!socket.Connected)
			{
				errMessage = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "NoRespondFromServer", "the server did not respond to the host after a period of time, the connection attempt failed.");
				connectDone.Set();
			}
			else
			{
				socket.EndConnect(ar);
				connectDone.Set();
			}
		}

		private void EndSend(IAsyncResult ar)
		{
			Socket socket = (Socket)ar.AsyncState;
			socket.EndSend(ar);
			sendDone.Set();
		}

		private void EndReceive(IAsyncResult ar)
		{
			StateObject stateObject = (StateObject)ar.AsyncState;
			Socket workSocket = stateObject.workSocket;
			int num = workSocket.EndReceive(ar);
			if (num > 0)
			{
				stateObject.sb.Append(gbkEncoding.GetString(stateObject.buffer, 0, num));
				workSocket.BeginReceive(stateObject.buffer, 0, 1024, SocketFlags.None, EndReceive, stateObject);
			}
			else
			{
				if (stateObject.sb.Length > 1)
				{
					response = stateObject.sb.ToString();
				}
				receiveDone.Set();
			}
		}

		private void DestroySocket(Socket socket)
		{
			if (socket != null)
			{
				if (socket.Connected)
				{
					socket.Shutdown(SocketShutdown.Both);
				}
				socket.Close();
			}
		}
	}
}
