using DrinksSystem.Models;
using DrinksSystem.Models.Vos;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrinksSystem.ViewModels.StaffVModel
{
    public class addOrUpdateStaffVModel:ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public addOrUpdateStaffVModel()
        {
            LoadedCommand = new RelayCommand(Load);//加载 查询下拉框
            CloseCommand = new RelayCommand<Window>(CloseWindow);//关闭窗口
            SaveCommand = new RelayCommand<Window>(Save);//关闭窗口
        }
        #region 属性
        //数据源
        private StaffVO _staffData;
        public StaffVO StaffData
        {
            get { return _staffData; }
            set
            {
                if (_staffData!=value)
                {
                    _staffData = value;
                    RaisePropertyChanged(() => StaffData);
                }
            }
        }
        //性别下拉框数据源
        private List<S_Dictionary> _positionComboBox;
        public List<S_Dictionary> PositionComboBox
        {
            get { return _positionComboBox; }
            set
            {
                if (_positionComboBox != value)
                {
                    _positionComboBox = value;
                    RaisePropertyChanged(() => PositionComboBox);
                }
            }
        }
        //职位下拉框数据源
        private List<S_Dictionary> _sexComboBox;
        public List<S_Dictionary> SexComboBox
        {
            get { return _sexComboBox; }
            set
            {
                if (_sexComboBox != value)
                {
                    _sexComboBox = value;
                    RaisePropertyChanged(() => SexComboBox);
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
        public delegate void CompletedEvent ();
        public event CompletedEvent StaffRefresh;
        #endregion

        #region 命令
        public RelayCommand LoadedCommand { get; set; }// 加载
        public RelayCommand<Window> CloseCommand { get; set; }//关闭窗口
        public RelayCommand<Window> SaveCommand { get; set; }//保存

        #endregion

        #region 函数
        //加载
        private void Load()
        {
            //查询下拉框数据
            var list1 = from tb in myModel.S_Dictionary where tb.dictionaryType == "性别" select tb;
            SexComboBox = list1.ToList();

            var list = from tb in myModel.S_Dictionary where tb.dictionaryType == "职位" select tb;
            PositionComboBox = list.ToList();
        }
        //关闭窗口
        private void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        //保存
        private void Save(Window window)
        {
            //判断页面数据是否填写完整
            if (StaffData.staffName!=""&& StaffData.positionID!=null&& StaffData.sexID!=null
                && StaffData.staffAddress!=""&& StaffData.staffNumber!=""&& StaffData.staffPassword!=""
                && StaffData.staffPhone!="")
            {
                //获取页面数据
                S_Staff myStaff = new S_Staff()
                {
                    staffID= StaffData.staffID,
                    staffNumber = StaffData.staffNumber.Trim(),
                    staffPassword = StaffData.staffPassword.Trim(),
                    staffName = StaffData.staffName.Trim(),
                    staffPhone = StaffData.staffPhone.Trim(),
                    sexID = StaffData.sexID,
                    positionID = StaffData.positionID,
                    staffAddress = StaffData.staffAddress.Trim(),
                    ifWarrant = StaffData.ifWarrant,
                };
                //查询重复
                var ifStaff = myModel.S_Staff.Where(m =>  m.staffID != StaffData.staffID &&( m.staffName.Trim() == myStaff.staffName || m.staffNumber.Trim() == myStaff.staffNumber) ).ToList().Count;
                if (ifStaff>0)
                {
                    Notice.Show("名称或账号重复!", "提示", 2, MessageBoxIcon.Error);
                }
                else
                {
                    if (IsAdd)
                    {
                        myModel.S_Staff.Add(myStaff);
                        if (myModel.SaveChanges() > 0)
                        {
                            Notice.Show("新增成功", "提示", 2, MessageBoxIcon.Success);
                            StaffRefresh();//委托刷新
                            window.Close();//关闭窗口
                        }
                        else
                        {
                            Notice.Show("新增失败", "提示", 2, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        myStaff.staffID = StaffData.staffID;//获取id
                        myModel.Entry(myStaff).State = System.Data.Entity.EntityState.Modified;
                        if (myModel.SaveChanges() > 0)
                        {
                            Notice.Show("修改成功", "提示", 2, MessageBoxIcon.Success);
                            StaffRefresh();//委托刷新
                            window.Close();//关闭窗口
                        }
                        else
                        {
                            Notice.Show("修改失败", "提示", 2, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                Notice.Show("请把页面数据填写完整", "提示", 2, MessageBoxIcon.Info);
            }
        }
        #endregion
    }
}
