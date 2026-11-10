namespace GarageManagement
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Phải gọi MainForm ở đây thì Menu mới hiện ra
            Application.Run(new GarageManagement.GUI.MainForm());
        }
    }
}