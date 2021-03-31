using JieNor.Megi.Core.Attribute;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.COM;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.COM
{
    public class COMAccessBusiness : ICOMAccessBusiness
    {
        public MAccessResponseModel GetAccessResult(MContext ctx, List<MAccessRequestModel> requestList = null)
        {
            return COMAccess.GetAccessResult(ctx, requestList);
        }

        public MAccessResponseModel GetAccessResultByListNoByCacche(MContext ctx, bool isGetCache, List<MAccessRequestModel> requestList = null)
        {
            return COMAccess.GetAccessResult(ctx, isGetCache, requestList);
        }

        public bool GetAccessResult(MContext ctx, MAccessRequestModel request)
        {
            return COMAccess.GetAccessResult(ctx, request);
        }

        public bool GetAccessResult(MContext ctx, string obj, string item, int type = 2)
        {
            return COMAccess.GetAccessResult(ctx, obj, item, type);
        }
        public List<PlanModel> GetPlan(MContext ctx)
        {
            return COMAccess.GetPlan(ctx);
        }
        [NoAuthorization]
        public List<PlanModel> GetPlanByEmail(MContext ctx,string email)
        {
            return COMAccess.GetPlan(ctx,email);
        }
    }
}
