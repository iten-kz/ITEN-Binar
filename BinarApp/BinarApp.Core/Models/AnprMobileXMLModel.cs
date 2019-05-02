using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Models
{

    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class root
    {

        private System.DateTime dATEField;

        private string tIMEField;

        private string pLATE_STRINGField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DATE
        {
            get
            {
                return this.dATEField;
            }
            set
            {
                this.dATEField = value;
            }
        }

        /// <remarks/>
        public string TIME
        {
            get
            {
                return this.tIMEField;
            }
            set
            {
                this.tIMEField = value;
            }
        }

        /// <remarks/>
        public string PLATE_STRING
        {
            get
            {
                return this.pLATE_STRINGField;
            }
            set
            {
                this.pLATE_STRINGField = value;
            }
        }
    }


}
