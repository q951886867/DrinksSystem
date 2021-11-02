using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class SalesOrderVos: B_SalesRecord
    {
        public string staffName { get; set; }//销售员姓名
        public string memberNumber { get; set; }//会员账号

    }
}
