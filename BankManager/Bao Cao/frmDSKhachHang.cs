using DevExpress.XtraEditors;
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
    public partial class frmDSKhachHang : DevExpress.XtraEditors.XtraForm
    {
        Boolean toanBo = false;
        private class Data
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public frmDSKhachHang()
        {
            InitializeComponent();
        }
        private void btnManHinh_Click(object sender, EventArgs e)
        {
            this.sp_BaoCaoDSKHTableAdapter.Connection.ConnectionString = Program.connstr;
            this.sp_BaoCaoDSKHTableAdapter.Fill(this.DS.sp_BaoCaoDSKH, toanBo);
            if (bdsBaoCaoDSKH.Count == 0)
            {
                MessageBox.Show("Danh sách trống. Không có dữ liệu để in", "Thông báo !", MessageBoxButtons.OK);
                btnXemTruoc.Enabled = false;
            }
            else
            {
                btnXemTruoc.Enabled = true;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn thật sự muốn hủy thao tác sao kê danh sách khách hàng?",
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

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnXemTruoc.Enabled = false;
            if (cmbChiNhanh.SelectedValue != null)
            {
                if (cmbChiNhanh.SelectedValue.ToString().Trim() == "true")
                {
                    toanBo = true;
                    return;
                }
                if (cmbChiNhanh.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    toanBo = false;
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

        private void btnXemTruoc_Click(object sender, EventArgs e)
        {
            sp_BaoCaoDSKHcs rpt = new sp_BaoCaoDSKHcs(toanBo);
            rpt.lbChiNhanh.Text = cmbChiNhanh.Text.Trim().ToUpper();
            ReportPrintTool print = new ReportPrintTool(rpt);
            print.ShowPreviewDialog();
        }

        private void frmDSKhachHang_Load_1(object sender, EventArgs e)
        {
            BindingList<Data> _comboItems = new BindingList<Data>();
            _comboItems.Add(new Data { Name = "Bến Thành", Value = "PIPI\\SERVER1" });
            _comboItems.Add(new Data { Name = "Tân Định", Value = "PIPI\\SERVER2" });
            _comboItems.Add(new Data { Name = "2 Chi Nhánh", Value = "true" });
            cmbChiNhanh.DisplayMember = "Name";
            cmbChiNhanh.ValueMember = "Value";
            cmbChiNhanh.DataSource = _comboItems;
            cmbChiNhanh.SelectedIndex = 0;

            if (Program.mGroup.Trim() == "NganHang")
            {

                cmbChiNhanh.Enabled = true;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
            }
            btnXemTruoc.Enabled = false;
        }
    }
}