using System;
using System.Drawing;
using System.Windows.Forms;
using GarageManagement.BUS;
using GarageManagement.DTO;

namespace GarageManagement.GUI
{
    public class FrmTiepNhanXe : Form
    {
        private TextBox txtBienSo, txtHieuXe, txtNamSanXuat,
                        txtTenKH, txtDienThoai, txtDiaChi;
        private Button btnTraCuu, btnLuu, btnLamMoi;
        private DataGridView dgvXe;

        private readonly XeBUS xeBUS = new XeBUS();
        private readonly KhachHangBUS khBUS = new KhachHangBUS();

        public FrmTiepNhanXe()
        {
            Text = "Tiếp nhận xe & Quản lý chủ xe";
            Width = 960; Height = 650;
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            // ---------- GroupBox tra cứu ----------
            var grpSearch = new GroupBox
            {
                Text = "Tra cứu biển số xe",
                Location = new Point(10, 10),
                Size = new Size(920, 70)
            };
            var lblBienSo = new Label { Text = "Biển số:", Location = new Point(15, 32), AutoSize = true };
            txtBienSo = new TextBox { Location = new Point(90, 28), Width = 220 };
            btnTraCuu = new Button { Text = "Tra cứu", Location = new Point(320, 26), Size = new Size(130, 30) };
            btnTraCuu.Click += BtnTraCuu_Click;
            grpSearch.Controls.AddRange(new Control[] { lblBienSo, txtBienSo, btnTraCuu });

            // ---------- GroupBox khách hàng ----------
            var grpKH = new GroupBox
            {
                Text = "Thông tin khách hàng",
                Location = new Point(10, 90),
                Size = new Size(450, 165)
            };
            grpKH.Controls.Add(new Label { Text = "Tên KH:", Location = new Point(10, 32), AutoSize = true });
            txtTenKH = new TextBox { Location = new Point(110, 28), Width = 320 };
            grpKH.Controls.Add(txtTenKH);

            grpKH.Controls.Add(new Label { Text = "Điện thoại:", Location = new Point(10, 70), AutoSize = true });
            txtDienThoai = new TextBox { Location = new Point(110, 66), Width = 320 };
            grpKH.Controls.Add(txtDienThoai);

            grpKH.Controls.Add(new Label { Text = "Địa chỉ:", Location = new Point(10, 108), AutoSize = true });
            txtDiaChi = new TextBox { Location = new Point(110, 104), Width = 320 };
            grpKH.Controls.Add(txtDiaChi);

            // ---------- GroupBox xe ----------
            var grpXe = new GroupBox
            {
                Text = "Thông tin xe",
                Location = new Point(470, 90),
                Size = new Size(460, 165)
            };
            grpXe.Controls.Add(new Label { Text = "Hiệu xe:", Location = new Point(10, 32), AutoSize = true });
            txtHieuXe = new TextBox { Location = new Point(110, 28), Width = 330 };
            grpXe.Controls.Add(txtHieuXe);

            grpXe.Controls.Add(new Label { Text = "Năm SX:", Location = new Point(10, 70), AutoSize = true });
            txtNamSanXuat = new TextBox { Location = new Point(110, 66), Width = 330 };
            grpXe.Controls.Add(txtNamSanXuat);

            // ---------- Buttons ----------
            btnLuu = new Button
            {
                Text = "Lưu tiếp nhận",
                Location = new Point(10, 265),
                Size = new Size(160, 40),
                BackColor = Color.LightGreen
            };
            btnLuu.Click += BtnLuu_Click;

            btnLamMoi = new Button
            {
                Text = "Làm mới",
                Location = new Point(180, 265),
                Size = new Size(120, 40)
            };
            btnLamMoi.Click += (s, e) => ClearFields();

            // ---------- DataGridView ----------
            dgvXe = new DataGridView
            {
                Location = new Point(10, 315),
                Size = new Size(920, 285),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvXe.CellClick += DgvXe_CellClick;

            Controls.AddRange(new Control[] { grpSearch, grpKH, grpXe, btnLuu, btnLamMoi, dgvXe });
        }

        private void LoadData()
        {
            dgvXe.DataSource = xeBUS.GetAllWithOwner();
        }

        private void BtnTraCuu_Click(object sender, EventArgs e)
        {
            string bienSo = txtBienSo.Text.Trim();
            if (string.IsNullOrEmpty(bienSo))
            {
                MessageBox.Show("Vui lòng nhập biển số.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var xe = xeBUS.GetByBienSo(bienSo);
            if (xe == null)
            {
                MessageBox.Show("Chưa có xe này. Vui lòng nhập thông tin để thêm mới.",
                    "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtHieuXe.Clear(); txtNamSanXuat.Clear();
                txtTenKH.Clear(); txtDienThoai.Clear(); txtDiaChi.Clear();
                return;
            }
            // Tìm khách hàng tương ứng (dùng GetByPhone sẽ không đúng vì cần theo MaKH)
            // Lấy lại thông tin khách hàng qua GetAll
            var kh = khBUS.GetAll().Find(x => x.MaKH == xe.MaKH);
            txtHieuXe.Text = xe.HieuXe;
            txtNamSanXuat.Text = xe.NamSanXuat?.ToString() ?? "";
            txtTenKH.Text = kh?.TenKH ?? "";
            txtDienThoai.Text = kh?.DienThoai ?? "";
            txtDiaChi.Text = kh?.DiaChi ?? "";

            MessageBox.Show("Đã tìm thấy xe trong hệ thống.", "Kết quả",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                int? nam = null;
                if (!string.IsNullOrWhiteSpace(txtNamSanXuat.Text))
                {
                    if (!int.TryParse(txtNamSanXuat.Text, out int n))
                        throw new Exception("Năm sản xuất phải là số.");
                    nam = n;
                }
                var xe = new XeDTO
                {
                    BienSo = txtBienSo.Text.Trim(),
                    HieuXe = txtHieuXe.Text.Trim(),
                    NamSanXuat = nam
                };
                var kh = new KhachHangDTO
                {
                    TenKH = txtTenKH.Text.Trim(),
                    DienThoai = txtDienThoai.Text.Trim(),
                    DiaChi = txtDiaChi.Text.Trim()
                };
                xeBUS.TiepNhanXe(xe, kh);

                MessageBox.Show("Tiếp nhận xe thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thất bại",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvXe_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvXe.Rows[e.RowIndex];
            txtBienSo.Text = row.Cells["BienSo"].Value?.ToString() ?? "";
            txtHieuXe.Text = row.Cells["HieuXe"].Value?.ToString() ?? "";
            txtNamSanXuat.Text = row.Cells["NamSanXuat"].Value?.ToString() ?? "";
            txtTenKH.Text = row.Cells["TenKH"].Value?.ToString() ?? "";
            txtDienThoai.Text = row.Cells["DienThoai"].Value?.ToString() ?? "";
            txtDiaChi.Text = row.Cells["DiaChi"].Value?.ToString() ?? "";
        }

        private void ClearFields()
        {
            txtBienSo.Clear(); txtHieuXe.Clear(); txtNamSanXuat.Clear();
            txtTenKH.Clear(); txtDienThoai.Clear(); txtDiaChi.Clear();
            txtBienSo.Focus();
        }
    }
}