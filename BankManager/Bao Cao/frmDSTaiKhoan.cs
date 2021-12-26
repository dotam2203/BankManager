using DevExpress.XtraReports.UI;
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
    public partial class frmDSTaiKhoan : Form
    {
        DateTime dateTo;
        DateTime dateFrom;
        DateTime now = DateTime.Now;
        Boolean all = false;
        public frmDSTaiKhoan()
        {
            InitializeComponent();
        }
        private class Data
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        
        private void taiKhoanBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsTaiKhoan.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);

        }

        private void frmDSTaiKhoan_Load(object sender, EventArgs e)
        {
            BindingList<Data> _comboItems = new BindingList<Data>();
            _comboItems.Add(new Data { Name = "Bến Thành", Value = "PIPI\\SERVER1" });
            _comboItems.Add(new Data { Name = "Tân Định", Value = "PIPI\\SERVER2" });
            _comboItems.Add(new Data { Name = "2 Chi Nhánh", Value = "true" });
            cmbChiNhanh.DisplayMember = "Name";
            cmbChiNhanh.ValueMember = "Value";
            cmbChiNhanh.DataSource = _comboItems;
            cmbChiNhanh.SelectedIndex = 0;
            txtTen.Text = Program.mHoten;
            txtNgay.Text = DateTime.Now.ToString("dd-MM-yyyy");

            if (Program.mGroup.Trim() == "NganHang")
            {

                cmbChiNhanh.Enabled = true;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
            }
            btnXemTruoc.Enabled = true;
            cmbDateFrom.DateTime = now;
            cmbDateTo.DateTime = now;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnXemTruoc.Enabled = false;
            if (cmbChiNhanh.SelectedValue != null)
            {
                if (cmbChiNhanh.SelectedValue.ToString().Trim() == "true")
                {
                    all = true;
                    return;
                }
                if (cmbChiNhanh.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    all = false;
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
            this.sp_BaoCaoDSTKTableAdapter.Connection.ConnectionString = Program.connstr;
            this.sp_BaoCaoDSTKTableAdapter.Fill(this.DS.sp_BaoCaoDSTK, cmbDateFrom.DateTime, cmbDateTo.DateTime, all);
            if (sp_BaoCaoDSTKBindingSource.Count == 0)
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnXemTruoc_Click(object sender, EventArgs e)
        {
            ReportDSTK rpt = new ReportDSTK(dateFrom, dateTo, all);
            rpt.lbChiNhanh.Text = cmbChiNhanh.Text;
            rpt.lbHoTenNV.Text = txtTen.Text;
            rpt.lbNgayThucHien.Text = txtNgay.Text;
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }
    }
}
