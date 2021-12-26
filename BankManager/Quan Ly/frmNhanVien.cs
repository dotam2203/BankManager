using DevExpress.XtraBars;
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
    public partial class frmNhanVien : DevExpress.XtraEditors.XtraForm
    {
        Int32 viTri = 0;
        String maCN = "";
        Boolean themNV = false;
        String maNVXoa = "";

        Stack<String> undoStack;
        String insertBack;
        String updateBack;
        String deleteBack;
        String ho, ten, diaChi, gioiTinh, sdt;
        int trangThaiXoa = 0;
        public frmNhanVien()
        {
            InitializeComponent();
        }

        public void checkUndoStack()
        {
            //trả về số lượng phần tử trong stack > 0
            if (undoStack.Count > 0)
                btnPhucHoi.Enabled = true;
            else
                btnPhucHoi.Enabled = false;
        }
        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            //không kiểm tra ràng buộc khóa ngoại
            DS.EnforceConstraints = false;

            //bảng chứa khóa ngoại lên đầu
            this.ChiNhanhTableAdapter.Connection.ConnectionString = Program.connstr;
            this.ChiNhanhTableAdapter.Fill(this.DS.ChiNhanh);

            //khi đổi mật khẩu nó vẫn lấy đúng tên user and pass
            this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
            this.NhanVienTableAdapter.Fill(this.DS.NhanVien);

            //kết nối với các bảng chứa khóa ngoại
            this.GOIRUTTableAdapter.Connection.ConnectionString = Program.connstr;
            this.GOIRUTTableAdapter.Fill(this.DS.GD_GOIRUT);

            //kết nối với các bảng chứa khóa ngoại
            this.CHUYENTIENTableAdapter.Connection.ConnectionString = Program.connstr;
            this.CHUYENTIENTableAdapter.Fill(this.DS.GD_CHUYENTIEN);


            /*================ STACK =================*/
            //cho phép stack lưu trữ 5 phần tử kiểu string  
            undoStack = new Stack<string>(5);

            //sao chép bds_dspm đã load trong form đăng nhập
            cmbChiNhanh.DataSource = Program.bds_dspm;
            cmbChiNhanh.DisplayMember = "TENCN";
            cmbChiNhanh.ValueMember = "TENSERVER";
            cmbChiNhanh.SelectedIndex = Program.mChinhanh;

            //ẩn hiện chức năng theo nhóm phân quyền;
            if (Program.mGroup.Trim() == "NganHang")
            {
                Program.bds_dspm.Filter = "TENCN <> 'SEARCH_KHACHHANG'";
                cmbChiNhanh.Enabled = true;
                btnThem.Enabled =  btnXoa.Enabled = btnGhi.Enabled = btnPhucHoi.Enabled = false;
                panelControlNV.Enabled = false;
            }
            else
            {
                cmbChiNhanh.Enabled = false;
                btnThem.Enabled =  btnXoa.Enabled = btnGhi.Enabled = btnPhucHoi.Enabled = true;
                panelControlNV.Enabled = true;
            }
            //sẽ phát sinh lỗi, tự fix
            maCN = ((DataRowView)bdsChiNhanh[0])["MACN"].ToString().Trim();
            maNVXoa = ((DataRowView)bdsNhanVien[bdsNhanVien.Position])["MANV"].ToString().Trim();
            if (maNVXoa == Program.username)//nếu mã nv cần xóa trống thì fail
            {
                btnXoa.Enabled = false;
            }
            DataRowView drv = (DataRowView)bdsNhanVien[bdsNhanVien.Position];
            ho = drv["HO"].ToString();
            ten = drv["TEN"].ToString();
            diaChi = drv["DIACHI"].ToString();
            gioiTinh = drv["PHAI"].ToString();
            sdt = drv["SODT"].ToString();
            trangThaiXoa = (int)drv["TrangThaiXoa"];
            txtMaNV.ReadOnly = true;
            txtChiNhanh.ReadOnly = true;
            checkUndoStack();//giữ thông tin thêm để undo
        }

        private void gridControlNhanVien_Click(object sender, EventArgs e)
        {
            maNVXoa = ((DataRowView)bdsNhanVien[bdsNhanVien.Position])["MANV"].ToString().Trim();
            if (maNVXoa == Program.username)
                btnXoa.Enabled = false;
            else
                btnXoa.Enabled = true;
            DataRowView drv = (DataRowView)bdsNhanVien[bdsNhanVien.Position];
            ho = drv["HO"].ToString();
            ten = drv["TEN"].ToString();
            diaChi = drv["DIACHI"].ToString();
            gioiTinh = drv["PHAI"].ToString();
            sdt = drv["SODT"].ToString();
            trangThaiXoa = (int)drv["TrangThaiXoa"];
        }

        private void btnThoat_ItemClick(object sender, ItemClickEventArgs e)
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

        private void btnThem_ItemClick(object sender, ItemClickEventArgs e)
        {
            txtMaNV.Focus();//trỏ chuột ở vị trí Mã NV
            //bắt đầu từ vị trí 0
            viTri = bdsNhanVien.Position;
            panelControlNV.Enabled = true;
            themNV = true;

            bdsNhanVien.AddNew();

            txtChiNhanh.Text = maCN;
            cbTTXoa.Checked = false;
            cmbPhai.SelectedIndex = 1;//nam
            cmbPhai.SelectedIndex = 0;//nữ
            txtMaNV.ReadOnly = false;
            txtMaNV.Focus();

            //ẩn và hiện
            btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = false;
            btnGhi.Enabled = btnPhucHoi.Enabled = true;
            gridControlNhanVien.Enabled = cbTTXoa.Enabled = false;

        }

        private void btnPhucHoi_ItemClick(object sender, ItemClickEventArgs e)
        {
            //trả lại vị trí trước khi thêm nếu chưa Ghi
            //nên làm phục hồi lại nhiều lần và sử dụng stack
            bdsNhanVien.CancelEdit();
            if (Program.KetNoi() == 0)
                return;
            String strPH = undoStack.Pop();//lấy về từ ngăn xếp
            int n = Program.ExecSqlNonQuery(strPH);
            this.NhanVienTableAdapter.Fill(this.DS.NhanVien);
            checkUndoStack();
            //ẩn và hiện
            gridControlNhanVien.Enabled = true;
            panelControlNV.Enabled = false;
            btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
            //đã phục hồi sẽ không cho sử dụng lại và ko cho ghi
            btnGhi.Enabled = btnPhucHoi.Enabled = false;
        }

        private void cmbChiNhanh_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChiNhanh.SelectedValue != null)
            {
                //kiểm tra cmb có số liệu chưa
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

                        //chỉ nhân viên nào thuộc nhóm Ngân Hàng mới rẽ chi nhánh
                        //sử dụng khi vừa rẽ Chi Nhánh vừa cập nhật số liệu
                        maCN = ((DataRowView)bdsChiNhanh[0])["MACN"].ToString().Trim();

                        //khi đổi mật khẩu nó vẫn lấy đúng tên user and pass
                        this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                        this.NhanVienTableAdapter.Fill(this.DS.NhanVien);

                        maNVXoa = ((DataRowView)bdsNhanVien[bdsNhanVien.Position])["MANV"].ToString().Trim();
                        if (maNVXoa == Program.username)
                            cbTTXoa.Enabled = false;
                        checkUndoStack();
                    }
                    catch (Exception) { }

                }
            }
        }

        //Không cần thiết
        private void nhanVienBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.bdsNhanVien.EndEdit();
            this.tableAdapterManager.UpdateAll(this.DS);
        }

        private void btnReload_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                bdsNhanVien.CancelEdit();
                //reload lại dữ liệu trên cả SQL
                this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                this.NhanVienTableAdapter.Fill(this.DS.NhanVien);
                if (themNV == true)
                {
                    
                    //nhân viên mới thêm cho lên đầu danh sách
                    bdsNhanVien.Position = viTri;
                    gridControlNhanVien.Enabled = true;
                    btnThem.Enabled = btnXoa.Enabled = btnPhucHoi.Enabled = btnThoat.Enabled = cmbChiNhanh.Enabled = true;
                    txtMaNV.ReadOnly = true;
                    themNV = false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Reload Error" + ex.Message, "Cảnh báo!!", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnXoa_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (bdsNhanVien.Count == 0)
                //btnXoa.Enabled = false;
                return;
            if (bdsGoiRut.Count > 0 || bdsChuyenTien.Count > 0)
            {
                MessageBox.Show("Nhân Viên đã thực hiện giao dịch. Không thể xóa!", "Message", MessageBoxButtons.OK);
            }
            else if (bdsNhanVien.Count != 0)
            {
                if (MessageBox.Show("Bạn thực sự muốn xóa Nhân Viên này?", "Cảnh báo!!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    //sp_KiemTraTrangThaiXoaNhanVien
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strSP = "sp_KiemTraTrangThaiXoaNhanVien";
                    Program.Sqlcmd = Program.conn.CreateCommand();
                    Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.Sqlcmd.CommandText = strSP;
                    //@MANV = @manv in sp
                    Program.Sqlcmd.Parameters.Add("@MANV", SqlDbType.VarChar).Value = txtMaNV.Text;
                    //trong sp return 1:có tk or 0:chưa có tk có thể xóa
                    Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    Program.Sqlcmd.ExecuteNonQuery();
                    String ret = Program.Sqlcmd.Parameters["@Ret"].Value.ToString();
                    if (ret == "1")
                    {
                        if (MessageBox.Show("Nhân Viên này đã có Tài Khoản, bạn chắc chắn muốn xóa?\nThao tác này không thể hoàn tác!!", "Cảnh báo!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            try
                            {
                                if (Program.conn.State == ConnectionState.Closed)
                                    Program.conn.Open();
                                String strSP1 = "SP_XoaLogin";
                                Program.Sqlcmd = Program.conn.CreateCommand();
                                Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                                Program.Sqlcmd.CommandText = strSP1;
                                Program.Sqlcmd.Parameters.Add("@TENUSER", SqlDbType.VarChar).Value = txtMaNV.Text.Trim();
                                Program.Sqlcmd.Parameters.Add("@GROUPNAME", SqlDbType.VarChar).Value = Program.mGroup;
                                Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                                Program.Sqlcmd.ExecuteNonQuery();
                                String ret1 = Program.Sqlcmd.Parameters["@Ret"].Value.ToString();
                                if (ret1 == "0")
                                {
                                    MessageBox.Show("Bạn không có quyền này!", "Cảnh báo!", MessageBoxButtons.OK);
                                    return;
                                }
                                else
                                {

                                    //Kết thúc chỉnh sửa, gửi data về dataset
                                    bdsNhanVien.EndEdit();
                                    //lấy dữ liệu trong panelControl gửi lên gridControl để đồng bộ (ghi vào dataset)
                                    bdsNhanVien.ResetCurrentItem();
                                    //cập nhật dữ liệu từ dataset về database thông qua tableAdapter
                                    //this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                                    //ghi trên db sau và update lại, bao gồm list thêm-xóa-sửa; nếu mình vừa thực hiện thao tác gì thì nó sẽ update thao tác đó
                                    this.NhanVienTableAdapter.Update(this.DS.NhanVien);
                                    MessageBox.Show("Xóa Nhân Viên thành công!", "Thông báo !", MessageBoxButtons.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi xóa Nhân Viên. Xóa thất bại!" + ex.Message, "Lỗi!", MessageBoxButtons.OK);
                            }
                        }
                        else return;
                    }
                    else
                    {
                        updateBack = "UPDATE NhanVien SET HO = N'" + ho + "', " +
                           "TEN = N'" + ten + "' , " +
                           "DIACHI = N'" + diaChi + "' , " +
                           "PHAI = '" + gioiTinh + "' , " +
                           "SODT = '" + sdt + "' , " +
                           "MACN = '" + maCN + "' , " +
                           "TrangThaiXoa = '" + trangThaiXoa + "' " +
                           "WHERE MANV = N'" + txtMaNV.Text.Trim() + "';";
                        try
                        {
                            DataRowView drv = (DataRowView)bdsNhanVien[bdsNhanVien.Position];
                            ho = drv["HO"].ToString();
                            ten = drv["TEN"].ToString();
                            diaChi = drv["DIACHI"].ToString();
                            gioiTinh = drv["PHAI"].ToString();
                            sdt = drv["SODT"].ToString();
                            trangThaiXoa = (int)drv["TrangThaiXoa"];
                            deleteBack = "insert into NhanVien(MANV,HO,TEN,DIACHI,PHAI,SODT,MACN,TrangThaiXoa)";
                            deleteBack += " values('" + txtMaNV.Text + "' , " + "N'" + ho + "', " +
                                "N'" + ten + "' , " +
                                "N'" + diaChi + "' , " +
                                "'" + gioiTinh + "' , " +
                                "'" + sdt + "' , " +
                                 "'" + maCN + "' , " +
                                "'" + trangThaiXoa + "' )";
                            undoStack.Push(deleteBack);//vào stack để xóa;
                            bdsNhanVien.RemoveCurrent();//xóa hàng đang chọn ra khỏi dataset
                            this.NhanVienTableAdapter.Update(this.DS.NhanVien);
                            bdsNhanVien.Position = 0;
                            MessageBox.Show("Xóa Nhân Viên thành công!", "Thông báo!", MessageBoxButtons.OK);
                            checkUndoStack();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi xóa Nhân Viên. Xóa thất bại!" + ex.Message, "Lỗi!", MessageBoxButtons.OK);
                        }

                    }
                }
                else
                    return;
            }
        }

        private void btnGhi_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (txtMaNV.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Mã Nhân Viên!", "Thông báo!", MessageBoxButtons.OK);
                txtMaNV.Focus();//trả về vị trí text để nhập lại
                return;
            }
            else if (txtHo.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Họ!", "Thông báo!", MessageBoxButtons.OK);
                txtHo.Focus();//trả về vị trí text để nhập lại
                return;
            }
            else if (txtTen.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Tên!", "Thông báo!", MessageBoxButtons.OK);
                txtTen.Focus();//trả về vị trí text để nhập lại
                return;
            }
            else if (txtDiaChi.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Địa Chỉ!", "Thông báo!", MessageBoxButtons.OK);
                txtDiaChi.Focus();//trả về vị trí text để nhập lại
                return;
            }
            else if (txtSDT.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Số Điện Thoại!", "Thông báo!", MessageBoxButtons.OK);
                txtSDT.Focus();//trả về vị trí text để nhập lại
                return;
            }
            /*else if (!Program.checkInputSDT.IsMatch(txtSDT.Text.Trim()))
            {
                MessageBox.Show("Số Điện Thoại không đúng định dạng.Vui lòng nhập lại!", "Thông báo !", MessageBoxButtons.OK);
                txtSDT.Focus();
            }*/
            else
            {
                if (themNV == true)
                {
                    if (Program.conn.State == ConnectionState.Closed)
                        Program.conn.Open();
                    String strSP = "sp_KiemTraMaNV";
                    Program.Sqlcmd = Program.conn.CreateCommand();
                    Program.Sqlcmd.CommandType = CommandType.StoredProcedure;
                    Program.Sqlcmd.CommandText = strSP;
                    //@MANV = @manv in sp
                    Program.Sqlcmd.Parameters.Add("@MANV", SqlDbType.VarChar).Value = txtMaNV.Text;
                    //trong sp return 1 or 0
                    Program.Sqlcmd.Parameters.Add("@Ret", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                    Program.Sqlcmd.ExecuteNonQuery();

                    String ret = Program.Sqlcmd.Parameters["@RET"].Value.ToString();
                    if (ret == "1")
                    {
                        MessageBox.Show("Mã Nhân Viên đã tồn tại. Vui lòng kiểm tra lại!", "Cảnh báo!", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        try
                        {
                            //Kết thúc chỉnh sửa, gửi data về dataset
                            bdsNhanVien.EndEdit();
                            //lấy dữ liệu trong panelControl gửi lên gridControl để đồng bộ (ghi vào dataset)
                            bdsNhanVien.ResetCurrentItem();
                            //cập nhật dữ liệu từ dataset về database thông qua tableAdapter
                            //this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                            //ghi trên db sau và update lại, bao gồm list thêm-xóa-sửa; nếu mình vừa thực hiện thao tác gì thì nó sẽ update thao tác đó
                            this.NhanVienTableAdapter.Update(this.DS.NhanVien);

                            themNV = false;
                            //ẩn và hiện
                            btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
                            btnGhi.Enabled = btnPhucHoi.Enabled = false;
                            gridControlNhanVien.Enabled = true;
                            panelControlNV.Enabled = false;
                            txtMaNV.ReadOnly = true;
                            MessageBox.Show("Lưu thành công!", "Thông báo!", MessageBoxButtons.OK);
                            insertBack = "DELETE FROM NhanVien WHERE MANV='" + txtMaNV.Text + "'";
                            undoStack.Push(insertBack);//đưa vào trong stack để hỗ trợ phục hồi
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi ghi Nhân Viên! " + ex.Message, "Cảnh báo!!", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    try
                    {
                        //Kết thúc chỉnh sửa, gửi data về dataset
                        bdsNhanVien.EndEdit();
                        //lấy dữ liệu trong panelControl gửi lên gridControl để đồng bộ (ghi vào dataset)
                        bdsNhanVien.ResetCurrentItem();
                        //cập nhật dữ liệu từ dataset về database thông qua tableAdapter
                        //this.NhanVienTableAdapter.Connection.ConnectionString = Program.connstr;
                        //ghi trên db sau và update lại, bao gồm list thêm-xóa-sửa; nếu mình vừa thực hiện thao tác gì thì nó sẽ update thao tác đó
                        this.NhanVienTableAdapter.Update(this.DS.NhanVien);

                        themNV = false;
                        //ẩn và hiện
                        btnThem.Enabled = btnXoa.Enabled = btnReload.Enabled = btnThoat.Enabled = true;
                        btnGhi.Enabled = btnPhucHoi.Enabled = false;
                        gridControlNhanVien.Enabled = true;
                        panelControlNV.Enabled = false;
                        txtMaNV.ReadOnly = true;
                        MessageBox.Show("Lưu thành công!", "Thông báo!", MessageBoxButtons.OK);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi ghi Nhân Viên! " + ex.Message, "Cảnh báo!!", MessageBoxButtons.OK);
                    }
                }
            }
            checkUndoStack();
        }

    }
}