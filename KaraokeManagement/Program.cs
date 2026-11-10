using Karaoke.GUI;

namespace Karaoke
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Nếu project không có ApplicationConfiguration (do tạo tay), dùng 3 dòng dưới:
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            ApplicationConfiguration.Initialize();
            Application.Run(new FormMain());
        }
    }
}