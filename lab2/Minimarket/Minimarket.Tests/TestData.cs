using System.Reflection;

namespace Minimarket.Tests;

internal static class TestData
{
    public static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    public static Product CreateProduct(
        string name = "Test product",
        string category = "Dairy",
        int price = 100,
        int shelfLifeDays = 30,
        string barcode = "4810000000001")
    {
        return new Product(
            name,
            new ProductCategory(category),
            new Manufacturer("Test manufacturer", Country.Belarus),
            price,
            barcode,
            shelfLifeDays);
    }

    public static ProductBatch CreateBatch(
        Product? product = null,
        int quantity = 5,
        int daysUntilExpiration = 10)
    {
        return new ProductBatch(
            product ?? CreateProduct(),
            quantity,
            Today.AddDays(daysUntilExpiration));
    }

    public static Customer CreateCardOnlyCustomer(string name = "Card customer")
    {
        for (int i = 0; i < 500; i++)
        {
            Customer customer = new(name);
            if (customer.hasBankCard && !customer.hasWallet)
            {
                customer.loyalityCard!.bonuses = 0;
                return customer;
            }
        }

        throw new InvalidOperationException("Could not generate a card-only customer.");
    }

    public static Customer CreateWalletOnlyCustomer(string name = "Cash customer")
    {
        for (int i = 0; i < 500; i++)
        {
            Customer customer = new(name);
            if (customer.hasWallet && !customer.hasBankCard)
            {
                customer.loyalityCard!.bonuses = 0;
                ResetWallet(customer.wallet!);
                return customer;
            }
        }

        throw new InvalidOperationException("Could not generate a wallet-only customer.");
    }

    public static Customer CreateCustomerWithBothPaymentMethods(string name = "Both methods")
    {
        for (int i = 0; i < 500; i++)
        {
            Customer customer = new(name);
            if (customer.hasWallet && customer.hasBankCard)
            {
                customer.loyalityCard!.bonuses = 0;
                ResetWallet(customer.wallet!);
                return customer;
            }
        }

        throw new InvalidOperationException("Could not generate a customer with both payment methods.");
    }

    public static void ResetWallet(CustomerWallet wallet)
    {
        if (wallet.nominals.Count > 0)
            wallet.Remove(wallet.nominals.ToDictionary(x => x.Key, x => x.Value));
    }

    public static CashRegister CreateOpenRegister()
    {
        CashRegister register = new();
        register.AssignCashier(new Cashier("Test cashier"));
        register.Open();
        return register;
    }

    public static void AddToCart(Customer customer, Product product, int quantity)
    {
        customer.cart.AddBatch(new ProductBatch(
            product,
            quantity,
            Today.AddDays(10)));
    }

    public static string CaptureOutput(Action action)
    {
        TextWriter original = Console.Out;
        using StringWriter writer = new();
        Console.SetOut(writer);
        try
        {
            action();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    public static void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo field = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException($"Field {fieldName} was not found.");

        field.SetValue(instance, value);
    }

    public static (Customer Customer, Receipt Receipt) CreateReceipt(
        Product product,
        bool cardPayment,
        int purchasedQuantity = 2)
    {
        Customer customer = cardPayment
            ? CreateCardOnlyCustomer()
            : CreateWalletOnlyCustomer();

        Cashier cashier = new("Receipt cashier");
        int total = product.price * purchasedQuantity;

        Payment payment = cardPayment
            ? new CardPayment(total, customer.card!)
            : new CashPayment(total, total, 0);

        Purchase purchase = new(
            customer,
            cashier,
            new List<PurchaseItem> { new(product, purchasedQuantity) },
            payment,
            0,
            0);

        return (customer, new Receipt(purchase));
    }
}
