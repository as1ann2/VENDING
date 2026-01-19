Avalonia.DataGrid 
AspNetCore.OpenApi
EntityFrameworkCore.Design 
EntityFrameworkCore.Sqlite 
EntityFrameworkCore.SqlServer 
EntityFrameworkCore.Tools 
VisualStudio.Web.CodeGeneration.Design 
Npgsql.EntityFrameworkCore.PostgreSQL 
Swashbuckle.AspNetCore.SwaggerGen 
Swashbuckle.AspNetCore.SeaggerUI
Avalonia.Reactive 
ReactiveUI

dotnet add package Selenium.WebDriver(/ChromeDriver)


dotnet ef dbcontext scaffold "Host=0.0.0.0;Username=postgres;Password=password;Database=DataBaseName" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir <Folder where you want to generate classes



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();





-- 1. Таблица производителей

CREATE TABLE manufacturers (

    manufacturer_id SERIAL PRIMARY KEY,
    
    name VARCHAR(100) NOT NULL UNIQUE,
    
    contact_person VARCHAR(100),
    
    phone VARCHAR(20),
    
    email VARCHAR(100),
    
    address TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    
);


-- 2. Таблица типов аппаратов

CREATE TABLE machine_types (

    type_id SERIAL PRIMARY KEY,
    
    type_name VARCHAR(50) NOT NULL UNIQUE,
    
    description TEXT,
    
    payment_methods VARCHAR(50) CHECK (payment_methods IN ('наличные', 'карта', 'два вида оплаты'))
    
);	

-- 3. Таблица пользователей/сотрудников

CREATE TABLE users (

    user_id SERIAL PRIMARY KEY,
    
    full_name VARCHAR(100) NOT NULL,
    
    email VARCHAR(100) UNIQUE NOT NULL,
    
    phone VARCHAR(20),
    
    role VARCHAR(20) CHECK (role IN ('администратор', 'оператор', 'инженер', 'франчайзер')),
    
    password_hash VARCHAR(255) NOT NULL,
    
    specialization TEXT, -- какие модели может обслуживать
    
    is_active BOOLEAN DEFAULT TRUE,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    
);

-- 4. Основная таблица вендинговых аппаратов (ВСЕ ПОЛЯ ПО ТЗ)

CREATE TABLE vending_machines (

    machine_id SERIAL PRIMARY KEY,
    
    serial_number VARCHAR(50) UNIQUE NOT NULL,
    
    inventory_number VARCHAR(50) UNIQUE NOT NULL,
    
    type_id INTEGER NOT NULL REFERENCES machine_types(type_id),
    
    manufacturer_id INTEGER NOT NULL REFERENCES manufacturers(manufacturer_id),
    
    model VARCHAR(100) NOT NULL,
    
    location TEXT NOT NULL, -- адрес установки
    
    production_date DATE NOT NULL, -- дата и
    зготовления
    commissioning_date DATE NOT NULL, -- дата ввода в эксплуатацию
    
    last_verification_date DATE, -- дата последней поверки
    
    verification_interval INTEGER, -- межповерочный интервал (месяцы)
    
    resource_hours INTEGER CHECK (resource_hours > 0), -- ресурс ТА в часах
    
    next_maintenance_date DATE, -- дата следующего ремонта/обслуживания
    
    service_time INTEGER CHECK (service_time >= 1 AND service_time <= 20), -- время обслуживания (часы)
    
    status VARCHAR(30) CHECK (status IN ('Работает', 'Вышел из строя', 'В ремонте/на обслуживании')),
    
    production_country VARCHAR(50) NOT NULL, -- страна производства
    
    inventory_date DATE, -- дата инвентаризации
    
    last_verification_employee INTEGER REFERENCES users(user_id), -- сотрудник последней поверки
    
    total_revenue DECIMAL(15,2) DEFAULT 0, -- совокупный доход
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    
    -- Проверки согласно ТЗ
    
    CONSTRAINT chk_commissioning_date CHECK (commissioning_date >= production_date),
    
    CONSTRAINT chk_verification_date CHECK (
    
        last_verification_date IS NULL OR 
        
        (last_verification_date >= production_date AND last_verification_date <= CURRENT_DATE)
        
    ),
    
    CONSTRAINT chk_inventory_date CHECK (
    
        inventory_date IS NULL OR 
        
        (inventory_date >= production_date AND inventory_date <= CURRENT_DATE)
        
    ),
    
    CONSTRAINT chk_next_maintenance CHECK (

        next_maintenance_date IS NULL OR 
        
        next_maintenance_date > created_at::DATE
        
    )
    
);

