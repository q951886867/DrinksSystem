using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
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

namespace DrinksSystem.ViewModels.CheckoutCounterVModel
{
    public class PayMentVModel : ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public PayMentVModel()
        {
            DragMoveCommand = new RelayCommand<Window>(DragMovewindow);//移动窗口
            CloseCommand = new RelayCommand<Window>(Close);//关闭窗口
            LoadedCommand = new RelayCommand(Load);//加载
            TemperatureCommand = new RelayCommand<bool>(TemperatureClick);
            LostFocusCommand = new RelayCommand(IfMember);//会员账号名称判断
            SaveCommand = new RelayCommand<Window>(Save);//确定
        }

        #region 属性
        //销售记录表
        private B_SalesRecord _salesRecord=new B_SalesRecord();
        public B_SalesRecord SalesRecord
        {
            get { return _salesRecord; }
            set
            {
                if (_salesRecord != value)
                {
                    _salesRecord = value;
                    RaisePropertyChanged(() => SalesRecord);
                }
            }
        }
        //销售记录明细列表 新增到数据库的
        private List<B_SalesRecordDetails> _salesRecordDetailsList;
        public List<B_SalesRecordDetails> SalesRecordDetailsList
        {
            get { return _salesRecordDetailsList; }
            set
            {
                if (_salesRecordDetailsList != value)
                {
                    _salesRecordDetailsList = value;
                    RaisePropertyChanged(() => SalesRecordDetailsList);
                }
            }
        }
        //销售区数据 接受传递过来的数据
        private ObservableCollection<ProductDetailVo> _salesAreaData = new ObservableCollection<ProductDetailVo>();
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
        //应收金额
        private decimal _totalAmount;
        public decimal TotalAmount
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
        //折扣金额
        private decimal _discountAmount=0;
        public decimal DiscountAmount
        {
            get { return _discountAmount; }
            set
            {
                if (_discountAmount != value)
                {
                    _discountAmount = value;
                    RaisePropertyChanged(() => DiscountAmount);
                }
            }
        }
        //实收金额
        private decimal _salesAmount=0;
        public decimal SalesAmount
        {
            get { return _salesAmount; }
            set
            {
                if (_salesAmount != value)
                {
                    _salesAmount = value;
                    RaisePropertyChanged(() => SalesAmount);
                }
            }
        }
        //微信支付
        private decimal _weChatPay;
        public decimal WeChatPay
        {
            get { return _weChatPay; }
            set
            {
                if (_weChatPay != value)
                {
                    _weChatPay = value;
                    RaisePropertyChanged(() => WeChatPay);
                }
            }
        }
        //现金支付
        private decimal _cashPayment;
        public decimal CashPayment
        {
            get { return _cashPayment; }
            set
            {
                if (_cashPayment != value)
                {
                    _cashPayment = value;
                    RaisePropertyChanged(() => CashPayment);
                }
            }
        }
        //会员余额
        private decimal _memberBalance=0;
        public decimal MemberBalance
        {
            get { return _memberBalance; }
            set
            {
                if (_memberBalance != value)
                {
                    _memberBalance = value;
                    RaisePropertyChanged(() => MemberBalance);
                }
            }
        }
        //会员卡号
        private string _memberNumber;
        public string MemberNumber
        {
            get { return _memberNumber; }
            set
            {
                if (_memberNumber != value)
                {
                    _memberNumber = value;
                    RaisePropertyChanged(() => MemberNumber);
                }
            }
        }
        //会员姓名
        private string _memberName;
        public string MemberName
        {
            get { return _memberName; }
            set
            {
                if (_memberName != value)
                {
                    _memberName = value;
                    RaisePropertyChanged(() => MemberName);
                }
            }
        }
        public bool IsMember;//是否会员消费标识
        public bool IfSave=false;//是否保存成功标识
        #endregion

        #region 命令
        public RelayCommand<Window> CloseCommand { get; set; }//关闭窗口
        public RelayCommand<Window> DragMoveCommand { get; set; }//移动窗口
        public RelayCommand LoadedCommand { get; set; }//加载
        public RelayCommand<bool> TemperatureCommand { get; set; }//是否会员消费
        public RelayCommand LostFocusCommand { get; set; }//是否会员消费
        public RelayCommand<Window> SaveCommand { get; set; }//是否会员消费

        #endregion

