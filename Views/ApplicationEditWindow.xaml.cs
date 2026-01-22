using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TechSystems.Models;
using TechSystems.Services;

namespace TechSystems.Views
{
    public partial class ApplicationEditWindow : Window
    {
        private readonly DataService _dataService;
        private readonly ServiceRequest _application; // Изменили тип
        private bool _isNewApplication;

        public ApplicationEditWindow(DataService dataService, ServiceRequest application = null) // Изменили тип
        {
            InitializeComponent();
            _dataService = dataService;
            _application = application ?? new ServiceRequest(); // Изменили создание
            _isNewApplication = application == null;

            LoadData();
            SetupEventHandlers();
        }

        private void LoadData()
        {
            try
            {
                // Загружаем клиентов
                var clients = _dataService.GetClients();
                cmbClient.ItemsSource = clients;

                // Загружаем тарифы
                var tariffs = _dataService.GetAllTariffs();
                cmbTariff.ItemsSource = tariffs;

                if (!_isNewApplication)
                {
                    Title = $"Редактирование заявки #{_application.Id}";

                    // Заполняем данные
                    foreach (User client in cmbClient.Items)
                    {
                        if (client.Id == _application.ClientId)
                        {
                            cmbClient.SelectedItem = client;
                            break;
                        }
                    }

                    foreach (Tariff tariff in cmbTariff.Items)
                    {
                        if (tariff.Id == _application.TariffId)
                        {
                            cmbTariff.SelectedItem = tariff;
                            UpdateTariffInfo(tariff);
                            break;
                        }
                    }

                    txtLicenseCount.Text = _application.LicenseCount.ToString();
                    txtTotalCost.Text = _application.TotalCost.ToString("F2");
                    txtComment.Text = _application.Comment;

                    // Выбираем статус
                    foreach (ComboBoxItem item in cmbStatus.Items)
                    {
                        if (item.Tag.ToString() == _application.StatusId.ToString())
                        {
                            cmbStatus.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    Title = "Создание новой заявки";

                    // Значения по умолчанию
                    if (clients.Any()) cmbClient.SelectedIndex = 0;
                    if (tariffs.Any()) cmbTariff.SelectedIndex = 0;
                    cmbStatus.SelectedIndex = 0; // Новая
                    txtLicenseCount.Text = "1";

                    // Рассчитываем стоимость при выборе тарифа
                    if (cmbTariff.SelectedItem is Tariff selectedTariff)
                    {
                        UpdateTariffInfo(selectedTariff);
                        CalculateTotalCost();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        private void SetupEventHandlers()
        {
            cmbTariff.SelectionChanged += (s, e) =>
            {
                if (cmbTariff.SelectedItem is Tariff selectedTariff)
                {
                    UpdateTariffInfo(selectedTariff);
                    CalculateTotalCost();
                }
            };

            txtLicenseCount.TextChanged += (s, e) =>
            {
                CalculateTotalCost();
            };
        }

        private void UpdateTariffInfo(Tariff tariff)
        {
            txtTariffInfo.Text = $"{tariff.Name}\n" +
                                $"Тип услуги: {tariff.ServiceType?.Name}\n" +
                                $"Срок подписки: {tariff.SubscriptionMonths} месяцев\n" +
                                $"Дата начала: {tariff.StartDate:dd.MM.yyyy}\n" +
                                $"Базовая стоимость за лицензию: {tariff.FinalPrice:F2} руб.";

            txtTariffLicenses.Text = $"Доступных лицензий: {tariff.AvailableLicenses}";

            if (tariff.AvailableLicenses == 0)
            {
                txtTariffLicenses.Foreground = System.Windows.Media.Brushes.Red;
            }
            else if (tariff.AvailableLicenses < 10)
            {
                txtTariffLicenses.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                txtTariffLicenses.Foreground = System.Windows.Media.Brushes.Green;
            }

            txtTariffPrice.Text = $"Цена за лицензию: {tariff.FinalPrice:F2} руб.";
        }

        private void CalculateTotalCost()
        {
            try
            {
                if (cmbTariff.SelectedItem is Tariff selectedTariff &&
                    int.TryParse(txtLicenseCount.Text, out int licenseCount) &&
                    licenseCount > 0)
                {
                    decimal totalCost = selectedTariff.FinalPrice * licenseCount;
                    txtTotalCost.Text = totalCost.ToString("F2");
                }
            }
            catch
            {
                txtTotalCost.Text = "0.00";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                // Заполняем данные заявки
                _application.ClientId = ((User)cmbClient.SelectedItem).Id;
                _application.TariffId = ((Tariff)cmbTariff.SelectedItem).Id;
                _application.LicenseCount = int.Parse(txtLicenseCount.Text);
                _application.TotalCost = decimal.Parse(txtTotalCost.Text);
                _application.Comment = txtComment.Text;
                _application.StatusId = int.Parse(((ComboBoxItem)cmbStatus.SelectedItem).Tag.ToString());

                if (_isNewApplication)
                {
                    _application.ApplicationDate = DateTime.Now;

                    // Проверяем свободные лицензии при подтверждении
                    if (_application.StatusId == 3)
                    {
                        var tariff = (Tariff)cmbTariff.SelectedItem;
                        if (tariff.AvailableLicenses < _application.LicenseCount)
                        {
                            MessageBox.Show($"Недостаточно свободных лицензий! Доступно: {tariff.AvailableLicenses}, требуется: {_application.LicenseCount}", "Ошибка");
                            return;
                        }
                    }

                    if (_dataService.AddApplication(_application))
                    {
                        MessageBox.Show("Заявка успешно создана!", "Успех");
                        DialogResult = true;
                        Close();
                    }
                }
                else
                {
                    if (_dataService.UpdateApplication(_application))
                    {
                        MessageBox.Show("Заявка успешно обновлена!", "Успех");
                        DialogResult = true;
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
            }
        }

        private bool ValidateInput()
        {
            if (cmbClient.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента", "Ошибка");
                return false;
            }

            if (cmbTariff.SelectedItem == null)
            {
                MessageBox.Show("Выберите тариф", "Ошибка");
                return false;
            }

            if (!int.TryParse(txtLicenseCount.Text, out int licenseCount) || licenseCount <= 0)
            {
                MessageBox.Show("Введите корректное количество лицензий", "Ошибка");
                return false;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус", "Ошибка");
                return false;
            }

            if (!decimal.TryParse(txtTotalCost.Text, out decimal totalCost) || totalCost <= 0)
            {
                MessageBox.Show("Введите корректную стоимость", "Ошибка");
                return false;
            }

            // Проверка на превышение максимальной вместимости
            var tariff = (Tariff)cmbTariff.SelectedItem;
            if (licenseCount > tariff.UserLimit)
            {
                MessageBox.Show($"Количество лицензий ({licenseCount}) превышает лимит тарифа ({tariff.UserLimit})", "Ошибка");
                return false;
            }

            // Проверка доступных лицензий для новых заявок
            if (_isNewApplication && licenseCount > tariff.AvailableLicenses)
            {
                MessageBox.Show($"Недостаточно свободных лицензий! Доступно: {tariff.AvailableLicenses}, требуется: {licenseCount}", "Ошибка");
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}