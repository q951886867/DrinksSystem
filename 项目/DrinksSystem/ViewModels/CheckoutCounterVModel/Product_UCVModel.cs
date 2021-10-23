using DrinksSystem.Models;
using DrinksSystem.Resources.PublicClass;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DrinksSystem.ViewModels.CheckoutCounterVModel
{
    public class Product_UCVModel:ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public Product_UCVModel()
        {
            LoadedCommand = new RelayCommand(Load);//加载事件
            AddProductCommand = new RelayCommand(AddProduct);//弹出添加产品明细窗口
        }
        #region 属性
        //保存传过来的ID
        private int _productID;
        public int ProductID
        {
            get { return _productID; }
            set
            {
                if (_productID != value)
                {
                    _productID = value;
                    RaisePropertyChanged(() => ProductID);
                }
            }
        }

        //点击此产品 添加到订单 委托事件
        public delegate void ChangeTextHandler(int id);
        public event ChangeTextHandler ChangeTextEvent;

        //产品名称
        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set
            {
                if (_productName != value)
                {
                    _productName = value;
                    RaisePropertyChanged(() => ProductName);
                }
            }
        }
        //产品图片
        private BitmapImage _productImage;
        public BitmapImage ProductImage
        {
            get { return _productImage; }
            set
            {
                if (_productImage != value)
                {
                    _productImage = value;
                    RaisePropertyChanged(() => ProductImage);
                }
            }
        }
        //产品价格
        private decimal? _productPrice;
        public decimal? ProductPrice
        {
            get { return _productPrice; }
            set
            {
                if (_productPrice != value)
                {
                    _productPrice = value;
                    RaisePropertyChanged(() => ProductPrice);
                }
            }
        }
        #endregion

        #region 命令
        public RelayCommand LoadedCommand { get; set; }//加载事件
        public RelayCommand AddProductCommand { get; set; }//添加按钮点击
        #endregion

        #region 函数
        public void Load()
        {
            //根据传过来的产品ID查询数据
            var list = (from tb in myModel.S_Product
                       where tb.productID == ProductID
                       select tb).Single();
            ProductName = list.productName.Trim();//名称
            ProductPrice = list.productPrice;//价格
            ProductImage = ByteConvertBitmapImage.ByteArrayToBitmapImage(list.productImage);//图片 将byte 转化为 bitImage
        }
        //调用委托方法 并传递当前点击产品的ID
        public void AddProduct()
        {
            ChangeTextEvent(ProductID);
        }
        #endregion
    }
}
