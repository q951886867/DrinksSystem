using DrinksSystem.Models;
using DrinksSystem.Views.CheckoutCounterView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DrinksSystem.ViewModels
{
    public class LoginWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public LoginWindowViewModel()
        {
            //移动窗口
            DragMoveCommand = new RelayCommand<Window>(DragMovewindow);
            //关闭窗口
            CloseClick_Command = new RelayCommand<Window>(Clossewindow);
            //最小化窗口
            MinClick_Command = new RelayCommand<Window>(Minwindow);
            //登录
            Login_Command = new RelayCommand<PasswordBox>(Login);
            LoadedCommand = new RelayCommand<Window>(load);
        }
        #region 【属性】
        //账号
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }
        //密码
        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged(() => Password);
                }
            }
        }
        //窗口
        public Window wd;
        #endregion
        #region 【命令】
        //移动窗口
        public RelayCommand<Window> DragMoveCommand { get; set; }
        //关闭窗口
        public RelayCommand<Window> CloseClick_Command { get; set; }
        //最小化窗口
        public RelayCommand<Window> MinClick_Command { get; set; }
        //登录
        public RelayCommand<PasswordBox> Login_Command { get; set; }
        //加载
        public RelayCommand<Window> LoadedCommand { get; private set; }
        #endregion
        #region 【函数】
        /// <summary>
        /// 移动窗口
        /// </summary>
        private void DragMovewindow(Window window)
        {
            window.DragMove();
        }
        /// <summary>
        /// 加载事件
        /// </summary>
        private void load(Window window)
        {
            wd = window;
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        private void Clossewindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
                System.Environment.Exit(0);
            }
        }
        /// <summary>
        /// 最小化窗口
        /// </summary>
        private void Minwindow(Window window)
        {
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        private void Login(PasswordBox pass)
        {
            //获取PasswordBox控件 获取密码
            var Password1 = pass.Password.ToString();
            if (Password1!=""&& UserName!="")
            {
                //根据账号密码查询数据库
                var list = (from tb in myModel.S_Staff
                            where tb.staffNumber == UserName && tb.staffPassword == Password1
                            select tb).ToList();
                if (list.Count == 1)
                {
                    if (wd != null)
                    {
                        CheckoutCounterView myCheckoutCounterView = new CheckoutCounterView();
                        var myViewModel = (myCheckoutCounterView.DataContext as DrinksSystem.ViewModels.CheckoutCounterVModel.CheckoutCounterVModel);
                        myViewModel.StaffNow = list[0];//传递员工数据
                        myCheckoutCounterView.Show();
                        wd.Close();
                        
                    }
                }
                else
                {
                    Notice.Show("账号密码或名称错误", "提示", 2, MessageBoxIcon.Error);
                }
            }
            else
            {
                Notice.Show("数据不能为空", "提示", 2, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
