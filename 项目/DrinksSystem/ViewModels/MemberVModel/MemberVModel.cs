using DrinksSystem.Models;
using DrinksSystem.Views.MemberView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrinksSystem.ViewModels.MemberVModel
{
    public class MemberVModel:ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public MemberVModel()
        {
            LoadedCommand = new RelayCommand(QueryCommandFunc);//页面加载分页查询
            InsertClickCommand = new RelayCommand(ApplyMembership);//会员办理
            UpdateClickCommand = new RelayCommand(UpdateMemberDetail);//修改会员信息
            DeleteClickCommand = new RelayCommand(DeleteClick);//修改会员信息
            RechargeClickCommand = new RelayCommand(MemberRecharge);//会员充值
            RechargeDetailClickCommand = new RelayCommand(MemberRechargeDetail);//会员充值记录明细
        }

        #region 属性
        //会员表选中行实例
        private S_Member _memberSelectEntity;
        public S_Member MemberSelectEntity
        {
            get { return _memberSelectEntity; }
            set
            {
                if (_memberSelectEntity != value)
                {
                    _memberSelectEntity = value;
                    RaisePropertyChanged(() => MemberSelectEntity);
                }
            }
        }
        //查询条件
        public string _selectTxt="";
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
        public int StaffIDNow;//当前用户
        #endregion

        #region 命令
        public RelayCommand InsertClickCommand { get; set; }//会员办理
        public RelayCommand UpdateClickCommand { get; set; }//修改会员信息
        public RelayCommand DeleteClickCommand { get; set; }//删除会员
        public RelayCommand RechargeClickCommand { get; set; }//会员充值
        public RelayCommand RechargeDetailClickCommand { get; set; }//会员充值记录明细
        #endregion

        #region 函数
        //会员充值记录明细
        private void MemberRechargeDetail()
        {
            MemberRechargeRecord myWindow = new MemberRechargeRecord();
            var myVModel = myWindow.DataContext as MemberRechargeRecordVModel;
            if (MemberSelectEntity!=null)
            {
                myVModel.SelectMemberRechargeRecord(MemberSelectEntity.memberNumber);
                myVModel.SelectTxt = MemberSelectEntity.memberNumber;
            }
            else
            {
                myVModel.SelectMemberRechargeRecord("");
            }
            myWindow.ShowDialog();
        }
        //会员充值
        public void MemberRecharge()
        {
            if (MemberSelectEntity!=null)
            {
                MemberRechargeView myWindow = new MemberRechargeView();
                var myVModel = myWindow.DataContext as MemberRechargeVModel;
                myVModel.MemberData = MemberSelectEntity;
                myVModel.StaffIDNowID = StaffIDNow;//当前用户ID
                myWindow.ShowDialog();
                myModel = new DrinksSystemEntities();//重新实例化数据库
                QueryCommandFunc();//刷新页面
            }
            else
            {
                Notice.Show("请选择要充值的会员", "提示", 2, MessageBoxIcon.Error);
            }
        }
        //删除会员
        private void DeleteClick()
        {
            if (MemberSelectEntity!=null)
            {
                var res = MessageBoxX.Show("确定要删除该数据吗？", "提示", Application.Current.MainWindow, MessageBoxButton.YesNo);
                if (res.ToString()== "Yes")
                {
                    try
                    {
                        //引用事务
                        using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                        {
                            S_Member myMember = MemberSelectEntity as S_Member;
                            myModel.S_Member.Remove(myMember);
                            myModel.SaveChanges();

                            //循环删除该会员的所有充值记录
                            var listCount = (from tb in myModel.B_MemberRechargeRecord
                                             where tb.memberID == myMember.memberID
                                             select tb).ToList();
                            int a = 0;
                            for (int i = 0; i < listCount.Count; i++)
                            {
                                B_MemberRechargeRecord myMemberRechargeRecord = new B_MemberRechargeRecord();
                                myMemberRechargeRecord = listCount[i];
                                myModel.B_MemberRechargeRecord.Remove(myMemberRechargeRecord);
                                myModel.SaveChanges();
                                a++;
                            }
                            if (a== listCount.Count)
                            {
                                Notice.Show("保存成功！", "提示", 2, MessageBoxIcon.Success);
                                scope.Complete();//事务提交
                                QueryCommandFunc();//刷新页面
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Notice.Show("删除失败！", "提示", 2, MessageBoxIcon.Error);
                        Debug.WriteLine(e);
                    }

                }
            }
        }
        //条件查询会员列表
        private List<S_Member> SelectMember(string SelectTxt)
        {
            try
            {
                var list = (from tb in myModel.S_Member 
                            select tb).ToList();
                //筛选数据
                if (!string.IsNullOrEmpty(SelectTxt))
                {
                    list = list.Where(m => m.memberName.Contains(SelectTxt) || m.memberNumber.Contains(SelectTxt)).ToList();
                }
                return list;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return null;
            }

        }
        //会员办理
        public void ApplyMembership()
        {
            MemberInformation myWindow = new MemberInformation();
            var myVModel=(myWindow.DataContext as MemberInformationVModel);
            myVModel.IsAdd = true;//会员办理 标识
            myVModel.MemberData = new S_Member(); //实例化
            myVModel.StaffIDNowID= StaffIDNow;//传递当前用户ID
            myVModel.MemberData.memberPoints = 0;

            //查询本数据库是否第一次办理会员
            var list = (from tb in myModel.S_Member select tb).ToList();
            if (list.Count() > 0)
            {
                myVModel.MemberData.memberNumber = GenerateMemberNumber();//生成会员卡号
            }
            else
            {
                myVModel.MemberData.memberNumber = "M100001";
            }

            myWindow.ShowDialog();
            QueryCommandFunc();
        }
        //递归 生成不与数据库重复的会员卡号
        private string GenerateMemberNumber()
        {
            //查询数据库中 最后一条数据的会员卡号
            var list = (from tb in myModel.S_Member orderby tb.memberNumber descending select tb).ToList();
            string number = "M" + (Convert.ToDecimal(list[0].memberNumber.ToString().Remove(0, 1)) + 1);//加1 
            var listcount = (from tb in myModel.S_Member where tb.memberNumber == number select tb).ToList().Count();//查询上面生成的编号 是否与数据库重复
            if (listcount > 0)
            {
                GenerateMemberNumber();//如果重复 则进行递归 继续生成 直至不重复
            }
            return number;
        }
        //修改会员信息
        private void UpdateMemberDetail()
        {
            MemberInformation myWindow = new MemberInformation();
            var myVModel = (myWindow.DataContext as MemberInformationVModel);
            myVModel.IsAdd = false;//会员办理 标识
            myVModel.MemberData = MemberSelectEntity; //传递当前选中行的会员信息
            myVModel.StaffIDNowID = StaffIDNow;//传递当前用户ID
            myVModel.MembershipAmount = MemberSelectEntity.memberBalance;//会员金额
            myWindow.ShowDialog();
            myModel = new DrinksSystemEntities();//重新实例化数据库
            QueryCommandFunc();//刷新页面
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
        private int _pageSize = 10;//默认6条
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
        //会员列表 数据源
        //分页数据源 实时更新 
        private ObservableCollection<S_Member> _member;
        public ObservableCollection<S_Member> Member
        {
            get { return _member; }
            set
            {
                _member = value;
                RaisePropertyChanged(() => Member);
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
                Member = GetData(PageSize, out totalCount);
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
                Member = QueryData(pageIndex, PageSize);
            });
        }
        /// <summary>
        /// 初次获取数据
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        private ObservableCollection<S_Member> GetData(int pageSize, out int totalCount)
        {
            //查询
            var myMember = SelectMember(SelectTxt);
            //分页
            List<S_Member> list = myMember
                .OrderBy(a => a.memberID)//根据商品ID排序
                .Skip(0)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            totalCount = myMember.Count;//总行数
            ObservableCollection<S_Member> MemberList = new ObservableCollection<S_Member>(list);
            return MemberList;
        }
        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        private ObservableCollection<S_Member> QueryData(int pageIndex, int pageSize)
        {
            //调用查询方法
            var myMember = SelectMember(SelectTxt);
            //分页
            List<S_Member> list = myMember
                .OrderBy(a => a.memberID)//根据商品ID排序
                .Skip((pageIndex - 1) * pageSize)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            ObservableCollection<S_Member> MemberList = new ObservableCollection<S_Member>(list);
            return MemberList;
        }
        #endregion
    }
}
