using System.Threading.Tasks;
using System.Web.Mvc;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.ServiceContract.SYS;
using Newtonsoft.Json;
using PaySharp.Alipay;
using PaySharp.Alipay.Response;
using PaySharp.Core;

namespace JieNor.Megi.My.Web.Areas.FW.Controllers
{
    [AllowAnonymous]
  public class NotifyController : Controller
    {
        private readonly IGateways _gateways;
        private ISYSOrder _orderSvc;
        private bool isRedirect;

        public NotifyController(IGateways gateways, ISYSOrder orderSvc)
        {
            _gateways = gateways;
            _orderSvc = orderSvc;
        }

        public async Task Index()
        {
            MLogger.Log($"支付回调开始..");

            // 订阅支付通知事件
            var notify = new Notify(_gateways);
            notify.PaySucceed += Notify_PaySucceed;
            notify.RefundSucceed += Notify_RefundSucceed;
            notify.CancelSucceed += Notify_CancelSucceed;
            notify.UnknownNotify += Notify_UnknownNotify;
            notify.UnknownGateway += Notify_UnknownGateway;

            // 接收并处理支付通知
            await notify.ReceivedAsync();

            // if (isRedirect)
            // {
            //     Response.Redirect("https://github.com/Varorbc/PaySharp");
            // }
        }

        private bool Notify_PaySucceed(object sender, PaySucceedEventArgs e)
        {
            MLogger.Log($"支付成功");

            // 支付成功时时的处理代码
            /* 建议添加以下校验。
             * 1、需要验证该通知数据中的OutTradeNo是否为商户系统中创建的订单号，
             * 2、判断Amount是否确实为该订单的实际金额（即商户订单创建时的金额），
             */
            if (e.GatewayType == typeof(AlipayGateway))
            {
                var alipayNotifyResponse = (NotifyResponse)e.NotifyResponse;

                //同步通知，即浏览器跳转返回
                if (e.NotifyType == NotifyType.Sync)
                {
                    isRedirect = true;
                }

                //MLogger.Log($"支付成功:{alipayNotifyResponse.OutTradeNo},{alipayNotifyResponse.TradeNo},{alipayNotifyResponse.TotalAmount}");
                _orderSvc.PayForOrder(alipayNotifyResponse.OutTradeNo, (decimal) alipayNotifyResponse.TotalAmount);
            }

            //处理成功返回true
            return true;
        }

        private bool Notify_RefundSucceed(object arg1, RefundSucceedEventArgs arg2)
        {
            MLogger.Log($"退款成功");
            // 订单退款时的处理代码
            return true;
        }

        private bool Notify_CancelSucceed(object arg1, CancelSucceedEventArgs arg2)
        {
            MLogger.Log($"撤销成功");
            // 订单撤销时的处理代码
            return true;
        }

        private bool Notify_UnknownNotify(object sender, UnKnownNotifyEventArgs e)
        {
            MLogger.Log($"未知时的处理代码");
            MLogger.Log(e.Message);
            MLogger.Log(JsonConvert.SerializeObject(e));
            // 未知时的处理代码
            return true;
        }

        private void Notify_UnknownGateway(object sender, UnknownGatewayEventArgs e)
        {
            MLogger.Log($"无法识别支付网关");
            // 无法识别支付网关时的处理代码
        }
    }
}