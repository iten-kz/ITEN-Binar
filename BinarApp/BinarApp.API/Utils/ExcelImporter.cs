using BinarApp.API.Models;
using BinarApp.Core.POCO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace BinarApp.API.Utils
{
    public class ExcelImporter : IDisposable
    {
        private readonly BinarContext _context = new BinarContext();

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
            _context.Dispose();
        }

        public void ImportFile(string filePath = "")
        {
            DataSet ds = ReadExcelFile(@"C:\shtraf_excel.xlsx", true);
            Int32 rowNumber = 1;
            var data = new List<Fixation>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                // Header row
                if (rowNumber == 1)
                {
                    rowNumber++;
                    continue;
                }

                var birthDay = string.IsNullOrWhiteSpace(row[5].ToString()) ? "01" : row[5].ToString();
                var birthMonth = string.IsNullOrWhiteSpace(row[6].ToString()) ? "01" : row[6].ToString();
                var birthYear = string.IsNullOrWhiteSpace(row[7].ToString()) || row[7].ToString().Length < 4
                    ? "1900" : row[7].ToString();

                var fixationDay = string.IsNullOrWhiteSpace(row[8].ToString()) ? "01" : row[8].ToString();
                var fixationMonth = string.IsNullOrWhiteSpace(row[9].ToString()) ? "01" : row[9].ToString();
                var fixationYear = string.IsNullOrWhiteSpace(row[10].ToString()) || row[10].ToString().Length < 4
                    ? "1900" : row[10].ToString();

                Decimal sum = new Decimal(0);
                Decimal.TryParse(row[11].ToString().Trim().Replace(',', '.'),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out sum);
                var intrBirthDate = new DateTime(Convert.ToInt32(birthYear),
                        Convert.ToInt32(birthMonth), Convert.ToInt32(birthDay));
                var intruder = _context.Intruders.FirstOrDefault(x =>
                x.FirstName == row[3].ToString().Trim() &&
                x.MiddleName == row[4].ToString().Trim() &&
                x.LastName == row[2].ToString().Trim() &&
                x.BirthDate == intrBirthDate);
                if(intruder == null)
                {
                    intruder = new Intruder()
                    {
                        FirstName = row[3].ToString().Trim(),
                        MiddleName = row[4].ToString().Trim(),
                        LastName = row[2].ToString().Trim(),
                        BirthDate = intrBirthDate
                    };
                }
                data.Add(new Fixation
                {
                    EntityId = row[0].ToString().Trim(),
                    PUNKT = row[1].ToString().Trim(),                    
                    FixationDate = new DateTime(Convert.ToInt32(fixationYear),
                        Convert.ToInt32(fixationMonth), Convert.ToInt32(fixationDay)),
                    PenaltySum = sum,
                    SY1 = row[12].ToString().Trim(),
                    R05 = row[13].ToString().Trim(),
                    GRNZ = row[14].ToString().Trim(),
                    VU = row[15].ToString().Trim(),
                    PDD = row[16].ToString().Trim(),
                    Description = row[17].ToString(),
                    //Intruder = intruder
                });

                rowNumber++;
            }

            var groupedBy1000 = data.Select((x, i) => new { i, x })
                .GroupBy(x => x.i / 1000);

            foreach (var group in groupedBy1000)
            {
                using (var context = new BinarContext())
                {
                    foreach (var item in group)
                    {
                        context.Fixations.Add(item.x);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}