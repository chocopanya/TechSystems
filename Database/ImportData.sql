-- Вставка данных в правильном порядке
PRINT 'Вставка данных в Roles...'
INSERT INTO Roles (RoleName) VALUES 
(N'Администратор'),
(N'Менеджер'),
(N'Авторизированный клиент');
GO

PRINT 'Вставка данных в ServiceTypes...'
INSERT INTO ServiceTypes (TypeName) VALUES 
(N'Облачный хостинг'),
(N'CRM-система'),
(N'BI-инструменты'),
(N'Виртуальные серверы'),
(N'Защита данных');
GO

PRINT 'Вставка данных в ApplicationStatuses...'
INSERT INTO ApplicationStatuses (StatusName) VALUES 
(N'Новая'),
(N'В обработке'),
(N'Подтверждена'),
(N'Отменена'),
(N'Завершена');
GO

PRINT 'Вставка данных в PaymentMethods...'
INSERT INTO PaymentMethods (MethodName) VALUES 
(N'Банковская карта'),
(N'Безналичный расчет'),
(N'Электронные деньги'),
(N'Наличные');
GO

PRINT 'Вставка данных в PaymentStatuses...'
INSERT INTO PaymentStatuses (StatusName) VALUES 
(N'Оплачено'),
(N'В ожидании'),
(N'Отменено'),
(N'Возврат');
GO

PRINT 'Вставка данных в Users...'
INSERT INTO Users (RoleID, FullName, Login, PasswordHash, Email, CompanyName) VALUES
(1, N'Соколов Андрей Иванович', 'admin@techsys.ru', 'a1b2c3', 'admin@techsys.ru', N'ТехноСистемы'),
(2, N'Ковалева Елена Петровна', 'manager@techsys.ru', 'd4e5f6', 'manager@techsys.ru', N'ТехноСистемы'),
(3, N'Белов Денис Сергеевич', 'd.belov@mail.ru', 'g7h8i9', 'd.belov@mail.ru', N'ИТ-Сервис'),
(3, N'Григорьева Анастасия Викторовна', 'a.grigorieva@yandex.ru', 'j0k1l2', 'a.grigorieva@yandex.ru', N'ТехноПрофи'),
(3, N'Иванов Петр Александрович', 'p.ivanov@company.ru', 'm3n4o5', 'p.ivanov@company.ru', N'Рога и копыта'),
(3, N'Смирнова Ольга Викторовна', 'o.smirnova@firm.com', 'p6q7r8', 'o.smirnova@firm.com', N'СмартСолюшенс');
GO

PRINT 'Вставка данных в Tariffs...'
INSERT INTO Tariffs (TariffName, ServiceTypeID, SubscriptionMonths, StartDate, Price, UserLimit, AvailableLicenses, Discount, PhotoFileName, Description) VALUES
(N'Базовый Cloud', 1, 12, '2024-06-01', 12000.00, 10, 45, 0.00, 'cloud_basic.png', N'Базовый облачный хостинг для небольших проектов'),
(N'Корпоративный CRM', 2, 6, '2024-07-01', 45000.00, 50, 15, 10.00, 'crm_corp.png', N'Полнофункциональная CRM-система для корпоративных клиентов'),
(N'Бизнес-аналитика', 3, 12, '2024-08-01', 78000.00, 25, 8, 20.00, 'bi_tools.png', N'Продвинутые инструменты бизнес-аналитики'),
(N'Стартап-пакет', 4, 3, '2024-09-01', 25000.00, 5, 20, 25.00, 'vps_startup.png', N'Виртуальные серверы для стартапов'),
(N'Enterprise Security', 5, 12, '2024-10-01', 120000.00, 100, 3, 0.00, 'security_ent.png', N'Комплексная защита данных для предприятий'),
(N'Продвинутый Cloud', 1, 24, '2024-06-15', 25000.00, 20, 30, 15.00, 'cloud_pro.png', N'Продвинутый облачный хостинг с SLA 99.9%'),
(N'Бюджетный VPS', 4, 1, '2024-07-10', 5000.00, 3, 50, 5.00, 'vps_budget.png', N'Бюджетные виртуальные серверы');
GO

