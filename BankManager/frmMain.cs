using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace BankManager
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmMain()
        {
            InitializeComponent();
        }
        //tạo kết nối database
        SqlConnection connect = new SqlConnection(@"Data Source=PIPI;Initial Catalog=NGANHANG;Persist Security Info=True;User ID=sa;Password=123");
        //SqlConnection connect = new SqlConnection(@"Data Source=DESKTOP-Q8STASM;Initial Catalog=NGANHANG;Persist Security Info=True;User ID=sa;Password=123");
        private Form CheckExitsts(Type ftype)
        {
            foreach (Form f in this.MdiChildren)//duyệt qua tất cả các thành phần con
                if (f.GetType() == ftype)
                    return f;
            return null;
        }
        public void HienThiMenu()
        {
            MANV.Text = "Mã NV: " + Program.username;
            HOTEN.Text = "Họ Tên: " + Program.mHoten;
            NHOM.Text = "Nhóm: " + Program.mGroup;
            //Phân quyền
            rbpBaoCao.Visible = rbpQuanLy.Visible = rbpNghiepVu.Visible = rbpHeThong.Visible = true;
            //tiếp tục if trên nhóm Program.mGroup để bật / tắt các nút lệnh trên menu chính
            btnDangNhap.Enabled =  false;
            btnDangXuat.Enabled = btnDangKy.Enabled = true;
            frmDangNhap fDN = new frmDangNhap();
            Form frm = this.CheckExitsts(typeof(frmDangNhap));
            if (frm != null) frm.Close();

        }
        private void btnDangNhap_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmDangNhap));
            if (frm != null)// đã mở form
                frm.Activate();
            else
            {
                frmDangNhap fDN = new frmDangNhap();//chưa mở form tạo form 
                fDN.MdiParent = this;//duyệt mình form cha
                fDN.Show();
            }
        }

        private void btnDangKy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            Form frm = this.CheckExitsts(typeof(frmDangKy));
            if (frm != null)
                frm.Activate();
            else
            {
                frmDangKy fDK = new frmDangKy();
                fDK.MdiParent = this;
                fDK.Show();
            }

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            rbpBaoCao.Visible = rbpNghiepVu.Visible = rbpQuanLy.Visible = false;
            btnDangXuat.Enabled = btnDangKy.Enabled = false;
        }

        private void btnNhanVien_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmNhanVien));
            if (frm != null)
                frm.Activate();
            else
            {
                frmNhanVien fNV = new frmNhanVien();
                fNV.MdiParent = this;
                fNV.Show();
            }
        }

        private void btnTaoTaiKhoanKH_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmTaoTaiKhoan));
            if (frm != null)
                frm.Activate();
            else
            {
                frmTaoTaiKhoan fTTK = new frmTaoTaiKhoan();
                fTTK.MdiParent = this;
                fTTK.Show();
            }
        }

        private void btnGuiRut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmGuiRut));
            if (frm != null)
                frm.Activate();
            else
            {
                frmGuiRut fGR = new frmGuiRut();
                fGR.MdiParent = this;
                fGR.Show();
            }
        }

        private void btnChuyenTien_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmChuyenTien));
            if (frm != null)
                frm.Activate();
            else
            {
                frmChuyenTien fCT = new frmChuyenTien();
                fCT.MdiParent = this;
                fCT.Show();
            }
        }

        private void btnDSKhachHang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmDSKH));
            if (frm != null)
                frm.Activate();
            else
            {
                frmDSKH fNV = new frmDSKH();
                fNV.MdiParent = this.MdiParent;
                fNV.Show();
            }
        }

        private void btnDSTaiKhoan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmDSTaiKhoan));
            if (frm != null)
                frm.Activate();
            else
            {
                frmDSTaiKhoan fDSTK = new frmDSTaiKhoan();
                fDSTK.MdiParent = this;
                fDSTK.Show();
            }
        }

        private void btnKhachHang_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmKhachHang));
            if (frm != null)
                frm.Activate();
            else
            {
                frmKhachHang fKH = new frmKhachHang();
                fKH.MdiParent = this;
                fKH.Show();
            }
        }

        

        private void btnSaoKeDG_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Form frm = this.CheckExitsts(typeof(frmSaoKeGD));
            if (frm != null)
                frm.Activate();
            else
            {
                frmSaoKeGD fSKGD = new frmSaoKeGD();
                fSKGD.MdiParent = this;
                fSKGD.Show();
            }
        }

        private void btnDangXuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn thật sự muốn Đăng Xuất", "Cảnh Báo", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                //Báo cáo
                Form frm = this.CheckExitsts(typeof(frmSaoKeGD));
                if (frm != null) frm.Close();
                frm = this.CheckExitsts(typeof(frmDSTaiKhoan));
                if (frm != null) frm.Close();
                frm = this.CheckExitsts(typeof(frmDSKH));
                if (frm != null) frm.Close();
                //Nghiệp vụ
                frm = this.CheckExitsts(typeof(frmTaoTaiKhoan));
                if (frm != null) frm.Close();
                frm = this.CheckExitsts(typeof(frmGuiRut));
                if (frm != null) frm.Close();
                frm = this.CheckExitsts(typeof(frmChuyenTien));
                if (frm != null) frm.Close();
                //Quản lý
                frm = this.CheckExitsts(typeof(frmNhanVien));
                if (frm != null) frm.Close();
                frm = this.CheckExitsts(typeof(frmKhachHang));
                if (frm != null) frm.Close();
                //Hệ thống
                frm = this.CheckExitsts(typeof(frmDangKy));
                if (frm != null) frm.Close();
                frm = this.CheckExitsts(typeof(frmDangNhap));
                if (frm != null) frm.Close();
                //ẩn hiện
                MANV.Text = HOTEN.Text = NHOM.Text = "";
                rbpQuanLy.Visible = rbpNghiepVu.Visible = rbpBaoCao.Visible = false;
                rbpHeThong.Visible = true;
                btnDangKy.Enabled = btnDangXuat.Enabled = false;
                btnDangNhap.Enabled = true;
                btnDangNhap_ItemClick(sender, e);
            }
            else
                return;
        }
    }
}