-- 5. Таблица товаров

CREATE TABLE products (

    product_id SERIAL PRIMARY KEY,
    
    name VARCHAR(100) NOT NULL,
    
    description TEXT,
    
    category VARCHAR(50),
    
    price DECIMAL(8,2) NOT NULL CHECK (price > 0),
    
    cost_price DECIMAL(8,2), -- себестоимость
    
    barcode VARCHAR(50),
    
    min_stock_level INTEGER DEFAULT 5, -- минимальный запас
    
    sales_tendency DECIMAL(8,2) DEFAULT 0, -- склонности к продажам (средние продажи в день)
    
    is_active BOOLEAN DEFAULT TRUE,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    
);

-- 6. Таблица инвентаря в аппаратах

CREATE TABLE machine_inventory (

    inventory_id SERIAL PRIMARY KEY,
    
    machine_id INTEGER NOT NULL REFERENCES vending_machines(machine_id),
    
    product_id INTEGER NOT NULL REFERENCES products(product_id),
    
    slot_number INTEGER NOT NULL,
    
    max_capacity INTEGER NOT NULL CHECK (max_capacity > 0),
    
    current_quantity INTEGER DEFAULT 0 CHECK (current_quantity >= 0),
    
    selling_price DECIMAL(8,2) NOT NULL CHECK (selling_price > 0),
    
    last_restocked DATE,
    
    
    UNIQUE(machine_id, slot_number)
    
);

-- 7. Таблица продаж

CREATE TABLE sales (

    sale_id SERIAL PRIMARY KEY,
    
    machine_id INTEGER NOT NULL REFERENCES vending_machines(machine_id),
    
    product_id INTEGER NOT NULL REFERENCES products(product_id),
    
    sale_datetime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    
    amount DECIMAL(8,2) NOT NULL CHECK (amount > 0),
    
    payment_method VARCHAR(10) CHECK (payment_method IN ('наличные', 'карта', 'QR'))
    
);

-- 8. Таблица технического обслуживания

CREATE TABLE maintenance_records (

    maintenance_id SERIAL PRIMARY KEY,
    
    machine_id INTEGER NOT NULL REFERENCES vending_machines(machine_id),
    
    maintenance_date DATE NOT NULL,
    
    maintenance_type VARCHAR(20) CHECK (maintenance_type IN ('плановое', 'аварийное', 'ремонт')),
    
    technician_id INTEGER REFERENCES users(user_id),
    
    description TEXT NOT NULL, -- описание работы
    
    problems TEXT, -- описание выявленных проблем
    
    parts_replaced TEXT, -- замененные детали
    
    work_duration_hours INTEGER CHECK (work_duration_hours > 0),
    
    next_maintenance_date DATE, -- следующее плановое обслуживание
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    
);

-- 9. Таблица задач/нарядов для инженеров

CREATE TABLE service_orders (

    order_id SERIAL PRIMARY KEY,
    
    machine_id INTEGER NOT NULL REFERENCES vending_machines(machine_id),
    
    assigned_engineer_id INTEGER REFERENCES users(user_id),
    
    order_type VARCHAR(20) CHECK (order_type IN ('плановое', 'аварийное')),
    
    status VARCHAR(20) CHECK (status IN ('Новая', 'В работе', 'Выполнена', 'Отменена')) DEFAULT 'Новая',
    
    scheduled_date DATE NOT NULL,
    
    deadline_date DATE,
    
    priority INTEGER DEFAULT 1 CHECK (priority >= 1 AND priority <= 3),
    
    description TEXT,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    
    accepted_at TIMESTAMP,
    
    completed_at TIMESTAMP
    
);

-- 10. Таблица истории статусов

CREATE TABLE status_history (

    history_id SERIAL PRIMARY KEY,
    
    machine_id INTEGER NOT NULL REFERENCES vending_machines(machine_id),
    
    old_status VARCHAR(30),

    new_status VARCHAR(30) NOT NULL,
    
    changed_by INTEGER REFERENCES users(user_id),
    
    change_reason TEXT,
    
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    
);


