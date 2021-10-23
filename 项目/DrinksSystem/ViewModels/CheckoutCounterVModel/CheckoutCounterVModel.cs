using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using DrinksSystem.Views.CheckoutCounterView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DrinksSystem.ViewModels.CheckoutCounterVModel
{
    public class CheckoutCounterVModel:ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public CheckoutCounterVModel()
        {
            LoadedCommand = new RelayCommand<WrapPanel>(Load);//页面加载
            ListBoxChangeCommand = new RelayCommand<WrapPanel>(SelectProduct);
            RefreshCommand = new RelayCommand<WrapPanel>(ConditionQuery);
            ReduceQuantityCommand = new RelayCommand(ReduceQuantity);//销售区 减数量
            PlusQuantityCommand = new RelayCommand(PlusQuantity);//销售区 加数量
            MinImizeCommand = new RelayCommand<Window>(Minwindow);//最小化
            CloseCommand = new RelayCommand<Window>(Clossewindow);//关闭系统
            PayCommand = new RelayCommand(Pay);//结账窗口
            DeleteClickCommand = new RelayCommand(DeleteClick);//销售区删除当前选中行
            FnishClickCommand = new RelayCommand(FnishClick);//制作完成点击

        }
        #region 属性
        //产品类型列表
        private List<S_Dictionary> _productTypeData;
        public List<S_Dictionary> ProductTypeData
        {
            get { return _productTypeData; }
            set
            {
                if (_productTypeData != value)
                {
                    _productTypeData = value;
                    RaisePropertyChanged(() => ProductTypeData);
                }
            }
        }
        //产品列表选中数值ID
        private int _productTypeID;
        public int ProductTypeID
        {
            get { return _productTypeID; }
            set
            {
                if (_productTypeID != value)
                {
                    _productTypeID = value;
                    RaisePropertyChanged(() => ProductTypeID);
                }
            }
        }
        //查询条件 文本
        private string _productText;
        public string ProductText
        {
            get { return _productText; }
            set
            {
                if (_productText != value)
                {
                    _productText = value;
                    RaisePropertyChanged(() => ProductText);
                }
            }
        }
        //销售区表格数据源
        private ObservableCollection<ProductDetailVo> _salesAreaData=new ObservableCollection<ProductDetailVo>();
        public ObservableCollection<ProductDetailVo> SalesAreaData
        {
            get { return _salesAreaData; }
            set
            {
                if (_salesAreaData != value)
                {
                    _salesAreaData = value;
                    RaisePropertyChanged(() => SalesAreaData);
                }
            }
        }
        //等待区表格数据源
        private ObservableCollection<SalesRecordVo> _aitingAreaData = new ObservableCollection<SalesRecordVo>();
        public ObservableCollection<SalesRecordVo> WaitingAreaData
        {
            get { return _aitingAreaData; }
            set
            {
                if (_aitingAreaData != value)
                {
                    _aitingAreaData = value;
                    RaisePropertyChanged(() => WaitingAreaData);
                }
            }
        }
        //销售区表格选中行
        private ProductDetailVo _salesAreaEntity;
        public ProductDetailVo SalesAreaEntity
        {
            get { return _salesAreaEntity; }
            set
            {
                if (_salesAreaEntity != value)
                {
                    _salesAreaEntity = value;
                    RaisePropertyChanged(() => SalesAreaEntity);
                }
            }
        }
        //等待区表格选中行
        private SalesRecordVo _waitingAreaEntity;
        public SalesRecordVo WaitingAreaEntity
        {
            get { return _waitingAreaEntity; }
            set
            {
                if (_waitingAreaEntity != value)
                {
                    _waitingAreaEntity = value;
                    RaisePropertyChanged(() => WaitingAreaEntity);
                }
            }
        }
        public bool ifExisting;//判断销售区产品是否重复标识
        //销售区产品总数量
        private decimal _totalQuantity=0;
        public decimal TotalQuantity
        {
            get { return _totalQuantity; }
            set
            {
                if (_totalQuantity != value)
                {
                    _totalQuantity = value;
                    RaisePropertyChanged(() => TotalQuantity);
                }
            }
        }
        //销售区产品总金额
        private decimal? _totalAmount=0;
        public decimal? TotalAmount
        {
            get { return _totalAmount; }
            set
            {
                if (_totalAmount != value)
                {
                    _totalAmount = value;
                    RaisePropertyChanged(() => TotalAmount);
                }
            }
        }
        //销售区当前时间
        private DateTime _currentTime;
        public DateTime CurrentTime
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    RaisePropertyChanged(() => CurrentTime);
                }
            }
        }
        //销售区销售单号
        private string _salesOrderNumber;
        public string SalesOrderNumber
        {
            get { return _salesOrderNumber; }
            set
            {
                if (_salesOrderNumber != value)
                {
                    _salesOrderNumber = value;
                    RaisePropertyChanged(() => SalesOrderNumber);
                }
            }
        }
        //当前员工
        private S_Staff _staffNow;
        public S_Staff StaffNow
        {
            get { return _staffNow; }
            set
            {
                if (_staffNow != value)
                {
                    _staffNow = value;
                    RaisePropertyChanged(() => StaffNow);
                }
            }
        }
        //当前用户
        private string _currentUser;
        public string CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    RaisePropertyChanged(() => CurrentUser);
                }
            }
        }
        //通告
        private string _noticeText;
        public string NoticeText
        {
            get { return _noticeText; }
            set
            {
                if (_noticeText != value)
                {
                    _noticeText = value;
                    RaisePropertyChanged(() => NoticeText);
                }
            }
        }
        #endregion

        #region 命令
        public RelayCommand<WrapPanel> LoadedCommand { get; set; }//页面加载
        public RelayCommand<WrapPanel> ListBoxChangeCommand { get; set; }//页面加载
        public RelayCommand<WrapPanel> RefreshCommand { get; set; }//页面加载
        public RelayCommand ReduceQuantityCommand { get; set; }//减数量
        public RelayCommand PlusQuantityCommand { get; set; }//加数量
        public RelayCommand<Window> MinImizeCommand { get; set; }//最小化
        public RelayCommand<Window> CloseCommand { get; set; }//关闭系统
        public RelayCommand PayCommand { get; set; }//结账窗口
        public RelayCommand DeleteClickCommand { get; set; }//销售区 删除当前选中行
        public RelayCommand FnishClickCommand { get; set; }//制作完成点击
        #endregion

        #region 函数
        //等待区 右键制作完成
        private void FnishClick()
        {
            liusadaw
            if (WaitingAreaEntity!=null)
            {
                //根据当前选中行ID 修改订单状态
                var salesRecordID = WaitingAreaEntity.salesRecordID;
                B_SalesRecord mySalesRecord = new B_SalesRecord();
                mySalesRecord = (from tb in myModel.B_SalesRecord where tb.salesRecordID == salesRecordID select tb).Single();
                mySalesRecord.orderStatus = true;//修改为制作完成
                myModel.Entry(mySalesRecord).State = EntityState.Modified;
                if (myModel.SaveChanges()>0)
                {
                    SelectWaitingAreaData();//刷新表格
                    NoticeText = "请单号" + mySalesRecord.salesNumber + "到前台取餐~";//修改公告
                }
                else
                {
                    Notice.Show("制作完成点击错误，请检查系统", "提示", 2, MessageBoxIcon.Error);
                }
            }
            else
            {
                return;
            }
        }
        //销售区 删除当前选中行
        private void DeleteClick()
        {
            if (SalesAreaEntity!=null)
            {
                ProductDetailVo myDetail = SalesAreaData.Where(m => m.productID == SalesAreaEntity.productID && m.taste == SalesAreaEntity.taste).Single();
                SalesAreaData.Remove(myDetail);
                UpdateData();
            }
        }
        //刷新等待区数据
        private void SelectWaitingAreaData()
        {
            WaitingAreaData = new ObservableCollection<SalesRecordVo>();
            //查询正在制作的订单
            var list = (from tb in myModel.B_SalesRecord
                         where tb.orderStatus == false
                         orderby tb.salesTime
                         select tb).ToList();
            //查询已制作完成的订单
            var list1 = (from tb in myModel.B_SalesRecord
                        where tb.orderStatus == true
                         orderby tb.salesTime descending
                         select tb).ToList();
            //把 list1追加到list 数据后面 (顶部优先显示 正在制作的订单 方便客人查看)
            for (int i = 0; i < list.Count(); i++)
            {
                SalesRecordVo mySalesRecord = new SalesRecordVo()
                {
                    salesRecordID= list[i].salesRecordID,//销售ID
                    salesNumber = list[i].salesNumber,//销售单号
                    salesTime = list[i].salesTime,//销售时间
                    staffID = list[i].staffID,//员工ID
                    ifTakeout = list[i].ifTakeout,//是否外卖
                    cashPay = list[i].cashPay,//现金支付
                    wechatPay = list[i].wechatPay,//微信支付
                    ifMember = list[i].ifMember,//是否会员消费
                    memberID = list[i].memberID,//会员ID
                    orderStatus = list[i].orderStatus,//订单状态
                    ifRedeem = list[i].ifRedeem,//是否积分兑换
                    salesAmount = list[i].salesAmount,//销售金额
                };
                //订单完成状态
                if (Convert.ToBoolean(list[i].orderStatus))
                {
                    mySalesRecord.orderStatus1 = "已完成";
                }
                else
                {
                    mySalesRecord.orderStatus1 = "制作中";
                }
                //销售记录ID
                var salesRecordID = list[i].salesRecordID;
                //订单产品总数量
                var salesQuantity1 = (from tb in myModel.B_SalesRecordDetails where tb.salesRecordID == salesRecordID select tb).ToList().Count();
                mySalesRecord.salesQuantity = salesQuantity1.ToString();
                WaitingAreaData.Add(mySalesRecord);
            }
            for (int i = 0; i < list1.Count(); i++)
            {
                SalesRecordVo mySalesRecord = new SalesRecordVo()
                {
                    salesRecordID = list1[i].salesRecordID,//销售ID
                    salesNumber = list1[i].salesNumber,//销售单号
                    salesTime = list1[i].salesTime,//销售时间
                    staffID = list1[i].staffID,//员工ID
                    ifTakeout = list1[i].ifTakeout,//是否外卖
                    cashPay = list1[i].cashPay,//现金支付
                    wechatPay = list1[i].wechatPay,//微信支付
                    ifMember = list1[i].ifMember,//是否会员消费
                    memberID = list1[i].memberID,//会员ID
                    orderStatus = list1[i].orderStatus,//订单状态
                    ifRedeem = list1[i].ifRedeem,//是否积分兑换
                    salesAmount = list1[i].salesAmount,//销售金额
                };
                //订单完成状态
                if (Convert.ToBoolean(list1[i].orderStatus))
                {
                    mySalesRecord.orderStatus1 = "已完成";
                }
                else
                {
                    mySalesRecord.orderStatus1 = "制作中";
                }
                //销售记录ID
                var salesRecordID = list1[i].salesRecordID;
                //订单产品总数量
                var salesQuantity1 = (from tb in myModel.B_SalesRecordDetails where tb.salesRecordID == salesRecordID select tb).ToList().Count();
                mySalesRecord.salesQuantity = salesQuantity1.ToString();
                WaitingAreaData.Add(mySalesRecord);
            }
        }
        //加载页面
        public void Load(WrapPanel wp)
        {
            //查询产品类别列表
            var list = from tb in myModel.S_Dictionary where tb.dictionaryType == "产品类别" select tb;
            //为列表添加一条 “全部”选项
            S_Dictionary mydic = new S_Dictionary()
            {
                dictionaryID = 0,
                dictionaryName = "全部",
                dictionaryType = "产品类别"
            };
            List<S_Dictionary> myList= list.ToList();
            myList.Insert(0,mydic);//将 "全部" 选项 添加到列表的指定索引位置 列表.Insert（索引,数值）
            ProductTypeData = myList;
            ProductTypeID = 0;//默认选中为 全部
            //清空容器的子项
            wp.Children.Clear();
            //生成菜单列表
            SelectProduct(wp);

            RandomNumber();//生成随机单号

            CurrentUser = "当前用户：" + StaffNow.staffName;//当前用户 

            //创建定时器 用于更新当前时间
            System.Timers.Timer timer;
            int interval = 1000;
            timer = new System.Timers.Timer(interval);
            //设置执行一次（false）还是一直执行(true)
            timer.AutoReset = true;
            //设置是否执行System.Timers.Timer.Elapsed事件
             timer.Enabled = true;
             //绑定Elapsed事件
             timer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateTime);//每秒更新当前时间

            SelectWaitingAreaData();//刷新 等待区数据  
        }
        //根据选中的产品类别查询数据 并生成菜单列表
        public void SelectProduct(WrapPanel wp)
        {
            //加载动画
            Resources.control.ProgressBar myBar = new Resources.control.ProgressBar();
            myBar.Show();

            //查询
            var list = from tb in myModel.S_Product select tb;
            //如果选择的是"全部" 则不筛选数据
            if (ProductTypeID!=0)
            {
                list = list.Where(m => m.productTypeID == ProductTypeID);
            }
            //转为list格式
            var list1=list.ToList();
            //清空容器的子项
            wp.Children.Clear();
            //循环 创建产品控件 并追加
            if (list1.Count() >0)
            {
                for (int i = 0; i < list1.Count(); i++)
                {
                    //实例化产品控件
                    Product_UC myUC = new Product_UC();
                    //传递参数
                    int ProductID = list1[i].productID;//获取当前行ID
                    var myUCViewModel = (myUC.DataContext as Product_UCVModel);//连接控件数据上下文
                    myUCViewModel.ProductID = ProductID;
                    myUCViewModel.ChangeTextEvent += AddProductDetail;//事件委托
                    wp.Children.Add(myUC);
                }
            }

            //关闭加载动画
            myBar.Close();
        }
        //条件查询 并生成菜单列表
        public void ConditionQuery(WrapPanel wp)
        {
            //加载动画
            Resources.control.ProgressBar myBar = new Resources.control.ProgressBar();
            myBar.Show();


            var list = from tb in myModel.S_Product select tb;//查询所有产品数据
            //如果不为空 则进行筛选
            if (ProductText != ""&& ProductText!=null)
            {
                string text1 = ProductText.Trim();//获取条件
                list = list.Where(m => m.productName.Contains(text1));
                ProductTypeID = 0;
            }
            //转为list格式
            var list1 = list.ToList();
            //清空容器的子项
            wp.Children.Clear();
            //循环 创建产品控件 并追加
            if (list1.Count() > 0)
            {
                for (int i = 0; i < list1.Count(); i++)
                {
                    //实例化产品控件
                    Product_UC myUC = new Product_UC();
                    //传递参数
                    int ProductID = list1[i].productID;//获取当前行ID
                    var myUCViewModel = (myUC.DataContext as Product_UCVModel);//连接控件数据上下文
                    myUCViewModel.ProductID = ProductID;
                    myUCViewModel.ChangeTextEvent += AddProductDetail;//事件委托
                    wp.Children.Add(myUC);
                }
            }
            //关闭加载动画
            myBar.Close();
        }
        //根据产品ID 弹出添加产品详细窗口
        public void AddProductDetail(int ProductID) 
        {
            ProductDetailWindow myWindow = new ProductDetailWindow();//实例化窗口
            var myWindowViewModel = (myWindow.DataContext as ProductDetailVModel);//连接数据上下文
            myWindowViewModel.ProductID = ProductID;//传递产品ID
            myWindow.ShowDialog();//弹出窗口
            ifExisting = false;
            //如果用户点击保存
            if (myWindowViewModel.windowState)
            {
                ProductDetailVo myDetail = new ProductDetailVo();
                myDetail = myWindowViewModel.ProductDetails;//获取返回的数据

                //循环判断重复
                
                for (int i = 0; i < SalesAreaData.Count(); i++)
                {
                    //如果已存在 并且产品口味相同
                    if (SalesAreaData[i].productID== myDetail.productID && SalesAreaData[i].taste== myDetail.taste)
                    {
                        SalesAreaData[i].quantity = SalesAreaData[i].quantity + myDetail.quantity;//相加数量
                        SalesAreaData[i].Subtotal = SalesAreaData[i].quantity * SalesAreaData[i].price;//更新小计金额
                        ifExisting = true;
                        UpdateData();//产品总数量 产品总金额更新
                    }
                }
                //如果不存在 或 口味不相同
                if (ifExisting==false)
                {
                    SalesAreaData.Add(myDetail);//追加新一条
                    UpdateData();//产品总数量 产品总金额更新
                }
            }
        }
        //减数量
        public void ReduceQuantity()
        {
            if (SalesAreaEntity.quantity > 1)
            {
                SalesAreaEntity.quantity = SalesAreaEntity.quantity - 1;
                SalesAreaEntity.Subtotal = SalesAreaEntity.quantity * SalesAreaEntity.price;//更新小计金额
                UpdateData();//产品总数量 产品总金额更新
            }
        }
        //加数量
        public void PlusQuantity()
        {
            SalesAreaEntity.quantity = SalesAreaEntity.quantity + 1;
            SalesAreaEntity.Subtotal = SalesAreaEntity.quantity * SalesAreaEntity.price;//更新小计金额
            UpdateData();//产品总数量 产品总金额更新
        }
        //销售区 当前时间更新
        public void UpdateTime(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now;
        }
        //销售区 产品总数量 产品总金额更新
        public void UpdateData()
        {
            if (SalesAreaData!=null)
            {
                decimal totalQuantity=0;
                decimal? totalAmount = 0;
                for (int i = 0; i < SalesAreaData.Count(); i++)
                {
                    totalAmount += SalesAreaData[i].quantity * SalesAreaData[i].price;//产品总金额
                    totalQuantity += SalesAreaData[i].quantity;//产品总条数
                }
                TotalQuantity = totalQuantity;//产品总条数
                TotalAmount = totalAmount;//产品总金额
            }
            else
            {
                TotalAmount = 0;
                TotalQuantity = 0;
            }
        }
        //生成随机销售单号
        public void RandomNumber()
        {
            string number = "";
            Random rd = new Random();
            number = "XS"+rd.Next(10000,99999).ToString();//生成随机数
            //判断单号是否长度为7
            if (number.Length==7)
            {
                var list = myModel.B_SalesRecord.Where(m => m.salesNumber == number).Count();//查询数据库是否已存在此单号
                if (list > 0)
                {
                    RandomNumber();//如果存在 则递归 重新生成单号 直至不重复
                }
                else
                {
                    SalesOrderNumber = number;//不重复 则直接加载至页面
                }
            }
            else
            {
                RandomNumber();
            }
        }
        // 关闭窗口
        private void Clossewindow(Window window)
        {
            if (window != null)
            {
                try
                {
                    var res = MessageBoxX.Show("确定要关闭系统吗? 销售区未保存的数据会被清除!", "提示", Application.Current.MainWindow, MessageBoxButton.YesNo);
                    if (res.ToString() == "Yes")
                    {
                        window.Close();
                        System.Environment.Exit(0);
                    }
                }
                catch (Exception)
                {
                    return;
                }
                
            }
        }
        // 最小化窗口
        private void Minwindow(Window window)
        {
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
        //结账窗口
        public void Pay()
        {
            if (SalesAreaData.Count()>0)
            {
                PayMentWindow myWindow = new PayMentWindow();
                var myPayMent = (myWindow.DataContext as PayMentVModel);//链接上下文
                myPayMent.SalesAreaData = SalesAreaData;//传输 销售区表格数据
                myPayMent.SalesRecord.salesNumber = SalesOrderNumber;//销售单号
                myPayMent.SalesRecord.salesTime = CurrentTime;//销售时间
                myPayMent.SalesRecord.staffID = StaffNow.staffID;//员工ID
                myPayMent.TotalAmount = Convert.ToDecimal(TotalAmount);//应收金额
                myPayMent.IfSave = false;//是否保存成功标识
                myWindow.ShowDialog();
                
                if (myPayMent.IfSave)
                {
                    SalesAreaData = new ObservableCollection<ProductDetailVo>();//清空销售区数据
                    UpdateData();//更新销售区数据
                    RandomNumber();//重新生成销售单号
                    SelectWaitingAreaData();//刷新等待区数据
                }
                
            }
            else
            {
                Notice.Show("当前销售区没有产品数据", "提示", 2, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
