using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DrinksSystem.Resources.control
{
    /// <summary>
    /// UCMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class UCMessageBox : Window
    {
        public UCMessageBox()
        {
            InitializeComponent();
        }
        //获取当前Window
        Window myWindow = new Window();

        public string BoxText;
        //创建定时器 
        /* DispatcherTimer定时器不是单独开启一个线程来运行定时器方法，而是和主线程是同一个线程，
         * 只是通过改变运行优先级来实现定时器，当定时器时间到了，主线程就转去执行定时器方法。
         * 因此DispatcherTimer定时器不要用来实现执行时间长的任务，不然会使主线程很卡，导致WPF界面很看，是用不友好！*/
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myWindow = this;
            myWindow.Topmost = true;//置顶窗口
            TbEmployee.Text = BoxText;

            //弹出动画
            Storyboard sbQue = new Storyboard();//创建故事版
            DoubleAnimation dou = new DoubleAnimation(1080, 980, TimeSpan.FromSeconds(0.6));
            sbQue.Children.Add(dou);//添加动画到故事版
            Storyboard.SetTargetProperty(dou, new PropertyPath("(Window.Top)"));//把动画设置到Window的Top属性
            sbQue.Completed += WindowsAnimation;//设置动画执行完后的事件(缩回窗口)
            myWindow.BeginStoryboard(sbQue);//开始执行


        }
        //开始计时
        public void WindowsAnimation(object sender, EventArgs e)
        {
            
            dispatcherTimer.Tick += new EventHandler(WindowsAnimation1);//设置定时器执行的事件
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);//（时，分，秒）
            dispatcherTimer.Start();//开始计时
        }
        //缩回窗口动画
        public void WindowsAnimation1(object sender,EventArgs e)
        {
            dispatcherTimer.Stop();//停止计时器
            Storyboard sbQue = new Storyboard();//创建故事版
            DoubleAnimation dou = new DoubleAnimation(980, 1080, TimeSpan.FromSeconds(0.5));//创建动画
            sbQue.Children.Add(dou);//添加动画到故事版
            Storyboard.SetTargetProperty(dou, new PropertyPath("(Window.Top)"));//把动画设置到Window的Top属性
            sbQue.Completed += closeWindow;//设置动画执行完后的事件(关闭窗口)
            myWindow.BeginStoryboard(sbQue);//开始执行
            
        }
        //关闭窗口
        public void closeWindow(object sender,EventArgs e) {
            myWindow.Close();
        }
    }
}
