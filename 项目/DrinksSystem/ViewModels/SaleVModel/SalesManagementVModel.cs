using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.ViewModels.SaleVModel
{
    public class SalesManagementVModel:ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public SalesManagementVModel()
        {
            RefreshCommand = new RelayCommand(SelectSales);
            LoadedCommand = new RelayCommand(Load);
            DataGridSelectCommand = new RelayCommand(SelectSaleDetail);
        }

        #region 属性
        //查询条件 销售单号
        private string saleNumberText;
        public string SaleNumberText
        {
            get { return saleNumberText; }
            set
            {
                if (saleNumberText != value)
                {
                    saleNumberText = value;
                    RaisePropertyChanged(() => SaleNumberText);
                }
            }
        }
        //查询条件 销售员姓名
        private string salePersonNameText;
        public string SalePersonNameText
        {
            get { return salePersonNameText; }
            set
            {
                if (salePersonNameText != value)
                {
                    salePersonNameText = value;
                    RaisePropertyChanged(() => SalePersonNameText);
                }
            }
        }
        //查询条件 会员账号
        private string memberNumberText;
        public string MemberNumberText
        {
            get { return memberNumberText; }
            set
            {
                if (memberNumberText != value)
                {
                    memberNumberText = value;
                    RaisePropertyChanged(() => MemberNumberText);
                }
            }
        }
        //销售记录 DataSoure
        private List<SalesOrderVos> saleOrder;
        public List<SalesOrderVos> SaleOrder
        {
            get { return saleOrder; }
            set
            {
                if (saleOrder != value)
                {
                    saleOrder = value;
                    RaisePropertyChanged(() => SaleOrder);
                }
            }
        }
        //销售记录选中行 SelectData
        private SalesOrderVos saleOrderSelectEntity;
        public SalesOrderVos SaleOrderSelectEntity
        {
            get { return saleOrderSelectEntity; }
            set
            {
                if (saleOrderSelectEntity != value)
                {
                    saleOrderSelectEntity = value;
                    RaisePropertyChanged(() => SaleOrderSelectEntity);
                }
            }
        }

        //销售记录明细 DataSoure
        private List<SalesOrderDetailVos> saleOrderDetail;
        public List<SalesOrderDetailVos> SaleOrderDetail
        {
            get { return saleOrderDetail; }
            set
            {
                if (saleOrderDetail != value)
                {
                    saleOrderDetail = value;
                    RaisePropertyChanged(() => SaleOrderDetail);
                }
            }
        }
        #endregion

        #region 命令
        public RelayCommand LoadedCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }//刷新
        public RelayCommand DataGridSelectCommand { get; set; }//销售记录表格选中
        #endregion

        #region 函数
        private void Load()
        {
            SelectSales();
        }

        private void SelectSales()
        {
            var list = (from tb in myModel.B_SalesRecord
                        join tbStaff in myModel.S_Staff on tb.staffID equals tbStaff.staffID
                        orderby tb.salesRecordID descending
                        select new SalesOrderVos
                        {
                            salesRecordID = tb.salesRecordID,
                            salesNumber = tb.salesNumber,
                            salesTime = tb.salesTime,
                            staffID = tb.staffID,
                            staffName = tbStaff.staffName,
                            ifTakeout = tb.ifTakeout,
                            cashPay = tb.cashPay,
                            wechatPay = tb.wechatPay,
                            ifMember = tb.ifMember,
                            memberID = tb.memberID,
                            orderStatus = tb.orderStatus,
                            ifRedeem = tb.ifRedeem,
                            salesAmount = tb.salesAmount,
                        }).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].memberID != null)
                {
                    var memberid = list[i].memberID;
                    var list2 = (from tb in myModel.S_Member where tb.memberID == memberid select tb).Single();
                    list[i].memberNumber = list2.memberNumber;
                }
                else
                {
                    list[i].memberNumber = "";
                }
            }
            //筛选销售单号
            if (SaleNumberText!=""&& SaleNumberText!=null)
            {
                list = list.Where(m => m.salesNumber.Contains(SaleNumberText)).ToList();
            }
            //筛选销售员姓名
            if (SalePersonNameText != "" && SalePersonNameText != null)
            {
                list = list.Where(m => m.staffName.Contains(SalePersonNameText)).ToList();
            }
            //筛选会员账号
            if (MemberNumberText != "" && MemberNumberText != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].memberNumber==""|| list[i].memberNumber == null)
                    {
                        list.RemoveAt(i);
                    }
                }
                list = list.Where(m => m.memberNumber.Contains(MemberNumberText)).ToList();
            }
            SaleOrder = list;
        }

        private void SelectSaleDetail()
        {
            if (SaleOrderSelectEntity!=null)
            {
                var list = (from tb in myModel.B_SalesRecordDetails 
                            join tbProduct in myModel.S_Product on tb.productID equals tbProduct.productID
                            where tb.salesRecordID == SaleOrderSelectEntity.salesRecordID 
                            select new SalesOrderDetailVos
                            {
                                salesDetailsID=tb.salesDetailsID,
                                salesRecordID = tb.salesRecordID,
                                productID = tb.productID,
                                productName = tbProduct.productName,
                                taste = tb.taste,
                                quantity = tb.quantity,
                                price = tb.price,
                                ifDamaged = tb.ifDamaged,
                            }).ToList();
                SaleOrderDetail = list;
            }
        }
        #endregion
    }
}
