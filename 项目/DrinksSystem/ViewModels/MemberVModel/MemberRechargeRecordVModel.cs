using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DrinksSystem.ViewModels.MemberVModel
{
    public class MemberRechargeRecordVModel:ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public MemberRechargeRecordVModel()
        {
            CloseCommand = new RelayCommand<Window>(Close);
            KeyUpCommand = new RelayCommand<TextBox>(KeyUpSelect);
        }

        #region 属性
        //表格源数据
        private ObservableCollection<MemberRechargeRecordVo> memberRechargeRecord;
        public ObservableCollection<MemberRechargeRecordVo> MemberRechargeRecord
        {
            get { return memberRechargeRecord; }
            set
            {

                if (memberRechargeRecord!=value)
                {
                    memberRechargeRecord = value;
                    RaisePropertyChanged(() => MemberRechargeRecord);
                }
            }
        }
        
        //搜索条件
        public string SelectTxt { set; get; }


        #endregion

        #region 命令
        public RelayCommand<TextBox> KeyUpCommand { get; set; }//KeyUp
        public RelayCommand<Window> CloseCommand { get; set; }//关闭窗口
        #endregion

        #region 函数
        //条件查询会员充值记录明细
        public void SelectMemberRechargeRecord(string condition)
        {
            var list = (from tbMemberRechargeRecord in myModel.B_MemberRechargeRecord
                        join tbMember in myModel.S_Member on tbMemberRechargeRecord.memberID equals tbMember.memberID
                        join tbStaff in myModel.S_Staff on tbMemberRechargeRecord.staffID equals tbStaff.staffID
                        orderby tbMemberRechargeRecord.memberRechargeID descending
                        select new MemberRechargeRecordVo
                        {
                            memberRechargeID= tbMemberRechargeRecord.memberRechargeID,
                            memberID = tbMemberRechargeRecord.memberID,
                            memberNumber = tbMember.memberNumber,
                            memberName = tbMember.memberName,
                            staffID = tbStaff.staffID,
                            staffName = tbStaff.staffName,
                            rechargeAmount = tbMemberRechargeRecord.rechargeAmount,
                            promotionalAmount = tbMemberRechargeRecord.promotionalAmount,
                            promotionalIntegral = tbMemberRechargeRecord.promotionalIntegral,
                            totalAmount = tbMemberRechargeRecord.totalAmount,
                            remark = tbMemberRechargeRecord.remark,
                        }).ToList();
            if (condition != "")
            {
                list = list.Where(m => m.memberName.Contains(condition) || m.memberNumber.Contains(condition)).ToList();
            }
            MemberRechargeRecord = new ObservableCollection<MemberRechargeRecordVo>(list);
        }
        //KeyUp
        private void KeyUpSelect(TextBox tb)
        {
            string tbText = tb.Text;
            SelectMemberRechargeRecord(tbText);
        }
        //关闭窗口
        private void Close(Window window)
        {
            window.Close();
        }
        #endregion
    }
}