PRINT 'Вставка данных в Applications...'
INSERT INTO Applications (TariffID, ClientID, ApplicationDate, StatusID, LicenseCount, TotalCost, Comment) VALUES
(1, 3, '2024-05-10', 3, 5, 60000.00, N'Срочное подключение'),
(2, 4, '2024-05-12', 1, 10, 450000.00, N'Тестовый период'),
(3, 3, '2024-05-15', 2, 3, 234000.00, N'Дополнительные опции'),
(4, 5, '2024-05-20', 3, 2, 50000.00, N'Для нового проекта'),
(5, 6, '2024-05-25', 4, 1, 120000.00, N'Отмена по инициативе клиента'),
(6, 4, '2024-06-01', 3, 15, 375000.00, N'Расширение инфраструктуры'),
(1, 5, '2024-06-05', 2, 8, 96000.00, NULL);
GO

PRINT 'Вставка данных в Payments...'
INSERT INTO Payments (ClientID, PaymentDate, Amount, PaymentMethodID, StatusID) VALUES
(3, '2024-05-05', 60000.00, 1, 1),
(4, '2024-05-08', 450000.00, 2, 2),
(5, '2024-05-22', 50000.00, 2, 1),
(6, '2024-05-28', 120000.00, 1, 3),
(4, '2024-06-03', 375000.00, 2, 1);
GO

PRINT 'Вставка данных в ApplicationStatusHistory...'
INSERT INTO ApplicationStatusHistory (ApplicationID, OldStatusID, NewStatusID, ChangeDate, ChangedByUserID, Comment) VALUES
(1, 1, 2, '2024-05-11 10:30:00', 2, N'Принято в работу'),
(1, 2, 3, '2024-05-12 14:15:00', 2, N'Подтверждено, отправлен счет'),
(2, 1, 2, '2024-05-13 09:20:00', 2, N'На проверке документов'),
(3, 1, 2, '2024-05-16 11:45:00', 2, N'Уточняются технические требования'),
(5, 1, 4, '2024-05-26 16:30:00', 2, N'Отменено по просьбе клиента');
GO

-- Проверка данных
PRINT ''
PRINT '=== ПРОВЕРКА ДАННЫХ ==='
PRINT ''

SELECT 'Roles' as TableName, COUNT(*) as Count FROM Roles
UNION ALL SELECT 'Users', COUNT(*) FROM Users
UNION ALL SELECT 'Tariffs', COUNT(*) FROM Tariffs
UNION ALL SELECT 'Applications', COUNT(*) FROM Applications
UNION ALL SELECT 'Payments', COUNT(*) FROM Payments
UNION ALL SELECT 'ServiceTypes', COUNT(*) FROM ServiceTypes
UNION ALL SELECT 'ApplicationStatuses', COUNT(*) FROM ApplicationStatuses
UNION ALL SELECT 'PaymentMethods', COUNT(*) FROM PaymentMethods
UNION ALL SELECT 'PaymentStatuses', COUNT(*) FROM PaymentStatuses
UNION ALL SELECT 'ApplicationStatusHistory', COUNT(*) FROM ApplicationStatusHistory
ORDER BY TableName;
GO

PRINT ''
PRINT '=== ТАРИФЫ ==='
SELECT 
    TariffID as 'Код',
    TariffName as 'Название',
    SubscriptionMonths as 'Месяцев',
    StartDate as 'Начало',
    Price as 'Цена',
    AvailableLicenses as 'Лицензий',
    Discount as 'Скидка %'
FROM Tariffs
ORDER BY TariffID;
GO

PRINT ''
PRINT '=== ЗАЯВКИ ==='
SELECT 
    a.ApplicationID as 'Код заявки',
    t.TariffName as 'Тариф',
    u.FullName as 'Клиент',
    a.ApplicationDate as 'Дата заявки',
    s.StatusName as 'Статус',
    a.LicenseCount as 'Лицензий',
    a.TotalCost as 'Стоимость'
FROM Applications a
JOIN Tariffs t ON a.TariffID = t.TariffID
JOIN Users u ON a.ClientID = u.UserID
JOIN ApplicationStatuses s ON a.StatusID = s.StatusID
ORDER BY a.ApplicationID;
GO

PRINT ''
PRINT '=== ТЕСТОВЫЕ ДАННЫЕ УСПЕШНО ЗАГРУЖЕНЫ ==='
PRINT ''
PRINT 'Тестовые учетные записи:'
PRINT '1. Администратор: admin@techsys.ru / a1b2c3'
PRINT '2. Менеджер: manager@techsys.ru / d4e5f6'
PRINT '3. Клиент: d.belov@mail.ru / g7h8i9'
PRINT ''
PRINT 'Для гостевого доступа просто нажмите "Войти" без ввода логина/пароля.'
GO