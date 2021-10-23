using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class ProductVo:S_Product
    {
        public string productTypeName { set; get; }//类别名称

        public string cupTypeName { set; get; }//杯型名称
    }
}
