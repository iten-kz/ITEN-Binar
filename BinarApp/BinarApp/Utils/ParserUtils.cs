using BinarApp.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BinarApp.Utils
{
    public class ParserUtils
    {
        /// <summary>
        /// Parse xml string and deserialize to StoryList
        /// </summary>
        /// <param name="data">xml string</param>
        /// <returns>StoryList object</returns>
        public StoryList ParseXmlString(string data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StoryList));
            using (TextReader reader = new StringReader(data))
            {
                var result = (StoryList)serializer.Deserialize(reader);
                return result;
            }
        }
    }
}
