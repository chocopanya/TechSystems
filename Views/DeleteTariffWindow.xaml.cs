using System.Windows;
using TechSystems.Services;

namespace TechSystems.Views
{
    public partial class DeleteTariffWindow : Window
    {
        private readonly DataService _dataService;

        public DeleteTariffWindow(DataService dataService)
        {
            InitializeComponent();
            _dataService = dataService;
            LoadTariffs();
        }

        private void LoadTariffs()
        {
            var tariffs = _dataService.GetAllTariffs();
            dgTariffs.ItemsSource = tariffs;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedTariff = dgTariffs.SelectedItem as Models.Tariff;
            if (selectedTariff != null)
            {
                var result = MessageBox.Show($"Удалить тариф '{selectedTariff.Name}'?", "Подтверждение",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_dataService.DeleteTariff(selectedTariff.Id))
                    {
                        MessageBox.Show("Тариф успешно удалён", "Успех");
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении тарифа", "Ошибка");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите тариф для удаления", "Внимание");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}