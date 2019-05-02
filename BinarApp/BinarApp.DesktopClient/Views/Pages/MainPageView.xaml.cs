using BinarApp.DesktopClient.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinarApp.DesktopClient.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPageView.xaml
    /// </summary>
    public partial class MainPageView : UserControl
    {
        MainPageViewModel model;

        public MainPageView()
        {
            InitializeComponent();
            model = SimpleIoc.Default.GetInstance<MainPageViewModel>();
        }

        [PreferredConstructor]
        public MainPageView(MainPageViewModel model) {
            this.model = model;
            InitializeComponent();
        }

        private void Flyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            if (!((Flyout)sender).IsOpen)
            {
                model.FlayoutMode = 0;
            }
        }
    }
}
