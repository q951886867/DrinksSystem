using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class MemberRechargeRecordVo: B_MemberRechargeRecord
    {
        public string memberNumber { get; set; }//会员卡号
        public string memberName { get; set; }//会员姓名
        public string staffName { get; set; }//员工姓名
    }
}
