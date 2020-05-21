using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Enum
{
   public enum LeaveStatusEnum
    {
        /// <summary>
        /// 未审核
        /// </summary>
        UnChecked=0,
        /// <summary>
        /// 未批准
        /// </summary>
        UnApproved=1,
        /// <summary>
        /// 审批通过
        /// </summary>
        Success = 2,
        /// <summary>
        /// 审批未通过
        /// </summary>
        Fail=3,
        /// <summary>
        /// 线下审批
        /// </summary>
        UnderLine=4,
        /// <summary>
        /// 销假未审核
        /// </summary>
        UnCheckedSellOff = 5,
        /// <summary>
        /// 销假成功
        /// </summary>
        SuccessSellOff = 6,
        /// <summary>
        /// 销假失败
        /// </summary>
        FailSellOff =7
    }
}
