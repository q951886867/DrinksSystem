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
                    PropertyChanged(this, new PropertyChangedEventArgs("businessAmount"));
                }
            }
        }

        private decimal? _cashIncome { get; set; }
        public new decimal? cashIncome
        {
            get
            {
                return _cashIncome;
            }
            set
            {
                _cashIncome = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("cashIncome"));
                }
            }
        }

        private decimal? _wechatIncome { get; set; }
        public new decimal? wechatIncome
        {
            get
            {
                return _wechatIncome;
            }
            set
            {
                _wechatIncome = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("wechatIncome"));
                }
            }
        }
    }
}
