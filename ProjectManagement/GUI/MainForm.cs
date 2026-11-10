using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ProjectManagement.BUS;
using ProjectManagement.DTO;

namespace ProjectManagement
{
    public class MainForm : Form
    {
        // ================== BUS ==================
        private readonly DuAnBUS busDuAn = new DuAnBUS();
        private readonly CongViecBUS busCV = new CongViecBUS();
        private readonly ThanhVienBUS busTV = new ThanhVienBUS();
        private readonly PhanCongBUS busPC = new PhanCongBUS();

        // ================== TAB 1 - LEFT (Dự án) ==================
        private DataGridView dgvDuAn;
        private TextBox txtTenDuAn, txtMoTaDuAn;
        private DateTimePicker dtpNgayBDDuAn, dtpNgayKTDuAn;
        private Button btnThemDuAn, btnSuaDuAn, btnXoaDuAn;

        // ================== TAB 1 - RIGHT (Công việc) ==================
        private DataGridView dgvCongViec;
        private TextBox txtTenCV, txtMoTaCV;
        private DateTimePicker dtpBDCV, dtpHanNopCV;
        private ComboBox cbTrangThaiCV;
        private CheckedListBox clbThanhVien;
        private Button btnThemCV;

        // ================== TAB 2 ==================
        private ComboBox cbDuAnTab2, cbTrangThaiTab2;
        private DataGridView dgvLoc;
        private Button btnLoc, btnChuyenDone;

        // ================== TAB 3 ==================
        private Button btnTreHan, btnThongKe;
        private DataGridView dgvTreHan, dgvThongKe;

        // ================== STATE ==================
        private int currentMaDuAn = -1;
        private List<ThanhVienDTO> dsThanhVien = new List<ThanhVienDTO>();

        // =========================================================
        public MainForm()
        {
            InitializeComponentCustom();
            LoadDuLieuBanDau();
        }

        // =========================================================
        //  DỰNG TOÀN BỘ GIAO DIỆN BẰNG CODE
        // =========================================================
        private void InitializeComponentCustom()
        {
            Text = "Hệ thống Quản lý Công việc & Tiến độ Dự án";
            Size = new Size(1200, 750);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            Font = new Font("Segoe UI", 9F);

            var tabMain = new TabControl { Dock = DockStyle.Fill };

            // ---------- TAB 1 ----------
            var tab1 = new TabPage("Quản lý Dự án & Task");
            tab1.Padding = new Padding(6);
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 6
            };
            // Đặt SplitterDistance sau khi form được layout
            split.HandleCreated += (s, e) => split.SplitterDistance = split.Width / 2;
            split.Resize += (s, e) =>
            {
                if (split.Width > 400) split.SplitterDistance = split.Width / 2;
            };

            BuildLeftPanel(split.Panel1);
            BuildRightPanel(split.Panel2);

            tab1.Controls.Add(split);

            // ---------- TAB 2 ----------
            var tab2 = new TabPage("Tiến độ & Trạng thái");
            tab2.Padding = new Padding(6);
            BuildTab2(tab2);

            // ---------- TAB 3 ----------
            var tab3 = new TabPage("Báo cáo & Cảnh báo");
            tab3.Padding = new Padding(6);
            BuildTab3(tab3);

            tabMain.TabPages.Add(tab1);
            tabMain.TabPages.Add(tab2);
            tabMain.TabPages.Add(tab3);

            Controls.Add(tabMain);
        }

