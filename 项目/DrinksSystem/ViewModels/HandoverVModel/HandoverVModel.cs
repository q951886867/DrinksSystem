using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using DrinksSystem.Views.HandoverView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.ViewModels.HandoverVModel
{
    public class HandoverVModel:ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public HandoverVModel()
        {
            LoadedCommand = new RelayCommand(QueryCommandFunc);//分页查询
            InsertClickCommand = new RelayCommand(InsertClick);//新增
            UpdateClickCommand = new RelayCommand(UpdateClick);//修改
            RefreshCommand = new RelayCommand(QueryCommandFunc);//刷新
            DeleteClickCommand = new RelayCommand(DeleteClick);//刷新
        }

        #region 属性
        //开始时间
        private string _startTime;
        public string StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime!=value)
                {
                    _startTime = value;
                    RaisePropertyChanged(() => StartTime);
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
        //选中行实体
        private HandoverVo _handoverSelectEntity;
        public HandoverVo HandoverSelectEntity
        {
            get { return _handoverSelectEntity; }
            set
            {
                if (_handoverSelectEntity != value)
                {
                    _handoverSelectEntity = value;
                    RaisePropertyChanged(() => HandoverSelectEntity);
                }
            }
        }
        #endregion

        #region 命令
        public RelayCommand InsertClickCommand { get; set; }//新增
        public RelayCommand UpdateClickCommand { get; set; }//修改
        public RelayCommand DeleteClickCommand { get; set; }//删除
        public RelayCommand RefreshCommand { get; set; }//刷新
        #endregion

        #region 函数
        //根据时间段查询交班记录表
        public List<HandoverVo> SelectHandoverData()
        {
            var list = from tb in myModel.B_Handover
                       join tbStaff in myModel.S_Staff on tb.staffID equals tbStaff.staffID
                       select new HandoverVo
                       {
                           handoverID = tb.handoverID,
                           staffID = tb.staffID,
                           staffName = tbStaff.staffName.Trim(),
                           startTime = tb.startTime,
                           endTime = tb.endTime,
                           cashIncome = tb.cashIncome,
                           wechatIncome = tb.wechatIncome,
                           amountHanded = tb.amountHanded,
                           reserveFund = tb.reserveFund,
                           businessAmount = tb.businessAmount,
                       };
            if (StartTime != null)
            {
                var StartTime1 = Convert.ToDateTime(StartTime);
                list = list.Where(m => m.startTime == StartTime1 || m.startTime > StartTime1);
            }
            if (EndTime != null)
            {
                var EndTime1 = Convert.ToDateTime(EndTime);
                list = list.Where(m => m.endTime == EndTime1 || m.endTime < EndTime1);
            }
            return list.ToList();
        }
        //新增
        public void InsertClick()
        {
            
            addOrUpdateHandover myWindow1 = new addOrUpdateHandover();
            var myDataContext1 = myWindow1.DataContext as addOrUpdateHandoverVModel;
            myDataContext1.HandoverData = new HandoverVo();
            myDataContext1.IsAdd = true;//新增修改 标识
            myDataContext1.WindowTitle = "新增交班记录";
            myDataContext1.HandoverRefresh += QueryCommandFunc;//刷新页面委托
            myWindow1.ShowDialog();
        }
        //修改
        public void UpdateClick()
        {
            if (HandoverSelectEntity!=null)
            {
                addOrUpdateHandover myWindow1 = new addOrUpdateHandover();
                var myDataContext1 = myWindow1.DataContext as addOrUpdateHandoverVModel;
                myDataContext1.HandoverData = HandoverSelectEntity;
                myDataContext1.IsAdd = false;//新增修改 标识
                myDataContext1.WindowTitle = "修改交班记录";
                myDataContext1.HandoverRefresh += QueryCommandFunc;//刷新页面委托
                myDataContext1.StartDate = HandoverSelectEntity.startTime.ToString();//开始日期
                myDataContext1.StartTime = HandoverSelectEntity.startTime.ToString();//开始时间
                myDataContext1.EndDate = HandoverSelectEntity.endTime.ToString();//结束日期
                myDataContext1.EndTime = HandoverSelectEntity.endTime.ToString();//结束时间
                myWindow1.ShowDialog();
            }
            else
            {
                Notice.Show("未选择数据", "提示", 2, MessageBoxIcon.Error);
            }
        }
        //删除
        public void DeleteClick()
        {
            if (HandoverSelectEntity != null)
            {
                B_Handover myHandover = myModel.B_Handover.Find(HandoverSelectEntity.handoverID);
                myModel.B_Handover.Remove(myHandover);
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
                Notice.Show("未选择数据", "提示", 2, MessageBoxIcon.Error);
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
        private int _pageSize = 6;//默认6条
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
        private ObservableCollection<HandoverVo> _HandoverPager;
        public ObservableCollection<HandoverVo> HandoverPager
        {
            get { return _HandoverPager; }
            set
            {
                _HandoverPager = value;
                RaisePropertyChanged(() => HandoverPager);
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
                HandoverPager = GetData(PageSize, out totalCount);
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
                HandoverPager = QueryData(pageIndex, PageSize);
            });
        }
        /// <summary>
        /// 初次获取数据
        /// </summary>
        /// <param name="pageSize">每页条数</param>
        /// <param name="totalCount">总数</param>
        /// <returns></returns>
        private ObservableCollection<HandoverVo> GetData(int pageSize, out int totalCount)
        {
            //查询
            var myHandover = SelectHandoverData();
            //分页
            List<HandoverVo> list = myHandover
                .OrderByDescending(a => a.handoverID)//根据记录ID倒序排序
                .Skip(0)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            totalCount = myHandover.Count;//总行数
            ObservableCollection<HandoverVo> HandoverList = new ObservableCollection<HandoverVo>(list);
            return HandoverList;
        }
        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        private ObservableCollection<HandoverVo> QueryData(int pageIndex, int pageSize)
        {
            //调用查询方法
            var myHandover = SelectHandoverData();
            //分页
            List<HandoverVo> list = myHandover
                .OrderByDescending(a => a.handoverID)//根据记录ID倒序排序
                .Skip((pageIndex - 1) * pageSize)//从索引（0）第一页开始
                .Take(pageSize)//查询本页数据的条数
                .ToList();//返回List集合
            ObservableCollection<HandoverVo> HandoverList = new ObservableCollection<HandoverVo>(list);
            return HandoverList;
        }
        #endregion

        #endregion
    }
}
