using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using TechSystems.Data;
using TechSystems.Models;

namespace TechSystems.Services
{
    public class DataService : IDisposable
    {
        private readonly AppDbContext _context;

        public DataService()
        {
            try
            {
                _context = new AppDbContext();
                // Проверяем подключение к БД
                _context.Database.Connection.Open();
                _context.Database.Connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания DataService: {ex.Message}", "Ошибка");
                throw;
            }
        }

        // === ТАРИФЫ ===
        public List<Tariff> GetAllTariffs()
        {
            try
            {
                return _context.Tariffs
                    .Include(t => t.ServiceType)
                    .AsNoTracking()
                    .OrderBy(t => t.StartDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки тарифов: {ex.Message}", "Ошибка");
                return new List<Tariff>();
            }
        }

        public List<Tariff> GetAvailableTariffs()
        {
            try
            {
                return _context.Tariffs
                    .Include(t => t.ServiceType)
                    .AsNoTracking()
                    .Where(t => t.AvailableLicenses > 0)
                    .OrderBy(t => t.StartDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки доступных тарифов: {ex.Message}", "Ошибка");
                return new List<Tariff>();
            }
        }

        public Tariff GetTariffById(int id)
        {
            try
            {
                return _context.Tariffs
                    .Include(t => t.ServiceType)
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Id == id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки тарифа: {ex.Message}", "Ошибка");
                return null;
            }
        }

        public bool UpdateTariffLicenses(int tariffId, int licenseChange)
        {
            try
            {
                var tariff = _context.Tariffs.Find(tariffId);
                if (tariff != null)
                {
                    tariff.AvailableLicenses += licenseChange;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления лицензий: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool AddTariff(Tariff tariff)
        {
            try
            {
                _context.Tariffs.Add(tariff);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления тарифа: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool UpdateTariff(Tariff tariff)
        {
            try
            {
                var existingTariff = _context.Tariffs.Find(tariff.Id);
                if (existingTariff != null)
                {
                    _context.Entry(existingTariff).CurrentValues.SetValues(tariff);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления тарифа: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool DeleteTariff(int tariffId)
        {
            try
            {
                var tariff = _context.Tariffs.Find(tariffId);
                if (tariff != null)
                {
                    // Проверяем, есть ли активные заявки на этот тариф
                    var hasApplications = _context.ServiceRequests.Any(a => a.TariffId == tariffId);
                    if (hasApplications)
                    {
                        MessageBox.Show("Нельзя удалить тариф, на который есть заявки", "Ошибка");
                        return false;
                    }

                    _context.Tariffs.Remove(tariff);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления тарифа: {ex.Message}", "Ошибка");
                return false;
            }
        }

        // === ТИПЫ УСЛУГ ===
        public List<ServiceType> GetServiceTypes()
        {
            try
            {
                return _context.ServiceTypes
                    .AsNoTracking()
                    .OrderBy(s => s.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов услуг: {ex.Message}", "Ошибка");
                return new List<ServiceType>();
            }
        }

        // === ПОЛЬЗОВАТЕЛИ ===
        public List<User> GetAllUsers()
        {
            try
            {
                return _context.Users
                    .AsNoTracking()
                    .OrderBy(u => u.FullName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка");
                return new List<User>();
            }
        }

        public List<User> GetClients()
        {
            try
            {
                return _context.Users
                    .Where(u => u.RoleId == 3) // Только клиенты
                    .AsNoTracking()
                    .OrderBy(u => u.FullName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка");
                return new List<User>();
            }
        }

        public User GetUserByLogin(string login)
        {
            try
            {
                return _context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Login.Equals(login));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска пользователя: {ex.Message}", "Ошибка");
                return null;
            }
        }

        public User GetUserById(int id)
        {
            try
            {
                return _context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Id == id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователя: {ex.Message}", "Ошибка");
                return null;
            }
        }

        // === ЗАЯВКИ (ServiceRequests) ===
        public List<ServiceRequest> GetAllApplications()
        {
            try
            {
                return _context.ServiceRequests
                    .Include(a => a.Tariff)
                    .Include(a => a.Client)
                    .AsNoTracking()
                    .OrderByDescending(a => a.ApplicationDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка");
                return new List<ServiceRequest>();
            }
        }

        public List<ServiceRequest> GetApplicationsFiltered(string searchText = "", string statusFilter = "Все")
        {
            try
            {
                IQueryable<ServiceRequest> query = _context.ServiceRequests
                    .Include(a => a.Tariff)
                    .Include(a => a.Client)
                    .AsNoTracking();

                // Фильтрация по поиску
                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(a =>
                        a.Id.ToString().Contains(searchText) ||
                        a.Client.FullName.Contains(searchText) ||
                        a.Tariff.Name.Contains(searchText));
                }

                // Фильтрация по статусу
                if (statusFilter != "Все")
                {
                    int statusId = 0;

                    if (statusFilter == "Новая")
                        statusId = 1;
                    else if (statusFilter == "В обработке")
                        statusId = 2;
                    else if (statusFilter == "Подтверждена")
                        statusId = 3;
                    else if (statusFilter == "Отменена")
                        statusId = 4;
                    else if (statusFilter == "Завершена")
                        statusId = 5;

                    if (statusId > 0)
                    {
                        query = query.Where(a => a.StatusId == statusId);
                    }
                }

                return query.OrderByDescending(a => a.ApplicationDate).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации заявок: {ex.Message}", "Ошибка");
                return new List<ServiceRequest>();
            }
        }

        public ServiceRequest GetApplicationById(int id)
        {
            try
            {
                return _context.ServiceRequests
                    .Include(a => a.Tariff)
                    .Include(a => a.Client)
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Id == id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявки: {ex.Message}", "Ошибка");
                return null;
            }
        }

        public bool AddApplication(ServiceRequest application)
        {
            try
            {
                _context.ServiceRequests.Add(application);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления заявки: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool UpdateApplication(ServiceRequest application)
        {
            try
            {
                var existingApp = _context.ServiceRequests.Find(application.Id);
                if (existingApp != null)
                {
                    // Проверяем изменение статуса на "Подтверждена"
                    if (existingApp.StatusId != 3 && application.StatusId == 3)
                    {
                        var tariff = _context.Tariffs.Find(application.TariffId);
                        if (tariff != null && tariff.AvailableLicenses < application.LicenseCount)
                        {
                            MessageBox.Show($"Недостаточно свободных лицензий! Доступно: {tariff.AvailableLicenses}, требуется: {application.LicenseCount}", "Ошибка");
                            return false;
                        }

                        // Уменьшаем количество свободных лицензий
                        tariff.AvailableLicenses -= application.LicenseCount;
                    }
                    // Если статус меняется с "Подтверждена" на другой
                    else if (existingApp.StatusId == 3 && application.StatusId != 3)
                    {
                        var tariff = _context.Tariffs.Find(application.TariffId);
                        if (tariff != null)
                        {
                            // Возвращаем лицензии
                            tariff.AvailableLicenses += existingApp.LicenseCount;
                        }
                    }

                    _context.Entry(existingApp).CurrentValues.SetValues(application);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления заявки: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool DeleteApplication(int applicationId)
        {
            try
            {
                var application = _context.ServiceRequests.Find(applicationId);
                if (application != null)
                {
                    // Если удаляем подтвержденную заявку, возвращаем лицензии
                    if (application.StatusId == 3)
                    {
                        var tariff = _context.Tariffs.Find(application.TariffId);
                        if (tariff != null)
                        {
                            tariff.AvailableLicenses += application.LicenseCount;
                        }
                    }

                    _context.ServiceRequests.Remove(application);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления заявки: {ex.Message}", "Ошибка");
                return false;
            }
        }

        // === ТЕСТ БД ===
        public string TestDatabase()
        {
            try
            {
                var tariffsCount = _context.Tariffs.Count();
                var usersCount = _context.Users.Count();
                var serviceTypesCount = _context.ServiceTypes.Count();
                var appsCount = _context.ServiceRequests.Count();

                return $"✅ База данных подключена!\n\n" +
                       $"📊 Тарифов: {tariffsCount}\n" +
                       $"👤 Пользователей: {usersCount}\n" +
                       $"🔧 Типов услуг: {serviceTypesCount}\n" +
                       $"📋 Заявок: {appsCount}";
            }
            catch (Exception ex)
            {
                return $"❌ Ошибка подключения к БД:\n{ex.Message}";
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}