        #region 函数
        //判断会员账号 和名称 是否正确
        public void IfMember()
        {
            //账号和名称不为空
            if (MemberName!=""&& MemberNumber!=""&& MemberNumber != null&& MemberName != null)
            {
                var list = (from tb in myModel.S_Member where tb.memberName == MemberName.Trim() && tb.memberNumber == MemberNumber.Trim() select tb).ToList();
                if (list.Count()>0)
                {
                    MemberBalance = Convert.ToDecimal(list[0].memberBalance);//会员余额
                    Notice.Show("会员账号名称正确~", "提示", 2, MessageBoxIcon.Success);
                }
                else
                {
                    Notice.Show("会员或账号名称错误！", "提示", 2, MessageBoxIcon.Error);
                }
            }
        }
        //单选按钮点击事件 改变是否会员消费标识
        public void TemperatureClick(bool IsMember1)
        {
            if (IsMember1)
            {
                IsMember = true;
                Load();//根据当前的消费类型 刷新当前页面金额
            }
            else
            {
                IsMember = false;
                Load();
            }
        }
        //关闭窗口
        public void Close(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
        //移动窗口
        public void DragMovewindow(Window window)
        {
            window.DragMove();
        }
        //加载页面
        public void Load()
        {
            //如果是会员消费
            if (IsMember)
            {
                decimal myMemberPrice = 0;
                //循环 计算销售区所有产品 会员单价的总和
                for (int i = 0; i < SalesAreaData.Count(); i++)
                {
                    //根据产品ID查询该产品会员单价
                    var pID = SalesAreaData[i].productID;//获取ID
                    var list = (from tb in myModel.S_Product where tb.productID == pID select tb).Single();
                    myMemberPrice += Convert.ToDecimal(SalesAreaData[i].quantity) * Convert.ToDecimal(list.memberPrice);//数量 * 产品会员单价
                }
                SalesAmount = myMemberPrice;//实收金额 (会员优惠后的价格)
                DiscountAmount = TotalAmount - SalesAmount;//折扣金额 (原价总和 - 会员实收金额总和)
                MemberNumber = "";//会员卡号
                MemberName = "";//会员姓名
                MemberBalance = 0;//会员余额
            }
            else
            {
                //正常消费
                DiscountAmount = 0;//折扣金额
                SalesAmount = TotalAmount;//实收金额
                
            }
        }
        //保存
        private void Save(Window window)
        {
            if (IsMember)
            {
                if (MemberNumber!=""&& MemberNumber!=null&& MemberName!=""&& MemberName!=null)
                {
                    //查询会员账号姓名是否正确
                    var member = (from tb in myModel.S_Member where tb.memberName == MemberName.Trim() && tb.memberNumber == MemberNumber.Trim() select tb).ToList();
                    if (member.Count()==1)
                    {
                        if (MemberBalance >= SalesAmount)
                        {
                            try
                            {
                                //引用事务
                                using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                                {
                                    //销售记录表B_SalesRecord
                                    B_SalesRecord mySalesRecord = new B_SalesRecord();
                                    mySalesRecord = SalesRecord;
                                    mySalesRecord.ifTakeout = false;//是否外卖
                                    mySalesRecord.cashPay = 0;//现金支付
                                    mySalesRecord.wechatPay = 0;//微信支付
                                    mySalesRecord.ifMember = true;//是否会员消费
                                    mySalesRecord.memberID = member[0].memberID;//会员ID
                                    mySalesRecord.orderStatus = false;//订单状态 (false 制作中 true 制作完成)
                                    mySalesRecord.ifRedeem = false;//是否积分兑换
                                    mySalesRecord.salesAmount = SalesAmount;//订单合计金额 (实收金额)
                                    myModel.B_SalesRecord.Add(mySalesRecord);
                                    myModel.SaveChanges();


                                    //销售记录明细表B_SalesRecordDetails
                                    string salesNumber = mySalesRecord.salesNumber;//根据销售单号查询上面新增的销售记录ID
                                    var list = (from tb in myModel.B_SalesRecord where tb.salesNumber == salesNumber select tb).Single();
                                    //循环修改销售记录明细中产品单价为会员单价
                                    for (int i = 0; i < SalesAreaData.Count(); i++)
                                    {
                                        //根据ID查询产品
                                        int? id = SalesAreaData[i].productID;
                                        var list1 = (from tb in myModel.S_Product where tb.productID == id select tb).Single();
                                        SalesAreaData[i].price = list1.memberPrice;
                                    }
                                    //循环传过来的 销售区数据 新增销售记录明细
                                    for (int i = 0; i < SalesAreaData.Count(); i++)
                                    {
                                        B_SalesRecordDetails mySalesRecordDetails = new B_SalesRecordDetails();
                                        mySalesRecordDetails.salesRecordID = list.salesRecordID;//获取上面新增的销售记录表ID
                                        mySalesRecordDetails.productID = SalesAreaData[i].productID;//产品ID
                                        mySalesRecordDetails.taste = SalesAreaData[i].taste;//口味
                                        mySalesRecordDetails.quantity = SalesAreaData[i].quantity;//数量
                                        mySalesRecordDetails.price = SalesAreaData[i].price;//单价
                                        mySalesRecordDetails.ifDamaged = false;//是否损杯
                                        myModel.B_SalesRecordDetails.Add(mySalesRecordDetails);
                                        myModel.SaveChanges();
                                    }

                                    //修改会员余额
                                    S_Member myMember = new S_Member();
                                    int MemberID = member[0].memberID;
                                    myMember = (from tb in myModel.S_Member where tb.memberID == MemberID select tb).Single();
                                    myMember.memberBalance = myMember.memberBalance - SalesAmount;
                                    myModel.Entry(myMember).State = EntityState.Modified;

                                    if (myModel.SaveChanges() > 0)
                                    {
                                        Notice.Show("保存成功！", "提示", 2, MessageBoxIcon.Success);
                                        scope.Complete();//事务提交
                                        window.Close();//关闭窗口
                                        IfSave = true;
                                    }
                                    else
                                    {
                                        Notice.Show("保存失败", "提示", 2, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                Notice.Show("提交错误,请检查系统", "提示", 2, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else
                        {
                            Notice.Show("会员余额不足，请充值~", "提示", 2, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Notice.Show("会员信息错误", "提示", 2, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Notice.Show("请把会员信息填写完整！", "提示", 2, MessageBoxIcon.Error);
                }
            }
            else //普通消费
            {
                //判断支付金额是否大于0
                if (WeChatPay>0|| CashPayment>0)
                {
                    //判断支付金额与实收金额是否相等
                    if ((WeChatPay + CashPayment) == SalesAmount)
                    {
                        try
                        {
                            //引用事务
                            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                            {
                                //销售记录表B_SalesRecord
                                B_SalesRecord mySalesRecord = new B_SalesRecord();
                                mySalesRecord = SalesRecord;
                                mySalesRecord.ifTakeout = false;//是否外卖
                                mySalesRecord.cashPay = CashPayment;//现金支付
                                mySalesRecord.wechatPay = WeChatPay;//微信支付
                                mySalesRecord.ifMember = false;//是否会员消费
                                mySalesRecord.orderStatus = false;//订单状态 (false 制作中 true 制作完成)
                                mySalesRecord.ifRedeem = false;//是否积分兑换
                                mySalesRecord.salesAmount = CashPayment + WeChatPay;//订单合计金额
                                myModel.B_SalesRecord.Add(mySalesRecord);
                                myModel.SaveChanges();
                                //销售记录明细表B_SalesRecordDetails
                                string salesNumber = mySalesRecord.salesNumber;//根据销售单号查询上面新增的销售记录ID
                                var list = (from tb in myModel.B_SalesRecord where tb.salesNumber == salesNumber select tb).Single();
                                //循环传过来的 销售区数据 新增销售记录明细
                                for (int i = 0; i < SalesAreaData.Count(); i++)
                                {
                                    B_SalesRecordDetails mySalesRecordDetails = new B_SalesRecordDetails();
                                    mySalesRecordDetails.salesRecordID = list.salesRecordID;//获取上面新增的销售记录表ID
                                    mySalesRecordDetails.productID = SalesAreaData[i].productID;//产品ID
                                    mySalesRecordDetails.taste = SalesAreaData[i].taste;//口味
                                    mySalesRecordDetails.quantity = SalesAreaData[i].quantity;//数量
                                    mySalesRecordDetails.price = SalesAreaData[i].price;//单价
                                    mySalesRecordDetails.ifDamaged = false;//是否损杯
                                    myModel.B_SalesRecordDetails.Add(mySalesRecordDetails);
                                }
                                if (myModel.SaveChanges()>0)
                                {
                                    Notice.Show("保存成功！", "提示", 2, MessageBoxIcon.Success);
                                    scope.Complete();//事务提交
                                    window.Close();//关闭窗口
                                    IfSave = true;
                                }
                                else
                                {
                                    Notice.Show("保存失败", "提示", 2, MessageBoxIcon.Error);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            Notice.Show("系统错误，提交失败", "提示", 2, MessageBoxIcon.Error);
                            return;
                        }

                    }
                    else
                    {
                        Notice.Show("支付金额不足", "提示", 2, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Notice.Show("支付金额不能为0！", "提示", 2, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
