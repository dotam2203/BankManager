using System;
using DevExpress.XtraReports.UI;
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
    public partial class frmSaoKeGD : Form
    {
        DateTime dateTo;
        DateTime dateFrom;

        DateTime now = DateTime.Now;
        public frmSaoKeGD()
        {
            InitializeComponent();
        }
        private class Data
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private void gD_GOIRUTBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsGuiRut.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void frmSaoKeGD_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dS.TaiKhoan' table. You can move, or remove it, as needed.
            DS.EnforceConstraints = false;
            
            // TODO: This line of code loads data into the 'dS.TaiKhoan' table. You can move, or remove it, as needed.
            this.taiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
            this.taiKhoanTableAdapter.Fill(this.DS.TaiKhoan);

            txtSoTK.Text = ((DataRowView)bdsDSTK[bdsDSTK.Position])["SOTK"].ToString().Trim();
            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;
            txtTen.Text = Program.mHoten;
            txtNgay.Text = DateTime.Now.ToString("dd-MM-yyyy");
            if (Program.mGroup.Trim() == "NganHang")
            {
                Program.bds_dspm.Filter = "TENCN <> 'Khách Hàng' ";
                cmbChiNhanh.Enabled = true;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
            }
            btnXemTruoc.Enabled = false;
            cmbDateFrom.DateTime = now;
            cmbDateTo.DateTime = now;

        }
        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnXemTruoc.Enabled = false;
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
                        this.taiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.taiKhoanTableAdapter.Fill(this.DS.TaiKhoan);
                        txtSoTK.Text = ((DataRowView)bdsDSTK[bdsDSTK.Position])["SOTK"].ToString().Trim();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Thông báo nè", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
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

        private void btnManHinh_Click(object sender, EventArgs e)
        {
            this.sP_SaoKeTK_CNTableAdapter.Connection.ConnectionString = Program.connstr;
            this.sP_SaoKeTK_CNTableAdapter.Fill(this.DS.SP_SaoKeTK_CN, txtSoTK.Text.Trim(), dateFrom, dateTo);
            if (bdsSP_SaoKeTK_CN.Count == 0)
            {
                MessageBox.Show("Danh sách trống. Không có dữ liệu để in", "Thông báo !", MessageBoxButtons.OK);
                btnXemTruoc.Enabled = false;
            }
            else
            {
                btnXemTruoc.Enabled = true;
            }
        }

        private void cmbDateFrom_EditValueChanged(object sender, EventArgs e)
        {
            dateFrom = cmbDateFrom.DateTime;
        }

        private void cmbDateTo_EditValueChanged(object sender, EventArgs e)
        {
            dateTo = cmbDateTo.DateTime;
        }

        private void btnXemTruoc_Click(object sender, EventArgs e)
        {
            SP_SaoKe_CN rpt = new SP_SaoKe_CN(txtSoTK.Text.Trim(), dateFrom, dateTo);
            rpt.lbChiNhanh.Text = cmbChiNhanh.Text;
            rpt.lbHoTenNV.Text = txtTen.Text;
            rpt.lbNgayThucHien.Text = txtNgay.Text;
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void sP_SaoKeTK_CNGridControl_Click(object sender, EventArgs e)
        {
            txtSoTK.Text = ((DataRowView)bdsDSTK[bdsDSTK.Position])["SOTK"].ToString().Trim();
        }

    }
}
