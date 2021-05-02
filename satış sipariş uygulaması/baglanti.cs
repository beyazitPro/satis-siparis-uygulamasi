using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data.OleDb;
using System.Windows.Forms;

namespace satıssipuyg
{
    class baglanti
    {
        string gerekliuygklasoru = Application.StartupPath + @"\satış sipariş uygulaması gerekli dosyalar";

        public OleDbConnection excelstokbilgizconnection()
        {
            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + gerekliuygklasoru + @"\stokbilgileri.xlsx;Extended Properties='Excel 8.0; HDR = YES';";
            conn.Open();
            return conn;
        }

        public OleDbConnection excelcarilerzconnection()
        {
            OleDbConnection cariler = new OleDbConnection();
            cariler.ConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source= " + gerekliuygklasoru + @"\cariler.xlsx; Extended Properties='Excel 8.0; HDR = YES';";
            cariler.Open();
            return cariler;

        }
    }
}
