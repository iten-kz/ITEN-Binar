using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace BinarApp.DesktopClient.Views.Components
{
    /// <summary>
    /// Логика взаимодействия для IncidentCollectionComponentView.xaml
    /// </summary>
    public partial class IncidentCollectionComponentView : UserControl
    {
        public IncidentCollectionComponentView()
        {
            InitializeComponent();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //var p = new ProcessStartInfo(((Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\osk.exe")));
            //p.UseShellExecute = false;
            //p.
            //var proc = Process.Start(p);

            //string windir = Environment.GetEnvironmentVariable("WINDIR");
            //string osk = null;

            //if (osk == null)
            //{
            //    osk = Path.Combine(Path.Combine(windir, "sysnative"), "osk.exe");
            //    if (!File.Exists(osk))
            //    {
            //        osk = null;
            //    }
            //}

            //if (osk == null)
            //{
            //    osk = Path.Combine(Path.Combine(windir, "system32"), "osk.exe");
            //    if (!File.Exists(osk))
            //    {
            //        osk = null;
            //    }
            //}

            //if (osk == null)
            //{
            //    osk = "osk.exe";
            //}

            //Process.Start(osk);
            //Process.Start(@"C:\Windows\System32\osk.exe");
            //string touchKeyboardPath = @"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe";
            //Process.Start(touchKeyboardPath);
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Process p = Process.GetProcessesByName("osk").FirstOrDefault();
            //if (p != null)
            //{
            //    p.Kill();
            //}
        }
    }
}
