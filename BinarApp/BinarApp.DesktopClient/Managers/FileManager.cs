using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Managers
{
    public class FileManager
    {
        public string ReadFile(string filePath)
        {
            int tryAttemptsCount = 10;
            for (int i = 0; i < tryAttemptsCount; i++)
            {
                FileInfo fi = new FileInfo(filePath);
                if (!IsFileLocked(fi))
                {
                    break;
                }
                else
                {
                    Thread.Sleep(300);
                }
            }

            var result = File.ReadAllText(filePath);

            return result;
        }

        public bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
