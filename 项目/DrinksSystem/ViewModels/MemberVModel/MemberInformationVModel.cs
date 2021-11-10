using DrinksSystem.Models;
using DrinksSystem.Views.MemberView;
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

namespace DrinksSystem.ViewModels.MemberVModel
{
    public class MemberInformationVModel:ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public MemberInformationVModel()
        {
            LoadedCommand = new RelayCommand(Load);//加载
            CloseCommand = new RelayCommand<Window>(Close);//关闭
            DragMoveCommand = new RelayCommand<Window>(DragMovewindow);//窗口移动
            SaveCommand = new RelayCommand<Window>(Save);//保存

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
        //会员卡号 双向绑定
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
        //开户金额
        private decimal? _openingAmount;
        public decimal? OpeningAmount
        {
            get { return _openingAmount; }
            set
            {
                if (_openingAmount != value)
                {
                    _openingAmount = value;
                    RaisePropertyChanged(() => OpeningAmount);
                }
            }
        }
        //窗口标题
        private string _windowTitleText;
        public string WindowTitleText
        {
            get { return _windowTitleText; }
            set
            {
                if (_windowTitleText != value)
                {
                    _windowTitleText = value;
                    RaisePropertyChanged(() => WindowTitleText);
                }
            }
        }
        public bool IsAdd;//办理会员 或是 修改会员信息 标识
        public int StaffIDNowID;//当前用户ID
        public decimal? MembershipAmount;//会员金额 （用于记录会员信息修改时  修改前的会员金额）
        #endregion

        #region 命令
        public RelayCommand<Window> SaveCommand { get; set; }//提交按钮
        public RelayCommand<Window> CloseCommand { get; set; }//关闭窗口
        public RelayCommand<Window> DragMoveCommand { get; set; }//窗口移动
        public RelayCommand LoadedCommand { get; set; }//页面加载
        #endregion

        #region 函数
        public void Load()
        {
            if (IsAdd)
            {
                WindowTitleText = "会员办理";
            }
            else
            {
                WindowTitleText = "修改会员信息";
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
        //保存
        private void Save(Window window)
        {
            
            if (IsAdd)
            {
                if (MemberData.memberName!="" && MemberData.memberBalance!=null && MemberData.memberPoints!=null)
                {
                    try
                    {
                        //引用事务
                        using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                        {
                            //新增会员表
                            S_Member myMember = new S_Member();
                            myMember = MemberData;
                            decimal rechargeamount = Convert.ToDecimal(MemberData.memberBalance);//充值金额
                            decimal promotionalamount = 0;//赠送金额
                            
                            //如果开户时充值金额 则计算赠送金额
                            if (MemberData.memberBalance > 0)
                            {
                                //计算赠送金额  每充一百元 赠送二十
                                string promotional = (myMember.memberBalance / 100).ToString();//充值金额 除以 100
                                //判断是否为整数
                                if ((Convert.ToDecimal(promotional) %1)==0)
                                {
                                    promotionalamount = int.Parse(promotional) * 20;
                                }
                                else
                                {
                                    promotionalamount = int.Parse(promotional.Substring(0, promotional.IndexOf('.'))) * 20;//去除小数点 转换为整数字符串
                                }
                                myMember.memberBalance = myMember.memberBalance + promotionalamount;//实际到账金额 充值金额+赠送金额
                            }
                            myModel.S_Member.Add(myMember);

                            //判断是否开户时 充值金额
                            if (MemberData.memberBalance > 0)
                            {
                                myModel.SaveChanges();
                                //获取上面新增的会员ID
                                var list = (from tb in myModel.S_Member where tb.memberNumber == MemberData.memberNumber select tb).Single();

                                //新增 会员充值记录表B_MemberRechargeRecord
                                B_MemberRechargeRecord myMemberRechargeRecord = new B_MemberRechargeRecord();
                                myMemberRechargeRecord.promotionalIntegral = 0;//赠送积分
                                myMemberRechargeRecord.staffID = StaffIDNowID;//员工ID
                                myMemberRechargeRecord.memberID = list.memberID;//会员ID
                                myMemberRechargeRecord.rechargeAmount = rechargeamount;//充值金额
                                myMemberRechargeRecord.promotionalAmount = promotionalamount;//赠送金额
                                myMemberRechargeRecord.rechargeTime = DateTime.Now;//充值时间
                                myMemberRechargeRecord.remark = "开户充值";
                                myMemberRechargeRecord.totalAmount = myMemberRechargeRecord.rechargeAmount + myMemberRechargeRecord.promotionalAmount;//合计金额
                                myModel.B_MemberRechargeRecord.Add(myMemberRechargeRecord);
                                if (myModel.SaveChanges() > 0)
                                {
                                    Notice.Show("保存成功！", "提示", 2, MessageBoxIcon.Success);
                                    scope.Complete();//事务提交
                                    window.Close();//关闭窗口
                                }
                            }
                            else
                            {
                                if (myModel.SaveChanges() > 0)
                                {
                                    Notice.Show("保存成功！", "提示", 2, MessageBoxIcon.Success);
                                    scope.Complete();//事务提交
                                    window.Close();//关闭窗口
                                }
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
                    Notice.Show("页面数据不能为空！", "提示", 2, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (MemberData.memberName != "" && MemberData.memberBalance != null && MemberData.memberPoints != null)
                {
                    try
                    {
                        //引用事务
                        using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                        {
                            //修改会员信息
                            S_Member myMember = new S_Member();
                            myMember = MemberData;
                            myModel.Entry(myMember).State = EntityState.Modified;
                            //判断是否修改了金额 如果修改了则需要新增会员充值记录表 如果没有则直接保存
                            if (myMember.memberBalance == MembershipAmount)
                            {
                                if (myModel.SaveChanges() > 0)
                                {
                                    Notice.Show("修改成功", "提示", 2, MessageBoxIcon.Success);
                                    scope.Complete();//事务提交
                                    window.Close();//关闭窗口
                                }
                                else
                                {
                                    Notice.Show("修改失败", "提示", 2, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                myModel.SaveChanges();//保存会员修改
                                //新增会员充值记录表
                                B_MemberRechargeRecord myMemberRechargeRecord = new B_MemberRechargeRecord();
                                myMemberRechargeRecord.memberID = myMember.memberID;//会员ID
                                myMemberRechargeRecord.staffID = StaffIDNowID;//当前用户ID
                                myMemberRechargeRecord.rechargeAmount = myMember.memberBalance - MembershipAmount;//充值金额 正数和负数
                                myMemberRechargeRecord.promotionalAmount = 0;//赠送金额
                                myMemberRechargeRecord.rechargeTime = DateTime.Now;//充值时间
                                myMemberRechargeRecord.promotionalIntegral = 0;//赠送积分
                                myMemberRechargeRecord.totalAmount = myMemberRechargeRecord.rechargeAmount;//合计金额
                                myMemberRechargeRecord.remark = "调整金额";//备注
                                myModel.B_MemberRechargeRecord.Add(myMemberRechargeRecord);
                                if (myModel.SaveChanges() > 0)
                                {
                                    Notice.Show("修改成功", "提示", 2, MessageBoxIcon.Success);
                                    scope.Complete();//事务提交
                                    window.Close();//关闭窗口
                                }
                                else
                                {
                                    Notice.Show("修改失败", "提示", 2, MessageBoxIcon.Error);
                                }
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
                    Notice.Show("页面数据不能为空！", "提示", 2, MessageBoxIcon.Error);
                }
            }
        }
        #endregion
    }
}
