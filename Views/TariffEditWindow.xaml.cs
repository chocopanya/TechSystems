using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using TechSystems.Models;
using TechSystems.Services;

namespace TechSystems.Views
{
    public partial class TariffEditWindow : Window
    {
        private readonly DataService _dataService;
        private readonly Tariff _tariff;
        private string _selectedImageFileName;

        public TariffEditWindow(DataService dataService, Tariff tariff = null)
        {
            InitializeComponent();
            _dataService = dataService;
            _tariff = tariff ?? new Tariff();
            _selectedImageFileName = tariff?.PhotoFileName ?? string.Empty;

            LoadData();
            CalculateFinalPrice();

            // Подписка на события изменения цены и скидки
            txtPrice.TextChanged += OnPriceChanged;
            txtDiscount.TextChanged += OnPriceChanged;
        }

        private void LoadData()
        {
            try
            {
                // Загружаем типы услуг
                var serviceTypes = _dataService.GetServiceTypes();
                cmbServiceType.ItemsSource = serviceTypes;
                cmbServiceType.DisplayMemberPath = "Name";

                if (_tariff.Id > 0) // Редактирование
                {
                    Title = $"Редактирование тарифа: {_tariff.Name}";

                    txtName.Text = _tariff.Name;
                    txtSubscriptionMonths.Text = _tariff.SubscriptionMonths.ToString();
                    dpStartDate.SelectedDate = _tariff.StartDate;
                    txtPrice.Text = _tariff.Price.ToString("F2");
                    txtDiscount.Text = _tariff.Discount.ToString("F2");
                    txtUserLimit.Text = _tariff.UserLimit.ToString();
                    txtAvailableLicenses.Text = _tariff.AvailableLicenses.ToString();
                    txtDescription.Text = _tariff.Description;

                    // Выбираем тип услуги
                    foreach (ServiceType serviceType in cmbServiceType.Items)
                    {
                        if (serviceType.Id == _tariff.ServiceTypeId)
                        {
                            cmbServiceType.SelectedItem = serviceType;
                            break;
                        }
                    }

                    // Загружаем изображение, если есть
                    if (!string.IsNullOrEmpty(_tariff.PhotoFileName))
                    {
                        LoadImageFromFile(_tariff.PhotoFileName);
                    }
                }
                else // Добавление
                {
                    Title = "Добавление нового тарифа";

                    dpStartDate.SelectedDate = DateTime.Today.AddDays(30);

                    // Значения по умолчанию
                    if (serviceTypes.Any()) cmbServiceType.SelectedIndex = 0;
                    txtUserLimit.Text = "10";
                    txtAvailableLicenses.Text = "10";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        private void LoadImageFromFile(string fileName)
        {
            try
            {
                string[] possiblePaths = {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Debug", "Images", fileName),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release", "Images", fileName),
                    Path.Combine(Environment.CurrentDirectory, "Images", fileName)
                };

                string foundPath = null;
                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        foundPath = path;
                        break;
                    }
                }

                if (foundPath != null)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(foundPath);
                    bitmap.EndInit();

                    imgTariff.Source = bitmap;
                    txtNoPhoto.Visibility = Visibility.Collapsed;
                    _selectedImageFileName = fileName;
                }
                else
                {
                    string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "placeholder.png");
                    if (File.Exists(placeholderPath))
                    {
                        var placeholder = new BitmapImage();
                        placeholder.BeginInit();
                        placeholder.CacheOption = BitmapCacheOption.OnLoad;
                        placeholder.UriSource = new Uri(placeholderPath);
                        placeholder.EndInit();

                        imgTariff.Source = placeholder;
                    }
                    txtNoPhoto.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка");
            }
        }

        private void BtnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения PNG (*.png)|*.png|Изображения JPG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Все файлы (*.*)|*.*",
                Title = "Выберите изображение для тарифа",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourcePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(sourcePath);

