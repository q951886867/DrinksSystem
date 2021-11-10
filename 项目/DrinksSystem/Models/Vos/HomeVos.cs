using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class HomeVos: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string dataType { get; set; } //数据类型名称
        public string DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DataType"));//对quantity进行监听
                }
            }
        }
        private decimal totalAmount { get; set; } //总金额
        public decimal TotalAmount
        {
            get
            {
                return totalAmount;
            }
            set
            {
                totalAmount = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TotalAmount"));//对quantity进行监听
                }
            }
        }
        private double? percentage { get; set; } //占百分比
        public double? Percentage
        {
            get
            {
                return percentage;
            }
            set
            {
                percentage = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Percentage"));//对quantity进行监听
                }
            }
        }
        private bool isUp { get; set; } //是否上升
        public bool IsUp
        {
            get
            {
                return isUp;
            }
            set
            {
                isUp = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsUp"));//对quantity进行监听
                }
            }
        }
        private string thanLast { get; set; } //较上次相比
        public string ThanLast
        {
            get
            {
                return thanLast;
            }
            set
            {
                thanLast = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ThanLast"));//对quantity进行监听
                }
            }
        }
        
    }
}