-- 11. Таблица уведомлений

CREATE TABLE notifications (

    notification_id SERIAL PRIMARY KEY,
    
    machine_id INTEGER REFERENCES vending_machines(machine_id),
    
    type VARCHAR(20) CHECK		 (type IN ('критическое', 'предупреждение', 'информационное')),
    
    title VARCHAR(100) NOT NULL,
    
    message TEXT NOT NULL,
    
    is_read BOOLEAN DEFAULT FALSE,
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    
);


-- 12. Таблица инкассации

CREATE TABLE cash_collections (	

    collection_id SERIAL PRIMARY KEY,
    
    machine_id INTEGER NOT NULL REFERENCES vending_machines(machine_id),
    
    collection_date DATE NOT NULL DEFAULT CURRENT_DATE,
    
    collector_name VARCHAR(100) NOT NULL,
    
    cash_amount DECIMAL(10,2) NOT NULL DEFAULT 0,
    
    card_amount DECIMAL(10,2) NOT NULL DEFAULT 0,
    
    total_amount DECIMAL(10,2) GENERATED ALWAYS AS (cash_amount + card_amount) STORED
);

indow xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="AvaloniaApplication.MainWindow"
        Width="1300"
        Height="800"
        Title="Личный кабинет">

    <Grid>

        <DockPanel LastChildFill="True">

            <!-- ===== ВЕРХНЯЯ БЕЛАЯ ПАНЕЛЬ ===== -->
            <Border DockPanel.Dock="Top"
                    Background="White"
                    Height="48">
                <Grid Margin="16,0">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*"/>
                        <ColumnDefinition Width="Auto"/>
                    </Grid.ColumnDefinitions>

                    <TextBlock Text="Лого"
                               VerticalAlignment="Center"
                               FontSize="16"/>

                    <Button x:Name="UserButton"
                            Grid.Column="1"
                            Background="Transparent"
                            BorderThickness="0"
                            Click="UserButton_Click">
                     
                            <StackPanel Orientation="Horizontal" Spacing="10">

                                <!-- Картинка слева -->
                                <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                                       Width="24"
                                       Height="24"
                                       VerticalAlignment="Center"/>

                                <!-- ФИО + роль -->
                                <StackPanel>
                                    <TextBlock Text="Автоматов А.А."
                                               FontWeight="SemiBold"/>
                                    <TextBlock Text="Администратор"
                                               FontSize="11"
                                               Foreground="Gray"/>
                                </StackPanel>

                                <!-- Стрелка -->
                                <TextBlock x:Name="UserArrow"
                                           Text="⌄"
                                           FontSize="14"
                                           VerticalAlignment="Center"/>
                            </StackPanel>

                    </Button>
                </Grid>
            </Border>

            <!-- ===== ЧЁРНАЯ ПАНЕЛЬ ===== -->
            <Border DockPanel.Dock="Top"
                    Background="Black"
                    Height="48">
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="260"/>
                        <ColumnDefinition Width="*"/>
                    </Grid.ColumnDefinitions>

                    <Button x:Name="BurgerButton"
                            Width="40"
                            Height="40"
                            Margin="8"
                            Background="Transparent"
                            Foreground="White"
                            FontSize="18"
                            Click="BurgerButton_Click">
                        ☰
                    </Button>


                    <TextBlock Grid.Column="1"
                               Text="ООО Торговые Автоматы"
                               Foreground="White"
                               FontSize="26"
                               VerticalAlignment="Center"
                               Margin="16,0,0,0"/>
                </Grid>
            </Border>

            <!-- ===== ОСНОВНОЙ КОНТЕНТ ===== -->
            <Grid x:Name="MainContentGrid">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="260"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>

              <!-- ===== ЛЕВОЕ МЕНЮ ===== -->
