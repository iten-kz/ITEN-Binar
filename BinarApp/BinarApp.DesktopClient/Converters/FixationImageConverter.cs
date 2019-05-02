using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BinarApp.DesktopClient.Converters
{
    public class FixationImageConverter
    {
        private static string _imageSourcePath = "images/";

        public FixationImageConverter()
        {
        }

        public static void SaveJpeg(string path, Image img, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("quality must be between 0 and 100.");

            // Encoder parameter for image quality 
            using (EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality))
            {
                // JPEG image codec 
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;
                img.Save(path, jpegCodec, encoderParams);
            }                     
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            for (int i = 0; i < codecs.Length; i++)
                if (codecs[i].MimeType == mimeType)
                    return codecs[i];

            return null;
        }

        public static string ConvertBase64ToImage(string base64)
        {
            string imgPath = string.Empty;
            //ImageSource result = null;

            if (!string.IsNullOrEmpty(base64))
            {
                byte[] binaryData;
                BitmapImage bi = new BitmapImage();
                try
                {
                    binaryData = Convert.FromBase64String(base64);
                }
                catch (FormatException ex)
                {
                    if (!base64.EndsWith("==") || !base64.EndsWith("="))
                    {
                        base64 += "==";
                    }
                    binaryData = Convert.FromBase64String(base64);
                }

                bi.BeginInit();
                bi.StreamSource = new MemoryStream(binaryData);
                bi.EndInit();

                var imageFolderPath = AppDomain.CurrentDomain.BaseDirectory + _imageSourcePath;
                if (!Directory.Exists(imageFolderPath))
                {
                    Directory.CreateDirectory(imageFolderPath);
                }

                imgPath = imageFolderPath + Guid.NewGuid().ToString() + ".jpg";

                using (FileStream filestream = new FileStream(imgPath, FileMode.Create))
                {
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bi));
                    encoder.Save(filestream);
                    encoder = null;
                }
            }

            return imgPath;
        }

        public static ImageSource ConvertToImageSource(string imgPath)
        {
            ImageSource result = null;

            using (Stream reader = File.OpenRead(imgPath))
            {
                Image img = Image.FromStream(reader);
                var finalStream = new MemoryStream();
                img.Save(finalStream, ImageFormat.Png);
                var decoder = new PngBitmapDecoder(finalStream,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.Default);
                result = decoder.Frames[0];
            }

            return result;
        }
    }
}
