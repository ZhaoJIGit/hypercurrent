using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataRepository.FC;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;

namespace JieNor.Megi.DataRepository.GL
{
	public class FCVoucherModuleEntryRepository : DataServiceT<FCVoucherModuleEntryModel>
	{
		private readonly FCVoucherModuleRepository voucher = new FCVoucherModuleRepository();

		public static List<CommandInfo> UpdateAccountId(MContext ctx, string oldAccountId, string newAccountId)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@OldAccountId", MySqlDbType.VarChar, 36)
				{
					Value = oldAccountId
				},
				new MySqlParameter("@NewAccountId", MySqlDbType.VarChar, 36)
				{
					Value = newAccountId
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_fc_vouchermoduleentry set MAccountID=@NewAccountId where MAccountID=@OldAccountId and MOrgID=@MOrgID and MIsDelete=0";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}
	}
}
