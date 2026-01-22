using System.Windows;
using TechSystems.Views;

namespace TechSystems
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Просто показываем окно входа
            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}