                    string destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);

                    if (File.Exists(destPath))
                    {
                        string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                        string extension = Path.GetExtension(fileName);
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        fileName = $"{nameWithoutExt}_{timestamp}{extension}";
                        destPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", fileName);
                    }

                    File.Copy(sourcePath, destPath, true);
                    LoadImageFromFile(fileName);
                    MessageBox.Show($"Изображение '{fileName}' успешно загружено!", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}", "Ошибка");
                }
            }
        }

        private void BtnClearImage_Click(object sender, RoutedEventArgs e)
        {
            imgTariff.Source = null;
            txtNoPhoto.Visibility = Visibility.Visible;
            _selectedImageFileName = string.Empty;
        }

        private void OnPriceChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculateFinalPrice();
        }

        private void CalculateFinalPrice()
        {
            try
            {
                if (decimal.TryParse(txtPrice.Text, out decimal price) &&
                    decimal.TryParse(txtDiscount.Text, out decimal discount))
                {
                    decimal finalPrice = price * (1 - discount / 100);
                    txtFinalPrice.Text = $"{finalPrice:F2} руб.";

                    // Подсветка спецпредложения
                    if (discount > 15)
                    {
                        txtFinalPrice.Foreground = System.Windows.Media.Brushes.Red;
                        txtFinalPrice.Text += " (Спецпредложение!)";
                        specialOfferBorder.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        txtFinalPrice.Foreground = System.Windows.Media.Brushes.Green;
                        specialOfferBorder.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch
            {
                txtFinalPrice.Text = "0 руб.";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                // Заполняем данные тарифа
                _tariff.Name = txtName.Text.Trim();
                _tariff.ServiceTypeId = ((ServiceType)cmbServiceType.SelectedItem).Id;
                _tariff.SubscriptionMonths = int.Parse(txtSubscriptionMonths.Text);
                _tariff.StartDate = dpStartDate.SelectedDate ?? DateTime.Today;
                _tariff.Price = decimal.Parse(txtPrice.Text);
                _tariff.Discount = decimal.Parse(txtDiscount.Text);
                _tariff.UserLimit = int.Parse(txtUserLimit.Text);
                _tariff.AvailableLicenses = int.Parse(txtAvailableLicenses.Text);
                _tariff.Description = txtDescription.Text;
                _tariff.PhotoFileName = _selectedImageFileName;

                bool success;
                if (_tariff.Id > 0)
                {
                    success = _dataService.UpdateTariff(_tariff);
                }
                else
                {
                    success = _dataService.AddTariff(_tariff);
                }

                if (success)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка сохранения тарифа", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private bool ValidateInput()
        {
            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorMessage += "• Введите название тарифа\n";
            }

            if (cmbServiceType.SelectedItem == null)
            {
                errorMessage += "• Выберите тип услуги\n";
            }

            if (!int.TryParse(txtSubscriptionMonths.Text, out int months) || months <= 0)
            {
                errorMessage += "• Введите корректный срок подписки (больше 0 месяцев)\n";
            }

            if (dpStartDate.SelectedDate == null)
            {
                errorMessage += "• Выберите дату начала\n";
            }
            else if (dpStartDate.SelectedDate < DateTime.Today)
            {
                errorMessage += "• Дата начала не может быть в прошлом\n";
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                errorMessage += "• Введите корректную стоимость (больше 0)\n";
            }

            if (!decimal.TryParse(txtDiscount.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                errorMessage += "• Скидка должна быть от 0 до 100%\n";
            }

            if (!int.TryParse(txtUserLimit.Text, out int userLimit) || userLimit <= 0)
            {
                errorMessage += "• Введите корректный лимит пользователей (больше 0)\n";
            }

            if (!int.TryParse(txtAvailableLicenses.Text, out int availableLicenses) || availableLicenses < 0 || availableLicenses > userLimit)
            {
                errorMessage += $"• Количество доступных лицензий должно быть от 0 до {userLimit}\n";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show("Исправьте следующие ошибки:\n\n" + errorMessage, "Ошибка валидации");
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