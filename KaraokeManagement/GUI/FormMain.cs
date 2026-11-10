using System.Data;
using Karaoke.BUS;
using Karaoke.DTO;

namespace Karaoke.GUI
{
    /// <summary>
    /// Form chính - TOÀN BỘ control được tạo bằng code (Code-First, không kéo thả)
    /// </summary>
    public class FormMain : Form
    {
        /* ================== TẦNG BUS ================== */
        private readonly PhongHatBUS _phongHatBUS = new();
        private readonly DichVuBUS _dichVuBUS = new();
        private readonly HoaDonBUS _hoaDonBUS = new();

        /* ================== CONTROL ================== */
        private FlowLayoutPanel flpPhong = null!;
        private Panel pnlRight = null!;
        private Label lblPhongDangChon = null!;
        private Button btnMoPhong = null!;
        private Button btnThanhToan = null!;
        private ComboBox cboDichVu = null!;
        private NumericUpDown numSoLuong = null!;
        private Button btnThemDichVu = null!;
        private DataGridView dgvDichVu = null!;
        private Label lblTongTien = null!;

        /* ================== TRẠNG THÁI ================== */
        private PhongHat? _phongDangChon;
        private HoaDon? _hoaDonDangMo;

        /* ================== CONSTRUCTOR ================== */
        public FormMain()
        {
            Text = "Hệ thống quản lý đặt phòng & dịch vụ Karaoke";
            ClientSize = new Size(1200, 720);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(1200, 720);

            BuildUI();                       // Tạo toàn bộ giao diện bằng code
            Load += FormMain_Load;
        }

