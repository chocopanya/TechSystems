using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TechSystems.Models;
using TechSystems.Services;

namespace TechSystems.Views  // ВАЖНО: Пространство имен должно быть TechSystems.Views
{
    public partial class MainWindow : Window
    {
        private DataService _dataService;
        private User _currentUser;
        private ObservableCollection<Tariff> _tariffs;
        private ObservableCollection<Tariff> _allTariffs;
        private bool _isManagerMode = false;

        public MainWindow(User user)
        {
            try
            {
                InitializeComponent();
                _dataService = new DataService();
                _currentUser = user;

                // Настройка интерфейса
                if (_currentUser != null && (_currentUser.IsManager || _currentUser.IsAdmin))
                {
                    _isManagerMode = true;
                    Title = $"ТехноСистемы - {(_currentUser.IsAdmin ? "Администратор" : "Менеджер")}: {_currentUser.FullName}";
                    txtUserInfo.Text = $"{(_currentUser.IsAdmin ? "Администратор" : "Менеджер")}: {_currentUser.FullName}";
                    ManagerPanel.Visibility = Visibility.Visible;
                    ManagerFilterPanel.Visibility = Visibility.Visible;
                    LoadServiceTypesFilter();
                }
                else
                {
                    Title = "ТехноСистемы - Гостевой режим";
                    txtUserInfo.Text = "Гость";
                    ManagerPanel.Visibility = Visibility.Collapsed;
                    ManagerFilterPanel.Visibility = Visibility.Collapsed;
                }

                // Загрузка данных
                LoadTariffs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания главного окна: {ex.Message}", "Ошибка");
                throw;
            }
        }

        private void LoadTariffs()
        {
            try
            {
                var tariffs = _isManagerMode ?
                    _dataService.GetAllTariffs() :
                    _dataService.GetAvailableTariffs();

                _allTariffs = new ObservableCollection<Tariff>(tariffs);
                _tariffs = new ObservableCollection<Tariff>(_allTariffs);
                icTariffs.ItemsSource = _tariffs;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки тарифов: {ex.Message}", "Ошибка");
            }
        }

        private void LoadServiceTypesFilter()
        {
            try
            {
                var serviceTypes = _dataService.GetServiceTypes();
                cmbServiceTypeFilter.ItemsSource = serviceTypes;
                cmbServiceTypeFilter.DisplayMemberPath = "Name";
                cmbServiceTypeFilter.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов услуг: {ex.Message}", "Ошибка");
            }
        }

        private void ApplyFilters()
        {
            if (_allTariffs == null) return;

            var filtered = _allTariffs.AsEnumerable();

            // Фильтрация по поиску
            string searchText = txtSearch.Text?.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                filtered = filtered.Where(t =>
                    t.Name.ToLower().Contains(searchText) ||
                    t.Description.ToLower().Contains(searchText) ||
                    t.ServiceType.Name.ToLower().Contains(searchText));
            }

            // Фильтрация по типу услуги
            if (cmbServiceTypeFilter.SelectedItem is ServiceType selectedType)
            {
                filtered = filtered.Where(t => t.ServiceTypeId == selectedType.Id);
            }

            _tariffs.Clear();
            foreach (var tariff in filtered)
            {
                _tariffs.Add(tariff);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbServiceTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cmbServiceTypeFilter.SelectedIndex = -1;
            LoadTariffs();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void BtnAddTariff_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new TariffEditWindow(_dataService);
            if (editWindow.ShowDialog() == true)
            {
                LoadTariffs();
            }
        }

        private void BtnEditTariff_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new TariffEditWindow(_dataService);
            if (editWindow.ShowDialog() == true)
            {
                LoadTariffs();
            }
        }

        private void BtnDeleteTariff_Click(object sender, RoutedEventArgs e)
        {
            var deleteWindow = new DeleteTariffWindow(_dataService);
            if (deleteWindow.ShowDialog() == true)
            {
                LoadTariffs();
            }
        }

        private void BtnApplications_Click(object sender, RoutedEventArgs e)
        {
            var applicationsWindow = new ApplicationsWindow(_dataService);
            applicationsWindow.Owner = this;
            applicationsWindow.ShowDialog();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            var clientsWindow = new ClientsWindow(_dataService);
            clientsWindow.Owner = this;
            clientsWindow.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _dataService?.Dispose();
        }
    }
}