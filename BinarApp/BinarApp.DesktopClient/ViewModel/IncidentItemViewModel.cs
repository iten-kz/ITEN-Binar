using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BinarApp.DesktopClient.ViewModel
{
    public class IncidentItemViewModel : ViewModelBase
    {
        public int Id { get; set; }
        public string NickName { get; set; }

        private string _carNumber;
        public string CarNumber
        {
            get => _carNumber;
            set
            {
                if (_carNumber != value)
                {
                    _carNumber = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _speed;
        public string Speed
        {
            get => $"{_speed} КМ/Ч";
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _dateTime;
        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            //get 
            //{
            //    if (_imageSource.UriSource == null)
            //    {
            //        // todo
            //        _imageSource.UriSource = new Uri(_imageSource + Guid.NewGuid().ToString() + ".png");
            //    }

            //    return _imageSource;
            //}
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged();
                }
            }
        }


        private bool _inSearch;
        public bool InSearch
        {
            get => _inSearch;
            set
            {
                if (_inSearch != value)
                {
                    _inSearch = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _flayoutText;
        public string FlayoutText
        {
            get => _flayoutText;
            set
            {
                if (_flayoutText != value)
                {
                    _flayoutText = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
