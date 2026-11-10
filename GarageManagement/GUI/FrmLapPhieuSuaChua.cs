using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GarageManagement.BUS;
using GarageManagement.DTO;

namespace GarageManagement.GUI
{
    // Lớp phụ trợ hiển thị trên DataGridView
    public class ChiTietItem
    {
        public int MaPhuTung { get; set; }
        public string TenPhuTung { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuong { get; set; }
        public decimal TienCong { get; set; }
        public decimal ThanhTien => SoLuong * DonGia + TienCong;
    }

    public class FrmLapPhieuSuaChua : Form
    {
        private ComboBox cboXe;
        private TextBox txtNoiDung;
        private DataGridView dgvPhuTung, dgvChiTiet;
        private NumericUpDown numSoLuong, numTienCong;
        private Button btnThem, btnXoaDong, btnLuu, btnThanhToan, btnDong;
        private Label lblTongTien;

        private readonly XeBUS xeBUS = new XeBUS();
        private readonly PhuTungBUS phuTungBUS = new PhuTungBUS();
        private readonly PhieuSuaChuaBUS phieuBUS = new PhieuSuaChuaBUS();

        private readonly BindingList<ChiTietItem> chiTietList = new BindingList<ChiTietItem>();
        private int currentMaPhieu = 0;

