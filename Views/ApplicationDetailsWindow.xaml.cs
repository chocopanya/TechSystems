using System.Windows;
using TechSystems.Services;

namespace TechSystems.Views
{
    public partial class ApplicationDetailsWindow : Window
    {
        private readonly DataService _dataService;
        private readonly int _applicationId;

        public ApplicationDetailsWindow(DataService service, int applicationId)
        {
            InitializeComponent();
            _dataService = service;
            _applicationId = applicationId;
            LoadApplicationDetails();
        }

        private void LoadApplicationDetails()
        {
            try
            {
                var application = _dataService.GetApplicationById(_applicationId);
                if (application != null)
                {
                    Title = $"Детали заявки #{application.Id}";
                    txtId.Text = application.Id.ToString();
                    txtClient.Text = application.Client?.FullName ?? "Неизвестно";
                    txtTariff.Text = $"{application.Tariff?.Name} ({application.Tariff?.ServiceType?.Name})";
                    txtDate.Text = application.ApplicationDate.ToString("dd.MM.yyyy HH:mm");
                    txtStatus.Text = application.Status;

                    // Цвет статуса
                    if (application.Status == "Новая")
                        txtStatus.Foreground = System.Windows.Media.Brushes.Blue;
                    else if (application.Status == "В обработке")
                        txtStatus.Foreground = System.Windows.Media.Brushes.Orange;
                    else if (application.Status == "Подтверждена")
                        txtStatus.Foreground = System.Windows.Media.Brushes.Green;
                    else if (application.Status == "Отменена")
                        txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                    else if (application.Status == "Завершена")
                        txtStatus.Foreground = System.Windows.Media.Brushes.Purple;
                    else
                        txtStatus.Foreground = System.Windows.Media.Brushes.Black;

                    txtLicenseCount.Text = application.LicenseCount.ToString();
                    txtTotalCost.Text = $"{application.TotalCost:F2} руб.";
                    txtComment.Text = application.Comment ?? "Нет комментария";
                }
                else
                {
                    MessageBox.Show("Заявка не найдена", "Ошибка");
                    Close();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки деталей заявки: {ex.Message}", "Ошибка");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}