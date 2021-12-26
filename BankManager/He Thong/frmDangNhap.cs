using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManager
{
    public partial class frmDangNhap : Form
    {
        //Lưu tên Server lúc đăng nhập
        private SqlConnection conn_publisher = new SqlConnection();

        private void LayDSPM(String cmd)//chuỗi lệnh
        {
            DataTable dt = new DataTable();
            if (conn_publisher.State == ConnectionState.Closed) 
                conn_publisher.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd, conn_publisher);//Open succsess!
            da.Fill(dt);//tải dữ liệu từ table vào datatable (chứa ds phân mảnh)
            conn_publisher.Close();
            Program.bds_dspm.DataSource = dt;
            //DataSource:chứa ds dữ liệu cung cấp cho nó
            cmbChiNhanh.DataSource = Program.bds_dspm;
            /*Đưa dữ liệu và liên kết dữ liệu với Combobox tương ứng*/
            //Chứa tên fill và click mũi tên Combobox thì hiển thị dữ liệu của fill
            cmbChiNhanh.DisplayMember = "TENCN";
            //Chứa tên fill khi chọn chi nhánh nào trong tên chi nhánh thì tự động trả về giá trị server tương ứng với chi nhánh đó 
            cmbChiNhanh.ValueMember = "TENSERVER";
        }

        public frmDangNhap()
        {
            InitializeComponent();
            if (KetNoi_CSDLGOC() == 0)
                return;
            LayDSPM("SELECT * FROM Get_Subscribes");
            //gán giá trị sẵn: mặc định là 0, khi chọn 1 thì đổi thành SelectetChange
            cmbChiNhanh.SelectedIndex = 1;
            cmbChiNhanh.SelectedIndex = 0;
        }

        private int KetNoi_CSDLGOC()
        {
            if (conn_publisher != null && conn_publisher.State == ConnectionState.Open)
                conn_publisher.Close();//mở connection sẽ phải đóng không sẽ lỗi
            try
            {
                conn_publisher.ConnectionString = Program.connstr_publisher;
                conn_publisher.Open();
                return 1; //Connect succsess!
            }
            catch (Exception e)
            {
                MessageBox.Show("Lỗi kết nối về cơ sở dữ liệu gốc.\nVui lòng kiểm tra lại Tên Server của Publisher và tên CSDL trong chuỗi kết nối!\n" + e.Message, "", MessageBoxButtons.OK);
                return 0;//Connect fail

            }

        }
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            //Bước 1: dùng account kết nối về server tương ứng và check
            if (txtTaiKhoan.Text.Trim() == "")
            {
                MessageBox.Show("Tên Đăng Nhập không được trống!", "Cảnh báo!!!", MessageBoxButtons.OK);
                txtTaiKhoan.Focus();
                return;
            }
            if (txtMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Mật Khẩu không được trống!", "Cảnh báo!!!", MessageBoxButtons.OK);
                txtMatKhau.Focus();
                return;
            }

            Program.mlogin = txtTaiKhoan.Text;
            Program.password = txtMatKhau.Text;

            //Program.servername = nameServerDN;
            if (Program.KetNoi() == 0) 
                return;
            //các thông tin sử dụng lại
            Program.mChinhanh = cmbChiNhanh.SelectedIndex;
            Program.mloginDN = Program.mlogin;
            Program.passwordDN = Program.password;
            Program.connstrDN = Program.connstr;
            //gọi SP
            String strLenh = "EXEC SP_DANGNHAP '" + Program.mlogin + "'";
            /*tạo chương trình con để gọi (VD: ExecSQLDataReader)
             *1. dữ liệu chỉ cho đọc: myReader thuộc kiểu dataReader, trả về dữ liệu nhanh, nhưng dữ liệu ko sửa được --chỉ đi xuống--
             *2. trả về dataTable nhiều dòng nhiều cột cho phép thêm-xóa-sửa --lên xuống thoải mái--
             *3. thực thi cập nhật trên sp mà không trả về giá trị
             */
            Program.myReader = Program.ExecSqlDataReader(strLenh);
            if (Program.myReader == null)
                return;


            //Bước 2: lấy mã nhân viên và tên nhóm

            if (Program.myReader.Read())
            {
                Program.username = Program.myReader.GetString(0);//Mã NV
                Program.mHoten = Program.myReader.GetString(1);//Họ Tên
                Program.mGroup = Program.myReader.GetString(2);//Nhóm
            }

            if (Convert.IsDBNull(Program.username))
            {
                MessageBox.Show("Tài khoản bạn nhập không có quyền truy cập dữ liệu\nVui lòng xem lại Username và Password!", "Lỗi!!!", MessageBoxButtons.OK);
                return;
            }

            Program.myReader.Close();
            Program.conn.Close();

            //Phân quyền
            Program.frmChinh.MANV.Text = "Mã NV" + Program.username;
            Program.frmChinh.HOTEN.Text = "Họ Tên" + Program.mHoten;
            Program.frmChinh.NHOM.Text = "Nhóm" + Program.mGroup;
            MessageBox.Show("Đăng nhập thành công", "Thông báo", MessageBoxButtons.OK);
            Program.frmChinh.HienThiMenu();
        }

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Program.servername = cmbChiNhanh.SelectedValue.ToString();

            }
            catch (Exception) { }
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            //kết nối cơ sở dữ liệu gốc
            if (KetNoi_CSDLGOC() == 0) return;
            //lấy về ds phân mảnh, select lấy dữ liệu của view
            LayDSPM("SELECT * FROM Get_Subscribes");
            //liên kết phân mảnh với form
            cmbChiNhanh.SelectedIndex = 1;
            cmbChiNhanh.SelectedIndex = 0;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn chắc chắn muốn thoát!", "Thông Báo", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                this.Close();
            else 
                return;
        }
    }
}
