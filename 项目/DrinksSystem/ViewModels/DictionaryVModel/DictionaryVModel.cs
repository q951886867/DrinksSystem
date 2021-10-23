using DrinksSystem.Models;
using DrinksSystem.Views.DictionaryView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrinksSystem.ViewModels.DictionaryVModel
{
    public class DictionaryVModel:ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public DictionaryVModel()
        {
            SelectDictionDataCommand = new RelayCommand<string>(SelectDictionData);//根据点击的字典类型 查询数据
            UpdateClickCommand = new RelayCommand(UpdataDictionaryData);//修改选中行数据
            InsertClickCommand = new RelayCommand(InsertDictionaryData);//新增
            DeleteClickCommand = new RelayCommand(DeleteDictionaryData);//删除
        }
        #region 属性
        //数据源
        private List<S_Dictionary> _dictionary;
        public List<S_Dictionary> Dictionary
        {
            get { return _dictionary; }
            set
            {
                if (_dictionary!=value)
                {
                    _dictionary = value;
                    RaisePropertyChanged(() => Dictionary);
                }
            }
        }
        //选中行实体
        private S_Dictionary _dictionaryEntity;
        public S_Dictionary DictionaryEntity
        {
            get { return _dictionaryEntity; }
            set
            {
                if (_dictionaryEntity != value)
                {
                    _dictionaryEntity = value;
                    RaisePropertyChanged(() => DictionaryEntity);
                }
            }
        }
        public string DictionaryTypeTxt;//储存当前新增的字典类型
        #endregion
        #region 命令
        //口味设置
        public RelayCommand<string> SelectDictionDataCommand { get; set; }
        //修改数据
        public RelayCommand UpdateClickCommand { get; set; }
        //删除选中红红
        public RelayCommand DeleteClickCommand { get; set; }
        //新增数据
        public RelayCommand InsertClickCommand { get; set; }
        #endregion
        #region 函数
        //根据点击的字典类型 查询数据
        private void SelectDictionData(string TxT)
        {
            var list = from tb in myModel.S_Dictionary
                       where tb.dictionaryType == TxT
                       select tb;
            Dictionary = list.ToList();
            DictionaryTypeTxt = TxT;
        }
        //修改选中的数据
        private void UpdataDictionaryData()
        {
            
            addOrUpadteDictionary myWindow = new addOrUpadteDictionary();
            var addOrUpadteDictionaryVModel = (myWindow.DataContext as addOrUpadteDictionaryVModel);
            addOrUpadteDictionaryVModel.Dictionary = DictionaryEntity;//传递选中行数据
            addOrUpadteDictionaryVModel.IsAdd = false;//修改标识
            addOrUpadteDictionaryVModel.WindowTitle = "修改字典";//窗口标题
            addOrUpadteDictionaryVModel.RefreshDataGrid += SelectDictionData;//传递刷新事件 委托
            myWindow.ShowDialog();
        }
        //删除选中行
        private void DeleteDictionaryData()
        {
            try
            {
                if (DictionaryEntity!=null)
                {
                    S_Dictionary myDictionary = myModel.S_Dictionary.Find(DictionaryEntity.dictionaryID);
                    myModel.S_Dictionary.Remove(myDictionary);
                    if (myModel.SaveChanges()>0)
                    {
                        Notice.Show("删除成功", "提示", 2, MessageBoxIcon.Info);
                        SelectDictionData(DictionaryTypeTxt);//刷新页面
                    }
                    else
                    {
                        Notice.Show("删除失败", "提示", 2, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Notice.Show("当前未选择数据", "提示", 2, MessageBoxIcon.Question);
                }
            }
            catch (Exception)
            {
                Notice.Show("删除失败", "提示", 2, MessageBoxIcon.Error);
            }
        }
        //新增
        private void InsertDictionaryData()
        {
            addOrUpadteDictionary myWindow = new addOrUpadteDictionary();
            var addOrUpadteDictionaryVModel = (myWindow.DataContext as addOrUpadteDictionaryVModel);//链接上下文
            addOrUpadteDictionaryVModel.Dictionary = new S_Dictionary();//实例化字典表
            addOrUpadteDictionaryVModel.Dictionary.dictionaryType = DictionaryTypeTxt;//传递字典类型
            addOrUpadteDictionaryVModel.IsAdd = true;//修改标识
            addOrUpadteDictionaryVModel.WindowTitle = "新增字典";//窗口标题
            addOrUpadteDictionaryVModel.RefreshDataGrid += SelectDictionData;//传递刷新事件 委托
            myWindow.ShowDialog();
        }
        #endregion
    }
}
