USE [TechSystemsDB];
GO

-- Очистка данных в правильном порядке
DELETE FROM Payments;
DELETE FROM Applications;
DELETE FROM Tariffs;
DELETE FROM Services;
DELETE FROM ServiceCategories;
DELETE FROM Users;
DELETE FROM PaymentStatuses;
DELETE FROM PaymentMethods;
DELETE FROM ApplicationStatuses;
DELETE FROM Roles;
GO

-- Сброс идентификаторов
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('ApplicationStatuses', RESEED, 0);
DBCC CHECKIDENT ('PaymentMethods', RESEED, 0);
DBCC CHECKIDENT ('PaymentStatuses', RESEED, 0);
DBCC CHECKIDENT ('ServiceCategories', RESEED, 0);
DBCC CHECKIDENT ('Services', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Tariffs', RESEED, 0);
DBCC CHECKIDENT ('Applications', RESEED, 0);
DBCC CHECKIDENT ('Payments', RESEED, 0);
GO

-- 1. Роли
INSERT INTO Roles (RoleName) VALUES 
(N'Администратор'),
(N'Менеджер'),
(N'Авторизированный клиент');
GO

-- 2. Статусы заявок
INSERT INTO ApplicationStatuses (StatusName) VALUES 
(N'Новая'),
(N'В обработке'),
(N'Подтверждена'),
(N'Отменена');
GO

-- 3. Способы оплаты
INSERT INTO PaymentMethods (MethodName) VALUES 
(N'Банковская карта'),
(N'Безналичный расчет'),
(N'Электронный кошелек'),
(N'Наличные');
GO

-- 4. Статусы платежей
INSERT INTO PaymentStatuses (StatusName) VALUES 
(N'Оплачено'),
(N'В ожидании'),
(N'Отменен'),
(N'Возврат');
GO

-- 5. Категории услуг
INSERT INTO ServiceCategories (CategoryName) VALUES 
(N'Cloud'),
(N'Infrastructure'),
(N'Backup'),
(N'Security'),
(N'CRM'),
(N'Analytics');
GO

-- 6. Услуги
INSERT INTO Services (ServiceName, CategoryID, Description) VALUES
(N'Облачный хостинг', 1, N'Размещение и поддержка веб-приложений в облаке'),
(N'Виртуальные серверы', 2, N'Выделенные VPS и контейнеры для инфраструктуры'),
(N'Резервное копирование', 3, N'Автоматическое бэкапирование данных'),
(N'Защита данных', 4, N'Комплексная система защиты информации'),
(N'CRM-система', 5, N'Управление взаимоотношениями с клиентами'),
(N'BI-инструменты', 6, N'Инструменты бизнес-аналитики');
GO

-- 7. Пользователи
INSERT INTO Users (RoleID, FullName, Email, Login, PasswordHash, CreatedDate) VALUES
(1, N'Соколов Андрей Иванович', 'admin@techsys.ru', 'admin@techsys.ru', 'a1b2c3', GETDATE()),
(2, N'Ковалева Елена Петровна', 'manager@techsys.ru', 'manager@techsys.ru', 'd4e5f6', GETDATE()),
(3, N'Белов Денис Сергеевич', 'd.belov@mail.ru', 'd.belov@mail.ru', 'g7h8i9', GETDATE()),
(3, N'Григорьева Анастасия Викторовна', 'a.grigorieva@yandex.ru', 'a.grigorieva@yandex.ru', 'j0k1l2', GETDATE()),
(3, N'Петров Иван Сергеевич', 'i.petrov@company.ru', 'i.petrov@company.ru', 'password123', GETDATE()),
(3, N'Сидорова Ольга Михайловна', 'o.sidorova@business.com', 'o.sidorova@business.com', 'securepass', GETDATE()),
(3, N'Кузнецов Алексей Дмитриевич', 'a.kuznetsov@startup.io', 'a.kuznetsov@startup.io', 'startup2024', GETDATE());
GO

-- 8. Тарифы
INSERT INTO Tariffs (TariffName, ServiceID, SubscriptionPeriodMonths, StartDate, Price, Discount, UserLimit, AvailableLicenses, PhotoFileName, Description) VALUES
(N'Базовый Cloud', 1, 12, '2024-06-01', 12000.00, 10.00, 10, 45, 'cloud_basic.png', N'Базовый облачный хостинг для небольших проектов'),
(N'Корпоративный CRM', 5, 6, '2024-07-01', 45000.00, 15.00, 50, 15, 'crm_corp.png', N'Полнофункциональная CRM система для корпоративных клиентов'),
(N'Бизнес-аналитика Pro', 6, 12, '2024-08-01', 78000.00, 5.00, 25, 8, 'bi_tools.png', N'Продвинутые инструменты бизнес-аналитики'),
(N'Стартап-пакет VPS', 2, 3, '2024-09-01', 25000.00, 20.00, 5, 20, 'vps_startup.png', N'Виртуальные серверы для стартапов'),
(N'Enterprise Security', 4, 12, '2024-10-01', 120000.00, 0.00, 100, 3, 'security_ent.png', N'Комплексная защита данных для предприятий'),
(N'Премиум Cloud', 1, 24, '2024-11-01', 85000.00, 25.00, 100, 12, 'cloud_premium.png', N'Премиум облачный хостинг с максимальными ресурсами'),
(N'Бизнес Backup', 3, 6, '2024-12-01', 35000.00, 12.00, 50, 25, 'backup_business.png', N'Резервное копирование для бизнеса');
GO

-- 9. Заявки
INSERT INTO Applications (TariffID, ClientID, ApplicationDate, StatusID, LicenseCount, TotalCost, Comment) VALUES
(1, 3, '2024-05-10', 3, 5, 54000.00, N'Срочное подключение для нового проекта'),
(2, 4, '2024-05-12', 1, 10, 382500.00, N'Тестовый период для оценки функциональности'),
(3, 3, '2024-05-15', 2, 3, 222300.00, N'Дополнительные опции для аналитики'),
(4, 5, '2024-05-20', 3, 2, 40000.00, N'Для разработки MVP'),
(5, 6, '2024-05-22', 1, 1, 120000.00, N'Защита финансовых данных'),
(1, 7, '2024-05-25', 3, 8, 86400.00, N'Миграция существующего проекта'),
(2, 5, '2024-05-28', 4, 5, 191250.00, N'Отмена по причине изменения требований'),
(3, 6, '2024-05-30', 2, 2, 148200.00, NULL),
(4, 7, '2024-06-01', 3, 3, 60000.00, N'Расширение инфраструктуры');
GO

-- 10. Платежи
INSERT INTO Payments (ClientID, PaymentDate, Amount, PaymentMethodID, PaymentStatusID, ApplicationID) VALUES
(3, '2024-05-05', 54000.00, 1, 1, 1),
(4, '2024-05-08', 382500.00, 2, 2, 2),
(5, '2024-05-18', 40000.00, 1, 1, 4),
(6, '2024-05-21', 120000.00, 2, 2, 5),
(7, '2024-05-24', 86400.00, 1, 1, 6),
(7, '2024-06-02', 60000.00, 2, 1, 9),
(3, '2024-05-16', 222300.00, 1, 2, 3),
(6, '2024-05-31', 148200.00, 2, 2, 8);
GO

-- Проверка данных
SELECT 'Roles' as TableName, COUNT(*) as Count FROM Roles
UNION ALL SELECT 'ApplicationStatuses', COUNT(*) FROM ApplicationStatuses
UNION ALL SELECT 'PaymentMethods', COUNT(*) FROM PaymentMethods
UNION ALL SELECT 'PaymentStatuses', COUNT(*) FROM PaymentStatuses
UNION ALL SELECT 'ServiceCategories', COUNT(*) FROM ServiceCategories
UNION ALL SELECT 'Services', COUNT(*) FROM Services
UNION ALL SELECT 'Users', COUNT(*) FROM Users
UNION ALL SELECT 'Tariffs', COUNT(*) FROM Tariffs
UNION ALL SELECT 'Applications', COUNT(*) FROM Applications
UNION ALL SELECT 'Payments', COUNT(*) FROM Payments;
GO

