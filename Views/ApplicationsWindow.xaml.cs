using System.Linq;
using System.Windows;
using TechSystems.Services;

namespace TechSystems.Views
{
    public partial class ApplicationsWindow : Window
    {
        private readonly DataService _dataService;
        private bool _sortAscending = false;

        public ApplicationsWindow(DataService service)
        {
            InitializeComponent();
            _dataService = service;
            LoadApplications();
        }

        private void LoadApplications()
        {
            try
            {
                string searchText = txtSearch.Text;
                string statusFilter = (cmbStatusFilter.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "Все";

                var applications = _dataService.GetApplicationsFiltered(searchText, statusFilter);

                // Сортировка
                if (_sortAscending)
                {
                    applications = applications.OrderBy(a => a.ApplicationDate).ToList();
                }

                dgApplications.ItemsSource = applications;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка");
            }
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            LoadApplications();
        }

        private void CmbStatusFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadApplications();
        }

        private void BtnSortDateDesc_Click(object sender, RoutedEventArgs e)
        {
            _sortAscending = false;
            LoadApplications();
        }

        private void BtnSortDateAsc_Click(object sender, RoutedEventArgs e)
        {
            _sortAscending = true;
            LoadApplications();
        }

        private void DgApplications_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new ApplicationEditWindow(_dataService);
            if (editWindow.ShowDialog() == true)
            {
                LoadApplications();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedApplication = dgApplications.SelectedItem as TechSystems.Models.ServiceRequest; // Изменили тип
            if (selectedApplication != null)
            {
                var editWindow = new ApplicationEditWindow(_dataService, selectedApplication);
                if (editWindow.ShowDialog() == true)
                {
                    LoadApplications();
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку для редактирования", "Внимание");
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedApplication = dgApplications.SelectedItem as TechSystems.Models.ServiceRequest; // Изменили тип
            if (selectedApplication != null)
            {
                if (selectedApplication.StatusId == 3)
                {
                    MessageBox.Show("Заявка уже подтверждена", "Информация");
                    return;
                }

                var result = MessageBox.Show($"Подтвердить заявку #{selectedApplication.Id}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    selectedApplication.StatusId = 3; // Подтверждена
                    if (_dataService.UpdateApplication(selectedApplication))
                    {
                        MessageBox.Show("Заявка успешно подтверждена", "Успех");
                        LoadApplications();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку для подтверждения", "Внимание");
            }
        }

        private void BtnCancelApp_Click(object sender, RoutedEventArgs e)
        {
            var selectedApplication = dgApplications.SelectedItem as TechSystems.Models.ServiceRequest; // Изменили тип
            if (selectedApplication != null)
            {
                if (selectedApplication.StatusId == 4)
                {
                    MessageBox.Show("Заявка уже отменена", "Информация");
                    return;
                }

                var result = MessageBox.Show($"Отменить заявку #{selectedApplication.Id}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    selectedApplication.StatusId = 4; // Отменена
                    if (_dataService.UpdateApplication(selectedApplication))
                    {
                        MessageBox.Show("Заявка успешно отменена", "Успех");
                        LoadApplications();
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку для отмены", "Внимание");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedApplication = dgApplications.SelectedItem as TechSystems.Models.ServiceRequest; // Изменили тип
            if (selectedApplication != null)
            {
                var result = MessageBox.Show($"Удалить заявку #{selectedApplication.Id}?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_dataService.DeleteApplication(selectedApplication.Id))
                    {
                        MessageBox.Show("Заявка успешно удалена", "Успех");
                        LoadApplications();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении заявки", "Ошибка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заявку для удаления", "Внимание");
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