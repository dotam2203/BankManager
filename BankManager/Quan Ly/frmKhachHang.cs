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
    public partial class frmKhachHang : DevExpress.XtraEditors.XtraForm
    {
        Int32 vitriKH = 0;
        String maCN = "";
        DateTime now = DateTime.Now;
        String CMNDXoa = "";
        Boolean themKH = false;
        String ho, ten, diaChi, gioiTinh, sdt;
        DateTime ngayCap = DateTime.Now;

        Stack<String> undoStack;
        String insertBack;
        String updateBack;
        String deleteBack;

        public frmKhachHang()
        {
            InitializeComponent();
        }

        public void checkUndoStack()
        {
            if (undoStack.Count > 0)
                btnPhucHoi.Enabled = true;
            else
                btnPhucHoi.Enabled = false;
        }
        //không cần thiết
        private void khachHangBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsKhachHang.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            //không kiểm tra ràng buộc khóa ngoại
            DS.EnforceConstraints = false;

            //bảng chứa khóa ngoại lên đầu
            this.ChiNhanhTableAdapter.Connection.ConnectionString = Program.connstr;
            this.ChiNhanhTableAdapter.Fill(this.DS.ChiNhanh);

            this.KhachHangTableAdapter.Connection.ConnectionString = Program.connstr;
            this.KhachHangTableAdapter.Fill(this.DS.KhachHang);

            // TODO: This line of code loads data into the 'DS.TaiKhoan' table. You can move, or remove it, as needed.
            this.TaiKhoanTableAdapter.Connection.ConnectionString = Program.connstr;
            this.TaiKhoanTableAdapter.Fill(this.DS.TaiKhoan);

            /*================ STACK =================*/
            //cho phép stack lưu trữ 5 phần tử  
            undoStack = new Stack<string>(5);
            //sao chép bds_dspm đã load trong form đăng nhập
            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;

            //ẩn hiện chức năng theo nhóm phân quyền;
            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;

            if (Program.mGroup.Trim() == "NganHang")
            {
                Program.bds_dspm.Filter = "TENCN <> 'SEARCH_KHACHHANG' ";
                cmbChiNhanh.Enabled = true;
                btnThem.Enabled = btnGhi.Enabled = btnPhucHoi.Enabled = btnXoa.Enabled = false;
                panelControlKH.Enabled = false;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
                btnThem.Enabled = btnXoa.Enabled = btnGhi.Enabled = btnPhucHoi.Enabled = true;
                panelControlKH.Enabled = true;
            }

            maCN = ((DataRowView)bdsChiNhanh[0])["MACN"].ToString().Trim();
            CMNDXoa = ((DataRowView)bdsKhachHang[bdsKhachHang.Position])["CMND"].ToString().Trim();

            DataRowView drv = (DataRowView)bdsKhachHang[bdsKhachHang.Position];
            ho = drv["HO"].ToString();
            ten = drv["TEN"].ToString();
            diaChi = drv["DIACHI"].ToString();
            gioiTinh = drv["PHAI"].ToString();
            sdt = drv["SODT"].ToString();
            ngayCap = Convert.ToDateTime(drv["NGAYCAP"]);
            
            txtCMND.ReadOnly = true;
            txtChiNhanh.ReadOnly = true;
            checkUndoStack();//giữ thông tin thêm để undo
        }

        private void gridControlKhachHang_Click(object sender, EventArgs e)
        {
            CMNDXoa = ((DataRowView)bdsKhachHang[bdsKhachHang.Position])["CMND"].ToString().Trim();
            btnXoa.Enabled = true;
            DataRowView drv = (DataRowView)bdsKhachHang[bdsKhachHang.Position];
            ho = drv["HO"].ToString();
            ten = drv["TEN"].ToString();
            diaChi = drv["DIACHI"].ToString();
            gioiTinh = drv["PHAI"].ToString();
            sdt = drv["SODT"].ToString();
            ngayCap = Convert.ToDateTime(drv["NGAYCAP"]);
            
        }

        private void btnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtCMND.Focus();//trỏ chuột ở vị trí Mã KH
            //bắt đầu từ vị trí 0
            vitriKH = bdsKhachHang.Position;
            panelControlKH.Enabled = true;
            themKH = true;

            bdsKhachHang.AddNew();
            
            txtChiNhanh.Text = maCN;
            cmbPhai.SelectedIndex = 1;
            cmbPhai.SelectedIndex = 0;
            txtCMND.Text = "";
            txtCMND.ReadOnly = false;
            txtCMND.Focus();
            dateEditNgayCap.EditValue = now;

            //ẩn và hiện
            btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gridControlKhachHang.Enabled = false;
        }

        private void btnGhi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (txtCMND.Text.Trim() == "")
            {
                MessageBox.Show("Chứng minh nhân dân không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtCMND.Focus();
            }
            else if (txtHo.Text.Trim() == "")
            {
                MessageBox.Show("Họ khách hàng không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtHo.Focus();
            }
            else if (txtHo.Text.Trim() == "")
            {
                MessageBox.Show("Tên khách hàng không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtHo.Focus();
            }
            else if (txtSDT.Text.Trim() == "")
            {
                MessageBox.Show("Số điện thoại không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtSDT.Focus();
            }
            else if (txtDiaChi.Text.Trim() == "")
            {
                MessageBox.Show("Địa chỉ không được bỏ trống !", "Thông báo !", MessageBoxButtons.OK);
                txtDiaChi.Focus();
            }
            else if (dateEditNgayCap.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng chọn ngày cấp!", "Thông báo !", MessageBoxButtons.OK);
                txtDiaChi.Focus();
            }
            else if (dateEditNgayCap.DateTime.CompareTo(now) == 1)
            {
                MessageBox.Show("Không được chọn quá ngày hiện tại!", "Thông báo!", MessageBoxButtons.OK);
            }
            else
            {
                if (themKH == true)
                {
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strSP = "sp_KiemTraCMND";
                    Program.Sqlcmd = Program.conn.CreateCommand();
                    Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.Sqlcmd.CommandText = strSP;
                    Program.Sqlcmd.Parameters.Add("@CMND", SqlDbType.VarChar).Value = txtCMND.Text;
                    Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    Program.Sqlcmd.ExecuteNonQuery();
                    String ret = Program.Sqlcmd.Parameters["@RET"].Value.ToString();
                    if (ret == "1")
                    {
                        MessageBox.Show("Chứng minh nhân dân đã tồn tại. Vui lòng kiểm tra lại !", "Thông báo !", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        try
                        {
                            bdsKhachHang.EndEdit();            // kết thúc quá trình hiệu chỉnh, gửi dl về dataset
                            bdsKhachHang.ResetCurrentItem();           // lấy dl của textbox control bên dưới đẩy lên gridcontrol đòng bộ 2 khu vực(ko còn ở dạng tạm nữa mà chính thức ghi vào dataset)
                            this.KhachHangTableAdapter.Update(this.DS.KhachHang);         // cập nhật dl từ dataset về database thông qua tableadapter
                            
                            themKH = false;
                            //ẩn và hiện
                            btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
                            btnGhi.Enabled = btnPhucHoi.Enabled = false;
                            gridControlKhachHang.Enabled = true;
                            panelControlKH.Enabled = false;
                            txtCMND.ReadOnly = true;
                            txtCMND.Text = txtSDT.Text = txtDiaChi.Text = dateEditNgayCap.Text = txtTen.Text = txtHo.Text = "";
                            MessageBox.Show("Lưu thành công!", "Thông báo !", MessageBoxButtons.OK);
                            insertBack = "DELETE FROM KhachHang WHERE CMND='" + txtCMND.Text + "'";
                            undoStack.Push(insertBack);//đưa vào trong stack để hỗ trợ phục hồi
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi ghi Khách Hàng. " + ex.Message, "Cảnh Báo!!", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    try
                    {
                        //Kết thúc chỉnh sửa, gửi data về dataset
                        bdsKhachHang.EndEdit();
                        //lấy dữ liệu trong panelControl gửi lên gridControl để đồng bộ (ghi vào dataset)
                        bdsKhachHang.ResetCurrentItem();
                        //cập nhật dữ liệu từ dataset về database thông qua tableAdapter
                        //this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                        //ghi trên db sau và update lại, bao gồm list thêm-xóa-sửa; nếu mình vừa thực hiện thao tác gì thì nó sẽ update thao tác đó
                        this.KhachHangTableAdapter.Update(this.DS.KhachHang);

                        themKH = false;
                        //ẩn và hiện
                        btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
                        btnGhi.Enabled = btnPhucHoi.Enabled = false;
                        gridControlKhachHang.Enabled = true;
                        panelControlKH.Enabled = false;
                        txtCMND.ReadOnly = true;
                        MessageBox.Show("Lưu thành công!", "Thông báo!", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi ghi Khách Hàng! " + ex.Message, "Cảnh báo!!", MessageBoxButtons.OK);
                    }
                }
                
            }
            checkUndoStack();
        }

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChiNhanh.SelectedValue != null)
            {//kiểm tra cmb có số liệu chưa
                if (cmbChiNhanh.SelectedValue.ToString() != "System.Data.DataRowView")
                {
                    Program.servername = cmbChiNhanh.SelectedValue.ToString();
                }
                //kiểm tra trùng với chi nhánh lúc đăng nhập ko
                if (cmbChiNhanh.SelectedIndex != Program.mChinhanh)
                {
                    //khác thì lấy tài khoản HTKN trên site phân mảnh
                    Program.mlogin = Program.remotelogin;
                    Program.password = Program.remotepassword;
                }
                else
                {
                    //trùng thì lấy site phân mảnh lúc đăng nhập
                    Program.mlogin = Program.mloginDN;
                    Program.password = Program.passwordDN;
                }
                if (Program.KetNoi() == 0)
                {
                    MessageBox.Show("Lỗi chuyển chi nhánh!", "Lỗi!", MessageBoxButtons.OK);
                    return;
                }
                else
                {
                    try
                    {
                        DS.EnforceConstraints = false;

                        //bảng chứa khóa ngoại lên đầu
                        this.ChiNhanhTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.ChiNhanhTableAdapter.Fill(this.DS.ChiNhanh);
                        
                        //sử dụng khi vừa rẽ Chi Nhánh vừa cập nhật số liệu
                        maCN = ((DataRowView)bdsChiNhanh[0])["MACN"].ToString().Trim();
                        
                        this.KhachHangTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.KhachHangTableAdapter.Fill(this.DS.KhachHang);

                        CMNDXoa = ((DataRowView)bdsKhachHang[bdsKhachHang.Position])["CMND"].ToString().Trim();
                        checkUndoStack();
                    }
                    catch (Exception) { }
                }
            }
        }

        private void btnXoa_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (bdsKhachHang.Count == 0)
                //btnXoa.Enabled = false;
                return;
            else
            {
                if (MessageBox.Show("Bạn thực sự muốn xóa Khách Hàng này?", "Cảnh báo!!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //sp_CheckExistCustomer
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strSP = "sp_CheckExistCustomer";
                    Program.Sqlcmd = Program.conn.CreateCommand();
                    Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.Sqlcmd.CommandText = strSP;
                    //@cmnd = @cmnd in sp
                    Program.Sqlcmd.Parameters.Add("@cmnd", SqlDbType.NChar).Value = txtCMND.Text;
                    //trong sp return 1: có tk or 0: chưa có tk có thể xóa
                    Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    Program.Sqlcmd.ExecuteNonQuery();
                    String ret = Program.Sqlcmd.Parameters["@Ret"].Value.ToString();
                    if (ret == "1")
                    {
                        MessageBox.Show("Khách Hàng đã đăng ký Tài Khoản ngân hàng. Xóa thất bại!", "  Thông Báo!", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        updateBack = "UPDATE KhachHang SET HO = N'" + ho + "', " +
                           "TEN = N'" + ten + "' , " +
                           "DIACHI = N'" + diaChi + "' , " +
                           "PHAI = '" + gioiTinh + "' , " +
                           "NGAYCAP = '" + ngayCap + "' , " +
                           "SODT = '" + sdt + "' , " +
                           "MACN = '" + maCN + "' " +
                           "WHERE CMND = '" + txtCMND.Text.Trim() + "';";
                        try
                        {
                            DataRowView drv = (DataRowView)bdsKhachHang[bdsKhachHang.Position];
                            ho = drv["HO"].ToString();
                            ten = drv["TEN"].ToString();
                            diaChi = drv["DIACHI"].ToString();
                            gioiTinh = drv["PHAI"].ToString();
                            ngayCap = Convert.ToDateTime(drv["NGAYCAP"].ToString());//=============SAI=================
                            sdt = drv["SODT"].ToString();
                            
                            deleteBack = "insert into NhanVien(MANV,HO,TEN,DIACHI,PHAI,NGAYCAP,SODT,MACN)";
                            deleteBack += " values('" + txtCMND.Text + "' , " + "N'" + ho + "', " +
                                "N'" + ten + "' , " +
                                "N'" + diaChi + "' , " +
                                "'" + gioiTinh + "' , " +
                                "'" + ngayCap + "' , " +
                                "'" + sdt + "' , " +
                                "'" + maCN + "' )";
                            undoStack.Push(deleteBack);//vào stack để xóa;
                            bdsKhachHang.RemoveCurrent();//xóa hàng đang chọn ra khỏi dataset
                            this.KhachHangTableAdapter.Update(this.DS.KhachHang);
                            bdsKhachHang.Position = 0;
                            MessageBox.Show("Xóa Khách Hàng thành công!", "Thông báo!", MessageBoxButtons.OK);
                            checkUndoStack();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi xóa Khách Hàng. Xóa thất bại!" + ex.Message, "Lỗi!", MessageBoxButtons.OK);
                        }

                    }
                }
                else return;
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

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                bdsKhachHang.CancelEdit();
                //reload lại dữ liệu trên cả SQL
                this.KhachHangTableAdapter.Connection.ConnectionString = Program.connstr;
                this.KhachHangTableAdapter.Fill(this.DS.KhachHang);
                if (themKH == true)
                {
                    bdsKhachHang.Position = vitriKH;
                    gridControlKhachHang.Enabled = true;
                    btnThem.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = cmbChiNhanh.Enabled = true;
                    txtCMND.ReadOnly = true;
                    themKH = false;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Reload Error" + ex.Message, "Cảnh báo!!", MessageBoxButtons.OK);
                return;
            } 
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //trả lại vị trí trước khi thêm nếu chưa Ghi
            //nên làm phục hồi lại nhiều lần và sử dụng stack
            bdsKhachHang.CancelEdit();
            if (Program.KetNoi() == 0)
                return;
            String strPH = undoStack.Pop();
            int n = Program.ExecSqlNonQuery(strPH);
            this.KhachHangTableAdapter.Fill(this.DS.KhachHang);
            checkUndoStack();
            //ẩn và hiện
            gridControlKhachHang.Enabled = true;
            panelControlKH.Enabled = false;
            btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            //đã phục hồi sẽ không cho sử dụng lại và ko cho ghi
            btnGhi.Enabled = btnPhucHoi.Enabled = false;
        }
    }
}