        public FrmLapPhieuSuaChua()
        {
            Text = "Lập phiếu sửa chữa";
            Width = 1180; Height = 720;
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            // ================= TOP =================
            Controls.Add(new Label { Text = "Chọn xe (Biển số):", Location = new Point(20, 25), AutoSize = true });
            cboXe = new ComboBox
            {
                Location = new Point(160, 22),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            Controls.Add(cboXe);

            Controls.Add(new Label { Text = "Nội dung sửa chữa:", Location = new Point(20, 68), AutoSize = true });
            txtNoiDung = new TextBox
            {
                Location = new Point(160, 65),
                Width = 990,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(txtNoiDung);

            // ================= LEFT: Phụ tùng có sẵn =================
            var lblPT = new Label
            {
                Text = "DANH SÁCH PHỤ TÙNG",
                Location = new Point(20, 115),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };
            Controls.Add(lblPT);

            dgvPhuTung = new DataGridView
            {
                Location = new Point(20, 140),
                Size = new Size(540, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            Controls.Add(dgvPhuTung);

            Controls.Add(new Label { Text = "Số lượng:", Location = new Point(20, 440), AutoSize = true });
            numSoLuong = new NumericUpDown
            {
                Location = new Point(110, 436),
                Width = 90,
                Minimum = 1,
                Maximum = 1000,
                Value = 1
            };
            Controls.Add(numSoLuong);

            Controls.Add(new Label { Text = "Tiền công:", Location = new Point(220, 440), AutoSize = true });
            numTienCong = new NumericUpDown
            {
                Location = new Point(310, 436),
                Width = 150,
                Minimum = 0,
                Maximum = 100_000_000,
                Increment = 50_000
            };
            Controls.Add(numTienCong);

            btnThem = new Button
            {
                Text = "Thêm vào phiếu",
                Location = new Point(20, 480),
                Size = new Size(160, 40),
                BackColor = Color.LightBlue
            };
            btnThem.Click += BtnThem_Click;
            Controls.Add(btnThem);

            // ================= RIGHT: Chi tiết =================
            var lblCT = new Label
            {
                Text = "CHI TIẾT PHỤ TÙNG THAY THẾ",
                Location = new Point(590, 115),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true
            };
            Controls.Add(lblCT);

            dgvChiTiet = new DataGridView
            {
                Location = new Point(590, 140),
                Size = new Size(560, 280),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = chiTietList
            };
            dgvChiTiet.DataBindingComplete += (s, e) =>
            {
                if (dgvChiTiet.Columns["MaPhuTung"] != null)
                    dgvChiTiet.Columns["MaPhuTung"].Visible = false;
                foreach (var name in new[] { "DonGia", "TienCong", "ThanhTien" })
                    if (dgvChiTiet.Columns[name] != null)
                        dgvChiTiet.Columns[name].DefaultCellStyle.Format = "N0";
            };
            Controls.Add(dgvChiTiet);

            btnXoaDong = new Button
            {
                Text = "Xóa dòng đã chọn",
                Location = new Point(590, 440),
                Size = new Size(180, 38),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnXoaDong.Click += BtnXoaDong_Click;
            Controls.Add(btnXoaDong);

            lblTongTien = new Label
            {
                Text = "Tổng tiền: 0 VNĐ",
                Location = new Point(790, 448),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.DarkRed
            };
            Controls.Add(lblTongTien);

            // ================= BOTTOM =================
            btnLuu = new Button
            {
                Text = "Lưu phiếu (Transaction)",
                Location = new Point(590, 500),
                Size = new Size(200, 48),
                BackColor = Color.LightGreen,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnLuu.Click += BtnLuu_Click;
            Controls.Add(btnLuu);

            btnThanhToan = new Button
            {
                Text = "Thanh toán",
                Location = new Point(800, 500),
                Size = new Size(160, 48),
                BackColor = Color.Khaki,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnThanhToan.Click += BtnThanhToan_Click;
            Controls.Add(btnThanhToan);

            btnDong = new Button
            {
                Text = "Đóng",
                Location = new Point(970, 500),
                Size = new Size(180, 48),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnDong.Click += (s, e) => Close();
            Controls.Add(btnDong);
        }

        private void LoadData()
        {
            var dsXe = xeBUS.GetAll();
            cboXe.DataSource = dsXe;
            cboXe.DisplayMember = "BienSo";
            cboXe.ValueMember = "BienSo";

            dgvPhuTung.DataSource = phuTungBUS.GetAll();
            RefreshChiTietUI();
        }

        private void RefreshChiTietUI()
        {
            dgvChiTiet.DataSource = null;
            dgvChiTiet.DataSource = chiTietList;
            decimal tong = chiTietList.Sum(x => x.ThanhTien);
            lblTongTien.Text = $"Tổng tiền: {tong:N0} VNĐ";
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            if (dgvPhuTung.CurrentRow?.DataBoundItem is not PhuTungDTO pt)
            {
                MessageBox.Show("Vui lòng chọn phụ tùng từ danh sách.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int sl = (int)numSoLuong.Value;
            decimal tienCong = numTienCong.Value;
            if (sl <= 0)
            {
                MessageBox.Show("Số lượng phải > 0.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var existing = chiTietList.FirstOrDefault(x => x.MaPhuTung == pt.MaPhuTung);
            if (existing != null)
            {
                existing.SoLuong += sl;
                existing.TienCong = tienCong;
            }
            else
            {
                chiTietList.Add(new ChiTietItem
                {
                    MaPhuTung = pt.MaPhuTung,
                    TenPhuTung = pt.TenPhuTung,
                    DonGia = pt.DonGia,
                    SoLuong = sl,
                    TienCong = tienCong
                });
            }
            RefreshChiTietUI();
        }

        private void BtnXoaDong_Click(object sender, EventArgs e)
        {
            if (dgvChiTiet.CurrentRow?.DataBoundItem is ChiTietItem item)
            {
                chiTietList.Remove(item);
                RefreshChiTietUI();
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (cboXe.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn xe.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (chiTietList.Count == 0)
            {
                MessageBox.Show("Vui lòng thêm ít nhất 1 phụ tùng.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            {
                MessageBox.Show("Vui lòng nhập nội dung sửa chữa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var phieu = new PhieuSuaChuaDTO
                {
                    BienSo = cboXe.SelectedValue.ToString(),
                    NoiDungSua = txtNoiDung.Text.Trim(),
                    TrangThai = "Chưa thanh toán"
                };
                var chiTiet = chiTietList.Select(x => new ChiTietSuaChuaDTO
                {
                    MaPhuTung = x.MaPhuTung,
                    SoLuong = x.SoLuong,
                    TienCong = x.TienCong
                }).ToList();

                int maPhieu = phieuBUS.LapPhieuSuaChua(phieu, chiTiet);
                currentMaPhieu = maPhieu;

                MessageBox.Show($"Lập phiếu thành công! Mã phiếu: {maPhieu}",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                chiTietList.Clear();
                RefreshChiTietUI();
                txtNoiDung.Clear();
                dgvPhuTung.DataSource = phuTungBUS.GetAll();   // Cập nhật tồn kho mới
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lập phiếu thất bại (đã rollback):\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnThanhToan_Click(object sender, EventArgs e)
        {
            if (currentMaPhieu == 0)
            {
                MessageBox.Show("Chưa có phiếu để thanh toán. Vui lòng lưu phiếu trước.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                decimal tong = phieuBUS.ThanhToan(currentMaPhieu);
                var chiTiet = phieuBUS.GetChiTietHienThi(currentMaPhieu);
                var phieu = phieuBUS.GetById(currentMaPhieu);

                var sb = new StringBuilder();
                sb.AppendLine("========== HÓA ĐƠN THANH TOÁN ==========");
                sb.AppendLine($"Mã phiếu : {phieu.MaPhieu}");
                sb.AppendLine($"Biển số  : {phieu.BienSo}");
                sb.AppendLine($"Ngày nhận: {phieu.NgayNhan:dd/MM/yyyy HH:mm}");
                sb.AppendLine($"Nội dung : {phieu.NoiDungSua}");
                sb.AppendLine("--------------------------------------");
                sb.AppendLine("Chi tiết phụ tùng:");
                foreach (DataRow row in chiTiet.Rows)
                {
                    sb.AppendLine(
                        $" - {row["TenPhuTung"]}: " +
                        $"SL {row["SoLuong"]} x {Convert.ToDecimal(row["DonGia"]):N0}" +
                        $" + Công {Convert.ToDecimal(row["TienCong"]):N0}" +
                        $" = {Convert.ToDecimal(row["ThanhTien"]):N0} VNĐ");
                }
                sb.AppendLine("--------------------------------------");
                sb.AppendLine($"TỔNG CỘNG : {tong:N0} VNĐ");
                sb.AppendLine($"Trạng thái: Đã thanh toán");

                MessageBox.Show(sb.ToString(), "Hóa đơn",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                currentMaPhieu = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Thanh toán thất bại: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}