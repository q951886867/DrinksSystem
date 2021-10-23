using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using DrinksSystem.Views.StaffView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.ViewModels.StaffVModel
{
    public class StaffVModel:ViewModelBase
    {
        //数据库实例
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public StaffVModel()
        {
            LoadedCommand = new RelayCommand(QueryCommandFunc);//查询员工分页
            staffInsertCommand = new RelayCommand(staffInsertClick);//打开新增窗口
            staffUpdateCommand = new RelayCommand(staffUpdateClick);//打开修改窗口
            staffDeleteCommand = new RelayCommand(staffDeleteClickv);//删除
        }
        #region 属性
        //Datagrid选中行
        private StaffVO _staffDataSelect;
        public StaffVO StaffDataSelect
        {
            get { return _staffDataSelect; }
            set
            {
                if (_staffDataSelect != value)
                {
                    _staffDataSelect = value;
                    RaisePropertyChanged(() => StaffDataSelect);
                }
            }
        }
        #endregion

        #region 命令
        public RelayCommand staffInsertCommand { get; set; }//新增
        public RelayCommand staffUpdateCommand { get; set; }//修改
        public RelayCommand staffDeleteCommand { get; set; }//修改

        #endregion

        #region 函数
        //查询员工数据
        public List<StaffVO> selectStaffData()
        {
            try
            {
                var list = (from tb in myModel.S_Staff
                            join tbSex in myModel.S_Dictionary on tb.sexID equals tbSex.dictionaryID
                            join tbPosition in myModel.S_Dictionary on tb.positionID equals tbPosition.dictionaryID
                            select new StaffVO
                            {
                                staffID = tb.staffID,
                                staffNumber = tb.staffNumber.Trim(),
                                staffPassword = tb.staffPassword.Trim(),
                                staffName = tb.staffName.Trim(),
                                staffPhone = tb.staffPhone.Trim(),
                                sexID = tb.sexID,
                                sexName = tbSex.dictionaryName.Trim(),
                                positionID = tb.positionID,
                                positionName = tbPosition.dictionaryName.Trim(),
                                staffAddress = tb.staffAddress.Trim(),
                                ifWarrant = tb.ifWarrant,
                            }).ToList();
                return list;
            }
            catch (Exception)
            {
                Notice.Show("加载数据失败", "提示", 2, MessageBoxIcon.Error);
                return null;
            }

        }
        //新增
        public void staffInsertClick()
        {
            addOrUpdateStaffView myWindow = new addOrUpdateStaffView();
            var Datacontent = (myWindow.DataContext as addOrUpdateStaffVModel);
            Datacontent.StaffData = new StaffVO();
            Datacontent.IsAdd = true;
            Datacontent.WindowTitle = "新增员工信息";
            Datacontent.StaffRefresh += QueryCommandFunc;//委托传递刷新表格数据方法
            myWindow.ShowDialog();
        }
        //修改
        public void staffUpdateClick()
        {
            addOrUpdateStaffView myWindow = new addOrUpdateStaffView();
            var Datacontent = (myWindow.DataContext as addOrUpdateStaffVModel);
            Datacontent.StaffData = StaffDataSelect;
            Datacontent.IsAdd = false;
            Datacontent.WindowTitle = "修改员工信息";
            Datacontent.StaffRefresh += QueryCommandFunc;//委托传递刷新表格数据方法
            myWindow.ShowDialog();
        }
        //删除
        public void staffDeleteClickv()
        {
            if (StaffDataSelect!=null)
            {
                S_Staff myStaff = myModel.S_Staff.Find(StaffDataSelect.staffID);
                myModel.S_Staff.Remove(myStaff);
                if (myModel.SaveChanges() > 0)
                {
                    Notice.Show("删除成功", "提示", 2, MessageBoxIcon.Info);
                    QueryCommandFunc();//刷新页面
                }
                else
                {
                    Notice.Show("删除失败", "提示", 2, MessageBoxIcon.Error);
                }
            }
            else
            {
                Notice.Show("还未选择数据", "提示", 2, MessageBoxIcon.Info);

            }
        }
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
        private int _pageSize = 10;//默认10条
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
        private ObservableCollection<StaffVO> _staffData;
        public ObservableCollection<StaffVO> StaffData
        {
            get { return _staffData; }
            set
            {
                _staffData = value;
                RaisePropertyChanged(() => StaffData);
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
                StaffData = GetData(PageSize, out totalCount);
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
                StaffData = QueryData(pageIndex, PageSize);
            });
        }
        /// <summary>
        /// 初次获取数据
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        private ObservableCollection<StaffVO> GetData(int pageSize, out int totalCount)
        {
            //查询
            var myStaff = selectStaffData();
            //分页
            List<StaffVO> list = myStaff
                .OrderBy(a => a.staffID)//根据商品ID排序
                .Skip(0)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            totalCount = myStaff.Count;//总行数
            ObservableCollection<StaffVO> StaffList = new ObservableCollection<StaffVO>(list);
            return StaffList;
        }
        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        private ObservableCollection<StaffVO> QueryData(int pageIndex, int pageSize)
        {
            //调用查询方法
            var myStaff = selectStaffData();
            //分页
            List<StaffVO> list = myStaff
                .OrderBy(a => a.staffID)//根据商品ID排序
                .Skip((pageIndex - 1) * pageSize)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            ObservableCollection<StaffVO> StaffList = new ObservableCollection<StaffVO>(list);
            return StaffList;
        }
        #endregion


        #endregion
    }
}
