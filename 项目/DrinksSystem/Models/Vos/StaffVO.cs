using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class StaffVO:S_Staff
    {
        public string sexName { get; set; }//性别名称

        public string positionName { get; set; }//职位名称
    }
}
