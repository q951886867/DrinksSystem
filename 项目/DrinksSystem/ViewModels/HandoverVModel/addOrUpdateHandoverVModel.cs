using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrinksSystem.ViewModels.HandoverVModel
{
    public class addOrUpdateHandoverVModel : ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public addOrUpdateHandoverVModel()
        {
            LoadedCommand = new RelayCommand(Load);//加载 查询下拉框
            CloseCommand = new RelayCommand<Window>(CloseWindow);//关闭窗口
            SaveCommand = new RelayCommand<Window>(Save);//关闭窗口
            TextChangeCommand = new RelayCommand(TextChange);//关闭窗口
            DateTieChangedCommand = new RelayCommand(CalculateBusinessAmount);//计算营业额
        }

        #region 属性
        //数据源
        private HandoverVo _handoverData;
        public HandoverVo HandoverData
        {
            get { return _handoverData; }
            set
            {
                if (_handoverData != value)
                {
                    _handoverData = value;
                    RaisePropertyChanged(() => HandoverData);
                }

            }
        }

        //员工下拉框数据源
        private List<S_Staff> _staffComboBox;
        public List<S_Staff> StaffComboBox
        {
            get { return _staffComboBox; }
            set
            {
                if (_staffComboBox != value)
                {
                    _staffComboBox = value;
                    RaisePropertyChanged(() => StaffComboBox);
                }
            }
        }
        //窗口title
        private string _windowTitle;
        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    RaisePropertyChanged(() => WindowTitle);
                }
            }
        }
        public bool IsAdd;//新增、修改 的标识
        //刷新表格委托
        public delegate void CompletedEvent();
        public event CompletedEvent HandoverRefresh;



        //起始日期
        private string _startDate;
        public string StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    RaisePropertyChanged(() => StartDate);
                }
            }
        }
        //起始时间
        private string _startTime;
        public string StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    RaisePropertyChanged(() => StartTime);
                }
            }
        }
        //结束日期
        private string _endDate;
        public string EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    RaisePropertyChanged(() => EndDate);
                }
            }
        }
        //结束时间
        private string _endTime;
        public string EndTime
        {
            get { return _endTime; }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    RaisePropertyChanged(() => EndTime);
                }
            }
        }
        public bool ifCheckout { get; set; } = false;//是否收银台操作标识
        #endregion

        #region 命令
        public RelayCommand LoadedCommand { get; set; }// 加载
        public RelayCommand<Window> CloseCommand { get; set; }//关闭窗口
        public RelayCommand<Window> SaveCommand { get; set; }//保存
        public RelayCommand TextChangeCommand { get; set; }//保存
        public RelayCommand DateTieChangedCommand { get; set; }//日期选中
        #endregion

        #region 函数
        //根据当前日期 查询该期间 该员工的 订单流水金额
        private void CalculateBusinessAmount()
        {
            if (ifCheckout)
            {
                if (StartDate!=null&& StartTime!=null&& EndDate!= null && EndTime!= null && HandoverData.staffID!=null)
                {
                    //起始
                    var dateStartDate = Convert.ToDateTime(StartDate);//字符串转换DateTime
                    var shortDateStartDate = dateStartDate.ToShortDateString();//DateTime转换 yyyy-MM-dd
                    var timeStartTime = Convert.ToDateTime(StartTime);//字符串转换DateTime
                    var shortStartTime = timeStartTime.ToLongTimeString();//DateTime转换 HH:mm:ss
                    string startdatetime = shortDateStartDate + " " + shortStartTime;//拼接日期和时间

                    //结束
                    var dateEndDate = Convert.ToDateTime(EndDate);//字符串转换DateTime
                    var shortDateEndDate = dateEndDate.ToShortDateString();//DateTime转换 yyyy-MM-dd
                    var timeEndTime = Convert.ToDateTime(EndTime);//字符串转换DateTime
                    var shortEndTime = timeEndTime.ToShortTimeString();//DateTime转换 HH:mm:ss
                    string enddatetime = shortDateEndDate + " " + shortEndTime;//拼接日期和时间

                    DateTime start = Convert.ToDateTime(startdatetime);
                    DateTime end = Convert.ToDateTime(enddatetime);
                    if (start < end)
                    {

                        //根据选中的时间段之间 和当前的员工ID 查询订单
                        var list = (from tb in myModel.B_SalesRecord
                                    where tb.staffID == HandoverData.staffID && tb.salesTime >= start && tb.salesTime <= end
                                    select tb).ToList();

                        //循环计算微信收入金额 和 现金收入金额
                        decimal cash = 0;
                        decimal wechat = 0;
                        for (int i = 0; i < list.Count; i++)
                        {
                            cash += Convert.ToDecimal(list[i].cashPay);//现金
                            wechat += Convert.ToDecimal(list[i].wechatPay);//微信
                        }
                        HandoverData.cashIncome = cash;
                        HandoverData.wechatIncome = wechat;
                        TextChange();//更新营业总额
                    }
                }
            }
        }
        //加载
        private void Load()
        {
            //查询下拉框数据
            var list1 = from tb in myModel.S_Staff select tb;
            StaffComboBox = list1.ToList();

        }
        //关闭窗口
        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        //保存
        private void Save(Window window)
        {
            //判断页面数据是否填写完整
            if (HandoverData.staffID != null && HandoverData.cashIncome != null && HandoverData.wechatIncome != null &&
                HandoverData.amountHanded != null && HandoverData.reserveFund != null && HandoverData.businessAmount != null
                && StartDate != null && StartTime != null && EndDate != null && EndTime != null)
            {
                //起始
                var dateStartDate = Convert.ToDateTime(StartDate);//字符串转换DateTime
                var shortDateStartDate = dateStartDate.ToShortDateString();//DateTime转换 yyyy-MM-dd
                var timeStartTime = Convert.ToDateTime(StartTime);//字符串转换DateTime
                var shortStartTime = timeStartTime.ToLongTimeString();//DateTime转换 HH:mm:ss
                string startdatetime = shortDateStartDate + " " + shortStartTime;//拼接日期和时间

                //结束
                var dateEndDate = Convert.ToDateTime(EndDate);//字符串转换DateTime
                var shortDateEndDate = dateEndDate.ToShortDateString();//DateTime转换 yyyy-MM-dd
                var timeEndTime = Convert.ToDateTime(EndTime);//字符串转换DateTime
                var shortEndTime = timeEndTime.ToShortTimeString();//DateTime转换 HH:mm:ss
                string enddatetime = shortDateEndDate + " " + shortEndTime;//拼接日期和时间

                if (Convert.ToDateTime(startdatetime) < Convert.ToDateTime(enddatetime))
                {
                    if ((HandoverData.amountHanded+ HandoverData.reserveFund)== HandoverData.businessAmount)
                    {
                        //获取页面数据
                        B_Handover myHandover = new B_Handover()
                        {
                            cashIncome = HandoverData.cashIncome,
                            wechatIncome = HandoverData.wechatIncome,
                            amountHanded = HandoverData.amountHanded,
                            reserveFund = HandoverData.reserveFund,
                            businessAmount = HandoverData.businessAmount,
                            staffID = HandoverData.staffID,
                        };



                        //赋值
                        myHandover.startTime = Convert.ToDateTime(startdatetime);
                        myHandover.endTime = Convert.ToDateTime(enddatetime);

                        if (IsAdd)
                        {
                            myModel.B_Handover.Add(myHandover);
                            if (myModel.SaveChanges() > 0)
                            {
                                Notice.Show("新增成功", "提示", 2, MessageBoxIcon.Success);
                                if (HandoverRefresh != null)
                                {
                                    HandoverRefresh();//委托刷新
                                }
                                window.Close();//关闭窗口
                            }
                            else
                            {
                                Notice.Show("新增失败", "提示", 2, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            myHandover.handoverID = HandoverData.handoverID;
                            myModel.Entry(myHandover).State = System.Data.Entity.EntityState.Modified;
                            if (myModel.SaveChanges() > 0)
                            {
                                Notice.Show("修改成功", "提示", 2, MessageBoxIcon.Success);
                                if (HandoverRefresh != null)
                                {
                                    HandoverRefresh();//委托刷新
                                }
                                window.Close();//关闭窗口
                            }
                            else
                            {
                                Notice.Show("修改失败", "提示", 2, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        Notice.Show("上交金额和下放金额与营业金额不匹配", "提示", 2, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Notice.Show("起始时间不能大于结束时间", "提示", 2, MessageBoxIcon.Error);
                }
            }
        }
        //TextChang  计算总营业额
        private void TextChange()
        {
            HandoverData.businessAmount = HandoverData.cashIncome + HandoverData.wechatIncome;
        }
        #endregion
    }
}
