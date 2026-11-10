using System;
using System.Drawing;
using System.Windows.Forms;

namespace GarageManagement.GUI
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Hệ thống Quản lý Garage";
            Width = 1000; Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.WhiteSmoke;

            // ---------- Menu ----------
            var menu = new MenuStrip();
            var mnuHeThong = new ToolStripMenuItem("Hệ thống");
            var mnuThoat = new ToolStripMenuItem("Thoát");
            mnuThoat.Click += (s, e) => Close();
            mnuHeThong.DropDownItems.Add(mnuThoat);

            var mnuNghiepVu = new ToolStripMenuItem("Nghiệp vụ");
            var mnuTiepNhan = new ToolStripMenuItem("Tiếp nhận xe");
            var mnuPhuTung = new ToolStripMenuItem("Quản lý phụ tùng");
            var mnuLapPhieu = new ToolStripMenuItem("Lập phiếu sửa chữa");
            mnuTiepNhan.Click += (s, e) => new FrmTiepNhanXe().ShowDialog();
            mnuPhuTung.Click += (s, e) => new FrmQuanLyPhuTung().ShowDialog();
            mnuLapPhieu.Click += (s, e) => new FrmLapPhieuSuaChua().ShowDialog();
            mnuNghiepVu.DropDownItems.AddRange(new ToolStripItem[]
                { mnuTiepNhan, mnuPhuTung, mnuLapPhieu });

            menu.Items.Add(mnuHeThong);
            menu.Items.Add(mnuNghiepVu);
            MainMenuStrip = menu;
            Controls.Add(menu);

            // ---------- Welcome Label ----------
            var lbl = new Label
            {
                Text = "CHÀO MỪNG ĐẾN VỚI HỆ THỐNG QUẢN LÝ GARAGE",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
            Controls.Add(lbl);
        }
    }
}