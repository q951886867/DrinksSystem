using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using DrinksSystem.Resources.PublicClass;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DrinksSystem.ViewModels.ProductVModel
{
    public class addOrUpadteProducVModel:ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public addOrUpadteProducVModel()
        {
            //查询下拉框
            SelectComboBox();
            //打开图片
            OpenCommand = new RelayCommand(OpenImage);
            //关闭窗口
            CloseCommand = new RelayCommand<Window>(ClosseWindow);
            //清空图片
            CleanCommand = new RelayCommand(CleanImage);
            //保存
            SaveCommand = new RelayCommand<Window>(Save);
            //页面加载
            LoadedCommand = new RelayCommand(Loaded);
        }
        #region 属性
        //产品数据类
        private ProductVo _productEntity;
        public ProductVo ProductEntity
        {
            get { return _productEntity; }
            set 
            {
                if (_productEntity!=value)
                {
                    _productEntity = value;
                    RaisePropertyChanged(() => ProductEntity);
                }
            }
        }
        //查重
        public string ifName;
        public string ifNumber;
        //产品类别下拉框
        private List<S_Dictionary> _productTypeComboBox;
        public List<S_Dictionary> ProductTypeComboBox
        {
            get { return _productTypeComboBox; }
            set
            {
                if (_productTypeComboBox != value)
                {
                    _productTypeComboBox = value;
                    RaisePropertyChanged(() => ProductTypeComboBox);
                }
            }
        }
        //杯型下拉框
        private List<S_Dictionary> _cupTypeComboBox;
        public List<S_Dictionary> CupTypeComboBox
        {
            get { return _cupTypeComboBox; }
            set
            {
                if (_cupTypeComboBox != value)
                {
                    _cupTypeComboBox = value;
                    RaisePropertyChanged(() => CupTypeComboBox);
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
                if (_productImage!=value)
                {
                    _productImage = value;
                    RaisePropertyChanged(() => ProductImage);
                }
            }
        }
        //记录上传的图片
        public List<byte[]> lstBytes = new List<byte[]>();
        //记录是新增或修改
        public bool IsAdd = false;
        //声明委托（刷新表格）
        public delegate void UpdateCompletedEventHandler(TextBox Text1);
        public event UpdateCompletedEventHandler ProductDatagrid;
        #endregion


        #region 命令
        //加载
        public RelayCommand LoadedCommand { get; set; }
        //选择图片
        public RelayCommand OpenCommand { get; set; }
        //关闭窗口
        public RelayCommand<Window> CloseCommand { get; set; }
        //清空图片
        public RelayCommand CleanCommand { set; get; }
        //保存
        public RelayCommand<Window> SaveCommand { set; get; }
        #endregion


        #region 函数
        //页面加载
        private void Loaded()
        {
        }
        //选择图片
        private void OpenImage()
        {
            try
            {
                //声明局部变量
                Stream phpto = null;
                //打开选择文件弹窗
                OpenFileDialog ofdWenJian = new OpenFileDialog();
                //是否允许多选
                ofdWenJian.Multiselect = false;
                //筛选文件类型
                ofdWenJian.Filter = "ALL Image Files|*.*";
                //显示对话框
                if ((bool)ofdWenJian.ShowDialog())
                {
                    //选定的图片
                    if ((phpto=ofdWenJian.OpenFile())!=null)
                    {
                        //获取文字字节长度
                        int length = (int)phpto.Length;
                        //声明数组
                        byte[] bytes = new byte[length];
                        //读取文件
                        phpto.Read(bytes, 0, length);
                        lstBytes.Add(bytes);
                        BitmapImage images = new BitmapImage(new Uri(ofdWenJian.FileName));
                        //绑定图片
                        ProductImage = images;
                    }
                }
                else
                {
                    Notice.Show("对话框显示失败,无法选择图片", "提示", 2, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                Notice.Show("绑定图片出错,请重试", "提示", 2, MessageBoxIcon.Error);
            }
        }
        //关闭窗口
        private void ClosseWindow(Window window)
        {
            if (window !=null)
            {
                window.Close();
            }
        }
        //查询下拉框
        private void SelectComboBox()
        {
            //产品类别
            var list = (from tb in myModel.S_Dictionary
                        where tb.dictionaryType == "产品类别"
                        select tb).ToList();
            ProductTypeComboBox = list;
            //杯型
            var list1 = (from tb in myModel.S_Dictionary
                         where tb.dictionaryType == "杯型"
                         select tb).ToList();
            CupTypeComboBox = list1;
        }
        //清空图片
        private void CleanImage()
        {
            ProductImage = new BitmapImage();
            lstBytes = new List<byte[]>();
        }
        //保存
        private void Save(Window window)
        {
            try
            {
                //实例化上传的图片
                byte[] bytepicture = new byte[lstBytes.Count];
                if (lstBytes.Count > 0)
                {
                    for (int i = 0; i < lstBytes.Count; i++)
                    {
                        bytepicture = lstBytes[i];
                    }
                }
                else
                {
                    bytepicture = null;
                }
                //判断页面数据是否为空
                if (ProductEntity.productName != "" && ProductEntity.productNumber != "" && ProductEntity.productTypeID != null &&
                    ProductEntity.cupTypeID != null && ProductEntity.productPrice != null && ProductEntity.productCost != null &&
                    ProductEntity.memberPrice != null && ProductEntity.productIntegral != null)
                {
                    //新建产品表 获取页面数据
                    S_Product myProduct = new S_Product()
                    {
                        productName = ProductEntity.productName.Trim(),
                        productNumber = ProductEntity.productNumber.Trim(),
                        productTypeID = ProductEntity.productTypeID,
                        cupTypeID = ProductEntity.cupTypeID,
                        productPrice = ProductEntity.productPrice,
                        productCost = ProductEntity.productCost,
                        memberPrice = ProductEntity.memberPrice,
                        productIntegral = ProductEntity.productIntegral,
                        productImage = bytepicture
                    };

                    //判断 新增 还是 修改
                    if (IsAdd)
                    {
                        //判断重复
                        var ifProduct = myModel.S_Product.Where(m => m.productName.Trim() == myProduct.productName || m.productNumber == myProduct.productNumber).ToList().Count;
                        if (ifProduct > 0)
                        {
                            Notice.Show("名称或编号重复,请变更", "提示", 2, MessageBoxIcon.Info);
                        }
                        else
                        {
                            //新增
                            myModel.S_Product.Add(myProduct);
                            if (myModel.SaveChanges() > 0)
                            {
                                Notice.Show("新增成功", "提示", 2, MessageBoxIcon.Success);
                                window.Close();
                                //委托 刷新表格
                                ProductDatagrid(null);
                            }
                            else
                            {
                                Notice.Show("新增失败", "提示", 2, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        //判断重复
                        var ifProduct = myModel.S_Product.Where(m => m.productName.Trim() != ifName && m.productNumber.Trim() != ifNumber &&( m.productName.Trim() == myProduct.productName.Trim() || m.productNumber.Trim() == myProduct.productNumber.Trim())).ToList().Count;
                        if (ifProduct > 0)
                        {
                            Notice.Show("名称或编号重复,请变更", "提示", 2, MessageBoxIcon.Info);
                        }
                        else
                        {
                            //修改
                            if (bytepicture == null)
                            {
                                if (ProductImage != null)
                                {
                                    myProduct.productImage = ByteConvertBitmapImage.BitmapImageToByteArray(ProductImage);
                                }
                            }
                            //获取主键 根据主键修改
                            myProduct.productID = ProductEntity.productID;
                            myModel.Entry(myProduct).State = EntityState.Modified;
                            if (myModel.SaveChanges() > 0)
                            {
                                Notice.Show("修改成功", "提示", 2, MessageBoxIcon.Success);
                                window.Close();
                                //委托刷新表格
                                ProductDatagrid(null);
                            }
                            else
                            {
                                Notice.Show("修改失败", "提示", 2, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    Notice.Show("请把页面数据填写完整", "提示", 2, MessageBoxIcon.Info);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
