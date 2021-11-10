using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Models.Vos
{
    public class ProductDetailVo: B_SalesRecordDetails, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string productName { get; set; }//产品名称
        public byte[] productImage { get; set; }//产品图片
        //小计
        private decimal? _Subtotal { get; set; }
        public decimal? Subtotal
        {
            get
            {
                return _Subtotal;
            }
            set
            {
                _Subtotal = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Subtotal"));//对quantity进行监听
                }
            }
        }

        //数量
        private decimal? _Quantity { get; set; }
        public decimal? quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                _Quantity = value;
                if (PropertyChanged != null)//有改变
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("quantity"));//对quantity进行监听
                }
            }
        }
        
    }
}
