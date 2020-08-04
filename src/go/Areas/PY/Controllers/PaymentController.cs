using JieNor.Megi.Go.Web.Controllers;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PY.Controllers
{
    public class PaymentController : GoControllerBase
    {
        public PaymentController()
        {

        }

        public ActionResult Index()
        {
            return View();
        }
    }
}