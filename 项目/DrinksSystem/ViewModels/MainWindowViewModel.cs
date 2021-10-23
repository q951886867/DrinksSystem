using DrinksSystem.Models;
using DrinksSystem.Resources.control;
using DrinksSystem.Views.CheckoutCounterView;
using DrinksSystem.Views.DictionaryView;
using DrinksSystem.Views.HandoverView;
using DrinksSystem.Views.Product;
using DrinksSystem.Views.StaffView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DrinksSystem.ViewModels
{
    public class MainWindowViewModel:ViewModelBase
    {
        /// <summary>
        /// 实例化实体数据模型
        /// </summary>
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public MainWindowViewModel()
        {
            //产品管理
            ProducPageCommand = new RelayCommand<TabControl>(ProducPage);
            //字典管理
            DictionaryCommand = new RelayCommand<TabControl>(DictionaryPage);
            //员工管理
            StaffPageCommand = new RelayCommand<TabControl>(StaffPage);
            //交接班管理
            HandoverPageCommand = new RelayCommand<TabControl>(HandoverPage);
            //进入收银台
            CheckoutCounterCommand = new RelayCommand<Window>(CheckoutCounter);
        }
        #region 属性
        /// <summary>
        /// 选项卡
        /// </summary>
        public static TabControl TC;
        #endregion

        #region 命令
        /// <summary>
        /// 产品管理
        /// </summary>
        public RelayCommand<TabControl> ProducPageCommand { get; set; }
        /// <summary>
        /// 字典管理
        /// </summary>
        public RelayCommand<TabControl> DictionaryCommand { get; set; }
        /// <summary>
        /// 员工管理
        /// </summary>
        public RelayCommand<TabControl> StaffPageCommand { get; set; }
        /// <summary>
        /// 员工管理
        /// </summary>
        public RelayCommand<TabControl> HandoverPageCommand { get; set; }
        /// <summary>
        /// 进入收银台
        /// </summary>
        public RelayCommand<Window> CheckoutCounterCommand { get; set; }
        #endregion

        #region 函数
        /// <summary>
        /// Tab选项卡
        /// </summary>
        public static void AddItem(string trtrname, UserControl uc)
        {
            //判断当前选项是否已存在
            bool bolWhetherBe = false;
            //判断是否大于0
            if (TC.Items.Count>0)
            {
                for (int i = 0; i < TC.Items.Count; i++)
                {
                    if (((UCTabItem)TC.Items[i]).Name==trtrname)
                    {
                        //选中当前选项卡
                        TC.SelectedItem = ((UCTabItem)TC.Items[i]);
                        bolWhetherBe = true;
                        break;
                    }
                }
                //不存在当前选项
                if (bolWhetherBe==false)
                {
                    //创建子选项
                    UCTabItem item = new UCTabItem();
                    item.Name = trtrname;
                    item.Width = 140;
                    item.Height = 40;
                    item.Header = string.Format(trtrname);
                    item.Content = uc;
                    TC.Items.Add(item);
                    TC.SelectedItem = item;
                }
            }
            else
            {
                //第一次添加TabItem
                UCTabItem item = new UCTabItem();
                item.Name = trtrname;
                item.Width = 140;
                item.Height = 40;
                item.Header = string.Format(trtrname);
                item.Content = uc;
                TC.Items.Add(item);
                TC.SelectedItem = item;
            }
        }

        #endregion

        #region 页面嵌套
        /// <summary>
        /// 产品管理
        /// </summary>
        private void ProducPage(TabControl tc)
        {
            TC = tc;
            Produc myProduc = new Produc();
            AddItem("产品管理", myProduc);
        }
        /// <summary>
        /// 字典管理
        /// </summary>
        private void DictionaryPage(TabControl tc)
        {
            TC = tc;
            DictionaryView myDictionary = new DictionaryView();
            AddItem("字典管理", myDictionary);
        }
        /// <summary>
        /// 员工管理
        /// </summary>
        private void StaffPage(TabControl tc)
        {
            TC = tc;
            StaffView myStaff = new StaffView();
            AddItem("员工管理", myStaff);
        }
        /// <summary>
        /// 交接班管理
        /// </summary>
        private void HandoverPage(TabControl tc)
        {
            TC = tc;
            HandoverView myHandover = new HandoverView();
            AddItem("交接班管理", myHandover);
        }
        /// <summary>
        /// 进入收银台
        /// </summary>
        private void CheckoutCounter(Window wd)
        {
            CheckoutCounterView myCheckoutCounte = new CheckoutCounterView();
            var myCheckoutCounteVModel = (myCheckoutCounte.DataContext as DrinksSystem.ViewModels.CheckoutCounterVModel.CheckoutCounterVModel);
            //myCheckoutCounteVModel.ProductTypeData = new List<S_Dictionary>();
            myCheckoutCounte.Show();
            wd.Close();
        }
        
        #endregion
    }
}
