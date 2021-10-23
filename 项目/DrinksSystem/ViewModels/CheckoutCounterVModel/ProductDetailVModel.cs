using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using DrinksSystem.Resources.PublicClass;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DrinksSystem.ViewModels.CheckoutCounterVModel
{
    public class ProductDetailVModel:ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public ProductDetailVModel()
        {
            TemperatureClick = new RelayCommand<RadioButton>(SelectTemperature);
            SweetnessClick = new RelayCommand<RadioButton>(SelectSweetness);
            ReduceQuantityCommand = new RelayCommand(ReduceQuantity);
            PlusQuantityCommand = new RelayCommand(PlusQuantity);
            LoadedCommand = new RelayCommand(Load);
            DragMoveCommand = new RelayCommand<Window>(DragMovewindow);
            CloseCommand = new RelayCommand<Window>(Close);
            AddProductCommand = new RelayCommand<Window>(AddProduct);
        }

        #region 属性
        public string Txt_Temperature;//温度
        public string Txt_Sweetness;//甜度
        public int ProductID;//产品ID

        //数量
        private decimal _quantity=1;
        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    RaisePropertyChanged(() => Quantity);
                }
            }
        }

        //图片
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

        //销售记录明细(记录产品明细)
        private ProductDetailVo _productDetails;
        public ProductDetailVo ProductDetails
        {
            get { return _productDetails; }
            set
            {
                if (_productDetails != value)
                {
                    _productDetails = value;
                    RaisePropertyChanged(() => ProductDetails);
                }
            }
        }

        public bool windowState=false;//窗口关闭时的标识 （记录关闭时用户点的是保存还是关闭）
        #endregion


        #region 命令
        public RelayCommand<RadioButton> TemperatureClick { get; set; }//温度
        public RelayCommand<RadioButton> SweetnessClick { get; set; }//甜度
        public RelayCommand ReduceQuantityCommand { get; set; }//减数量
        public RelayCommand PlusQuantityCommand { get; set; }//加数量
        public RelayCommand LoadedCommand { get; set; }//加载
        public RelayCommand<Window> CloseCommand { get; set; }//关闭窗口
        public RelayCommand<Window> DragMoveCommand { get; set; }//移动窗口
        public RelayCommand<Window> AddProductCommand { get; set; }//保存添加
        #endregion


        #region 函数
        //获取温度选中项
        public void SelectTemperature(RadioButton rb)
        {
            Txt_Temperature = rb.Content.ToString();
        }
        //获取甜度选中项
        public void SelectSweetness(RadioButton rb)
        {
            Txt_Sweetness = rb.Content.ToString();
        }
        //减数量
        public void ReduceQuantity()
        {
            if (Quantity>1)
            {
                Quantity = Quantity - 1;
            }
        }
        //加数量
        public void PlusQuantity()
        {
            Quantity = Quantity+1;
        }
        //加载
        public void Load()
        {
            ProductDetails = new ProductDetailVo();//实例化
            var list = (from tb in myModel.S_Product where tb.productID == ProductID select tb).Single();
            ProductImage = ByteConvertBitmapImage.ByteArrayToBitmapImage(list.productImage);//根据ID获取图片
            ProductDetails.price = list.productPrice;//单价
            ProductDetails.productName = list.productName;//产品名称
        }
        //关闭窗口
        public void Close(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        //移动窗口
        public void DragMovewindow(Window window)
        {
            window.DragMove();
        }
        //添加保存
        public void AddProduct(Window window)
        {
                ProductDetails.quantity = Quantity;//数量
                ProductDetails.taste = Txt_Temperature + "/" + Txt_Sweetness;//口味
                ProductDetails.productID = ProductID;//ID
                ProductDetails.Subtotal = ProductDetails.quantity* ProductDetails.price;//小计
                windowState = true;
                window.Close();
        }
        #endregion
    }
}