<Border Grid.Column="0"
        Background="Black">
    <StackPanel Margin="12">

        <TextBlock Text="Навигация"
                   Foreground="LightGray"
                   FontSize="14"
                   FontWeight="SemiBold"
                   Margin="8,8,0,12"/>

        <!-- ===== Обычные пункты ===== -->
        <StackPanel Orientation="Horizontal" Height="40" Spacing="10">
            <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                   Width="18" Height="18"/>
            <TextBlock Text="Главная"
                       Foreground="White"
                       VerticalAlignment="Center"/>
        </StackPanel>

        <Button Background="Transparent"
                BorderThickness="0"
                Click="Monitoring_Click">
            <Grid Height="40">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>

                <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                       Width="18"
                       Height="18"
                       Margin="0,0,10,0"/>

                <TextBlock Grid.Column="1"
                           Text="Монитор ТА"
                           Foreground="White"
                           VerticalAlignment="Center"/>
            </Grid>
        </Button>

        <StackPanel Orientation="Horizontal" Height="40" Spacing="10">
            <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                   Width="18" Height="18"/>
            <TextBlock Text="Детальные отчеты   ∨"
                       Foreground="White"
                       VerticalAlignment="Center"/>
        </StackPanel>

        <StackPanel Orientation="Horizontal" Height="40" Spacing="10">
            <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                   Width="18" Height="18"/>
            <TextBlock Text="Учет ТМЦ   ∨"
                       Foreground="White"
                       VerticalAlignment="Center"/>
        </StackPanel>

        <!-- ===== АДМИНИСТРИРОВАНИЕ ===== -->
        <Button Background="Transparent"
                BorderThickness="0"
                Click="AdminButton_Click">
            <Grid Height="40">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto"/>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>

                <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                       Width="18"
                       Height="18"
                       Margin="0,0,10,0"/>

                <TextBlock Grid.Column="1"
                           Text="Администрирование   ∨"
                           Foreground="White"
                           VerticalAlignment="Center"/>
            </Grid>
        </Button>

        <!-- ===== ПОДМЕНЮ ===== -->
        <StackPanel x:Name="AdminSubMenu"
                    IsVisible="False"
                    Margin="28,0,0,0"
                    Spacing="6">

            <Button Background="Transparent"
                    BorderThickness="0"
                    HorizontalContentAlignment="Left"
                    Click="TradingMachines_Click">

                <StackPanel Orientation="Horizontal" Height="32" Spacing="10">
                    <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                           Width="16"
                           Height="16"/>

                    <TextBlock Text="Торговые автоматы"
                               Foreground="LightGray"
                               VerticalAlignment="Center"/>
                </StackPanel>

            </Button>   


            <StackPanel Orientation="Horizontal" Height="32" Spacing="10">
                <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                       Width="16" Height="16"/>
                <TextBlock Text="Компании"
                           Foreground="LightGray"
                           VerticalAlignment="Center"/>
            </StackPanel>

            <StackPanel Orientation="Horizontal" Height="32" Spacing="10">
                <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                       Width="16" Height="16"/>
                <TextBlock Text="Пользователи"
                           Foreground="LightGray"
                           VerticalAlignment="Center"/>
            </StackPanel>

            <StackPanel Orientation="Horizontal" Height="32" Spacing="10">
                <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                       Width="16" Height="16"/>
                <TextBlock Text="Модемы"
                           Foreground="LightGray"
                           VerticalAlignment="Center"/>
            </StackPanel>

            <StackPanel Orientation="Horizontal" Height="32" Spacing="10">
                <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                       Width="16" Height="16"/>
                <TextBlock Text="Дополнительные"
                           Foreground="LightGray"
                           VerticalAlignment="Center"/>
            </StackPanel>
        </StackPanel>

    </StackPanel>
