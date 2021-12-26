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
    public partial class frmGuiRut : DevExpress.XtraEditors.XtraForm
    {
        String loaiGD;
        private class Data
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public frmGuiRut()
        {
            InitializeComponent();
            txtMaNV.Text = Program.username;
            txtHoTen.Text = Program.mHoten;
        }


        private void frmGuiRut_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dS.TaiKhoan' table. You can move, or remove it, as needed.
            DS.EnforceConstraints = false;
            this.TaiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
            this.TaiKhoanTableAdapter.Fill(this.DS.TaiKhoan);
            BindingList<Data> _comboItems = new BindingList<Data>();
            _comboItems.Add(new Data { Name = "Gửi Tiền", Value = "GT" });
            _comboItems.Add(new Data { Name = "Rút Tiền", Value = "RT" });
            cmbLoaiGD.DataSource = _comboItems;
            cmbLoaiGD.DisplayMember = "Name";
            cmbLoaiGD.ValueMember = "Value";
            txtHoTen.Text = Program.mHoten;
            txtMaNV.Text = Program.username;
            txtNgayGD.Text = DateTime.Now.ToString("dd-MM-yyyy");
            cmbLoaiGD.SelectedIndex = -1;
            cmbLoaiGD.SelectedIndex = 0;
        }

        private void btnDongY_Click(object sender, EventArgs e)
        {
            if (txtSTK.Text.Trim() == "")
            {
                MessageBox.Show("Số tài khoản không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtSTK.Focus();
            }
            else if (txtSoTien.Text.Trim() == "")
            {
                MessageBox.Show("Số tiền không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtSoTien.Focus();
            }
            else if (int.Parse(txtSoTien.Text.Trim()) < 100000)
            {
                MessageBox.Show("Số tiền phải lớn hơn 100.000 !", "Thông báo !", MessageBoxButtons.OK);
                txtSoTien.Focus();
            }
            else
            {
                try
                {
                    if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                    String str_sp = "sp_GuiRutTien";
                    Program.Sqlcmd = Program.conn.CreateCommand();
                    Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.Sqlcmd.CommandText = str_sp;
                    Program.Sqlcmd.Parameters.Add("@stk", SqlDbType.VarChar).Value = txtSTK.Text;
                    Program.Sqlcmd.Parameters.Add("@loai", SqlDbType.VarChar).Value = loaiGD;
                    Program.Sqlcmd.Parameters.Add("@money", SqlDbType.Money).Value = txtSoTien.Text;
                    Program.Sqlcmd.Parameters.Add("@manv", SqlDbType.VarChar).Value = txtMaNV.Text;
                    Program.Sqlcmd.Parameters.Add("@ngaygd", SqlDbType.VarChar).Value = DateTime.Now;
                    Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    Program.Sqlcmd.ExecuteNonQuery();
                    String ret = Program.Sqlcmd.Parameters["@RET"].Value.ToString();
                    if (ret == "0")
                    {
                        MessageBox.Show("Số tài khoản không tồn tại. Vui lòng kiểm tra lại !", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }
                    else if (ret == "1")
                    {
                        MessageBox.Show("Số dư không đủ để rút. Vui lòng kiểm tra lại !", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }
                    else if (ret == "2" && loaiGD == "GT")
                    {
                        MessageBox.Show("Gửi tiền thành công!", "Thông báo !", MessageBoxButtons.OK);
                        txtSTK.Text = txtSoTien.Text = "";
                        return;
                    }
                    else if (ret == "2" && loaiGD == "RT")
                    {
                        MessageBox.Show("Rút tiền thành công!", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Giao dịch thất bại!", "Thông báo !", MessageBoxButtons.OK);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      

        private void cmbLoaiGD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLoaiGD.SelectedValue != null)
            {
                loaiGD = cmbLoaiGD.SelectedValue.ToString().Trim();
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

       
    }
}