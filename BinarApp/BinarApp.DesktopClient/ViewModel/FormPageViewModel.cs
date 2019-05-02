using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Models;
using BinarApp.DesktopClient.Units;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.ViewModel
{
    public class FormPageViewModel : ViewModelBase
    {
        private NetworkUtils _networkUtils;
        private IProxyService<Fixation> _fixationService;

        public FormPageViewModel(IProxyService<Fixation> service)
        {
            _fixationService = service;

            _networkUtils = new NetworkUtils();
        }

        private string _grnz;
        public string GRNZ {
            get { return _grnz;  }
            set {
                if (_grnz != value)
                {
                    _grnz = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _speed;
        public int Speed {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    RaisePropertyChanged();
                }
            }
        }

        private decimal _penaltySum;
        public decimal PenaltySum {
            get { return _penaltySum; }
            set
            {
                if (_penaltySum != value)
                {
                    _penaltySum = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _description;
        public string Description {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _firstName;
        public string FirstName {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _lastName;
        public string LastName {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _middleName;
        public string MiddleName {
            get { return _middleName; }
            set
            {
                if (_middleName != value)
                {
                    _middleName = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _birthDate = DateTime.Now;
        public DateTime BirthDate {
            get { return _birthDate; }
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _fixationDate = DateTime.Now;
        public DateTime FixationDate {
            get { return _fixationDate; }
            set
            {
                if (_fixationDate != value)
                {
                    _fixationDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Fixation GetFixationModel()
        {
            // Get default image
            var noImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images/no-image.jpg");
            byte[] imageArray = File.ReadAllBytes(noImagePath);
            string base64 = Convert.ToBase64String(imageArray);

            return new Fixation()
            {
                BirthDate = DateTime.SpecifyKind(BirthDate, DateTimeKind.Local),
                Description = Description,
                FirstName = FirstName,
                FixationDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local),
                GRNZ = GRNZ,
                Image = base64,
                LastName = LastName,
                MiddleName = MiddleName,
                PenaltySum = PenaltySum,
                Speed = Speed
            };
        }

        public async void Save()
        {
            var fixation = GetFixationModel();
            _networkUtils.CheckConnection();
            Task postFixationTask = null;
            if (NetworkUtils.NetworkConnectionIsAvailable)
            {
                postFixationTask = _fixationService.PostEntity(fixation);
            }

            await postFixationTask;
        }
    }
}