</Border>


             <ScrollViewer Grid.Column="1"
              Background="Gainsboro">

    <StackPanel Margin="24" Spacing="16">

        <!-- Заголовок страницы -->
        <TextBlock Text="Личный кабинет. Главная"
                   FontSize="22"
                   FontWeight="SemiBold"/>

        <!-- ===== ВЕРХНИЕ 3 БЛОКА ===== -->
        <Grid ColumnDefinitions="*,*,*" ColumnSpacing="16">

            <!-- Эффективность сети -->
            <Border Background="White" CornerRadius="6">
                <StackPanel>
                    <Border Background="LightGray"
                            Padding="10"
                            CornerRadius="6,6,0,0">
                        <TextBlock Text="Эффективность сети"
                                   FontWeight="SemiBold"/>
                    </Border>
                    <Border Height="90"/>
                </StackPanel>
            </Border>

            <!-- Состояние сети -->
            <Border Grid.Column="1"
                    Background="White"
                    CornerRadius="6">
                <StackPanel>
                    <Border Background="LightGray"
                            Padding="10"
                            CornerRadius="6,6,0,0">
                        <TextBlock Text="Состояние сети"
                                   FontWeight="SemiBold"/>
                    </Border>
                    <Border Height="90"/>
                </StackPanel>
            </Border>

            <!-- Сводка -->
            <Border Grid.Column="2"
                    Background="White"
                    CornerRadius="6">
                <StackPanel>
                    <Border Background="LightGray"
                            Padding="10"
                            CornerRadius="6,6,0,0">
                        <TextBlock Text="Сводка"
                                   FontWeight="SemiBold"/>
                    </Border>
                    <Border Height="90"/>
                </StackPanel>
            </Border>

        </Grid>

        <!-- ===== НИЖНИЙ РЯД ===== -->
        <Grid ColumnDefinitions="2*,*" ColumnSpacing="16">

            <!-- Динамика продаж -->
            <Border Background="White"
                    CornerRadius="6">
                <StackPanel>
                    <Border Background="LightGray"
                            Padding="10"
                            CornerRadius="6,6,0,0">
                        <TextBlock Text="Динамика продаж за последние 10 дней"
                                   FontWeight="SemiBold"/>
                    </Border>
                    <Border Height="260"/>
                </StackPanel>
            </Border>

            <!-- Новости -->
            <Border Grid.Column="1"
                    Background="White"
                    CornerRadius="6">
                <StackPanel>
                    <Border Background="LightGray"
                            Padding="10"
                            CornerRadius="6,6,0,0">
                        <TextBlock Text="Новости"
                                   FontWeight="SemiBold"/>
                    </Border>
                    <Border Height="260"/>
                </StackPanel>
            </Border>

        </Grid>

    </StackPanel>
</ScrollViewer>

            </Grid>

        </DockPanel>

       <!-- ===== USER POPUP ===== -->
<Border x:Name="UserPopup"
        IsVisible="False"
        Background="White"
        Width="240"
        CornerRadius="6"
        BoxShadow="0 6 16 Gray"
        HorizontalAlignment="Right"
        VerticalAlignment="Top"
        Margin="0,48,16,0"
        ZIndex="100">

    <StackPanel>

        <!-- ===== HEADER ===== -->
        <Grid Margin="12">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>

            <!-- Флаг -->
            <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                   Width="28"
                   Height="18"
                   VerticalAlignment="Center"/>

            <!-- Имя + роль -->
            <StackPanel Grid.Column="1" Margin="10,0">
                <TextBlock Text="Автоматов А.А."
                           FontWeight="SemiBold"/>
                <TextBlock Text="Администратор"
                           FontSize="11"
                           Foreground="Gray"/>
            </StackPanel>

            <!-- Стрелка -->
            <TextBlock Grid.Column="2"
                       Text="⌃"
                       FontSize="16"
                       VerticalAlignment="Center"/>
        </Grid>

        <!-- Разделитель -->
        <Border Height="1"
                Background="#E0E0E0"
                Margin="0,4"/>

        <!-- ===== МЕНЮ ===== -->
        <StackPanel Margin="6">

            <!-- Мой профиль -->
            <Button Background="Transparent"
                    BorderThickness="0"
                    HorizontalContentAlignment="Left">
                <StackPanel Orientation="Horizontal" Spacing="10">
                    <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                           Width="18" Height="18"/>
                    <TextBlock Text="Мой профиль"
                               VerticalAlignment="Center"
                               Foreground="#555"/>
                </StackPanel>
            </Button>

            <!-- Мои сессии -->
            <Button Background="Transparent"
                    BorderThickness="0"
                    HorizontalContentAlignment="Left">
                <StackPanel Orientation="Horizontal" Spacing="10">
                    <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                           Width="18" Height="18"/>
                    <TextBlock Text="Мои сессии"
                               VerticalAlignment="Center"
                               Foreground="#555"/>
                </StackPanel>
            </Button>

            <!-- Выход -->
            <Button Background="Transparent"
                    BorderThickness="0"
                    Click="Exit_Click"
                    HorizontalContentAlignment="Left">
                <StackPanel Orientation="Horizontal" Spacing="10">
                    <Image Source="avares://AvaloniaApplication/Assets/profile.png"
                           Width="18" Height="18"/>
                    <TextBlock Text="Выход"
                               VerticalAlignment="Center"
                               Foreground="#555"/>
                </StackPanel>
            </Button>

        </StackPanel>

    </StackPanel>
