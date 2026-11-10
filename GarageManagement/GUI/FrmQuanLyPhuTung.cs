using System;
using System.Drawing;
using System.Windows.Forms;
using GarageManagement.BUS;
using GarageManagement.DTO;

namespace GarageManagement.GUI
{
    public class FrmQuanLyPhuTung : Form
    {
        private TextBox txtTen, txtDonGia, txtSoLuong;
        private Button btnThem, btnSua, btnXoa, btnLamMoi;
        private DataGridView dgv;
        private int selectedMa = 0;

        private readonly PhuTungBUS bus = new PhuTungBUS();

        public FrmQuanLyPhuTung()
        {
            Text = "Quản lý danh mục phụ tùng";
            Width = 820; Height = 580;
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            Controls.Add(new Label { Text = "Tên phụ tùng:", Location = new Point(15, 22), AutoSize = true });
            txtTen = new TextBox { Location = new Point(120, 18), Width = 350 };

            Controls.Add(new Label { Text = "Đơn giá:", Location = new Point(15, 62), AutoSize = true });
            txtDonGia = new TextBox { Location = new Point(120, 58), Width = 150 };

            Controls.Add(new Label { Text = "Số lượng tồn:", Location = new Point(290, 62), AutoSize = true });
            txtSoLuong = new TextBox { Location = new Point(400, 58), Width = 100 };

            btnThem = new Button { Text = "Thêm", Location = new Point(15, 105), Size = new Size(100, 35) };
            btnThem.Click += BtnThem_Click;

            btnSua = new Button { Text = "Sửa", Location = new Point(125, 105), Size = new Size(100, 35) };
            btnSua.Click += BtnSua_Click;

            btnXoa = new Button { Text = "Xóa", Location = new Point(235, 105), Size = new Size(100, 35) };
            btnXoa.Click += BtnXoa_Click;

            btnLamMoi = new Button { Text = "Làm mới", Location = new Point(345, 105), Size = new Size(100, 35) };
            btnLamMoi.Click += (s, e) => { ClearFields(); dgv.ClearSelection(); };

            dgv = new DataGridView
            {
                Location = new Point(15, 155),
                Size = new Size(780, 370),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.CellClick += Dgv_CellClick;

            Controls.AddRange(new Control[] {
                txtTen, txtDonGia, txtSoLuong,
                btnThem, btnSua, btnXoa, btnLamMoi, dgv });
        }

        private void LoadData() => dgv.DataSource = bus.GetAll();

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv.Rows[e.RowIndex].DataBoundItem is PhuTungDTO pt)
            {
                selectedMa = pt.MaPhuTung;
                txtTen.Text = pt.TenPhuTung;
                txtDonGia.Text = pt.DonGia.ToString("0.##");
                txtSoLuong.Text = pt.SoLuongTon.ToString();
            }
        }

        private PhuTungDTO ReadInput()
        {
            if (!decimal.TryParse(txtDonGia.Text, out decimal dg))
                throw new Exception("Đơn giá phải là số hợp lệ.");
            if (!int.TryParse(txtSoLuong.Text, out int sl))
                throw new Exception("Số lượng tồn phải là số nguyên.");
            return new PhuTungDTO
            {
                MaPhuTung = selectedMa,
                TenPhuTung = txtTen.Text.Trim(),
                DonGia = dg,
                SoLuongTon = sl
            };
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            try { bus.Insert(ReadInput()); MessageBox.Show("Thêm thành công!"); LoadData(); ClearFields(); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnSua_Click(object sender, EventArgs e)
        {
            try { bus.Update(ReadInput()); MessageBox.Show("Cập nhật thành công!"); LoadData(); ClearFields(); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (selectedMa == 0) { MessageBox.Show("Chưa chọn phụ tùng."); return; }
            if (MessageBox.Show("Xác nhận xóa phụ tùng này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { bus.Delete(selectedMa); LoadData(); ClearFields(); }
            catch (Exception ex) { MessageBox.Show("Không thể xóa (có thể đang được dùng trong phiếu):\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ClearFields()
        {
            selectedMa = 0;
            txtTen.Clear(); txtDonGia.Clear(); txtSoLuong.Clear();
            txtTen.Focus();
        }
    }
}