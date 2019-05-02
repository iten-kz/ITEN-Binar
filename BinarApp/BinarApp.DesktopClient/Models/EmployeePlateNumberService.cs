using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Models
{
    public class EmployeePlateNumberService
    {
        private readonly string _key = "EMPLOYEE_PLATE_NUMBER";

        public string PlateNumber
        {
            get
            {
                return ConfigurationManager.AppSettings.Get(_key);
            }

            set
            {
                ConfigurationManager.AppSettings.Set(_key, value);
            }
        }

    }
}
