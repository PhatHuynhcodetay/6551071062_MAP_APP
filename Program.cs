using HotelManagement.DangNhap;

namespace HotelManagement
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args.Length > 0 && args[0] == "--test")
            {
                RunSelfTests();
                return;
            }

            // Hiện form đăng nhập trước; đăng nhập thành công (DialogResult.OK) mới mở form chính
            using (var formDangNhap = new FormDangNhap())
            {
                if (formDangNhap.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new FormMain());
        }

        static void RunSelfTests()
        {
            Console.WriteLine("=== BẮT ĐẦU KIỂM THỬ TỰ ĐỘNG ===");

            // 1. Kiểm thử kết nối CSDL và đăng nhập
            Console.WriteLine("[1/6] Kiểm thử kết nối CSDL & Xác thực tài khoản admin/1234...");
            using (var db = new Models.HotelManagementContext())
            {
                var tk = db.TaiKhoans
                    .Where(t => t.TenTk == "admin" && t.DaXoa != true)
                    .Select(t => new { t.Password, t.MaNv, t.CapDoQuyen, TenNv = t.MaNvNavigation.TenNv })
                    .FirstOrDefault();

                if (tk == null || tk.Password != "1234")
                {
                    Console.WriteLine("THẤT BẠI: Không tìm thấy tài khoản admin hoặc sai mật khẩu!");
                    Environment.Exit(1);
                }
                FormDangNhap.MaNV_DangNhap = tk.MaNv;
                FormDangNhap.TenNV_DangNhap = tk.TenNv;
                FormDangNhap.CapDoQuyen_DangNhap = tk.CapDoQuyen;
                Console.WriteLine($"   Thành công: Đăng nhập với {FormDangNhap.TenNV_DangNhap} (Mã NV: {FormDangNhap.MaNV_DangNhap})");
            }

            // 2. Kiểm thử FormTrangChu & FormMain
            Console.WriteLine("[2/6] Kiểm thử FormTrangChu & FormMain...");
            using (var fHome = new FormTrangChu())
            {
                if (fHome.Controls.Count == 0) throw new Exception("FormTrangChu không có control nào");
            }
            using (var fMain = new FormMain())
            {
                var lbl = fMain.Controls.Find("lblTenNguoiDung", true).FirstOrDefault() as Label;
                Console.WriteLine($"   FormMain hiển thị người dùng: '{lbl?.Text}'");
            }

            // 3. Kiểm thử FormSoDoPhong
            Console.WriteLine("[3/6] Kiểm thử FormSoDoPhong...");
            using (var fSoDo = new SoDoPhong.FormSoDoPhong())
            {
                fSoDo.Show();
                var dgv = fSoDo.Controls.Find("dataGridViewPhong", true).FirstOrDefault() as DataGridView;
                Console.WriteLine($"   FormSoDoPhong nạp được {dgv?.Rows.Count} phòng");
                fSoDo.Close();
            }

            // 4. Kiểm thử FormDanhSachPhong, FormThemPhong, FormSuaPhong
            Console.WriteLine("[4/6] Kiểm thử quản lý phòng (Danh sách, Thêm, Sửa)...");
            using (var fDsPhong = new Phong.FormDanhSachPhong())
            {
                fDsPhong.Show();
                var dgv = fDsPhong.Controls.Find("dtgDanhSachPhong", true).FirstOrDefault() as DataGridView;
                Console.WriteLine($"   FormDanhSachPhong nạp được {dgv?.Rows.Count} phòng");
                fDsPhong.Close();
            }
            using (var fThemPhong = new Phong.FormThemPhong())
            {
                fThemPhong.Show();
                fThemPhong.Close();
            }
            using (var fSuaPhong = new Phong.FormSuaPhong("P101"))
            {
                fSuaPhong.Show();
                fSuaPhong.Close();
            }

            // 5. Kiểm thử FormDanhSachLoaiPhong, FormThemLoaiPhong, FormSuaLoaiPhong
            Console.WriteLine("[5/6] Kiểm thử quản lý loại phòng (Danh sách, Thêm, Sửa)...");
            using (var fDsLP = new LoaiPhong.FormDanhSachLoaiPhong())
            {
                fDsLP.Show();
                var dgv = fDsLP.Controls.Find("dtgDanhSachLoaiPhong", true).FirstOrDefault() as DataGridView;
                Console.WriteLine($"   FormDanhSachLoaiPhong nạp được {dgv?.Rows.Count} loại phòng");
                fDsLP.Close();
            }
            using (var fThemLP = new LoaiPhong.FormThemLoaiPhong())
            {
                fThemLP.Show();
                fThemLP.Close();
            }
            using (var fSuaLP = new LoaiPhong.FormSuaLoaiPhong("NOR01"))
            {
                fSuaLP.Show();
                fSuaLP.Close();
            }

            // 6. Kiểm thử nhúng Resources
            Console.WriteLine("[6/6] Kiểm thử truy cập Properties.Resources...");
            if (Properties.Resources.pictureIcon == null) throw new Exception("pictureIcon is null");
            if (Properties.Resources.MenuQuanLyTaiKhoan == null) throw new Exception("MenuQuanLyTaiKhoan is null");
            if (Properties.Resources.search == null) throw new Exception("search is null");
            if (Properties.Resources.Phong == null) throw new Exception("Phong is null");
            if (Properties.Resources.edit == null) throw new Exception("edit is null");
            if (Properties.Resources.delete == null) throw new Exception("delete is null");
            if (Properties.Resources.Giuong == null) throw new Exception("Giuong is null");
            Console.WriteLine("   Tất cả tài nguyên hình ảnh được nạp chính xác!");

            Console.WriteLine("=== TOÀN BỘ KIỂM THỬ THÀNH CÔNG 100%! ===");
        }
    }
}
