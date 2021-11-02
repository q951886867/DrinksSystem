using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class SalesOrderDetailVos: B_SalesRecordDetails
    {
        public string productName { set; get; }//产品名称
    }
}
