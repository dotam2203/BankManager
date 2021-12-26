using DevExpress.XtraEditors;
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
    public partial class frmChuyenTien : DevExpress.XtraEditors.XtraForm
    {
        public frmChuyenTien()
        {
            InitializeComponent();
        }

        private void frmChuyenTien_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dS.TaiKhoan' table. You can move, or remove it, as needed.
            DS.EnforceConstraints = false;
            this.TaiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
            this.TaiKhoanTableAdapter.Fill(this.DS.TaiKhoan);
            txtMaNV.Text = Program.username;
            txtTenNV.Text = Program.mHoten;
            txtNgayChuyenTien.Text = DateTime.Now.ToString("dd-MM-yyyy");
        }


        private void btnHuy_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn thật sự muốn hủy thao tác chuyển tiền?",
                    "Xác thực", MessageBoxButtons.YesNo);

            if (dr == DialogResult.No)
            {
                return;
            }
            else if (dr == DialogResult.Yes)
            {
                this.Close();

            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn thật sự muốn thoát?",
                     "Xác thực", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                this.Close();
            }
            else
                return;
        }

        private void taiKhoanBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsTaiKhoan.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void btnDongY_Click(object sender, EventArgs e)
        {
            if (txtSoTKChuyen.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập số tài khoản chuyển", "Thông báo !", MessageBoxButtons.OK);
                txtSoTKChuyen.Focus();
                return;
            }
            else if (txtSTKNhan.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập số tài khoản nhận", "Thông báo !", MessageBoxButtons.OK);
                txtSTKNhan.Focus();
                return;
            }
            else if (txtSoTKChuyen.Text.Trim() == txtSTKNhan.Text.Trim())
            {
                MessageBox.Show("Tài khoản nhận và tài khoản chuyển không được trùng nhau", "Thông báo !", MessageBoxButtons.OK);
                txtSTKNhan.Focus();
                return;
            }
            else if (txtSoTien.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập số tiền cần chuyển", "Thông báo !", MessageBoxButtons.OK);
                txtSoTien.Focus();
                return;
            }
            else
            {

                try
                {
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                    String str_sp = "sp_ThucHienChuyenTien";
                    Program.Sqlcmd = Program.conn.CreateCommand();
                    Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.Sqlcmd.CommandText = str_sp;
                    Program.Sqlcmd.Parameters.Add("@stkfrom", SqlDbType.NChar).Value = txtSoTKChuyen.Text.Trim();
                    Program.Sqlcmd.Parameters.Add("@stkto", SqlDbType.NChar).Value = txtSTKNhan.Text.Trim();
                    Program.Sqlcmd.Parameters.Add("@money", SqlDbType.Money).Value = txtSoTien.Text.Trim();
                    Program.Sqlcmd.Parameters.Add("@manv", SqlDbType.NChar).Value = txtMaNV.Text.Trim();
                    Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    Program.Sqlcmd.ExecuteNonQuery();
                    String ret = Program.Sqlcmd.Parameters["@RET"].Value.ToString();


                    if (ret == "0")
                    {
                        MessageBox.Show("Số dư không đủ.", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }
                    else if (ret == "1")
                    {
                        txtSoTKChuyen.Text = "";
                        txtSTKNhan.Text = "";
                        txtSoTien.Text = "";
                        MessageBox.Show("Chuyển tiền thành công", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }
                    else if (ret == "2")
                    {
                        MessageBox.Show("Chuyển tiền thất bại",
                             "Xác thực", MessageBoxButtons.OK);
                        return;
                    }
                    else if (ret == "3")
                    {
                        MessageBox.Show("Tài khoản không chuyển tồn tại", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }
                    else if (ret == "4")
                    {
                        MessageBox.Show("Tài khoản nhận không tồn tại", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }


                }
                catch (Exception)
                {
                    MessageBox.Show("Chuyển tiền thất bại",
                    "Xác thực", MessageBoxButtons.OK);
                    return;
                }
            }
        }

    }
}