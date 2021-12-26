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
    public partial class frmTaoTaiKhoan : DevExpress.XtraEditors.XtraForm
    {
        int vitriTK = 0;

        Boolean isEditingTK = false;
        String maCN;
        DateTime now = DateTime.Now;
        public frmTaoTaiKhoan()
        {
            InitializeComponent();
        }

        private void taiKhoanBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsTaiKhoan.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void frmTaoTaiKhoan_Load(object sender, EventArgs e)
        {
            
            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;

            if (Program.mGroup.Trim() == "NganHang")
            {
                Program.bds_dspm.Filter = "TENCN <> 'Khách Hàng' ";
                cmbChiNhanh.Enabled = true;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
            }
            

            maCN = ((DataRowView)bdsChiNhanh[0])["MACN"].ToString().Trim();
            txtMaCN.Text = maCN;

            DS.EnforceConstraints = false;
            // TODO: This line of code loads data into the 'ChiNhanh' table. You can move, or remove it, as needed.
            this.chiNhanhTableAdapter.Connection.ConnectionString = Program.connstr;
            this.chiNhanhTableAdapter.Fill(this.DS.ChiNhanh);
            // TODO: This line of code loads data into the 'dS.KhachHang' table. You can move, or remove it, as needed.
            this.KhachHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.KhachHangTableAdapter.Fill(this.DS.KhachHang);
            // TODO: This line of code loads data into the 'dS.TaiKhoan' table. You can move, or remove it, as needed.
            this.TaiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
            this.TaiKhoanTableAdapter.Fill(this.DS.TaiKhoan);
            // TODO: This line of code loads data into the 'DS.GD_GOIRUT' table. You can move, or remove it, as needed.
            this.GOIRUTTableAdapter.Connection.ConnectionString = Program.connstr;
            this.GOIRUTTableAdapter.Fill(this.DS.GD_GOIRUT);
            // TODO: This line of code loads data into the 'DS.GD_CHUYENTIEN' table. You can move, or remove it, as needed.
            this.CHUYENTIENTableAdapter.Connection.ConnectionString = Program.connstr;
            this.CHUYENTIENTableAdapter.Fill(this.DS.GD_CHUYENTIEN);

            txtCMND.ReadOnly = true;
            txtMaCN.ReadOnly = true;
            gridControlDSKH.Enabled = false;
            txtSoDu.ReadOnly = false;
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtSoTK.Focus();
            vitriTK = bdsTaiKhoan.Position;
            isEditingTK = true;
            bdsTaiKhoan.AddNew();
            gridControlTKKH.Enabled = false;
            txtMaCN.Text = maCN;
            txtCMND.Text = "";
            txtCMND.ReadOnly = false;
            txtCMND.Focus();
            gridControlDSKH.Enabled = true;
            txtCMND.Text = ((DataRowView)bdsKhachHang[bdsKhachHang.Position])["CMND"].ToString().Trim();
            dtNgayMoTK.EditValue = now;
            btnThem.Enabled = btnXoa.Enabled = btnThoat.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtSoDu.Text.Trim() == "")
            {
                MessageBox.Show("Số dư không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtSoDu.Focus();
            }
            else if (txtSoTK.Text.Trim() == "")
            {
                MessageBox.Show("Số tài khoản không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtSoTK.Focus();
            }
            else if (dtNgayMoTK.DateTime.CompareTo(now) == 1)
            {
                MessageBox.Show("Không được chọn quá ngày hiện tại!", "Thông báo!", MessageBoxButtons.OK);
            }
            else if (dtNgayMoTK.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng chọn ngày mở TK!", "Thông báo!", MessageBoxButtons.OK);
            }
            else
            {
                if (Program.conn.State == ConnectionState.Closed) Program.conn.Open();
                String str_sp = "sp_CheckExistAccount";
                Program.Sqlcmd = Program.conn.CreateCommand();
                Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                Program.Sqlcmd.CommandText = str_sp;
                Program.Sqlcmd.Parameters.Add("@STK", SqlDbType.VarChar).Value = txtSoTK.Text;
                Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                Program.Sqlcmd.ExecuteNonQuery();
                String ret = Program.Sqlcmd.Parameters["@RET"].Value.ToString();
                if (ret == "1" && isEditingTK == true)
                {
                    MessageBox.Show("Số tài khoản đã tồn tại. Vui lòng kiểm tra lại !", "Thông báo !", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    try
                    {
                        bdsTaiKhoan.EndEdit();            // kết thúc quá trình hiệu chỉnh, gửi dl về dataset
                        bdsTaiKhoan.ResetCurrentItem();           // lấy dl của textbox control bên dưới đẩy lên gridcontrol đòng bộ 2 khu vực(ko còn ở dạng tạm nữa mà chính thức ghi vào dataset)
                        this.TaiKhoanTableAdapter.Update(this.DS.TaiKhoan);         // cập nhật dl từ dataset về database thông qua tableadapter
                        isEditingTK = false;
                        gridControlTKKH.Enabled = true;
                        btnThem.Enabled = btnXoa.Enabled = btnThoat.Enabled = true;
                        gridControlDSKH.Enabled = false;
                        MessageBox.Show("Lưu thành công!", "Thông báo !", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi ghi tài khoản. " + ex.Message, "Thông báo !", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bdsTaiKhoan.Count == 0)
            {
                return;
            }
            else
            {
                if (bdsChuyenTien.Count > 0)
                {
                    MessageBox.Show("Tài khoản đã có giao dịch chuyển tiền, không thể xóa !", "Thông báo !", MessageBoxButtons.OK);
                    return;
                }
                if (bdsGoiRut.Count > 0)
                {
                    MessageBox.Show("Tài khoản đã có giao dịch gửi hoặc rút tiền, không thể xóa !", "Thông báo !", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    DialogResult ds = MessageBox.Show("Bạn chắc chắn muốn xóa tài khoản ?", "Thông báo !", MessageBoxButtons.YesNo);
                    if (ds == DialogResult.Yes)
                    {
                        try
                        {
                            bdsTaiKhoan.RemoveCurrent();         //xóa row đang chọn ra khỏi dataset
                            this.TaiKhoanTableAdapter.Update(this.DS.TaiKhoan);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi xóa tài khoản. " + ex.Message, "Thông báo !", MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.TaiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
            this.TaiKhoanTableAdapter.Fill(this.DS.TaiKhoan);
            this.KhachHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.KhachHangTableAdapter.Fill(this.DS.KhachHang);
            if (isEditingTK == true)
            {
                bdsTaiKhoan.CancelEdit();
                bdsTaiKhoan.Position = vitriTK;
                gridControlTKKH.Enabled = true;
                txtCMND.ReadOnly = true;
                btnThem.Enabled = btnXoa.Enabled = btnThoat.Enabled = true;
                isEditingTK = false;
                gridControlDSKH.Enabled = false;
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

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChiNhanh.SelectedValue != null)
            {
                if (cmbChiNhanh.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    Program.servername = cmbChiNhanh.SelectedValue.ToString();
                }
                if (cmbChiNhanh.SelectedIndex != Program.mChinhanh)
                {
                    Program.mlogin = Program.remotelogin;
                    Program.password = Program.remotepassword;
                }
                else
                {
                    Program.mlogin = Program.mloginDN;
                    Program.password = Program.passwordDN;
                }
                if (Program.KetNoi() == 0)
                {
                    MessageBox.Show("Lỗi chuyển chi nhánh", "Thông báo !", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    try
                    {
                        DS.EnforceConstraints = false;
                        this.chiNhanhTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.chiNhanhTableAdapter.Fill(this.DS.ChiNhanh);
                        maCN = ((DataRowView)bdsChiNhanh[0])["MACN"].ToString().Trim();
                        txtMaCN.Text = maCN;
                        this.TaiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.TaiKhoanTableAdapter.Fill(this.DS.TaiKhoan);
                        this.CHUYENTIENTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.CHUYENTIENTableAdapter.Fill(this.DS.GD_CHUYENTIEN);
                        this.GOIRUTTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.GOIRUTTableAdapter.Fill(this.DS.GD_GOIRUT);
                        this.KhachHangTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.KhachHangTableAdapter.Fill(this.DS.KhachHang);
                    }
                    catch (Exception) { }
                }
            }
        }

        private void gridControlDSKH_Click(object sender, EventArgs e)
        {
            txtCMND.Text = ((DataRowView)bdsKhachHang[bdsKhachHang.Position])["CMND"].ToString().Trim();
            txtMaCN.Text = maCN;
        }
    }
}