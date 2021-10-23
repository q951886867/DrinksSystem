using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class HandoverVo:B_Handover, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string staffName { get; set; }

        private decimal? _businessAmount { get; set; }
        public new decimal? businessAmount
        {
            get
            {
                return _businessAmount;
            }
            set
            {
                _businessAmount = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("businessAmount"));//对quantity进行监听
                }
            }
        }
    }
}
