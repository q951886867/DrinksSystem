using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using DrinksSystem.Resources.control;
using DrinksSystem.Resources.PublicClass;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DrinksSystem.ViewModels.HomeVModel
{
    public class HomeVModel:ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public HomeVModel()
        {
            LoadedCommand = new RelayCommand(DownloadData);
            ComboBoxChangedCommand = new RelayCommand(DownloadData) ;
        }

        #region 属性
        public class TimeCondition
        {
            //选择时间段 下拉框
            public int timeID { get; set; }
            public string timeName { get; set; }
        }

        public class ProductSalesClass
        { 
            public string productName { get; set; }//产品名称
            public decimal sales { get; set; }//销售量
            public BitmapImage productImage { get; set; }//图片
        }
        //比较
        public class ProductSalesComparer : IComparer<ProductSalesClass>
        {
            public int Compare(ProductSalesClass p1, ProductSalesClass p2)
            {
                return p2.sales.CompareTo(p1.sales);
            }
        }
        //营业金额
        private ObservableCollection<HomeVos> amountData;
        public ObservableCollection<HomeVos> AmountData
        {
            get { return amountData; }
            set
            {
                amountData = value;
                RaisePropertyChanged(() => AmountData);
            }
        }
        
        //下拉框
        private List<TimeCondition> timeData=null;
        public List<TimeCondition> TimeData
        {
            get 
            {
                if (timeData==null)
                {
                    timeData = new List<TimeCondition>()
                    {
                        new TimeCondition(){ timeID =1,timeName="今日"},
                        new TimeCondition(){ timeID =2,timeName="最近一周"},
                        new TimeCondition(){ timeID =3,timeName="最近一个月"},
                        new TimeCondition(){ timeID =4,timeName="今年"},
                    };
                }
                return timeData;
            }
            set
            {
                timeData = value;
                RaisePropertyChanged(() => TimeData);
            }
        }

        //当前选中时间
        private int comboBoxSelectedValue=1;
        public int ComboBoxSelectedValue
        {
            get { return comboBoxSelectedValue; }
            set
            {
                if (comboBoxSelectedValue != value)
                {
                    comboBoxSelectedValue = value;
                    RaisePropertyChanged(() => ComboBoxSelectedValue);
                }
            }
        }

        //产品销量标题
        private string productSalesTitle;
        public string ProductSalesTitle
        {
            get { return productSalesTitle; }
            set
            {
                if (productSalesTitle != value)
                {
                    productSalesTitle = value;
                    RaisePropertyChanged(() => ProductSalesTitle);
                }
            }
        }

        //X轴Labels
        private string[] axisXLabels = new string[]{};
        public string[] AxisXLabels
        {
            get { return axisXLabels; }
            set
            {
                if (axisXLabels != value)
                {
                    axisXLabels = value;
                    RaisePropertyChanged(() => AxisXLabels);
                }
            }
        }

        //产品销量排行Data
        private List<ProductSalesClass> productSalesData ;
        public List<ProductSalesClass> ProductSalesData
        {
            get { return productSalesData; }
            set
            {
                if (productSalesData != value)
                {
                    productSalesData = value;
                    RaisePropertyChanged(() => ProductSalesData);
                }
            }
        }

        //SeriesValues
        private ChartValues<double> seriesValues = new ChartValues<double> {10,20,50,60 };
        public ChartValues<double> SeriesValues
        {
            get { return seriesValues; }
            set
            {
                if (seriesValues != value)
                {
                    seriesValues = value;
                    RaisePropertyChanged(() => SeriesValues);
                }
            }
        }


        //SeriesValues
        //private SeriesCollection seriesCollection = 
        //    new SeriesCollection() {
        //        new LineSeries{  Values=new ChartValues<double>{ 3,5,7,4} },
        //    };
        //public SeriesCollection SeriesCollection
        //{
        //    get { return seriesCollection; }
        //    set
        //    {
        //        if (seriesCollection != value)
        //        {
        //            seriesCollection = value;
        //            RaisePropertyChanged(() => SeriesCollection);
        //        }
        //    }
        //}
        #endregion

        #region 命令
        public RelayCommand LoadedCommand { get; set; }//页面加载
        public RelayCommand ComboBoxChangedCommand { get; set; }//时间选中事件
        #endregion

        #region 函数
        //页面加载事件
        //private void Load() 
        //{


        //    SelectBusiness();
        //}
        //数据加载
        private void DownloadData()
        {
            SetChart();
            SelectBusiness();
            ProductSales();
        }
        //图表
        private void SetChart()
        {
            var list = (from tb in myModel.B_SalesRecord select tb).ToList();
            ChartValues<double> myDouble = new ChartValues<double>();
            if (ComboBoxSelectedValue==1)
            {//近七日
                AxisXLabels = new string[7];
                DateTime dateTime1 = DateTime.Today;
                DateTime dateTime2 = DateTime.Today.AddDays(1);
                for (int i = 0; i < 7; i++)
                {
                    decimal salesAmount = 0;//金额
                    for (int a = 0; a < list.Count; a++)
                    {
                        if (list[a].salesTime>= dateTime1 && list[a].salesTime< dateTime2)
                        {
                            salesAmount += Convert.ToDecimal(list[a].salesAmount);
                        }
                    }
                    AxisXLabels[6-i] = dateTime1.ToString("yyyy-MM-dd");
                    myDouble.Insert(0,Convert.ToDouble(salesAmount));
                    dateTime1=dateTime1.AddDays(-1);
                    dateTime2=dateTime2.AddDays(-1);
                }
                SeriesValues = myDouble;
            }
            else if(ComboBoxSelectedValue==2)
            {//最近七周
                AxisXLabels = new string[7];
                DateTime dateTime1 = DateTime.Today.AddDays(-6);
                DateTime dateTime2 = DateTime.Today.AddDays(1);
                for (int i = 0; i < 7; i++)
                {
                    decimal salesAmount = 0;//金额
                    for (int a = 0; a < list.Count; a++)
                    {
                        if (list[a].salesTime >= dateTime1 && list[a].salesTime < dateTime2)
                        {
                            salesAmount += Convert.ToDecimal(list[a].salesAmount);
                        }
                    }
                    AxisXLabels[6 - i] = dateTime1.ToString("yyyy-MM-dd");
                    myDouble.Insert(0,Convert.ToDouble(salesAmount));
                    dateTime1 = dateTime1.AddDays(-6);
                    dateTime2 = dateTime2.AddDays(-7);
                }
                SeriesValues = myDouble;
            }
            else if (ComboBoxSelectedValue==3)
            {
                //最近七月
                AxisXLabels = new string[7];
                DateTime dateTime1 = DateTime.Today.AddMonths(-1);
                DateTime dateTime2 = DateTime.Today.AddDays(1);
                for (int i = 0; i < 7; i++)
                {
                    decimal salesAmount = 0;//金额
                    for (int a = 0; a < list.Count; a++)
                    {
                        if (list[a].salesTime >= dateTime1 && list[a].salesTime < dateTime2)
                        {
                            salesAmount += Convert.ToDecimal(list[a].salesAmount);
                        }
                    }
                    AxisXLabels[6 - i] = dateTime1.ToString("yyyy-MM-dd");
                    myDouble.Insert(0, Convert.ToDouble(salesAmount));
                    dateTime1 = dateTime1.AddMonths(-1);
                    dateTime2 = dateTime2.AddMonths(-1);
                }
                SeriesValues = myDouble;
            }
            else if (ComboBoxSelectedValue==4)
            {
                //最近七年
                AxisXLabels = new string[7];
                DateTime dateTime1 = Convert.ToDateTime(DateTime.Today.Year.ToString() + "/1/1 0:00:00");
                DateTime dateTime2 = dateTime1.AddYears(1);
                for (int i = 0; i < 7; i++)
                {
                    decimal salesAmount = 0;//金额
                    for (int a = 0; a < list.Count; a++)
                    {
                        if (list[a].salesTime >= dateTime1 && list[a].salesTime < dateTime2)
                        {
                            salesAmount += Convert.ToDecimal(list[a].salesAmount);
                        }
                    }
                    AxisXLabels[6 - i] = dateTime1.ToString("yyyy");
                    myDouble.Insert(0, Convert.ToDouble(salesAmount));
                    dateTime1 = dateTime1.AddYears(-1);
                    dateTime2 = dateTime2.AddYears(-1);
                }
                SeriesValues = myDouble;
            }
        }
        //查询营业金额
        private void SelectBusiness()
        {
            var selectValue = ComboBoxSelectedValue;
            var totalAmountData = (from tb in myModel.B_SalesRecord select tb).ToList();
            var totalAmountDataLast = (from tb in myModel.B_SalesRecord select tb).ToList();

            var memberData = (from tb in myModel.B_MemberRechargeRecord select tb).ToList();
            var memberDataLast = (from tb in myModel.B_MemberRechargeRecord select tb).ToList();
            if (selectValue==1)
            {
                //今日
                totalAmountData = totalAmountData.Where(m => m.salesTime >= DateTime.Today).ToList();
                memberData = memberData.Where(m => m.rechargeTime >= DateTime.Today).ToList();
                //昨日 作比较
                DateTime lastData = DateTime.Today.AddDays(-1);
                totalAmountDataLast = totalAmountDataLast.Where(m => m.salesTime >= lastData && m.salesTime< DateTime.Today).ToList();
                memberDataLast = memberDataLast.Where(m => m.rechargeTime >= lastData && m.rechargeTime < DateTime.Today).ToList();
            }
            else if (selectValue==2)
            {
                //最近一周
                //根据当前时间 减去6天 包含今天就是7天
                DateTime lastWeekTime = DateTime.Today.AddDays(-6);
                totalAmountData = totalAmountData.Where(m => m.salesTime >= lastWeekTime && m.salesTime < DateTime.Today.AddDays(1)).ToList();
                memberData = memberData.Where(m => m.rechargeTime >= lastWeekTime && m.rechargeTime < DateTime.Today.AddDays(1)).ToList();

                //最近一周的前一周 作比较
                DateTime lastWeekTime1 = DateTime.Today.AddDays(-13); 
                totalAmountDataLast = totalAmountDataLast.Where(m => m.salesTime >= lastWeekTime1 && m.salesTime < lastWeekTime).ToList();
                memberDataLast = memberDataLast.Where(m => m.rechargeTime >= lastWeekTime1 && m.rechargeTime < lastWeekTime).ToList();
            }
            else if (selectValue==3)
            {
                //最近一个月
                //根据当前时间 减去30天 包含今天就是31天
                DateTime lastMonthTime = DateTime.Today.AddDays(-30);
                totalAmountData = totalAmountData.Where(m => m.salesTime >= lastMonthTime && m.salesTime < DateTime.Today.AddDays(1)).ToList();
                memberData = memberData.Where(m => m.rechargeTime >= lastMonthTime && m.rechargeTime < DateTime.Today.AddDays(1)).ToList();

                //最近一个月的前一个月 作比较
                DateTime lastMonthTime1 = DateTime.Today.AddDays(-61);
                totalAmountDataLast = totalAmountDataLast.Where(m => m.salesTime >= lastMonthTime1 && m.salesTime < lastMonthTime).ToList();
                memberDataLast = memberDataLast.Where(m => m.rechargeTime >= lastMonthTime1 && m.rechargeTime < lastMonthTime).ToList();
            }
            else if (selectValue==4)
            {
                //今年
                DateTime thisYear = Convert.ToDateTime(DateTime.Today.Year.ToString()+ "/1/1 0:00:00");
                totalAmountData = totalAmountData.Where(m => m.salesTime >= thisYear).ToList();
                memberData = memberData.Where(m => m.rechargeTime >= thisYear).ToList();

                //去年 作比较
                DateTime lastYear = Convert.ToDateTime(DateTime.Today.Year.ToString() + "/1/1 0:00:00");
                lastYear.AddYears(-1);
                totalAmountDataLast = totalAmountDataLast.Where(m => m.salesTime >= lastYear && m.salesTime < thisYear).ToList();
                memberDataLast = memberDataLast.Where(m => m.rechargeTime >= lastYear && m.rechargeTime < thisYear).ToList();
            }
            HomeVos myTotalAmountData = new HomeVos(){DataType="营业额",Percentage=100};//营业额
            HomeVos myWechatData = new HomeVos() { DataType = "微信收入" };//微信
            HomeVos myCashData = new HomeVos() { DataType = "现金收入" };//现金
            HomeVos myOrderData = new HomeVos() { DataType = "订单数",TotalAmount= totalAmountData .Count};//订单
            HomeVos myMemberData = new HomeVos() { DataType = "会员充值" };//会员充值

            //金额 和 订单数
            for (int i = 0; i < totalAmountData.Count; i++)
            {
                myTotalAmountData.TotalAmount += Convert.ToDecimal(totalAmountData[i].salesAmount);
                myWechatData.TotalAmount += Convert.ToDecimal(totalAmountData[i].wechatPay);
                myCashData.TotalAmount += Convert.ToDecimal(totalAmountData[i].cashPay);
            }
            //会员充值金额（不包含赠送金额）
            for (int i = 0; i < memberData.Count; i++)
            {
                myMemberData.TotalAmount += Convert.ToDecimal(memberData[i].rechargeAmount);
            }
            //微信和现金占总营业额 百分比
            if (myTotalAmountData.TotalAmount!=0)
            {
                myWechatData.Percentage = Convert.ToInt32((myWechatData.TotalAmount / myTotalAmountData.TotalAmount) * 100);
                myCashData.Percentage = Convert.ToInt32((myCashData.TotalAmount / myTotalAmountData.TotalAmount) * 100);
            }
            else
            {
                myWechatData.Percentage = 0;
                myCashData.Percentage = 0;
                myTotalAmountData.Percentage = 0;
            }

            //循环 上一次的金额和订单数 和 本次的金额和订单数的差价
            
            decimal lastTotalAmount = 0;//上一次营业额
            decimal lastWechat = 0;//上一次微信
            decimal lastCash = 0;//上一次现金
            decimal lastOrder = totalAmountDataLast.Count;//上一次订单
            for (int i = 0; i < totalAmountDataLast.Count; i++)
            {
                lastTotalAmount += Convert.ToDecimal(totalAmountDataLast[i].salesAmount);
                lastWechat += Convert.ToDecimal(totalAmountDataLast[i].wechatPay);
                lastCash += Convert.ToDecimal(totalAmountDataLast[i].cashPay);
            }
            lastTotalAmount = myTotalAmountData.TotalAmount - lastTotalAmount;
            lastWechat = myWechatData.TotalAmount - lastWechat;
            lastCash = myCashData.TotalAmount - lastCash;
            lastOrder = myOrderData.TotalAmount - lastOrder;

            //循环 上一次会员充值金额和本次充值金额 差价
            decimal lastMember = 0;
            for (int i = 0; i < memberDataLast.Count; i++)
            {
                lastMember+= Convert.ToDecimal(memberDataLast[i].rechargeAmount);
            }
            lastMember = myMemberData.TotalAmount - lastMember;

            //判断和上一次的差量 如果大于等于0 则是上升icon
            if (lastTotalAmount>=0)
            {
                myTotalAmountData.IsUp = true;
                if (selectValue==1)
                {
                    myTotalAmountData.ThanLast = "较昨日：+" + lastTotalAmount;
                }
                if (selectValue == 2)
                {
                    myTotalAmountData.ThanLast = "较前一周：+" + lastTotalAmount;
                }
                if (selectValue == 3)
                {
                    myTotalAmountData.ThanLast = "较上一月：+" + lastTotalAmount;
                }
                if (selectValue == 4)
                {
                    myTotalAmountData.ThanLast = "较去年：+" + lastTotalAmount;
                }
            }
            else
            {
                myTotalAmountData.IsUp = false;
                if (selectValue == 1)
                {
                    myTotalAmountData.ThanLast = "较昨日：" + lastTotalAmount;
                }
                if (selectValue == 2)
                {
                    myTotalAmountData.ThanLast = "较前一周：" + lastTotalAmount;
                }
                if (selectValue == 3)
                {
                    myTotalAmountData.ThanLast = "较上一月：" + lastTotalAmount;
                }
                if (selectValue == 4)
                {
                    myTotalAmountData.ThanLast = "较去年：" + lastTotalAmount;
                }
            }

            if (lastWechat >= 0)
            {
                myWechatData.IsUp = true;
                if (selectValue == 1)
                {
                    myWechatData.ThanLast = "较昨日：+" + lastWechat;
                }
                if (selectValue == 2)
                {
                    myWechatData.ThanLast = "较前一周：+" + lastWechat;
                }
                if (selectValue == 3)
                {
                    myWechatData.ThanLast = "较上一月：+" + lastWechat;
                }
                if (selectValue == 4)
                {
                    myWechatData.ThanLast = "较去年：+" + lastWechat;
                }
            }
            else
            {
                myWechatData.IsUp = false;
                if (selectValue == 1)
                {
                    myWechatData.ThanLast = "较昨日：" + lastWechat;
                }
                if (selectValue == 2)
                {
                    myWechatData.ThanLast = "较前一周：" + lastWechat;
                }
                if (selectValue == 3)
                {
                    myWechatData.ThanLast = "较上一月：" + lastWechat;
                }
                if (selectValue == 4)
                {
                    myWechatData.ThanLast = "较去年：" + lastWechat;
                }
            }

            if (lastCash >= 0)
            {
                myCashData.IsUp = true;
                if (selectValue == 1)
                {
                    myCashData.ThanLast = "较昨日：+" + lastCash;
                }
                if (selectValue == 2)
                {
                    myCashData.ThanLast = "较前一周：+" + lastCash;
                }
                if (selectValue == 3)
                {
                    myCashData.ThanLast = "较上一月：+" + lastCash;
                }
                if (selectValue == 4)
                {
                    myCashData.ThanLast = "较去年：+" + lastCash;
                }
            }
            else
            {
                myCashData.IsUp = false;
                if (selectValue == 1)
                {
                    myCashData.ThanLast = "较昨日：" + lastCash;
                }
                if (selectValue == 2)
                {
                    myCashData.ThanLast = "较前一周：" + lastCash;
                }
                if (selectValue == 3)
                {
                    myCashData.ThanLast = "较上一月：" + lastCash;
                }
                if (selectValue == 4)
                {
                    myCashData.ThanLast = "较去年：" + lastCash;
                }
            }

            if (lastOrder >= 0)
            {
                myOrderData.IsUp = true;
                if (selectValue == 1)
                {
                    myOrderData.ThanLast = "较昨日：+" + lastOrder;
                }
                if (selectValue == 2)
                {
                    myOrderData.ThanLast = "较前一周：+" + lastOrder;
                }
                if (selectValue == 3)
                {
                    myOrderData.ThanLast = "较上一月：+" + lastOrder;
                }
                if (selectValue == 4)
                {
                    myOrderData.ThanLast = "较去年：+" + lastOrder;
                }
            }
            else
            {
                myOrderData.IsUp = false;
                if (selectValue == 1)
                {
                    myOrderData.ThanLast = "较昨日：" + lastOrder;
                }
                if (selectValue == 2)
                {
                    myOrderData.ThanLast = "较前一周：" + lastOrder;
                }
                if (selectValue == 3)
                {
                    myOrderData.ThanLast = "较上一月：" + lastOrder;
                }
                if (selectValue == 4)
                {
                    myOrderData.ThanLast = "较去年：" + lastOrder;
                }
            }

            if (lastMember >= 0)
            {
                myMemberData.IsUp = true;
                if (selectValue == 1)
                {
                    myMemberData.ThanLast = "较昨日：+" + lastMember;
                }
                if (selectValue == 2)
                {
                    myMemberData.ThanLast = "较前一周：+" + lastMember;
                }
                if (selectValue == 3)
                {
                    myMemberData.ThanLast = "较上一月：+" + lastMember;
                }
                if (selectValue == 4)
                {
                    myMemberData.ThanLast = "较去年：+" + lastMember;
                }
            }
            else
            {
                myMemberData.IsUp = false;
                if (selectValue == 1)
                {
                    myMemberData.ThanLast = "较昨日：" + lastMember;
                }
                if (selectValue == 2)
                {
                    myMemberData.ThanLast = "较前一周：" + lastMember;
                }
                if (selectValue == 3)
                {
                    myMemberData.ThanLast = "较上一月：" + lastMember;
                }
                if (selectValue == 4)
                {
                    myMemberData.ThanLast = "较去年：" + lastMember;
                }
            }

            AmountData = new ObservableCollection<HomeVos>();
            AmountData.Add(myTotalAmountData);
            AmountData.Add(myWechatData);
            AmountData.Add(myCashData);
            AmountData.Add(myOrderData);
            AmountData.Add(myMemberData);
        }
        //产品销量排行
        private void ProductSales()
        {
            var list = (from tb in myModel.B_SalesRecord select tb).ToList();
            if (ComboBoxSelectedValue == 1)
            {
                //今日
                list = list.Where(m => m.salesTime >= DateTime.Today ).ToList();
                ProductSalesTitle = "今日销量排行";
            }
            else if (ComboBoxSelectedValue ==2)
            {
                //最近一周
                DateTime lastWeekTime = DateTime.Today.AddDays(-6);
                list = list.Where(m => m.salesTime >= lastWeekTime && m.salesTime < DateTime.Today.AddDays(1)).ToList();
                ProductSalesTitle = "最近一周销量排行";
            }
            else if (ComboBoxSelectedValue == 3)
            {
                //最近一个月
                DateTime lastMonthTime = DateTime.Today.AddDays(-30);
                list = list.Where(m => m.salesTime >= lastMonthTime && m.salesTime < DateTime.Today.AddDays(1)).ToList();
                ProductSalesTitle = "最近一个月销量排行";
            }
            else if (ComboBoxSelectedValue == 4)
            {
                //今年
                DateTime thisYear = Convert.ToDateTime(DateTime.Today.Year.ToString() + "/1/1 0:00:00");
                list = list.Where(m => m.salesTime >= thisYear).ToList();
                ProductSalesTitle = "今年销量排行";
            }
            List<ProductSalesClass> myProductSales = new List<ProductSalesClass>();
            for (int i = 0; i < list.Count; i++)
            {
                int salesID = list[i].salesRecordID;
                var list1 = (from tb in myModel.B_SalesRecordDetails where tb.salesRecordID == salesID 
                             join tbProduct in myModel.S_Product on tb.productID equals tbProduct.productID
                             select new ProductDetailVo() {
                                productName=tbProduct.productName,
                                 quantity = tb.quantity,
                                 productImage=tbProduct.productImage
                             }).ToList();
                for (int a = 0; a < list1.Count; a++)
                {
                    bool ifExist=true;//产品是否已存在标识
                    for (int b = 0; b < myProductSales.Count; b++)
                    {
                        if (myProductSales[b].productName== list1[a].productName)
                        {
                            myProductSales[b].sales += Convert.ToDecimal(list1[a].quantity);
                            ifExist = false;//已存在
                        }
                    }
                    if (ifExist)//如果不存在
                    {
                        ProductSalesClass myProduct = new ProductSalesClass() { 
                            productName= list1[a].productName,sales= Convert.ToDecimal(list1[a].quantity),productImage= ByteConvertBitmapImage.ByteArrayToBitmapImage(list1[a].productImage) };
                        myProductSales.Add(myProduct);
                    }
                }
            }
            myProductSales.Sort(new ProductSalesComparer());
            if (myProductSales.Count>5)
            {
                int shu = myProductSales.Count - 5;
                myProductSales.RemoveRange(4, shu);
            }
            ProductSalesData = myProductSales;
        }
        #endregion
    }
}
