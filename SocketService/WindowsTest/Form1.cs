using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketCore;
using WebSocketDB;
namespace WindowsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sql = "select * from obd_data";
            DataSet ds= MySqlHelper.GetDataSet(MySqlHelper.Conn, CommandType.Text,sql, null);
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string data = row["data"].ToString().Trim();
                    int length = data.Length / 2;
                    byte[] buffer= DataHander.hexStringToByte(data);

                    DataHeader dh = new DataHeader(buffer, length);
                
                    if (dh.IsValid)
                    {
                        IRec irec = new ReceiveData();
                        switch (dh.Type)
                        {
                            case "1001":
                                var model = DataHander.LoginHander(dh);
                                irec.InsertGPSData(model);
                                break;
                            case "4001":
                                var model1 = DataHander.GPSHander(dh);
                                irec.InsertGPSData(model1);
                                break;
                            default:
                                break;
                        }


                    }

                }
            }

        }
    }
}