        /* =========================================================
           BƯỚC 1: DỰNG GIAO DIỆN BẰNG CODE
           ========================================================= */
        private void BuildUI()
        {
            /* ---------- TIÊU ĐỀ SƠ ĐỒ PHÒNG ---------- */
            var lblSoDo = new Label
            {
                Text = "SƠ ĐỒ PHÒNG",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(12, 10),
                Size = new Size(745, 26),
                ForeColor = Color.DarkBlue
            };

            /* ---------- FLOWLAYOUTPANEL CHỨA CÁC NÚT PHÒNG ---------- */
            flpPhong = new FlowLayoutPanel
            {
                Location = new Point(12, 42),
                Size = new Size(745, 666),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(8)
            };

            /* ---------- PANEL BÊN PHẢI ---------- */
            pnlRight = new Panel
            {
                Location = new Point(770, 12),
                Size = new Size(415, 696),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblTitle = new Label
            {
                Text = "THÔNG TIN PHÒNG",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(390, 24),
                ForeColor = Color.DarkBlue
            };

            lblPhongDangChon = new Label
            {
                Text = "Chưa chọn phòng",
                Location = new Point(10, 42),
                Size = new Size(390, 24),
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.DimGray
            };

            btnMoPhong = new Button
            {
                Text = "MỞ PHÒNG",
                Location = new Point(10, 72),
                Size = new Size(185, 42),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            btnMoPhong.FlatAppearance.BorderSize = 0;
            btnMoPhong.Click += BtnMoPhong_Click;

            btnThanhToan = new Button
            {
                Text = "THANH TOÁN",
                Location = new Point(210, 72),
                Size = new Size(185, 42),
                BackColor = Color.OrangeRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            btnThanhToan.FlatAppearance.BorderSize = 0;
            btnThanhToan.Click += BtnThanhToan_Click;

            var lblDichVu = new Label
            {
                Text = "Chọn dịch vụ:",
                Location = new Point(10, 128),
                Size = new Size(390, 20)
            };

            cboDichVu = new ComboBox
            {
                Location = new Point(10, 150),
                Size = new Size(250, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            numSoLuong = new NumericUpDown
            {
                Location = new Point(270, 150),
                Size = new Size(60, 28),
                Minimum = 1,
                Maximum = 999,
                Value = 1,
                Enabled = false
            };

            btnThemDichVu = new Button
            {
                Text = "THÊM DỊCH VỤ",
                Location = new Point(10, 186),
                Size = new Size(385, 34),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            btnThemDichVu.FlatAppearance.BorderSize = 0;
            btnThemDichVu.Click += BtnThemDichVu_Click;

            dgvDichVu = new DataGridView
            {
                Location = new Point(10, 230),
                Size = new Size(385, 300),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTongTien = new Label
            {
                Text = "Tạm tính dịch vụ: 0 đ",
                Location = new Point(10, 545),
                Size = new Size(385, 26),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };

            /* ---------- ADD CONTROL ---------- */
            pnlRight.Controls.AddRange(new Control[]
            {
                lblTitle, lblPhongDangChon, btnMoPhong, btnThanhToan,
                lblDichVu, cboDichVu, numSoLuong, btnThemDichVu,
                dgvDichVu, lblTongTien
            });

            Controls.AddRange(new Control[] { lblSoDo, flpPhong, pnlRight });
        }

        /* =========================================================
           FORM LOAD
           ========================================================= */
        private void FormMain_Load(object? sender, EventArgs e)
        {
            try
            {
                LoadDanhSachDichVu();
                LoadDanhSachPhong();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không kết nối được CSDL!\n{ex.Message}",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* =========================================================
           LOAD SƠ ĐỒ PHÒNG (Visual Grid động)
           ========================================================= */
        private void LoadDanhSachPhong()
        {
            var dsPhong = _phongHatBUS.LayDanhSach();

            // Đồng bộ lại đối tượng phòng đang chọn theo dữ liệu mới
            if (_phongDangChon != null)
                _phongDangChon = dsPhong.FirstOrDefault(x => x.MaPhong == _phongDangChon.MaPhong);

            flpPhong.SuspendLayout();
            flpPhong.Controls.Clear();

            foreach (var p in dsPhong)
            {
                bool dangHat = p.TrangThai != TrangThaiPhong.Trong;

                var btn = new Button
                {
                    Size = new Size(165, 105),
                    Margin = new Padding(6),
                    Tag = p,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = dangHat ? Color.Firebrick : Color.SeaGreen,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    Text = $"{p.TenPhong}\n{p.LoaiPhong}\n{p.GiaTheoGio:N0} đ/giờ\n[{p.TrangThai}]"
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += BtnPhong_Click;

                flpPhong.Controls.Add(btn);
            }

            flpPhong.ResumeLayout();

            ToMauPhongDangChon();
            CapNhatTrangThaiDieuKhien();
        }

        /* =========================================================
           LOAD COMBOBOX DỊCH VỤ
           ========================================================= */
        private void LoadDanhSachDichVu()
        {
            var dsDichVu = _dichVuBUS.LayDanhSach();

            cboDichVu.DataSource = dsDichVu;
            cboDichVu.DisplayMember = nameof(DichVu.TenDV);
            cboDichVu.ValueMember = nameof(DichVu.MaDV);
            cboDichVu.SelectedIndex = dsDichVu.Count > 0 ? 0 : -1;
        }

        /* =========================================================
           CLICK VÀO NÚT PHÒNG
           ========================================================= */
        private void BtnPhong_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag is not PhongHat p) return;

            _phongDangChon = p;
            ToMauPhongDangChon();
            CapNhatTrangThaiDieuKhien();
        }

        /* Tô viền vàng cho phòng đang chọn */
        private void ToMauPhongDangChon()
        {
            foreach (Control c in flpPhong.Controls)
            {
                if (c is not Button b || b.Tag is not PhongHat p) continue;

                bool dangChon = _phongDangChon != null && p.MaPhong == _phongDangChon.MaPhong;
                b.FlatAppearance.BorderSize = dangChon ? 4 : 0;
                b.FlatAppearance.BorderColor = Color.Gold;
            }
        }

        /* =========================================================
           CẬP NHẬT TRẠNG THÁI CÁC NÚT / LOAD CHI TIẾT DỊCH VỤ
           ========================================================= */
        private void CapNhatTrangThaiDieuKhien()
        {
            if (_phongDangChon == null)
            {
                lblPhongDangChon.Text = "Chưa chọn phòng";
                btnMoPhong.Enabled = false;
                btnThanhToan.Enabled = false;
                btnThemDichVu.Enabled = false;
                cboDichVu.Enabled = false;
                numSoLuong.Enabled = false;
                _hoaDonDangMo = null;
                dgvDichVu.DataSource = null;
                lblTongTien.Text = "Tạm tính dịch vụ: 0 đ";
                return;
            }

            var p = _phongDangChon;
            bool dangHat = p.TrangThai != TrangThaiPhong.Trong;

            lblPhongDangChon.Text =
                $"Phòng: {p.TenPhong}  |  {p.LoaiPhong}  |  {p.GiaTheoGio:N0} đ/giờ  |  {p.TrangThai}";

            btnMoPhong.Enabled = !dangHat;   // chỉ mở khi phòng trống
            btnThanhToan.Enabled = dangHat;    // chỉ thanh toán khi đang hát

            _hoaDonDangMo = dangHat ? _hoaDonBUS.LayHoaDonDangMo(p.MaPhong) : null;

            bool coHoaDon = _hoaDonDangMo != null;
            btnThemDichVu.Enabled = coHoaDon;
            cboDichVu.Enabled = coHoaDon;
            numSoLuong.Enabled = coHoaDon;

            if (coHoaDon) LoadChiTietDichVu();
            else
            {
                dgvDichVu.DataSource = null;
                lblTongTien.Text = "Tạm tính dịch vụ: 0 đ";
            }
        }

        /* =========================================================
           LOAD DATAGRIDVIEW CHI TIẾT DỊCH VỤ
           ========================================================= */
        private void LoadChiTietDichVu()
        {
            if (_hoaDonDangMo == null) return;

            DataTable dt = _hoaDonBUS.LayChiTietDichVu(_hoaDonDangMo.MaHD);
            dgvDichVu.DataSource = dt;

            if (dgvDichVu.Columns["MaDV"] != null)
                dgvDichVu.Columns["MaDV"]!.Visible = false;

            if (dgvDichVu.Columns["TenDV"] != null)
                dgvDichVu.Columns["TenDV"]!.HeaderText = "Tên dịch vụ";

            if (dgvDichVu.Columns["SoLuong"] != null)
                dgvDichVu.Columns["SoLuong"]!.HeaderText = "SL";

            if (dgvDichVu.Columns["DonGia"] != null)
            {
                dgvDichVu.Columns["DonGia"]!.HeaderText = "Đơn giá";
                dgvDichVu.Columns["DonGia"]!.DefaultCellStyle.Format = "N0";
            }

            if (dgvDichVu.Columns["ThanhTien"] != null)
            {
                dgvDichVu.Columns["ThanhTien"]!.HeaderText = "Thành tiền";
                dgvDichVu.Columns["ThanhTien"]!.DefaultCellStyle.Format = "N0";
            }

            decimal tong = 0;
            foreach (DataRow row in dt.Rows)
                tong += Convert.ToDecimal(row["ThanhTien"]);

            lblTongTien.Text = $"Tạm tính dịch vụ: {tong:N0} đ";
        }

        /* =========================================================
           SỰ KIỆN: MỞ PHÒNG
           ========================================================= */
        private void BtnMoPhong_Click(object? sender, EventArgs e)
        {
            if (_phongDangChon == null) return;

            try
            {
                _hoaDonBUS.MoPhong(_phongDangChon);

                MessageBox.Show($"Đã mở phòng {_phongDangChon.TenPhong} lúc {DateTime.Now:HH:mm:ss}.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDanhSachPhong();     // load lại sơ đồ
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi mở phòng",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* =========================================================
           SỰ KIỆN: THÊM DỊCH VỤ
           ========================================================= */
        private void BtnThemDichVu_Click(object? sender, EventArgs e)
        {
            if (_hoaDonDangMo == null)
            {
                MessageBox.Show("Phòng chưa được mở!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboDichVu.SelectedItem is not DichVu dv)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int soLuong = (int)numSoLuong.Value;
                _hoaDonBUS.ThemDichVu(_hoaDonDangMo.MaHD, dv, soLuong);

                LoadChiTietDichVu();     // load lại DataGridView
                numSoLuong.Value = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thêm dịch vụ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /* =========================================================
           SỰ KIỆN: THANH TOÁN
           ========================================================= */
        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            if (_hoaDonDangMo == null || _phongDangChon == null) return;

            var confirm = MessageBox.Show(
                $"Xác nhận thanh toán phòng {_phongDangChon.TenPhong}?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                KetQuaThanhToan kq = _hoaDonBUS.ThanhToan(_hoaDonDangMo, _phongDangChon);

                MessageBox.Show(
                    $"===== HÓA ĐƠN THANH TOÁN =====\n\n" +
                    $"Phòng         : {_phongDangChon.TenPhong}\n" +
                    $"Giờ vào       : {kq.GioVao:dd/MM/yyyy HH:mm:ss}\n" +
                    $"Giờ ra        : {kq.GioRa:dd/MM/yyyy HH:mm:ss}\n" +
                    $"Số giờ tính   : {kq.SoGio} giờ\n" +
                    $"--------------------------------\n" +
                    $"Tiền phòng    : {kq.TienPhong:N0} đ\n" +
                    $"Tiền dịch vụ  : {kq.TienDichVu:N0} đ\n" +
                    $"--------------------------------\n" +
                    $"TỔNG CỘNG     : {kq.TongTien:N0} đ",
                    "Thanh toán thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDanhSachPhong();     // load lại sơ đồ (phòng về trạng thái Trống)
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi thanh toán",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}