</Border>


    </Grid>
</Window>





КОД БЕХИНГ

using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AvaloniaApplication
{
    public partial class MainWindow : Window
    {
        private bool _menuOpened = true;
        private bool _adminOpened;
        private bool _userOpened;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BurgerButton_Click(object? sender, RoutedEventArgs e)
        {
            var col = MainContentGrid.ColumnDefinitions[0];
            col.Width = _menuOpened ? new GridLength(0) : new GridLength(260);
            _menuOpened = !_menuOpened;
        }

        private void AdminButton_Click(object? sender, RoutedEventArgs e)
        {
            _adminOpened = !_adminOpened;
            AdminSubMenu.IsVisible = _adminOpened;
        }

        private void UserButton_Click(object? sender, RoutedEventArgs e)
        {
            _userOpened = !_userOpened;
            UserPopup.IsVisible = _userOpened;
            UserArrow.Text = _userOpened ? "⌄" : "›";
        }
        
        private void TradingMachines_Click(object? sender, RoutedEventArgs e)
        {
            var window = new TradingMachineWindow();
            window.Show();
            this.Close();
        }
        
        private void Monitoring_Click(object? sender, RoutedEventArgs e)
        {
            var window = new MonitoringWindow();
            window.Show();
            this.Close();
        }
        
        private void Exit_Click(object? sender, RoutedEventArgs e)
        {
            this.Close();
        }
     

    }
}


AuthController.cs

using Microsoft.AspNetCore.Mvc;

namespace VendingAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request.Username != "admin" || request.Password != "password")
        {
            return Unauthorized();
        }

        return Ok(new
        {
            accessToken = "fake-access-token",
            refreshToken = "fake-refresh-token"
        });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}







________________________________________________________________________________________
using Microsoft.EntityFrameworkCore;
using VendingAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// 🔹 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// 🔹 DbContext — ОБЯЗАТЕЛЬНО ДО Build()
builder.Services.AddDbContext<VendingprofContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// 🔹 Build — ТОЛЬКО ПОСЛЕ ВСЕХ Add*
var app = builder.Build();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
____________________________________________________________________________________--
<img width="373" height="517" alt="image" src="https://github.com/user-attachments/assets/870890a5-ff8f-431e-978b-14ea05dfdf83" />
POST Login

// Debug logging
console.log('Request URL:', pm.request.url.toString());
console.log('Response Status:', pm.response.code);
console.log('Response Body:', pm.response.text());

// Handle responses
if (pm.response.code === 404) {
    pm.test('Endpoint exists', function () {
        pm.expect.fail(
            'Endpoint not found (404). ' +
            'Check baseUrl and make sure /api/auth/login exists on the server.'
        );
    });
} 
else if (pm.response.code === 200) {

    pm.test('Status code is 200', function () {
        pm.response.to.have.status(200);
    });

    const json = pm.response.json();

    pm.test('Response has accessToken', function () {
        pm.expect(json).to.have.property('accessToken');
    });

    pm.test('Response has refreshToken', function () {
        pm.expect(json).to.have.property('refreshToken');
    });

    // Store tokens in collection variables
    pm.collectionVariables.set('accessToken', json.accessToken);
    pm.collectionVariables.set('refreshToken', json.refreshToken);
} 
else {
    pm.test('Unexpected status code', function () {
        pm.expect.fail(
            'Unexpected status code: ' + pm.response.code +
            '. Expected 200 or handled 404.'
        );
    });
}
POT Refreh Token 

pm.response.to.have.status(200);
const json = pm.response.json();
pm.expect(json).to.have.property('accessToken');
pm.environment.set('accessToken', json.accessToken);

POST Logout

pm.response.to.have.status(200);

GET Список аппаратов 

pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

const data = pm.response.json();

pm.test("Response is array", () => {
    pm.expect(data).to.be.an("array");
});

pm.test("If array not empty, vending machine has required fields", () => {
    if (data.length > 0) {
        pm.expect(data[0]).to.have.property("name");
        pm.expect(data[0]).to.have.property("location");
        pm.expect(data[0]).to.have.property("status");
    }
});

АППАРАТ ПО АЙДИ 







