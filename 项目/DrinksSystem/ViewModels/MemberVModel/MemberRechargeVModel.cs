using DrinksSystem.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DrinksSystem.ViewModels.MemberVModel
{
    public class MemberRechargeVModel : ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public MemberRechargeVModel()
        {
            CloseCommand = new RelayCommand<Window>(Close);//关闭
            LoadedCommand = new RelayCommand(Load);//加载
            DragMoveCommand = new RelayCommand<Window>(DragMovewindow);//窗口移动
            SaveCommand = new RelayCommand<Window>(Save);//保存
            LostFocusCommand = new RelayCommand(LostFocus);//会员卡号 会员姓名 鼠标焦点失去事件
            KeyUpCommand = new RelayCommand<TextBox>(KeyUpTextBox);//KeyUp
        }

        #region 属性
        //会员表
        private S_Member _memberData;
        public S_Member MemberData
        {
            get { return _memberData; }
            set
            {
                if (_memberData != value)
                {
                    _memberData = value;
                    RaisePropertyChanged(() => MemberData);
                }
            }
        }
        //赠送金额
        private decimal _promotionalAmount;
        public decimal PromotionalAmount
        {
            get { return _promotionalAmount; }
            set
            {
                if (_promotionalAmount != value)
                {
                    _promotionalAmount = value;
                    RaisePropertyChanged(() => PromotionalAmount);
                }
            }
        }
        //充值金额
        private decimal _rechargeAmount;
        public decimal RechargeAmount
        {
            get { return _rechargeAmount; }
            set
            {
                if (_rechargeAmount != value)
                {
                    _rechargeAmount = value;
                    RaisePropertyChanged(() => RechargeAmount);

                }
            }
        }

        //会员信息是否正确标识
        private bool _IsCorrect;
        public bool IsCorrect
        {
            get { return _IsCorrect; }
            set
            {
                if (_IsCorrect != value)
                {
                    _IsCorrect = value;
                    RaisePropertyChanged(() => IsCorrect);
                }
            }
        }
        public int StaffIDNowID;//当前用户ID
        #endregion

        #region 命令
        public RelayCommand<Window> SaveCommand { get; set; }//提交按钮
        public RelayCommand<Window> CloseCommand { get; set; }//关闭窗口
        public RelayCommand<Window> DragMoveCommand { get; set; }//窗口移动
        public RelayCommand LoadedCommand { get; set; }//页面加载
        public RelayCommand LostFocusCommand { get; set; }//会员卡号 会员姓名 鼠标焦点失去事件
        public RelayCommand<TextBox> KeyUpCommand { get; set; }//KeyUp
        #endregion

        #region 函数
        //页面加载
        private void Load()
        {
            if (MemberData.memberName!=""&& MemberData.memberNumber!="")
            {
                LostFocus();
            }
        }
        //计算赠送金额
        private void KeyUpTextBox(TextBox tb)
        {
            if (tb.Text !="")
            {
                string amount = (Convert.ToDecimal(tb.Text) / 100).ToString();
                if ((Convert.ToDecimal(amount) % 1) == 0)
                {
                    PromotionalAmount = int.Parse(amount) * 20;
                }
                else
                {
                    PromotionalAmount = int.Parse(amount.Substring(0, amount.IndexOf('.'))) * 20;//去除小数点 转换为整数字符串
                }
            }
        }
        //检验会员账号是否正确
        private void LostFocus()
        {
            //判断会员信息是否正确
            var list = (from tb in myModel.S_Member
                        where tb.memberName == MemberData.memberName && tb.memberNumber == MemberData.memberNumber
                        select tb).ToList();
            if (list.Count == 1)
            {
                IsCorrect = true;
            }
            else
            {
                IsCorrect = false;
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
        //提交
        private void Save(Window window)
        {
            //判断页面数据是否为空
            if (RechargeAmount!=0&& MemberData.memberName!=""&& MemberData.memberNumber!="")
            {
                //判断会员信息是否正确
                var list = (from tb in myModel.S_Member
                            where tb.memberName == MemberData.memberName && tb.memberNumber == MemberData.memberNumber
                            select tb).ToList();
                if (list.Count == 1)
                {
                    try
                    {
                        //引用事务
                        using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                        {
                            //修改会员表
                            S_Member myMember = list[0] as S_Member;
                            myMember.memberBalance = myMember.memberBalance + RechargeAmount + PromotionalAmount;//会员金额 + 充值金额 + 赠送金额
                            myModel.Entry(myMember).State = EntityState.Modified;
                            myModel.SaveChanges();

                            //新增会员充值记录表
                            B_MemberRechargeRecord myMemberRechargeRecord = new B_MemberRechargeRecord();
                            myMemberRechargeRecord.memberID = myMember.memberID;//会员ID
                            myMemberRechargeRecord.staffID = StaffIDNowID;//员工ID
                            myMemberRechargeRecord.rechargeAmount = RechargeAmount;//充值金额
                            myMemberRechargeRecord.promotionalAmount = PromotionalAmount;//赠送金额
                            myMemberRechargeRecord.promotionalIntegral = 0;//赠送积分
                            myMemberRechargeRecord.totalAmount = RechargeAmount + PromotionalAmount;//合计金额
                            myMemberRechargeRecord.rechargeTime = DateTime.Now;//充值时间
                            myMemberRechargeRecord.remark = "会员充值";//备注
                            myModel.B_MemberRechargeRecord.Add(myMemberRechargeRecord);
                            if (myModel.SaveChanges() > 0)
                            {
                                Notice.Show("保存成功！", "提示", 2, MessageBoxIcon.Success);
                                scope.Complete();//事务提交
                                window.Close();//关闭窗口
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                    
                }
                else
                {
                    Notice.Show("会员信息错误！", "提示", 2, MessageBoxIcon.Error);
                    IsCorrect = false;//会员信息错误
                }
            }
            else
            {
                Notice.Show("页面数据不能为空！", "提示", 2, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
