# Лабораторная работа №2. Крупный ООП-проект

**Предметная область:** Минимаркет (магазин самообслуживания).

Консольное приложение на C# (.NET 10), моделирующее работу минимаркета: покупатели с кошельками и банковскими картами, корзины, кассы с выдачей сдачи конкретными номиналами, чеки и возвраты, система скидок и бонусная карта, склад, поставщики, пополнение торгового зала, инвентаризация. Решение состоит из двух проектов: `Minimarket` (основной код) и `Minimarket.Tests` (модульные тесты на xUnit).

## Структура проекта

| Папка             | Содержимое                                                                 |
| ----------------- | -------------------------------------------------------------------------- |
| `Customer`        | Покупатель, корзина, кошелёк, карты, бонусные операции                     |
| `Purchase`        | Касса, покупка, чек, виды оплаты, работа с номиналами                      |
| `ProductManaging` | Товар, партия, полка, отдел, производитель, этикетка, местоположение       |
| `Supplies`        | Склад, поставщики, поставки, перемещения, инвентаризация, каталог          |
| `Discounts`       | Скидки и их расчёт                                                         |
| `ShopRules`       | Магазин (центральный класс), возвраты, история цен                         |
| `Staff`           | Сотрудники: кассир, менеджер возвратов, менеджер инвентаризации            |
| `Exceptions`      | Собственные классы исключений                                              |
| `UI`              | Консольное меню                                                            |

## Описание классов

Формат строки: `Класс | число полей | число методов | связанные классы`.

> Поля — это поля класса и автосвойства (вычисляемые свойства и константы не считаются). Методы — объявленные методы, без конструкторов. Ассоциации — классы проекта, которые входят в класс как тип поля, свойства или параметра метода/конструктора (наследование не считается).

| Класс | Поля | Методы | Ассоциации (связанные классы) |
| ----- | ---- | ------ | ----------------------------- |
| BankCard | 1 | 2 | — |
| BonusTransaction | 6 | 0 | LoyalityCard |
| Card | 3 | 0 | — |
| CardPayment | 1 | 0 | BankCard |
| CartItem | 3 | 3 | Product, ProductBatch |
| Cashier | 0 | 0 | — |
| CashManager | 0 | 5 | — |
| CashPayment | 2 | 0 | — |
| CashRegister | 10 | 8 | Cashier, Customer, DiscountCalculator, Purchase, Receipt |
| Customer | 14 | 4 | BankCard, CustomerWallet, LoyalityCard, PhoneNumber, Product, ProductCatalog, ShoppingCart |
| CustomerWallet | 2 | 3 | — |
| DateManager | 2 | 1 | — |
| Department | 4 | 5 | Product, ProductBatch, Shelf |
| Discount | 5 | 3 | Product |
| DiscountCalculator | 1 | 3 | Discount, Product, ShoppingCart |
| Employee | 3 | 0 | — |
| InventoryCheck | 6 | 2 | Employee, InventoryCheckItem |
| InventoryCheckItem | 5 | 1 | Location, Product |
| InventoryManager | 0 | 1 | Department, Warehouse |
| InventoryReport | 1 | 0 | InventoryCheck, InventoryCheckItem |
| Item | 3 | 0 | Manufacturer, ProductCategory |
| Label | 4 | 0 | ProductBatch |
| Location | 3 | 1 | Department, Shelf |
| LoyalityCard | 2 | 2 | BonusTransaction |
| Manufacturer | 2 | 0 | — |
| Minimarket | 0 | 1 | — |
| Payment | 6 | 0 | — |
| PhoneNumber | 3 | 2 | — |
| PriceHistory | 2 | 1 | Product |
| Product | 4 | 1 | Manufacturer, PriceHistory, ProductCategory |
| ProductBatch | 6 | 3 | Label, Product |
| ProductCatalog | 1 | 3 | Product |
| ProductCategory | 1 | 0 | — |
| Purchase | 9 | 0 | Cashier, Customer, Payment, PurchaseItem |
| PurchaseItem | 4 | 0 | Product |
| RandomHelper | 0 | 1 | — |
| Receipt | 4 | 2 | Cashier, Payment, Product, Purchase, PurchaseItem |
| Return | 7 | 0 | Product, Receipt |
| ReturnManager | 0 | 1 | CashRegister, Product, Receipt |
| ScheduledReturn | 6 | 3 | Product, Receipt |
| Shelf | 5 | 4 | Product, ProductBatch |
| Shop | 19 | 26 | CashRegister, Customer, Department, DiscountCalculator, Employee, InventoryCheck, Product, ProductCatalog, Receipt, Return, ScheduledReturn, ShopReplenishmentOrder, StockMovement, Supplier, Warehouse, WarehouseReplenishmentRequest |
| ShoppingCart | 3 | 3 | CartItem, ProductBatch |
| ShopReplenishmentOrder | 8 | 1 | Product, StockTransfer |
| StockMovement | 6 | 0 | Location, Product |
| StockTransfer | 10 | 1 | Department, Product, ProductBatch, StockMovement, Warehouse |
| Supplier | 2 | 1 | Supply |
| Supply | 4 | 1 | ProductBatch, Supplier |
| UI | 0 | 14 | Customer, Shop |
| Warehouse | 2 | 5 | Product, ProductBatch, Supply |
| WarehouseReplenishmentRequest | 5 | 2 | Product, Supplier |