        // ---------------- TAB 1 - LEFT ----------------
        private void BuildLeftPanel(Control parent)
        {
            // Panel nhập liệu (Dock Bottom)
            var pnlInput = new Panel { Dock = DockStyle.Bottom, Height = 200, Padding = new Padding(8) };

            pnlInput.Controls.Add(new Label { Text = "Tên dự án:", Location = new Point(10, 15), AutoSize = true });
            txtTenDuAn = new TextBox { Location = new Point(110, 12), Width = 320 };
            pnlInput.Controls.Add(txtTenDuAn);

            pnlInput.Controls.Add(new Label { Text = "Ngày BĐ:", Location = new Point(10, 48), AutoSize = true });
            dtpNgayBDDuAn = new DateTimePicker { Location = new Point(110, 45), Width = 130, Format = DateTimePickerFormat.Short };
            pnlInput.Controls.Add(dtpNgayBDDuAn);

            pnlInput.Controls.Add(new Label { Text = "Ngày KT:", Location = new Point(260, 48), AutoSize = true });
            dtpNgayKTDuAn = new DateTimePicker { Location = new Point(330, 45), Width = 130, Format = DateTimePickerFormat.Short, ShowCheckBox = true };
            pnlInput.Controls.Add(dtpNgayKTDuAn);

            pnlInput.Controls.Add(new Label { Text = "Mô tả:", Location = new Point(10, 81), AutoSize = true });
            txtMoTaDuAn = new TextBox { Location = new Point(110, 78), Width = 350, Multiline = true, Height = 55 };
            pnlInput.Controls.Add(txtMoTaDuAn);

            btnThemDuAn = new Button { Text = "Thêm", Location = new Point(110, 145), Width = 100, Height = 32, BackColor = Color.LightGreen };
            btnSuaDuAn = new Button { Text = "Sửa", Location = new Point(220, 145), Width = 100, Height = 32, BackColor = Color.Khaki };
            btnXoaDuAn = new Button { Text = "Xóa", Location = new Point(330, 145), Width = 100, Height = 32, BackColor = Color.LightCoral };
            pnlInput.Controls.Add(btnThemDuAn);
            pnlInput.Controls.Add(btnSuaDuAn);
            pnlInput.Controls.Add(btnXoaDuAn);

            // Grid dự án (Fill phần còn lại)
            dgvDuAn = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            // Gắn vào parent: Grid add trước (Fill), Panel add sau (Bottom)
            parent.Controls.Add(dgvDuAn);
            parent.Controls.Add(pnlInput);

            // Sự kiện
            btnThemDuAn.Click += BtnThemDuAn_Click;
            btnSuaDuAn.Click += BtnSuaDuAn_Click;
            btnXoaDuAn.Click += BtnXoaDuAn_Click;
            dgvDuAn.SelectionChanged += DgvDuAn_SelectionChanged;
        }

