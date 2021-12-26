using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankManager
{
    public partial class frmDangKy : Form
    {
        String nUserlogin = "";
        String nPassword = "";
        String nUsername = "";
        String nRole = "";
        String trangThaiXoa;

        private class Data
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public frmDangKy()
        {
            InitializeComponent();
        }
        private void frmDangKy_Load(object sender, EventArgs e)
        {

            //khóa check khóa ngoại
            DS.EnforceConstraints = false;
            //Lấy accout đăng nhập
            this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstrDN;
            this.NhanVienTableAdapter.Fill(this.DS.NhanVien);
            //phân quyền
            if (Program.mGroup == "NganHang")
            {
                cmbRole.Items.Add("NganHang");
                cmbRole.Items.Add("ChiNhanh");
            }
            else if (Program.mGroup == "ChiNhanh")
            {
                cmbRole.Items.Add("ChiNhanh");
            }
            cmbRole.SelectedIndex = 0;
            //ẩn pass
            txtMatKhau.Properties.UseSystemPasswordChar = true;
            txtNhapLai.Properties.UseSystemPasswordChar = true;
        }

        private void checkedShowPass_CheckedChanged(object sender, EventArgs e)
        {
            //Hiển thị pass
            txtMatKhau.Properties.UseSystemPasswordChar = (checkedShowPass.Checked) ? false : true;
            txtNhapLai.Properties.UseSystemPasswordChar = (checkedShowPass.Checked) ? false : true;
        }

        private void gridControlNV_Click(object sender, EventArgs e)
        {
            txtHoTen.Text = ((DataRowView)bdsNhanVien[bdsNhanVien.Position])["HO"].ToString().Trim() + " " + ((DataRowView)bdsNhanVien[bdsNhanVien.Position])["TEN"].ToString().Trim();
            txtMaNV.Text = ((DataRowView)bdsNhanVien[bdsNhanVien.Position])["MANV"].ToString().Trim();
            trangThaiXoa = ((DataRowView)bdsNhanVien[bdsNhanVien.Position])["TrangThaiXoa"].ToString().Trim();
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            //nhân viên đã bị xóa
            if (trangThaiXoa == "1")
            {
                MessageBox.Show("Nhân Viên này đã xóa không thể tạo Tài khoản!", "Lỗi", MessageBoxButtons.OK);
                txtTaiKhoan.Focus();
                return;
            }
            if (txtTaiKhoan.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Tên Tài Khoản!", "Cảnh Báo!", MessageBoxButtons.OK);
                txtTaiKhoan.Focus();
                return;
            }
            else if (txtMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Mật Khẩu!", "Cảnh Báo!", MessageBoxButtons.OK);
                txtMatKhau.Focus();
                return;
            }
            else if (txtNhapLai.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập lại Mật Khẩu!", "Cảnh Báo!", MessageBoxButtons.OK);
                txtNhapLai.Focus();
                return;
            }
            else if (txtMaNV.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Mã Nhân Viên!", "Cảnh Báo!", MessageBoxButtons.OK);
                txtMaNV.Focus();
                return;
            }
            else
            {
                nUserlogin = txtTaiKhoan.Text.Trim();
                nPassword = txtMatKhau.Text.Trim();
                nUsername = txtMaNV.Text.Trim();
                nRole = cmbRole.Text.Trim();
                try
                {
                    //check nếu kết nối đã đóng thì mở lại
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strSP = "sp_TaoTK";
                    Program.Sqlcmd = Program.conn.CreateCommand();
                    Program.Sqlcmd.CommandType = CommandType.StoredProcedure;//lấy sp
                    Program.Sqlcmd.CommandText = strSP;
                    Program.Sqlcmd.Parameters.Add("@userlogin", SqlDbType.NChar).Value = nUserlogin;
                    Program.Sqlcmd.Parameters.Add("@password", SqlDbType.NChar).Value = nPassword;
                    Program.Sqlcmd.Parameters.Add("@username", SqlDbType.NChar).Value = nUsername;
                    Program.Sqlcmd.Parameters.Add("@roles", SqlDbType.NChar).Value = nRole;
                    Program.Sqlcmd.ExecuteNonQuery();
                    MessageBox.Show("Tạo Tài Khoản đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK);
                    txtTaiKhoan.Text = txtMatKhau.Text = txtNhapLai.Text = txtHoTen.Text = txtMaNV.Text = "";

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("server principal"))
                    {
                        MessageBox.Show("Tên Đăng Nhập trùng. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK);
                        return;
                    }
                    else if (ex.Message.Contains("User, group, or role"))
                    {
                        MessageBox.Show("Nhân Viên này đã có Tài Khoản. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult dr_huy = MessageBox.Show("Bạn thật sử muốn Hủy Đăng Kí Tài Khoản?", "Cảnh Báo", MessageBoxButtons.YesNo);
            if (dr_huy == DialogResult.Yes)
                this.Close();
            else if (dr_huy == DialogResult.No)
                return;
        }
    }
}
