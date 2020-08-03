using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace JieNor.Megi.DataRepository.IV
{
	public static class IVBankAPIReqRepository
	{
		private static string dse_sessionId = string.Empty;

		private static readonly string logonReqUrl = IVSignOpStepRepository.reqUrl + "/CM/APISessionReqServlet?";

		private static readonly string logoutReqUrl = IVSignOpStepRepository.reqUrl + "/CM/APISignOffReqServlet?";

		private static readonly string commReqUrl = IVSignOpStepRepository.reqUrl + "/CM/APIReqServlet?";

		private static string errMessage = string.Empty;

		private const string SESSION_TIMEOUT_CODE = "ebank00131";

		public static OperationResult LogonOnlineBank(MContext ctx, string userid, string password)
		{
			OperationResult operationResult = new OperationResult();
			string empty = string.Empty;
			string returnStr = new IVSignOpStepRepository("<?xml version=\"1.0\" encoding=\"gb2312\"?><BOSEBankData><opReq><serialNo>" + GetSerialNo() + "</serialNo><reqTime>" + ctx.DateNow.GetDtFormatString() + "</reqTime><ReqParam><userID>" + userid + "</userID><userPWD>" + password + "</userPWD></ReqParam></opReq></BOSEBankData>", ctx).sign(out empty);
			if (!string.IsNullOrWhiteSpace(empty))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = empty
				});
				return operationResult;
			}
			string text = BOS_URLencode(IVSignOpStepRepository.getNodeValue(returnStr, "signed_data"));
			if (string.IsNullOrWhiteSpace(text))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = errMessage
				});
				return operationResult;
			}
			string sendData = "opName=CebankUserLogon1_1Op&reqData=" + text;
			string text2 = ConnBankSystem(logonReqUrl, sendData);
			if (string.IsNullOrWhiteSpace(text2))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = errMessage
				});
				return operationResult;
			}
			List<string> list = text2.Split('|').ToList();
			if (list.Count > 1)
			{
				dse_sessionId = list[0];
			}
			else if (!IsSuccess(text2))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ReqErrMessage(text2, ctx)
				});
				return operationResult;
			}
			return operationResult;
		}

		public static OperationResult GetBankFeeds(MContext ctx, IVBankFeedsModel feedModel)
		{
			OperationResult operationResult = new OperationResult();
			List<IVBankBillModel> dataModelList = ModelInfoManager.GetDataModelList<IVBankBillModel>(ctx, new SqlWhere().Equal("MBankID", feedModel.AcctID), false, false);
			dataModelList = dataModelList.Where(delegate(IVBankBillModel w)
			{
				if (w.MStartDate <= feedModel.StartDate && w.MEndDate >= feedModel.StartDate)
				{
					goto IL_00c6;
				}
				if (w.MStartDate <= feedModel.EndDate && w.MEndDate >= feedModel.EndDate)
				{
					goto IL_00c6;
				}
				if (feedModel.StartDate <= w.MStartDate && feedModel.EndDate >= w.MStartDate)
				{
					goto IL_00c6;
				}
				int result = (feedModel.StartDate <= w.MEndDate && feedModel.EndDate >= w.MEndDate) ? 1 : 0;
				goto IL_00c7;
				IL_00c7:
				return (byte)result != 0;
				IL_00c6:
				result = 1;
				goto IL_00c7;
			}).ToList();
			if (dataModelList != null && dataModelList.Count > 0)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "InThePeriodHavebeenImported", "some data in the period have been imported.")
				});
				return operationResult;
			}
			operationResult = SetDse_sessionId(ctx);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			BDBankAccountEditModel bankModelById = GetBankModelById(ctx, feedModel.AcctID);
			if (bankModelById == null)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "banknohadnotbeensearched", "bankno had not been searched.")
				});
				return operationResult;
			}
			feedModel.AcctTypeID = bankModelById.MBankTypeID;
			string sendData = "dse_sessionId=" + dse_sessionId + "&opName=queryHistoryCurrentList1_1Op&reqData=<?xml version=\"1.0\" encoding=\"gb2312\"?><BOSEBankData><opReq><serialNo>" + GetSerialNo() + "</serialNo><reqTime>" + ctx.DateNow.GetDtFormatString() + "</reqTime><ReqParam><ACNO>" + bankModelById.MBankNo + "</ACNO><QSRQ>" + feedModel.StartDate.GetDtFormatString() + "</QSRQ><ZZRQ>" + feedModel.EndDate.GetDtFormatString() + "</ZZRQ><MIFS>0.00</MIFS><MAFS>9999999999.99</MAFS></ReqParam></opReq></BOSEBankData>";
			string text = ConnBankSystem(commReqUrl, sendData);
			if (string.IsNullOrWhiteSpace(text))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = errMessage
				});
				return operationResult;
			}
			if (!IsSuccess(text))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = ReqErrMessage(text, ctx)
				});
				return operationResult;
			}
			return ImportBankBillByOnline(ctx, feedModel, text);
		}

		public static string GetEnterpriseAccounts(MContext ctx)
		{
			SetDse_sessionId(ctx);
			string sendData = "dse_sessionId=" + dse_sessionId + "&opName=queryEnterpriseAccounts1_1Op&reqData=<?xml version=\"1.0\" encoding=\"GBK\"?><BOSEBankData><opReq><serialNo>" + GetSerialNo() + "</serialNo><reqTime>" + ctx.DateNow.GetDtFormatString() + "</reqTime></opReq></BOSEBankData>";
			return ConnBankSystem(commReqUrl, sendData);
		}

		public static string GetAccountInfoDetail(MContext ctx, string accNo)
		{
			SetDse_sessionId(ctx);
			string sendData = "dse_sessionId=" + dse_sessionId + "&opName=queryAccountInfoDetail1_1Op&reqData=<?xml version=\"1.0\" encoding=\"GBK\"?><BOSEBankData><opReq><serialNo>" + GetSerialNo() + "</serialNo><reqTime>" + ctx.DateNow.GetDtFormatString() + "</reqTime><ReqParam><ACNO>" + accNo + "</ACNO></ReqParam></opReq></BOSEBankData>";
			return ConnBankSystem(commReqUrl, sendData);
		}

		public static string GetHistoryCurrentBalance(MContext ctx, string accNo, DateTime dt)
		{
			SetDse_sessionId(ctx);
			string sendData = "dse_sessionId=" + dse_sessionId + "&opName=queryHistoryCurrentBalance1_1Op&reqData=<?xml version=\"1.0\" encoding=\"GBK\"?><BOSEBankData><opReq><serialNo>" + GetSerialNo() + "</serialNo><reqTime>" + ctx.DateNow.GetDtFormatString() + "</reqTime><ReqParam><ACNO>" + accNo + "</ACNO><DATE>" + dt.GetDtFormatString() + "</DATE></ReqParam></opReq></BOSEBankData>";
			return ConnBankSystem(commReqUrl, sendData);
		}

		public static string LogoutOnlineBank(MContext ctx)
		{
			SetDse_sessionId(ctx);
			string sendData = "dse_sessionId=" + dse_sessionId;
			return ConnBankSystem(logoutReqUrl, sendData);
		}

		private static string BOS_URLencode(string orgStr)
		{
			string result = null;
			try
			{
				result = HttpContext.Current.Server.UrlEncode(orgStr);
			}
			catch (Exception ex)
			{
				errMessage = ex.Message;
			}
			return result;
		}

		private static string GetDtFormatString(this DateTime dt)
		{
			return dt.ToString("yyyyMMdd");
		}

		private static string ConnBankSystem(string reqUrl, string sendData)
		{
			try
			{
				Uri requestUri = new Uri(reqUrl);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
				httpWebRequest.Method = "post";
				httpWebRequest.Accept = "text/html, application/xhtml+xml, */*";
				httpWebRequest.ContentType = "application/x-www-form-urlencoded";
				httpWebRequest.UserAgent = "MSIE";
				byte[] bytes = IVSignOpStepRepository.gbkEncoding.GetBytes(sendData);
				httpWebRequest.ContentLength = bytes.Length;
				if (bytes != null && bytes.Length != 0)
				{
					httpWebRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
				}
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				BinaryReader binaryReader = new BinaryReader(httpWebResponse.GetResponseStream());
				int num = Convert.ToInt32(httpWebResponse.ContentLength);
				byte[] array = new byte[num];
				int num3;
				for (int i = 0; i < num; i += num3)
				{
					int num2 = num - i;
					if (num2 > 1024)
					{
						num2 = 1024;
					}
					num3 = binaryReader.Read(array, i, num2);
					if (num3 == -1 || num3 == 0)
					{
						break;
					}
				}
				string @string = IVSignOpStepRepository.gbkEncoding.GetString(array);
				httpWebResponse.Close();
				binaryReader.Close();
				return @string;
			}
			catch (Exception ex)
			{
				errMessage = ex.Message;
				return string.Empty;
			}
		}

		private static OperationResult ImportBankBillByOnline(MContext ctx, IVBankFeedsModel feedModel, string xmlStr)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlStr);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("opResult");
			if (elementsByTagName.Count <= 1)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "NoRecordInThisPeriod", "There is no record in this period.")
				});
				return operationResult;
			}
			string mID = string.Empty;
			for (int i = 0; i < elementsByTagName.Count; i++)
			{
				XmlNode xdNode = elementsByTagName[i];
				if (i == 0)
				{
					IVBankBillModel iVBankBillModel = new IVBankBillModel();
					iVBankBillModel.MStartDate = feedModel.StartDate;
					iVBankBillModel.MEndDate = feedModel.EndDate;
					iVBankBillModel.MImportDate = ctx.DateNow;
					iVBankBillModel.MBankID = feedModel.AcctID;
					iVBankBillModel.MBankTypeID = feedModel.AcctTypeID;
					iVBankBillModel.MOrgID = ctx.MOrgID;
					iVBankBillModel.MStartBalance = GetHistoryBalanceAmt(ctx, GetNodeInnerText(xdNode, "ACNO"), feedModel.StartDate.AddDays(-1.0));
					iVBankBillModel.MEndBalance = iVBankBillModel.MStartBalance - Convert.ToDecimal(GetNodeInnerText(xdNode, "JFJE")) + Convert.ToDecimal(GetNodeInnerText(xdNode, "DFJE"));
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillModel>(ctx, iVBankBillModel, null, true));
					mID = iVBankBillModel.MID;
				}
				else
				{
					IVBankBillEntryModel iVBankBillEntryModel = new IVBankBillEntryModel();
					iVBankBillEntryModel.MSeq = i;
					iVBankBillEntryModel.MID = mID;
					iVBankBillEntryModel.MDate = Convert.ToDateTime(GetNodeInnerText(xdNode, "JYRQ"));
					iVBankBillEntryModel.MTime = GetNodeInnerText(xdNode, "FSSJ");
					iVBankBillEntryModel.MTransNo = GetNodeInnerText(xdNode, "T24F");
					iVBankBillEntryModel.MTransAcctNo = GetNodeInnerText(xdNode, "DFZH");
					iVBankBillEntryModel.MTransAcctName = GetNodeInnerText(xdNode, "DFHM");
					iVBankBillEntryModel.MBalance = Convert.ToDecimal(GetNodeInnerText(xdNode, "YUER"));
					iVBankBillEntryModel.MDesc = GetNodeInnerText(xdNode, "BEZH");
					SetAmtByTranType(iVBankBillEntryModel, xdNode);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVBankBillEntryModel>(ctx, iVBankBillEntryModel, null, true));
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			if (num <= 0)
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "getbankfeedsfailed", "get bank feeds failed.")
				});
			}
			return operationResult;
		}

		private static decimal GetHistoryBalanceAmt(MContext ctx, string accNo, DateTime dt)
		{
			decimal result = default(decimal);
			List<string> list = new List<string>();
			XmlDocument xmlDocument = new XmlDocument();
			string historyCurrentBalance = GetHistoryCurrentBalance(ctx, accNo, dt);
			if (string.IsNullOrWhiteSpace(historyCurrentBalance))
			{
				return decimal.Zero;
			}
			if (!IsSuccess(historyCurrentBalance))
			{
				return decimal.Zero;
			}
			xmlDocument.LoadXml(historyCurrentBalance);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("opResult");
			if (elementsByTagName.Count > 0)
			{
				result = Convert.ToDecimal(elementsByTagName[0].SelectSingleNode("AVLB").InnerText);
			}
			return result;
		}

		private static string GetNodeInnerText(XmlNode xdNode, string xpath)
		{
			return xdNode.SelectSingleNode(xpath).InnerText;
		}

		private static bool IsSuccess(string xmlStr)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlStr);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("opRep");
			if (elementsByTagName.Count > 0)
			{
				string innerText = elementsByTagName[0].SelectSingleNode("retCode").InnerText;
				if (innerText.EqualsIgnoreCase("0"))
				{
					return true;
				}
				if (innerText.EqualsIgnoreCase("ebank00131"))
				{
					dse_sessionId = string.Empty;
				}
			}
			return false;
		}

		private static string ReqErrMessage(string xmlStr, MContext ctx)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xmlStr);
			XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("opRep");
			if (elementsByTagName.Count > 0)
			{
				string innerText = elementsByTagName[0].SelectSingleNode("retCode").InnerText;
				string result = elementsByTagName[0].SelectSingleNode("errMsg").InnerText;
				if (innerText.EqualsIgnoreCase("ebank00131"))
				{
					result = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "BankSessionTimeout", "Illegal SESSION or SESSION has timed out，Please try again!");
				}
				return result;
			}
			return string.Empty;
		}

		private static void SetAmtByTranType(IVBankBillEntryModel modelEntry, XmlNode xdNode)
		{
			string nodeInnerText = GetNodeInnerText(xdNode, "CDFG");
			if (!(nodeInnerText == "1"))
			{
				if (nodeInnerText == "2")
				{
					modelEntry.MTransType = "借";
					modelEntry.MBillType = "Receive";
					modelEntry.MReceivedAmt = Convert.ToDecimal(GetNodeInnerText(xdNode, "FSJE"));
				}
			}
			else
			{
				modelEntry.MTransType = "贷";
				modelEntry.MBillType = "Spent";
				modelEntry.MSpentAmt = Convert.ToDecimal(GetNodeInnerText(xdNode, "FSJE"));
			}
		}

		private static string GetSerialNo()
		{
			return UUIDHelper.GetGuid().Substring(0, 10);
		}

		private static BDBankAccountEditModel GetBankModelById(MContext ctx, string acctId)
		{
			return ModelInfoManager.GetDataEditModel<BDBankAccountEditModel>(ctx, acctId, false, true);
		}

		private static OperationResult SetDse_sessionId(MContext ctx)
		{
			OperationResult result = new OperationResult();
			if (string.IsNullOrWhiteSpace(dse_sessionId))
			{
				result = LogonOnlineBank(ctx, ConfigurationManager.AppSettings["SHBankMGAcctNo"], ConfigurationManager.AppSettings["SHBankMGAcctPw"]);
			}
			return result;
		}
	}
}
