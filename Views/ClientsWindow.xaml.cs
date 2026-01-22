using System.Linq;
using System.Windows;
using TechSystems.Services;

namespace TechSystems.Views
{
    public partial class ClientsWindow : Window
    {
        private readonly DataService _dataService;

        public ClientsWindow(DataService service)
        {
            InitializeComponent();
            _dataService = service;
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                string searchText = txtSearch.Text.ToLower();
                var clients = _dataService.GetAllUsers()
                    .Where(c =>
                        c.FullName.ToLower().Contains(searchText) ||
                        c.CompanyName?.ToLower().Contains(searchText) == true ||
                        c.Email?.ToLower().Contains(searchText) == true ||
                        c.Login.ToLower().Contains(searchText))
                    .ToList();

                dgClients.ItemsSource = clients;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка");
            }
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoadClients();
        }

        private void BtnViewApplications_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = dgClients.SelectedItem as TechSystems.Models.User;
            if (selectedClient != null)
            {
                MessageBox.Show("Функция просмотра заявок клиента будет добавлена в следующей версии", "Информация");
                // TODO: Реализовать позже
            }
            else
            {
                MessageBox.Show("Выберите клиента", "Внимание");
            }
        }

        // УДАЛИТЕ или ЗАКОММЕНТИРУЙТЕ ЭТУ ЧАСТЬ:
        private void BtnViewPayments_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = dgClients.SelectedItem as TechSystems.Models.User;
            if (selectedClient != null)
            {
                // КОММЕНТИРУЕМ эту строку:
                // var paymentsWindow = new ClientPaymentsWindow(_dataService, selectedClient.Id);
                // paymentsWindow.Owner = this;
                // paymentsWindow.ShowDialog();

                // ВМЕСТО ЭТОГО показываем сообщение:
                MessageBox.Show("Модуль платежей в разработке", "Информация");
            }
            else
            {
                MessageBox.Show("Выберите клиента для просмотра платежей", "Внимание");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}