using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Utils
{
    public class ExcelImporter : IDisposable
    {
        public static String GetConnectionString(String filePath, bool noHDR)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            // XLSX - Excel 2007, 2010, 2012, 2013
            //props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
            //props["Extended Properties"] = "Excel 12.0 XML";
            //props["Data Source"] = "C:\\MyExcel.xlsx";

            // XLS - Excel 2003 and Older
            //props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            // props["Extended Properties"] = "\"Excel 8.0; IMEX = 1\"";

            props["Provider"] = "Microsoft.ACE.OLEDB.12.0";
            props["Extended Properties"] = noHDR ? "'Excel 12.0 Xml; HDR=NO; IMEX=1;'" : "'Excel 12.0 Xml; IMEX=1;'";
            props["Data Source"] = filePath;

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        public static DataSet ReadExcelFile(String filePath, bool noHDR = false)
        {
            DataSet ds = new DataSet();

            string connectionString = GetConnectionString(filePath, noHDR);

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbCommand cmd = new OleDbCommand();
                cmd.Connection = conn;

                // Get all Sheets in Excel File
                DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                //DataTable dtSheetColumns = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, null);

                // Loop through all Sheets to get data
                foreach (DataRow dr in dtSheet.Rows)
                {
                    string sheetName = dr["TABLE_NAME"].ToString();


                    if (!sheetName.TrimEnd(new char[1] { '\'' }).EndsWith("$"))
                        continue;

                    // Get all rows from the Sheet
                    cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                    DataTable dt = new DataTable();
                    dt.TableName = sheetName;

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    da.Fill(dt);

                    ds.Tables.Add(dt);
                    break;
                }

                cmd = null;
                conn.Close();
            }

            return ds;
        }

        public void Dispose()
        {
        }

        public void ImportFile(string filePath = "")
        {
            DataSet ds = ReadExcelFile(filePath, true);
            Int32 rowNumber = 1;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                // Header row
                if (rowNumber == 1)
                {
                    rowNumber++;
                    continue;
                }

                // TODO: parse logic
            }
        }

        public static int GetTotalRowCount(string filePath = "")
        {
            //DataSet ds = ReadExcelFile(filePath, true);

            int tryAttemptsCount = 10;
            for (int i = 0; i < tryAttemptsCount; i++)
            {
                FileInfo fi = new FileInfo(filePath);
                if (!IsFileLocked(fi))
                {
                    break;
                }
            }

            var fileStream = new FileStream(filePath, FileMode.Open);
            var fileStreamReader = new StreamReader(fileStream);
            int lineCounter = 0;

            while (!fileStreamReader.EndOfStream)
            {
                var line = fileStreamReader.ReadLine();
                lineCounter++;

                //int result = ds.Tables[0].Rows.Count;
            }

            return lineCounter;
        }

        private static bool IsFileLocked(FileInfo file)
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