### Наследование

| Базовый класс | Производные классы |
| ------------- | ------------------ |
| Card | BankCard, LoyalityCard |
| Employee | Cashier, InventoryManager, ReturnManager |
| Item | Product |
| Payment | CardPayment, CashPayment |

### Перечисления

`Country`, `OperationType`, `Nominals`, `SupplyOperations`, `Status`, `CustomerName` (файл `Enums.cs`).

### Исключения (14)

Все наследуются от `Exception`.

| Исключение | Когда возникает | Где выбрасывается |
| ---------- | --------------- | ----------------- |
| ExpiredProductException | Партия товара просрочена | StockTransfer |
| InsufficientFundsException | У покупателя нечем платить | CashRegister, Customer, CustomerWallet |
| InsufficientStockException | Недостаточно товара | Department, Shelf, Warehouse |
| InvalidBarcodeException | Штрихкод содержит не только цифры | Label, Product |
| InvalidChangeException | Не хватает денег для выдачи сдачи | CashManager, ReturnManager |
| InvalidPaymentException | Некорректная оплата | BankCard, CashRegister, Payment, ReturnManager |
| InvalidPhoneNumberException | Неверный номер телефона | PhoneNumber |
| InvalidProductDataException | Некорректные данные товара | Item, Product, ProductCatalog |
| InvalidReturnException | Некорректный возврат | Receipt, ReturnManager, ScheduledReturn |
| InvalidSupplyException | Некорректные данные поставки | Supply, WarehouseReplenishmentRequest |
| ProductNotFoundException | Товар не найден | ReturnManager, Shop |
| ReceiptNotFoundException | Чек не найден | ReturnManager, Shop |
| ShopAlreadyOpenedException | Магазин уже открыт | Shop |
| SupplierNotFoundException | Поставщик не найден | Shop, Supply, WarehouseReplenishmentRequest |

### Итоговая статистика

| Показатель                          | Значение | Требование |
| ----------------------------------- | -------- | ---------- |
| Классы (всего с исключениями)      | 65       | —          |
| Классы всего (без исключений) | 51       | не менее 50 |
| Поля                                | 200      | не менее 150 |
| Поведения (методы)                  | 120      | не менее 100 |
| Ассоциации                          | 100      | не менее 30 |
| Собственные исключения              | 14       | не менее 12 |

## Запуск

```
dotnet run --project Minimarket/Minimarket
dotnet test
```
