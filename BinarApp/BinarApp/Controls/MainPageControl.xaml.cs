using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using BinarApp.Providers;
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

namespace BinarApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для MainPageControl.xaml
    /// </summary>
    public partial class MainPageControl : UserControl
    {
        private IDataCacheProvider _dataCacheProvider;

        private string[] KeyWords =
        {
            "ориентировка",
            "грабеж",
            "разбой",
            "разбоя",
            "контрабанда",
            "контробанда",
            "задержать",
            "кража",
            "преступлен",
            "розыск",
            "уголовн",
            "угон"
        };

        public MainPageControl()
        {
            InitializeComponent();

            _dataCacheProvider = new DataCacheProvider();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void listImagesRibbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var args = e.AddedItems[0] as ListViewEventArgs;
                imgMain.Source = args.Image as BitmapImage;
                lblMainPlateNumber.Content = args.PlateNumber;

                var fixations = _dataCacheProvider.GetDataFromCache<Fixation>(CacheDataType.DatabaseCache);
                bool inSearch = fixations
                       .Where(x => x.GRNZ == args.PlateNumber)
                       .Any(x => KeyWords.Any(y => !string.IsNullOrWhiteSpace(x.Description) && x.Description.ToLower().Contains(y)));

                if (inSearch)
                {
                    lblWarning.Content = "Внимание, ТС находится в розыске!";
                    lblWarning.Background = Brushes.Red;
                } 
                else
                {
                    lblWarning.Content = string.Empty;
                    lblWarning.Background = Brushes.LightGray;
                }

                //if (KeyWords.Any(x => args.Description.ToLower().Contains(x)))
                //{
                //    lblWarning.Content = "Внимание, ТС находится в розыске!";
                //    lblWarning.Background = Brushes.Red;
                //}
                //else
                //{
                //    lblWarning.Content = string.Empty;
                //    lblWarning.Background = Brushes.LightGray;
                //}

                // Display speed
                lblSpeed.Content = $"{args.Speed} КМ/Ч";
                // Display plate number
                lblPlateNumber.Content = args.PlateNumber;
                // Display date and time
                lblTime.Content = args.FixationDate.ToString("HH:mm:ss");
                lblDate.Content = args.FixationDate.ToString("dd-MM-yyyy");
            }
        }
    }
}
