using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.COM;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.COM
{
    public class COMAccessService : ServiceT<IBASOrganisation>, ICOMAccess
    {
        private ICOMAccessBusiness biz = new COMAccessBusiness();

        public MActionResult<MAccessResponseModel> GetAccessResultByList(List<MAccessRequestModel> requestList = null, string accessToken = null)
        {
            ICOMAccessBusiness iCOMAccessBusiness = biz;
            return base.RunFunc(iCOMAccessBusiness.GetAccessResult, requestList, accessToken);
        }

        public MActionResult<MAccessResponseModel> GetAccessResultByListNoByCacche(bool isGetCache, List<MAccessRequestModel> requestList = null, string accessToken = null)
        {
            ICOMAccessBusiness iCOMAccessBusiness = biz;
            return base.RunFunc(iCOMAccessBusiness.GetAccessResultByListNoByCacche, isGetCache, requestList, accessToken);
        }

        public MActionResult<List<PlanModel>> GetPlan()
        {
            ICOMAccessBusiness iCOMAccessBusiness = biz;
            return base.RunFunc(iCOMAccessBusiness.GetPlan);
        }
    }
}
