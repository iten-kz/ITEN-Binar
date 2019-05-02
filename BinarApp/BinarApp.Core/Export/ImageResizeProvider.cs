using ImageProcessor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinarApp.Core.Export
{
    public class ImageResizeProvider
    {
        public byte[] Resize(byte[] image)
        {
            using (var outStream = new MemoryStream())
            {
                using (var imageFactory = new ImageFactory(preserveExifData: true))
                {
                    imageFactory.Load(image)
                        .Constrain(new System.Drawing.Size(400, 400))
                        .Save(outStream);

                    return outStream.GetBuffer();
                }
            }
        }

    }
}