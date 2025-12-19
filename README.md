# Bookstore Management API

## Опис проєкту

**Bookstore Management API** — це backend-платформа для автоматизації роботи мережі книжкових магазинів. Проєкт реалізований на архітектурі **N-Layer** і надає RESTful API для управління каталогом книг, складськими запасами, обробки замовлень, роботи з клієнтами та аналізу бізнес-показників.

## Призначення

Проєкт реалізує повний цикл управління книжковим бізнесом:
* **Управління користувачами:** Реєстрація, автентифікація, профілі покупців.
* **Каталог товарів:** Книги, автори, видавництва з детальним описом та цінами.
* **Складський облік (Multi-store Inventory):** Відстеження залишків книг у різних фізичних магазинах мережі.
* **Система замовлень:** Оформлення покупок, історія замовлень, деталізація чеків.
* **Соціальна взаємодія:** Система відгуків та рейтингів книг.
* **Аналітика:** Складні запити для бізнес-аналізу (виявлення неліквіду, топ продажів, ефективність магазинів).

## Технологічний стек

### Мова програмування та Фреймворк
* **C#** (.NET 8)
* **ASP.NET Core Web API** — фреймворк для побудови масштабованих HTTP сервісів.

### Робота з даними (ORM & DB)
* **PostgreSQL** — основна реляційна база даних.
* **Entity Framework Core** — ORM для .NET.
* **LINQ & Raw SQL** — використання оптимізованих запитів для аналітики.

### Інструменти та Бібліотеки
* **AutoMapper** — мапінг об'єктів (DTO ↔ Entity).
* **Swagger / OpenAPI** — автоматична документація та тестування API.
* **Docker & Docker Compose** — контейнеризація додатку та бази даних.
* **xUnit** — модульне тестування.

---

## Інструкції з налаштування

### Передумови
* **Docker** та **Docker Compose** встановлені на вашій системі.
* **Git** для клонування репозиторію.

### Швидкий старт (Docker)

1. **Клонування репозиторію**
   ```bash
   git clone https://github.com/olexandrakk/courseWork.git
   ```
2. **Запуск сервісів**

```bash

docker-compose up -d --build
```

Це автоматично:

* **Збудує Docker образ для Backend API.

* **Запустить PostgreSQL.

* **Застосує міграції.

* **Застосунок буде доступний за адресою: http://localhost:8080/swagger/index.html

## Структура проєкту

courseWork/
├── courseWork.API/             # Presentation Layer (API)
│   ├── Controllers/            # Точки входу (HTTP Endpoints)
│   │   ├── BooksController.cs
│   │   ├── OrdersController.cs
│   │   ├── InventoryController.cs
│   │   ├── AnalyticsController.cs
│   │   └── ...
│   ├── appsettings.json        # Конфігурація
│   └── Dockerfile              # Інструкції для збірки образу
│
├── courseWork.BLL/             # Business Logic Layer (Сервіси)
│   ├── Services/               # Бізнес-логіка
│   │   ├── Interfaces/         # Абстракції (IOrderService, IBookService...)
│   │   ├── BookService.cs
│   │   ├── OrderService.cs
│   │   └── AnalyticsService.cs # Логіка складних запитів
│   ├── Common/
│   │   ├── DTO/                # Data Transfer Objects (BookDto, FrozenAssetDto...)
│   │   ├── Requests/           # Вхідні моделі (CreateOrderRequest...)
│   │   └── Profiles/           # Налаштування AutoMapper
│
├── courseWork.DAL/             # Data Access Layer (Дані)
│   ├── Entities/               # Моделі бази даних (Book, Order, Inventory...)
│   ├── DBContext/              # ApplicationDbContext
│   ├── Repository/             # Generic Repository Pattern
│   └── Migrations/             # Історія змін структури БД
│
├── docs/                       # Документація
│   ├── README.md               # Цей файл
│   ├── ERD_Diagram.png         # Схема бази даних
│   └── Complex_Queries.md      # Опис аналітичних SQL-запитів
│
└── docker-compose.yml          # Оркестрація контейнерів
