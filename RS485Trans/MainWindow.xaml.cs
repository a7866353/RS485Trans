using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RS485Trans
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LabelTreeView.SelectedItemChanged += LabelTreeView_SelectedItemChanged;
            
            this.Loaded += MainWindow_Loaded;


            MainWin.Children.Add(new UICreater() { WinName = "InitWin" }.Get());
        }

        void LabelTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // throw new NotImplementedException();

            if( e.NewValue.GetType() == typeof(UICreater))
            {
                MainWin.Children.Clear();
                UICreater c = (UICreater)e.NewValue;
                MainWin.Children.Add(c.Get());
            }
            
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }

    public class UICreater : TreeViewItem
    {
        public string WinName { set; get; }
        public UIElement Get()
        {
            StackPanel sp = new StackPanel();
            sp.Children.Add(new Label() { Content = "SubWindows: " + WinName });
            sp.Children.Add(new Border(){ HorizontalAlignment = HorizontalAlignment.Stretch, BorderBrush = Brushes.Black, BorderThickness = new Thickness(4)});
            sp.Children.Add(new Label() { Content = "Regiest 01 :"});
            sp.Children.Add(new Label() { Content = "Regiest 02 :" });
            sp.Children.Add(new Label() { Content = "Regiest 02 :" });

            TreeViewItem item = new TreeViewItem();


            return sp;
        }

    }
    public class ContainerTreeViewItem : TreeViewItem
    {
        public ContainerTreeViewItem()
        {
            this.DefaultStyleKey = typeof(ContainerTreeViewItem);
        }
        public UICreater UI { set; get; }

        public override string ToString()
        {
            return "afsdfdsdsf";
        }
            
    }
}