        // ---------------- TAB 1 - RIGHT ----------------
        private void BuildRightPanel(Control parent)
        {
            var pnlInput = new Panel { Dock = DockStyle.Bottom, Height = 280, Padding = new Padding(8) };

            pnlInput.Controls.Add(new Label { Text = "Tên công việc:", Location = new Point(10, 15), AutoSize = true });
            txtTenCV = new TextBox { Location = new Point(120, 12), Width = 340 };
            pnlInput.Controls.Add(txtTenCV);

            pnlInput.Controls.Add(new Label { Text = "Ngày BĐ:", Location = new Point(10, 48), AutoSize = true });
            dtpBDCV = new DateTimePicker { Location = new Point(120, 45), Width = 130, Format = DateTimePickerFormat.Short, ShowCheckBox = true };
            pnlInput.Controls.Add(dtpBDCV);

            pnlInput.Controls.Add(new Label { Text = "Hạn nộp:", Location = new Point(270, 48), AutoSize = true });
            dtpHanNopCV = new DateTimePicker { Location = new Point(340, 45), Width = 130, Format = DateTimePickerFormat.Short, ShowCheckBox = true };
            pnlInput.Controls.Add(dtpHanNopCV);

            pnlInput.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(10, 81), AutoSize = true });
            cbTrangThaiCV = new ComboBox { Location = new Point(120, 78), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cbTrangThaiCV.Items.AddRange(new object[] { "To Do", "In Progress", "Review", "Done" });
            cbTrangThaiCV.SelectedIndex = 0;
            pnlInput.Controls.Add(cbTrangThaiCV);

            pnlInput.Controls.Add(new Label { Text = "Mô tả:", Location = new Point(10, 114), AutoSize = true });
            txtMoTaCV = new TextBox { Location = new Point(120, 111), Width = 340, Height = 40, Multiline = true };
            pnlInput.Controls.Add(txtMoTaCV);

            pnlInput.Controls.Add(new Label { Text = "Giao việc cho thành viên:", Location = new Point(10, 160), AutoSize = true });
            clbThanhVien = new CheckedListBox
            {
                Location = new Point(120, 158),
                Width = 340,
                Height = 80,
                CheckOnClick = true
            };
            pnlInput.Controls.Add(clbThanhVien);

            btnThemCV = new Button
            {
                Text = "Thêm Task",
                Location = new Point(120, 245),
                Width = 140,
                Height = 32,
                BackColor = Color.LightSkyBlue
            };
            pnlInput.Controls.Add(btnThemCV);

            dgvCongViec = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            parent.Controls.Add(dgvCongViec);
            parent.Controls.Add(pnlInput);

            btnThemCV.Click += BtnThemCV_Click;
        }

        // ---------------- TAB 2 ----------------
        private void BuildTab2(TabPage tab)
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(6) };

            pnlTop.Controls.Add(new Label { Text = "Dự án:", Location = new Point(10, 18), AutoSize = true });
            cbDuAnTab2 = new ComboBox { Location = new Point(70, 15), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            cbDuAnTab2.SelectedIndexChanged += (s, e) => LoadGridTab2();
            pnlTop.Controls.Add(cbDuAnTab2);

            pnlTop.Controls.Add(new Label { Text = "Trạng thái:", Location = new Point(410, 18), AutoSize = true });
            cbTrangThaiTab2 = new ComboBox { Location = new Point(485, 15), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            cbTrangThaiTab2.Items.AddRange(new object[] { "(Tất cả)", "To Do", "In Progress", "Review", "Done" });
            cbTrangThaiTab2.SelectedIndex = 0;
            pnlTop.Controls.Add(cbTrangThaiTab2);

            btnLoc = new Button { Text = "Lọc", Location = new Point(660, 13), Width = 100, Height = 30 };
            btnLoc.Click += (s, e) => LoadGridTab2();
            pnlTop.Controls.Add(btnLoc);

            btnChuyenDone = new Button
            {
                Text = "Chuyển sang Done",
                Location = new Point(770, 13),
                Width = 160,
                Height = 30,
                BackColor = Color.LightGreen
            };
            btnChuyenDone.Click += BtnChuyenDone_Click;
            pnlTop.Controls.Add(btnChuyenDone);

            dgvLoc = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White
            };

            tab.Controls.Add(dgvLoc);
            tab.Controls.Add(pnlTop);
        }

        // ---------------- TAB 3 ----------------
        private void BuildTab3(TabPage tab)
        {
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 55, Padding = new Padding(6) };

            btnTreHan = new Button
            {
                Text = "Hiển thị công việc trễ hạn",
                Location = new Point(10, 10),
                Width = 220,
                Height = 34,
                BackColor = Color.LightCoral
            };
            btnTreHan.Click += BtnTreHan_Click;
            pnlTop.Controls.Add(btnTreHan);

            btnThongKe = new Button
            {
                Text = "Thống kê % Hoàn thành",
                Location = new Point(240, 10),
                Width = 200,
                Height = 34,
                BackColor = Color.LightSkyBlue
            };
            btnThongKe.Click += BtnThongKe_Click;
            pnlTop.Controls.Add(btnThongKe);

            // Tiêu đề 2 grid
            var lbl1 = new Label { Text = "⚠ Công việc trễ hạn:", Location = new Point(10, 55), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            var lbl2 = new Label { Text = "📊 Thống kê % hoàn thành (dự án đang chọn):", Location = new Point(10, 290), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

            dgvTreHan = new DataGridView
            {
                Location = new Point(10, 80),
                Width = 1140,
                Height = 200,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White
            };

            dgvThongKe = new DataGridView
            {
                Location = new Point(10, 315),
                Width = 1140,
                Height = 200,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White
            };

            tab.Controls.Add(lbl1);
            tab.Controls.Add(lbl2);
            tab.Controls.Add(dgvTreHan);
            tab.Controls.Add(dgvThongKe);
            tab.Controls.Add(pnlTop);
        }

        // =========================================================
        //  NẠP DỮ LIỆU BAN ĐẦU
        // =========================================================
        private void LoadDuLieuBanDau()
        {
            // 1. Thành viên vào CheckedListBox
            dsThanhVien = busTV.GetAll();
            clbThanhVien.Items.Clear();
            foreach (var tv in dsThanhVien)
                clbThanhVien.Items.Add(tv);

            // 2. Dự án
            LoadDsDuAn();

            // 3. Combobox tab 2
            var dsDuAn = busDuAn.GetAll();
            cbDuAnTab2.DataSource = dsDuAn;
            cbDuAnTab2.DisplayMember = "TenDuAn";
            cbDuAnTab2.ValueMember = "MaDuAn";
        }

        private void LoadDsDuAn()
        {
            var ds = busDuAn.GetAll();
            dgvDuAn.DataSource = ds;
            // Ẩn cột MaDuAn? Giữ để debug cũng được. Ẩn cho gọn:
            if (dgvDuAn.Columns.Contains("MaDuAn"))
                dgvDuAn.Columns["MaDuAn"].Visible = false;
            dgvDuAn.Columns["TenDuAn"].HeaderText = "Tên dự án";
            dgvDuAn.Columns["NgayBatDau"].HeaderText = "Ngày BĐ";
            dgvDuAn.Columns["NgayKetThuc"].HeaderText = "Ngày KT";
            dgvDuAn.Columns["MoTa"].HeaderText = "Mô tả";
        }

        // =========================================================
        //  SỰ KIỆN TAB 1 - DỰ ÁN
        // =========================================================
        private void DgvDuAn_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDuAn.CurrentRow?.DataBoundItem is DuAnDTO dto)
            {
                currentMaDuAn = dto.MaDuAn;
                txtTenDuAn.Text = dto.TenDuAn;
                txtMoTaDuAn.Text = dto.MoTa;
                dtpNgayBDDuAn.Value = dto.NgayBatDau;
                if (dto.NgayKetThuc.HasValue)
                {
                    dtpNgayKTDuAn.Value = dto.NgayKetThuc.Value;
                    dtpNgayKTDuAn.Checked = true;
                }
                else dtpNgayKTDuAn.Checked = false;

                LoadDsCongViec();
            }
        }

        private void LoadDsCongViec()
        {
            if (currentMaDuAn <= 0) { dgvCongViec.DataSource = null; return; }
            var ds = busCV.GetByDuAn(currentMaDuAn);
            dgvCongViec.DataSource = ds;

            if (dgvCongViec.Columns.Contains("MaCV")) dgvCongViec.Columns["MaCV"].Visible = false;
            if (dgvCongViec.Columns.Contains("MaDuAn")) dgvCongViec.Columns["MaDuAn"].Visible = false;
            if (dgvCongViec.Columns.Contains("TenCV")) dgvCongViec.Columns["TenCV"].HeaderText = "Tên công việc";
            if (dgvCongViec.Columns.Contains("TrangThai")) dgvCongViec.Columns["TrangThai"].HeaderText = "Trạng thái";
            if (dgvCongViec.Columns.Contains("HanNop")) dgvCongViec.Columns["HanNop"].HeaderText = "Hạn nộp";
        }

        private void BtnThemDuAn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenDuAn.Text))
            {
                MessageBox.Show("Vui lòng nhập tên dự án!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var dto = new DuAnDTO
                {
                    TenDuAn = txtTenDuAn.Text.Trim(),
                    NgayBatDau = dtpNgayBDDuAn.Value.Date,
                    NgayKetThuc = dtpNgayKTDuAn.Checked ? dtpNgayKTDuAn.Value.Date : (DateTime?)null,
                    MoTa = txtMoTaDuAn.Text.Trim()
                };
                busDuAn.Insert(dto);
                LoadDsDuAn();
                MessageBox.Show("Thêm dự án thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm dự án: " + ex.Message);
            }
        }

        private void BtnSuaDuAn_Click(object sender, EventArgs e)
        {
            if (currentMaDuAn <= 0) { MessageBox.Show("Vui lòng chọn dự án!"); return; }
            try
            {
                var dto = new DuAnDTO
                {
                    MaDuAn = currentMaDuAn,
                    TenDuAn = txtTenDuAn.Text.Trim(),
                    NgayBatDau = dtpNgayBDDuAn.Value.Date,
                    NgayKetThuc = dtpNgayKTDuAn.Checked ? dtpNgayKTDuAn.Value.Date : (DateTime?)null,
                    MoTa = txtMoTaDuAn.Text.Trim()
                };
                busDuAn.Update(dto);
                LoadDsDuAn();
                MessageBox.Show("Cập nhật dự án thành công!");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnXoaDuAn_Click(object sender, EventArgs e)
        {
            if (currentMaDuAn <= 0) { MessageBox.Show("Vui lòng chọn dự án!"); return; }
            if (MessageBox.Show("Xóa dự án này sẽ xóa toàn bộ task liên quan. Tiếp tục?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                busDuAn.Delete(currentMaDuAn);
                currentMaDuAn = -1;
                LoadDsDuAn();
                dgvCongViec.DataSource = null;
                MessageBox.Show("Đã xóa dự án.");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // =========================================================
        //  SỰ KIỆN TAB 1 - CÔNG VIỆC
        // =========================================================
        private void BtnThemCV_Click(object sender, EventArgs e)
        {
            if (currentMaDuAn <= 0) { MessageBox.Show("Vui lòng chọn dự án bên trái trước!"); return; }
            if (string.IsNullOrWhiteSpace(txtTenCV.Text)) { MessageBox.Show("Nhập tên công việc!"); return; }

            try
            {
                var cvDto = new CongViecDTO
                {
                    TenCV = txtTenCV.Text.Trim(),
                    MaDuAn = currentMaDuAn,
                    NgayBatDau = dtpBDCV.Checked ? dtpBDCV.Value.Date : (DateTime?)null,
                    HanNop = dtpHanNopCV.Checked ? dtpHanNopCV.Value.Date : (DateTime?)null,
                    TrangThai = cbTrangThaiCV.SelectedItem?.ToString() ?? "To Do",
                    MoTa = txtMoTaCV.Text.Trim()
                };
                int maCV = busCV.Insert(cvDto);

                // Tạo bản ghi phân công cho các thành viên được tick
                foreach (var item in clbThanhVien.CheckedItems)
                {
                    if (item is ThanhVienDTO tv)
                        busPC.Insert(maCV, tv.MaTV, tv.VaiTro);
                }

                LoadDsCongViec();

                // Reset form
                txtTenCV.Clear();
                txtMoTaCV.Clear();
                for (int i = 0; i < clbThanhVien.Items.Count; i++) clbThanhVien.SetItemChecked(i, false);

                MessageBox.Show("Thêm công việc và phân công thành công!");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // =========================================================
        //  TAB 2
        // =========================================================
        private void LoadGridTab2()
        {
            if (cbDuAnTab2.SelectedValue is not int maDuAn) return;
            string tt = cbTrangThaiTab2.SelectedItem?.ToString();
            if (tt == "(Tất cả)") tt = null;

            var ds = busCV.GetFiltered(maDuAn, tt);
            dgvLoc.DataSource = ds;

            if (dgvLoc.Columns.Contains("MaCV")) dgvLoc.Columns["MaCV"].Visible = false;
            if (dgvLoc.Columns.Contains("MaDuAn")) dgvLoc.Columns["MaDuAn"].Visible = false;
            if (dgvLoc.Columns.Contains("TenCV")) dgvLoc.Columns["TenCV"].HeaderText = "Tên công việc";
            if (dgvLoc.Columns.Contains("TrangThai")) dgvLoc.Columns["TrangThai"].HeaderText = "Trạng thái";
            if (dgvLoc.Columns.Contains("HanNop")) dgvLoc.Columns["HanNop"].HeaderText = "Hạn nộp";
        }

        private void BtnChuyenDone_Click(object sender, EventArgs e)
        {
            if (dgvLoc.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một task!");
                return;
            }
            int count = 0;
            foreach (DataGridViewRow row in dgvLoc.SelectedRows)
            {
                if (row.DataBoundItem is CongViecDTO cv)
                {
                    busCV.UpdateTrangThai(cv.MaCV, "Done");
                    count++;
                }
            }
            LoadGridTab2();
            MessageBox.Show($"Đã chuyển {count} task sang Done.");
        }

        // =========================================================
        //  TAB 3
        // =========================================================
        private void BtnTreHan_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = busCV.GetTreHan();  // DAL đọc bằng SqlDataReader
                dgvTreHan.DataSource = dt;
                if (dt.Rows.Count == 0)
                    MessageBox.Show("Không có công việc nào trễ hạn 🎉");
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnThongKe_Click(object sender, EventArgs e)
        {
            if (cbDuAnTab2.SelectedValue is not int maDuAn)
            {
                MessageBox.Show("Hãy chọn dự án ở tab 'Tiến độ & Trạng thái' trước.");
                return;
            }
            try
            {
                dgvThongKe.DataSource = busCV.ThongKeTheoDuAn(maDuAn);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }
}