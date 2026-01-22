using System.Linq;
using System.Windows;
using TechSystems.Services;

namespace TechSystems.Views
{
    public partial class ClientApplicationsWindow : Window
    {
        private readonly DataService _dataService;
        private readonly int _clientId;

        public ClientApplicationsWindow(DataService service, int clientId)
        {
            InitializeComponent();
            _dataService = service;
            _clientId = clientId;
            LoadClientInfo();
            LoadApplications();
        }

        private void LoadClientInfo()
        {
            try
            {
                var client = _dataService.GetUserById(_clientId);
                if (client != null)
                {
                    txtTitle.Text = $"Заявки клиента: {client.FullName}";
                    Title = $"Заявки клиента: {client.FullName}";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки информации о клиенте: {ex.Message}", "Ошибка");
            }
        }

        private void LoadApplications()
        {
            try
            {
                var applications = _dataService.GetAllApplications()
                    .Where(a => a.ClientId == _clientId)
                    .OrderByDescending(a => a.ApplicationDate)
                    .ToList();

                dgApplications.ItemsSource = applications;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка");
            }
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var selectedApplication = dgApplications.SelectedItem as Models.ServiceRequest;
            if (selectedApplication != null)
            {
                var detailsWindow = new ApplicationDetailsWindow(_dataService, selectedApplication.Id);
                detailsWindow.Owner = this;
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите заявку для просмотра деталей", "Внимание");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadApplications();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}