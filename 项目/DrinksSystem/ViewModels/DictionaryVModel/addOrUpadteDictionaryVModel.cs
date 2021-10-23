using DrinksSystem.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DrinksSystem.ViewModels.DictionaryVModel
{
    public class addOrUpadteDictionaryVModel:ViewModelBase
    {
        DrinksSystemEntities myModel = new DrinksSystemEntities();
        public addOrUpadteDictionaryVModel()
        {
            CloseCommand = new RelayCommand<Window>(CloseWindow);//关闭窗口
            SaveCommand = new RelayCommand<Window>(SavaDiationaryData);//保存
        }

        #region 属性
        //数据源
        public S_Dictionary _dictionary;
        public S_Dictionary Dictionary
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
        //窗口标题
        public string _WindowTitle;
        public string WindowTitle
        {
            get { return _WindowTitle; }
            set
            {
                if (_WindowTitle != value)
                {
                    _WindowTitle = value;
                    RaisePropertyChanged(() => WindowTitle);
                }
            }
        }
        //声明委托(刷新页面)
        public delegate void CompletedEvent(string TxT);
        public event CompletedEvent RefreshDataGrid;


        public bool IsAdd;//窗口标识
        #endregion

        #region 命令
        //关闭窗口
        public RelayCommand<Window> CloseCommand { get; set; }
        //保存
        public RelayCommand<Window> SaveCommand { get; set; }
        #endregion

        #region 函数
        //关闭窗口
        public void CloseWindow(Window window)
        {
            if (window!=null)
            {
                window.Close();
            }
        }
        //保存
        private void SavaDiationaryData(Window window)
        {
            if (Dictionary.dictionaryName!="")
            {
                //获取到页面当前数据
                S_Dictionary myDictionary = new S_Dictionary()
                {
                    dictionaryName = Dictionary.dictionaryName.Trim(),
                    dictionaryType = Dictionary.dictionaryType.Trim()
                };
                //判断是新增还是修改
                if (IsAdd)
                {
                    var ifDictionary = myModel.S_Dictionary.Where(m => m.dictionaryName == myDictionary.dictionaryName).ToList().Count;
                    if (ifDictionary>0)
                    {
                        Notice.Show("字典名称重复,请变更", "提示", 2, MessageBoxIcon.Info);
                    }
                    else
                    {
                        myModel.S_Dictionary.Add(myDictionary);
                        if (myModel.SaveChanges()>0)
                        {
                            Notice.Show("修改成功", "提示", 2, MessageBoxIcon.Success);
                            RefreshDataGrid(myDictionary.dictionaryType);//委托刷新
                            window.Close();//关闭窗口
                        }
                        else
                        {
                            Notice.Show("修改失败", "提示", 2, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    //判断是否重复
                    var ifDictionary = myModel.S_Dictionary.Where(m => m.dictionaryName == myDictionary.dictionaryName && m.dictionaryID!= Dictionary.dictionaryID).ToList().Count;
                    if (ifDictionary>0)
                    {
                        Notice.Show("字典名称重复,请变更", "提示", 2, MessageBoxIcon.Info);
                    }
                    else
                    {
                        myDictionary.dictionaryID = Dictionary.dictionaryID;//获取到字典表ID
                        myModel.Entry(myDictionary).State = EntityState.Modified;//修改
                        if (myModel.SaveChanges()>0)
                        {
                            Notice.Show("修改成功", "提示", 2, MessageBoxIcon.Success);
                            RefreshDataGrid(myDictionary.dictionaryType);//委托刷新
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
                Notice.Show("请把页面数据填写完整", "提示", 2, MessageBoxIcon.Question);
            }
        }
        #endregion
    }
}
