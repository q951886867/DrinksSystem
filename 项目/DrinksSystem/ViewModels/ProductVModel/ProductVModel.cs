using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using DrinksSystem.Resources.control;
using DrinksSystem.Resources.PublicClass;
using DrinksSystem.Views.Product;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DrinksSystem.ViewModels.ProductVModel
{
    public class ProductVModel : ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public ProductVModel()
        {
            LoadedCommand = new RelayCommand(QueryCommandFunc);//页面加载分页查询
            InsertClickCommand = new RelayCommand(InsertClick);//打开新增窗口
            UpdateClickCommand = new RelayCommand(UpdateClick);//打开修改窗口
            RefreshCommand = new RelayCommand<TextBox>(RefreshButton);//条件查询
            NextPageSearchCommand = new RelayCommand(NextPageSearchCommandFunc);//页面跳转
            DeleteClickCommand = new RelayCommand(DeleteClick);//删除
        }
        #region 属性
        //产品表选中行实体
        private ProductVo _productSelectEntity;
        public ProductVo ProductSelectEntity
        {
            get { return _productSelectEntity; }
            set
            {
                if (_productSelectEntity != value)
                {
                    _productSelectEntity = value;
                    RaisePropertyChanged(() => ProductSelectEntity);
                }
            }
        }
        //查询条件
        public string _selectTxt;
        public string SelectTxt
        {
            get { return _selectTxt; }
            set
            {
                if (_selectTxt != value)
                {
                    _selectTxt = value;
                    RaisePropertyChanged(() => SelectTxt);

                }
            }
        }
        #endregion

        #region 命令
        //打开新增窗口
        public RelayCommand InsertClickCommand { get; set; }
        //打开修改窗口
        public RelayCommand UpdateClickCommand { get; set; }
        //刷新
        public RelayCommand<System.Windows.Controls.TextBox> RefreshCommand { get; set; }
        //删除
        public RelayCommand DeleteClickCommand { get; set; }
        #endregion

        #region 函数
        //刷新
        private void RefreshButton(TextBox textbox1)
        {
            if (textbox1!=null)
            {
                SelectTxt = textbox1.Text.Trim();
            }
            else
            {
                SelectTxt = "";
            }
            QueryCommandFunc();
        }
        //条件查询
        private List<ProductVo> SelectProduct(string SelectTxt)
        {
            try
            {
                var list = (from tb in myModel.S_Product
                            join tbProductType in myModel.S_Dictionary on tb.productTypeID equals tbProductType.dictionaryID
                            join tbCupType in myModel.S_Dictionary on tb.cupTypeID equals tbCupType.dictionaryID
                            select new ProductVo
                            {
                                productID = tb.productID,
                                productName = tb.productName.Trim(),
                                productNumber = tb.productNumber.Trim(),
                                productTypeID = tb.productTypeID,
                                productTypeName = tbProductType.dictionaryName.Trim(),
                                cupTypeID = tb.cupTypeID,
                                cupTypeName = tbCupType.dictionaryName.Trim(),
                                productPrice = tb.productPrice,
                                productCost = tb.productCost,
                                memberPrice = tb.memberPrice,
                                productIntegral = tb.productIntegral,
                                productImage = tb.productImage
                            });
                //筛选数据
                //商品名称
                if (!string.IsNullOrEmpty(SelectTxt))
                {
                    list = list.Where(m => m.productName.Contains(SelectTxt)||m.productNumber.Contains(SelectTxt));
                }
                return list.ToList();
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        //打开新增窗口
        private void InsertClick()
        {
            addOrUpadteProduc myWindow = new addOrUpadteProduc();
            var addOrEditViewModel = (myWindow.DataContext as addOrUpadteProducVModel);
            addOrEditViewModel.ProductEntity = new ProductVo();
            addOrEditViewModel.IsAdd = true;
            addOrEditViewModel.ProductDatagrid += RefreshButton;

            //自动生成产品编号
            //根据产品编号排序 倒序
            var list = (from tb in myModel.S_Product
                        orderby tb.productNumber descending
                        select tb).ToList();
            string Number = "";
            if (list.Count()>0)
            {
                Number = list[0].productNumber.Remove(0, 1);//去除编号第一个字母A

            }
            else
            {
                Number =  1000.ToString();
            }
            string Number1 = "A" + (Convert.ToDecimal(Number) + 1).ToString();//编号加1 产品编号格式 A1001
            var ifProduct = myModel.S_Product.Where(m => m.productNumber.Trim() == Number1).ToList().Count;//判断计算的编号是否与数据库重复
            if (ifProduct > 0)//如果重复
            {
                //无限循环
                for (; ; )
                {
                    //重新计算 +1
                    Number = (Convert.ToDecimal(Number) + 1).ToString();
                    Number1 = "A" + Number;
                    var ifProduct1 = myModel.S_Product.Where(m => m.productNumber.Trim() == Number1).ToList().Count;
                    //直到编号不重复时
                    if (ifProduct1 == 0)
                    {
                        //传递编号
                        addOrEditViewModel.ProductEntity.productNumber = Number1;
                        //跳出循环
                        break;
                    }
                }
            }
            else
            {
                //不重复 则直接传递
                addOrEditViewModel.ProductEntity.productNumber = Number1;
            }


            myWindow.ShowDialog();
        }
        //打开修改窗口
        private void UpdateClick()
        {
            if (ProductSelectEntity!=null)
            {
                addOrUpadteProduc myWindow = new addOrUpadteProduc();
                var addOrEditViewModel = (myWindow.DataContext as addOrUpadteProducVModel);
                addOrEditViewModel.ProductEntity = ProductSelectEntity;
                addOrEditViewModel.ifName = ProductSelectEntity.productName.Trim();
                addOrEditViewModel.ifNumber = ProductSelectEntity.productNumber.Trim();
                addOrEditViewModel.ProductImage = ByteConvertBitmapImage.ByteArrayToBitmapImage(ProductSelectEntity.productImage);
                addOrEditViewModel.IsAdd = false;
                addOrEditViewModel.ProductDatagrid += RefreshButton;
                myWindow.ShowDialog();
            }
            else
            {
                Notice.Show("您还未选择数据", "提示", 2, MessageBoxIcon.Error);
            }
        }

        //删除
        private void DeleteClick()
        {
            try
            {
                if (ProductSelectEntity != null)
                {
                    S_Product myProductEntity = myModel.S_Product.Find(ProductSelectEntity.productID);
                    myModel.S_Product.Remove(myProductEntity);
                    if (myModel.SaveChanges() > 0)
                    {
                        Notice.Show("删除成功", "提示", 2, MessageBoxIcon.Info);
                        QueryCommandFunc();//刷新
                    }
                    else
                    {
                        Notice.Show("删除失败", "提示", 2, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Notice.Show("您还未选择数据", "提示", 2, MessageBoxIcon.Error);
                }
            }
            catch (Exception)
            {
                Notice.Show("删除失败", "提示", 2, MessageBoxIcon.Error);

            }

        }
        #endregion

        #region 分页
        //记录总页数
        private string _totalPage = string.Empty;
        public string TotalPage
        {
            get { return _totalPage; }
            set
            {
                _totalPage = value;
                RaisePropertyChanged(() => TotalPage);
            }
        }
        //记录当前页
        private string _currentPage = "1";
        public string CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                RaisePropertyChanged(() => CurrentPage);
            }
        }
        //设置每页显示条数
        private int _pageSize = 6;//默认6条
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                RaisePropertyChanged(() => PageSize);
            }
        }
        private int _pageIndex;
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;
                RaisePropertyChanged(() => PageIndex);
            }
        }
        private int _totalCount;
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                RaisePropertyChanged(() => TotalCount);
            }
        }
        //分页数据源 实时更新
        private ObservableCollection<ProductVo> _productPager;
        public ObservableCollection<ProductVo> ProductPager
        {
            get { return _productPager; }
            set
            {
                _productPager = value;
                RaisePropertyChanged(() => ProductPager);
            }
        }





        //页面加载查询分页
        public RelayCommand LoadedCommand { get; set; }
        /// <summary>
        /// 分页管理 上一页下一页等
        /// </summary>
        public RelayCommand NextPageSearchCommand { get; set; }





        /// <summary>
        /// 命令执行方法
        /// </summary>
        private async void QueryCommandFunc()
        {
            Resources.control.ProgressBar myBar = new Resources.control.ProgressBar();
            myBar.Show();
            await Task.Run(() =>
            {
                int totalCount = 0;
                ProductPager = GetData(PageSize, out totalCount);
                if (totalCount % PageSize == 0)
                {
                    TotalPage = (totalCount / PageSize).ToString();
                }
                else
                {
                    TotalPage = ((totalCount / PageSize) + 1).ToString();
                }
            });
            myBar.Close();
        }
        /// <summary>
        /// 分页查询命令
        /// </summary>
        private async void NextPageSearchCommandFunc()
        {
            await Task.Run(() =>
            {
                var pageIndex = System.Convert.ToInt32(CurrentPage);
                ProductPager = QueryData(pageIndex, PageSize);
            });
        }
        /// <summary>
        /// 初次获取数据
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        private ObservableCollection<ProductVo> GetData(int pageSize, out int totalCount)
        {
            //查询
            var myProduct = SelectProduct(SelectTxt);
            //分页
            List<ProductVo> list = myProduct
                .OrderBy(a => a.productID)//根据商品ID排序
                .Skip(0)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            totalCount = myProduct.Count;//总行数
            ObservableCollection<ProductVo> ProductList = new ObservableCollection<ProductVo>(list);
            return ProductList;
        }
        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        private ObservableCollection<ProductVo> QueryData(int pageIndex, int pageSize)
        {
            //调用查询方法
            var myProduct = SelectProduct(SelectTxt);
            //分页
            List<ProductVo> list = myProduct
                .OrderBy(a => a.productID)//根据商品ID排序
                .Skip((pageIndex - 1) * pageSize)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            ObservableCollection<ProductVo> ProductList = new ObservableCollection<ProductVo>(list);
            return ProductList;
        }
        #endregion
    }
}