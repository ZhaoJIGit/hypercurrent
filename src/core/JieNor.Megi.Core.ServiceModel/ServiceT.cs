using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DisLock;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MResource;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JieNor.Megi.Core.ServiceModel
{
	public class ServiceT<T>
	{
		public MActionResult<bool> Exists(Func<MContext, string, bool, bool> func, string pkID, bool includeDelete = false, string accessToken = null)
		{
			MActionResult<bool> mActionResult = new MActionResult<bool>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<bool>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, pkID, includeDelete);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.AddRange(ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			MResourceHelper.ReleaseResource(mContext);
			return mActionResult;
		}

		public MActionResult<bool> ExistsByFilter(Func<MContext, SqlWhere, bool> func, SqlWhere filter, string accessToken = null)
		{
			MActionResult<bool> mActionResult = new MActionResult<bool>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<bool>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, filter);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.Add(ex.Message);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			MResourceHelper.ReleaseResource(mContext);
			return mActionResult;
		}

		public MActionResult<OperationResult> InsertOrUpdate(Func<MContext, T, string, OperationResult> func, T modelData, string fields = null, string accessToken = null)
		{
			MActionResult<OperationResult> mActionResult = new MActionResult<OperationResult>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<OperationResult>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, modelData, fields);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.AddRange(ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<OperationResult> Delete(Func<MContext, string, OperationResult> func, string pkID, string accessToken = null)
		{
			MActionResult<OperationResult> mActionResult = new MActionResult<OperationResult>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<OperationResult>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, pkID);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.AddRange(ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<OperationResult> DeleteModels(Func<MContext, List<string>, OperationResult> func, List<string> pkID, string accessToken = null)
		{
			MActionResult<OperationResult> mActionResult = new MActionResult<OperationResult>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<OperationResult>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, pkID);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.AddRange(ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<T> GetDataModelByFilter(Func<MContext, SqlWhere, T> func, SqlWhere filter, string accessToken = null)
		{
			MActionResult<T> mActionResult = new MActionResult<T>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<T>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, filter);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.AddRange(ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			MResourceHelper.ReleaseResource(mContext);
			return mActionResult;
		}

		public MActionResult<T> GetDataModel(Func<MContext, string, bool, T> func, string pkID, bool includeDelete = false, string accessToken = null)
		{
			MActionResult<T> mActionResult = new MActionResult<T>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<T>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, pkID, includeDelete);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.AddRange(ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			MResourceHelper.ReleaseResource(mContext);
			return mActionResult;
		}

		public MActionResult<List<T>> GetModelList(Func<MContext, SqlWhere, bool, List<T>> func, SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			MActionResult<List<T>> mActionResult = new MActionResult<List<T>>();
			MContext mContext = new MContext();
			try
			{
				if (ContextHelper.ValidateAccessToken<List<T>>(accessToken, out mContext, ref mActionResult))
				{
					DisLockManager.GetLock(mContext, func.Method);
					mActionResult.ResultData = func(mContext, filter, includeDelete);
				}
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange(ex.Codes);
				mActionResult.Message.AddRange(ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			MResourceHelper.ReleaseResource(mContext);
			return mActionResult;
		}

		public MActionResult<S> RunFunc<S>(Func<MContext, S> func, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				mActionResult.ResultData = func(mContext);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunFunc<S, T1>(Func<MContext, T1, S> func, T1 t1, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				else if (t1 != null)
				{
					mContext.MLCID = ModelHelper.TryGetModelValue<T1>(t1, "MLCID");
				}
				mActionResult.ResultData = func(mContext, t1);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunFunc<S, T1, T2>(Func<MContext, T1, T2, S> func, T1 t1, T2 t2, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				mActionResult.ResultData = func(mContext, t1, t2);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunFunc<S, T1, T2, T3>(Func<MContext, T1, T2, T3, S> func, T1 t1, T2 t2, T3 t3, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				mActionResult.ResultData = func(mContext, t1, t2, t3);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunFunc<S, T1, T2, T3, T4>(Func<MContext, T1, T2, T3, T4, S> func, T1 t1, T2 t2, T3 t3, T4 t4, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				mActionResult.ResultData = func(mContext, t1, t2, t3, t4);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunFunc<S, T1, T2, T3, T4, T5>(Func<MContext, T1, T2, T3, T4, T5, S> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				mActionResult.ResultData = func(mContext, t1, t2, t3, t4, t5);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunFunc<S, T1, T2, T3, T4, T5, T6>(Func<MContext, T1, T2, T3, T4, T5, T6, S> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				mActionResult.ResultData = func(mContext, t1, t2, t3, t4, t5, t6);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunAction<S>(Action<MContext> func, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				func(mContext);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunAction<S, T1>(Action<MContext, T1> func, T1 t1, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				func(mContext, t1);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunAction<S, T1, T2>(Action<MContext, T1, T2> func, T1 t1, T2 t2, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				func(mContext, t1, t2);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunAction<S, T1, T2, T3>(Action<MContext, T1, T2, T3> func, T1 t1, T2 t2, T3 t3, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				func(mContext, t1, t2, t3);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunAction<S, T1, T2, T3, T4>(Action<MContext, T1, T2, T3, T4> func, T1 t1, T2 t2, T3 t3, T4 t4, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				func(mContext, t1, t2, t3, t4);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		public MActionResult<S> RunAction<S, T1, T2, T3, T4, T5>(Action<MContext, T1, T2, T3, T4, T5> func, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, string accessToken = null)
		{
			MActionResult<S> mActionResult = new MActionResult<S>();
			MContext mContext = new MContext();
			try
			{
				if (!this.HasNoAuthorizationAttribute(func.Method))
				{
					ContextHelper.ValidateAccessToken<S>(accessToken, out mContext, ref mActionResult);
					DisLockManager.GetLock(mContext, func.Method);
				}
				func(mContext, t1, t2, t3, t4, t5);
			}
			catch (MActionException ex)
			{
				mActionResult.ResultCode.AddRange((IEnumerable<MActionResultCodeEnum>)ex.Codes);
				mActionResult.Message.AddRange((IEnumerable<string>)ex.Messages);
			}
			catch (Exception ex2)
			{
				MLogger.Log(func.Method.Name, ex2, mContext);
				throw ex2;
			}
			finally
			{
				DisLockManager.ReleaseLock(mContext, func.Method);
				MResourceHelper.ReleaseResource(mContext);
			}
			return mActionResult;
		}

		private bool HasNoAuthorizationAttribute(MethodInfo method)
		{
			object[] customAttributes = method.GetCustomAttributes(false);
			foreach (object obj in customAttributes)
			{
				if (obj.ToString().ToUpper() == "JieNor.Megi.Core.Attribute.NoAuthorizationAttribute".ToUpper())
				{
					return true;
				}
			}
			return false;
		}
	}
}
