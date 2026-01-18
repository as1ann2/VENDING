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
