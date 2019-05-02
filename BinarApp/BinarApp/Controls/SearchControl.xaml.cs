using BinarApp.Controls.Windows;
using BinarApp.Core.POCO;
using BinarApp.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BinarApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();

            // Default
            txtQuery.Text = "082MAZ02";
        }

        private void btnLetter_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            txtQuery.Text += btn.Name.Split('_')[1];
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtQuery.Clear();
        }      

        private void btnHidePopup_Click(object sender, RoutedEventArgs e)
        {
            FixationsPopup.IsOpen = false;
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
        }
    }
}
