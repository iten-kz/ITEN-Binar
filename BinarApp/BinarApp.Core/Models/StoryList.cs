using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BinarApp.Core.Models
{

    // Примечание. Для запуска созданного кода может потребоваться NET Framework версии 4.5 или более поздней версии и .NET Core или Standard версии 2.0 или более поздней.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class StoryList
    {

        private StoryListStory storyField;

        /// <remarks/>
        public StoryListStory Story
        {
            get
            {
                return this.storyField;
            }
            set
            {
                this.storyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StoryListStory
    {

        private StoryListStoryStoryInfo storyInfoField;

        private StoryListStoryImage[] imageListField;

        /// <remarks/>
        public StoryListStoryStoryInfo StoryInfo
        {
            get
            {
                return this.storyInfoField;
            }
            set
            {
                this.storyInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Image", IsNullable = false)]
        public StoryListStoryImage[] ImageList
        {
            get
            {
                return this.imageListField;
            }
            set
            {
                this.imageListField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StoryListStoryStoryInfo
    {

        private uint sessionIdField;

        private string dateField;

        private System.DateTime timeField;

        private object streetField;

        /// <remarks/>
        public uint SessionId
        {
            get
            {
                return this.sessionIdField;
            }
            set
            {
                this.sessionIdField = value;
            }
        }

        /// <remarks/>
        public string Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "time")]
        public System.DateTime Time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        public object Street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StoryListStoryImage
    {

        private StoryListStoryImageImageInfo imageInfoField;

        private StoryListStoryImageDirection directionField;

        private string dataField;

        /// <remarks/>
        public StoryListStoryImageImageInfo ImageInfo
        {
            get
            {
                return this.imageInfoField;
            }
            set
            {
                this.imageInfoField = value;
            }
        }

        /// <remarks/>
        public StoryListStoryImageDirection Direction
        {
            get
            {
                return this.directionField;
            }
            set
            {
                this.directionField = value;
            }
        }

        /// <remarks/>
        public string Data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StoryListStoryImageImageInfo
    {

        private string typeField;

        private byte speedField;

        private byte thresholdField;

        private string dateField;

        private System.DateTime timeField;

        private string sDNumberField;

        private ushort deviceSNumberField;

        private string checksumField;

        /// <remarks/>
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public byte Speed
        {
            get
            {
                return this.speedField;
            }
            set
            {
                this.speedField = value;
            }
        }

        /// <remarks/>
        public byte Threshold
        {
            get
            {
                return this.thresholdField;
            }
            set
            {
                this.thresholdField = value;
            }
        }

        /// <remarks/>
        public string Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "time")]
        public System.DateTime Time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <remarks/>
        public string SDNumber
        {
            get
            {
                return this.sDNumberField;
            }
            set
            {
                this.sDNumberField = value;
            }
        }

        /// <remarks/>
        public ushort DeviceSNumber
        {
            get
            {
                return this.deviceSNumberField;
            }
            set
            {
                this.deviceSNumberField = value;
            }
        }

        /// <remarks/>
        public string Checksum
        {
            get
            {
                return this.checksumField;
            }
            set
            {
                this.checksumField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StoryListStoryImageDirection
    {

        private string directionField;

        /// <remarks/>
        public string Direction
        {
            get
            {
                return this.directionField;
            }
            set
            {
                this.directionField = value;
            }
        }
    }
}

