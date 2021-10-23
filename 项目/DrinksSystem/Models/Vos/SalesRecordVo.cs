using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class SalesRecordVo: B_SalesRecord
    {
        public string orderStatus1 { get; set; }//订单状态 true：已完成 false：制作中
        public string salesQuantity { get; set; }//数量
    }
}
