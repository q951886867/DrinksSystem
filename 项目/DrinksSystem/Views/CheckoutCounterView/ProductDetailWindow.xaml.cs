﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrinksSystem.Views.CheckoutCounterView
{
    /// <summary>
    /// ProductDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ProductDetailWindow : Window
    {
        public ProductDetailWindow()
        {
            InitializeComponent();
            DataContext = new DrinksSystem.ViewModels.CheckoutCounterVModel.ProductDetailVModel();
        }
    }
}
