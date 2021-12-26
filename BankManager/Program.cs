using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BankManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        public static SqlConnection conn = new SqlConnection();
        public static SqlCommand Sqlcmd = new SqlCommand();
        public static String connstr;

        /*======================== Database name của Tâm ==================*/
        //Data Source = PIPI; Initial Catalog = NGANHANG; Persist Security Info=True;User ID = sa
        public static String connstr_publisher = " Data Source = PIPI; Initial Catalog = NGANHANG;Integrated Security=True;";

        /*======================== Database name của Thương ==================*/
        // public static String connstr_publisher = "Data Source=DESKTOP-UPGKA2J;Initial Catalog=NGANHANG;Integrated Security=True";


        public static String database = "NGANHANG";
        public static String remotelogin = "HTKN";
        public static String remotepassword = "123";

        public static SqlDataReader myReader;//thuộc dataReader - chỉ cho đọc
        public static String servername = "";
        public static String username = "";
        public static String mlogin = "";
        public static String password = "";

        
        
        public static String mloginDN = "";
        public static String passwordDN = "";
        public static String mGroup = "";
        public static String connstrDN = "";
        public static String mHoten = "";
        public static int mChinhanh = 0;

        //giữ dbsPM khi đăng nhập
        public static BindingSource bds_dspm = new BindingSource();
        //khai báo con trỏ frmChinh làm đối tượng cho form main
        public static frmMain frmChinh;

        public static int KetNoi()
        {
            if (Program.conn != null && Program.conn.State == ConnectionState.Open)
                Program.conn.Close();
            try
            {
                Program.connstr = "Data Source=" + Program.servername + ";Initial Catalog=" + Program.database + ";User ID=" + Program.mlogin + ";password=" + Program.password;
                Program.conn.ConnectionString = Program.connstr;
                Program.conn.Open();
                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show("Bạn xem lại UserName và Password!" + e, "Lỗi đăng nhập", MessageBoxButtons.OK);
                return 0;
            }
        }

        public static SqlDataReader ExecSqlDataReader(String strLenh)
        {
            SqlDataReader myReader;
            //(Tham số 1 - Chuỗi lệnh, Tham số 2 - Đường kết nối là đối tượng SqlConnection)
            SqlCommand sqlcmd = new SqlCommand(strLenh, Program.conn);
            //chuỗi lệnh, sử dụng text
            sqlcmd.CommandType = CommandType.Text;
            try
            {
                //thực thi câu lệnh chỉ cho đọc
                myReader = sqlcmd.ExecuteReader();
                return myReader;
            }
            catch (SqlException ex)
            {
                Program.conn.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static DataTable ExecSqlDataTable(String cmd)
        {
            DataTable dt = new DataTable();
            if (Program.conn.State == ConnectionState.Closed)
                Program.conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn);
            //tải về bằng fill
            da.Fill(dt);
            conn.Close();
            return dt;
        }

        public static int ExecSqlNonQuery(String strLenh)
        {
            SqlCommand Sqlcmd = new SqlCommand(strLenh, conn);
            Sqlcmd.CommandType = CommandType.Text;
            Sqlcmd.CommandTimeout = 600;//sửa mặc định tối đa là 10 phút (mặc định 1p)
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            try
            {
                Sqlcmd.ExecuteNonQuery();
                conn.Close();
                return 0;
            }
            catch (SqlException ex)
            {
                //demo
                if (ex.Message.Contains("Error converting data type varchar to int"))
                    MessageBox.Show("Bạn fomat Cell lại cột \"Ngày tạo\" qua kiểu Number hoặc mở File Excel.");
                else
                    MessageBox.Show(ex.Message);
                conn.Close();
                //trạng thái lỗi gửi từ RaisError trong SQL Server qua
                return ex.State;
            }
        }


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            frmChinh = new frmMain();
            Application.Run(frmChinh);
        }
    }
}
