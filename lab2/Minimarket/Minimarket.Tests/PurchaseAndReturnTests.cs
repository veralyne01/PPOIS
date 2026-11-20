using System.Reflection;

namespace Minimarket.Tests;

public class PaymentAndPurchaseTests
{
    [Fact]
    public void CashPaymentStoresAmountsAndMethod()
    {
        CashPayment payment = new(150, 200, 50);

        Assert.Equal(150, payment.sum);
        Assert.Equal(200, payment.receivedSum);
        Assert.Equal(50, payment.change);
        Assert.Equal("Cash", payment.method);
        Assert.True(payment.status);
        Assert.True(payment.id > 0);
    }

    [Fact]
    public void CardPaymentStoresCardAndMethod()
    {
        BankCard card = new();
        CardPayment payment = new(250, card);

        Assert.Same(card, payment.card);
        Assert.Equal(250, payment.sum);
        Assert.Equal("Card", payment.method);
    }

    [Fact]
    public void PurchaseItemCapturesPriceAtPurchaseTimeAndAppliesDiscount()
    {
        Product product = TestData.CreateProduct(price: 100);
        PurchaseItem item = new(product, 3, 15);
        product.ChangePrice(200);

        Assert.Equal(100, item.price);
        Assert.Equal(15, item.discount);
        Assert.Equal(255, item.totalSum);
    }

    [Fact]
    public void PurchaseCalculatesTotalAndStoresDetails()
    {
        Customer customer = TestData.CreateCardOnlyCustomer();
        Cashier cashier = new("Cashier");
        Product first = TestData.CreateProduct("Milk", price: 100);
        Product second = TestData.CreateProduct("Bread", "Bread and cereal", 50);
        List<PurchaseItem> items = new()
        {
            new PurchaseItem(first, 2),
            new PurchaseItem(second, 3, 5)
        };
        CardPayment payment = new(335, customer.card!);

        Purchase purchase = new(customer, cashier, items, payment, 0, 0);

        Assert.True(purchase.id > 0);
        Assert.Same(customer, purchase.customer);
        Assert.Same(cashier, purchase.cashier);
        Assert.Equal(335, purchase.total);
        Assert.Equal(2, purchase.Items.Count);
        Assert.Same(payment, purchase.payment);
    }

    [Fact]
    public void ReceiptExposesPurchaseDataAndTracksReturnableQuantity()
    {
        Product product = TestData.CreateProduct();
        var (customer, sourceReceipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 4);
        Receipt receipt = sourceReceipt;

        Assert.Same(receipt.purchase.customer, customer);
        Assert.Equal(receipt.purchase.total, receipt.sum);
        Assert.Equal(receipt.purchase.cashier, receipt.cashier);
        Assert.Equal(4, receipt.GetReturnableQuantity(product));
        Assert.Equal(0, receipt.GetReturnableQuantity(TestData.CreateProduct("Other")));

        receipt.MarkReturned(product, 2);

        Assert.Equal(2, receipt.GetReturnableQuantity(product));
    }

    [Fact]
    public void ReturnStoresProductQuantityReasonAndAmount()
    {
        Product product = TestData.CreateProduct();
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 2);

        Return result = new(receipt, product, 1, product.price, "Defective");

        Assert.True(result.id > 0);
        Assert.Same(receipt, result.receipt);
        Assert.Equal(product.price, result.sum);
        Assert.Equal("Defective", result.reason);
        Assert.Equal(1, result.products[product]);
    }
}

public class CashRegisterTests
{
    private static (Customer? Customer, Receipt? Receipt) ProcessUntilFinished(CashRegister register)
    {
        for (int i = 0; i < 10; i++)
        {
            Customer? customer = register.NextStep(out Receipt? receipt);
            if (customer != null)
                return (customer, receipt);
        }

        throw new TimeoutException("Register did not finish service within ten ticks.");
    }

    [Fact]
    public void RegisterCannotOpenWithoutCashier()
    {
        CashRegister register = new();
        Assert.Throws<InvalidOperationException>(() => register.Open());
        Assert.False(register.IsOpen);
    }

    [Fact]
    public void RegisterRejectsSecondCashier()
    {
        CashRegister register = new();
        register.AssignCashier(new Cashier("First"));
        Assert.Throws<InvalidOperationException>(() =>
            register.AssignCashier(new Cashier("Second")));
    }

    [Fact]
    public void RegisterOpensAfterCashierAssigned()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Assert.True(register.IsOpen);
    }

    [Fact]
    public void GiveChangeReturnsAndRemovesNotes()
    {
        CashRegister register = TestData.CreateOpenRegister();

        Dictionary<Nominals, int> notes = register.GiveChange(50);

        Assert.Equal(50, CashManager.Sum(notes));
        Assert.Single(notes);
        Assert.Equal(1, notes[Nominals.FiftyPounds]);
    }

    [Fact]
    public void GiveChangeRejectsUnavailableCombination()
    {
        CashRegister register = TestData.CreateOpenRegister();
        TestData.SetPrivateField(register, "nominals", new Dictionary<Nominals, int>());

        Assert.Throws<InvalidChangeException>(() => register.GiveChange(1));
    }

    [Fact]
    public void EnqueueRejectsClosedRegister()
    {
        CashRegister register = new();
        Assert.Throws<InvalidOperationException>(() => register.Enqueue(new Customer("Alice")));
    }

    [Fact]
    public void EnqueueSetsCustomerStatusAndQueueLength()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = new("Alice");

        register.Enqueue(customer);

        Assert.Equal(Status.InLineAtCheckout, customer.status);
        Assert.Equal(1, register.QueueLength);
    }

    [Fact]
    public void NextStepReturnsNullForClosedRegister()
    {
        CashRegister register = new();
        Customer? result = register.NextStep(out Receipt? receipt);

        Assert.Null(result);
        Assert.Null(receipt);
    }

    [Fact]
    public void NextStepReturnsNullWhenOpenAndQueueEmpty()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer? result = register.NextStep(out Receipt? receipt);

        Assert.Null(result);
        Assert.Null(receipt);
        Assert.Equal(0, register.QueueLength);
    }

    [Fact]
    public void NextStepCompletesCardPurchaseAndStoresReceipt()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = TestData.CreateCardOnlyCustomer();
        customer.card!.Deposit(5000);
        Product product = TestData.CreateProduct(price: 150);
        TestData.AddToCart(customer, product, 2);
        int before = customer.card.money;

        register.Enqueue(customer);
        Assert.Equal(Status.InLineAtCheckout, customer.status);
        var (served, receipt) = ProcessUntilFinished(register);

        Assert.Same(customer, served);
        Assert.NotNull(receipt);
        Assert.Equal(Status.Paying, customer.status);
        Assert.IsType<CardPayment>(receipt!.payment);
        Assert.Single(register.Purchases);
        Assert.Equal(300, receipt.sum);
        Assert.Equal(before - 300, customer.card.money);
        Assert.Equal(0, register.QueueLength);
    }

    [Fact]
    public void NextStepCompletesCashPurchaseAndReturnsChangeToWallet()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = TestData.CreateWalletOnlyCustomer();
        customer.wallet!.Add(new Dictionary<Nominals, int>
        {
            [Nominals.OneDollar] = 2
        });
        Product product = TestData.CreateProduct(price: 150);
        TestData.AddToCart(customer, product, 1);

        register.Enqueue(customer);
        var (served, receipt) = ProcessUntilFinished(register);

        Assert.Same(customer, served);
        Assert.NotNull(receipt);
        CashPayment payment = Assert.IsType<CashPayment>(receipt!.payment);
        Assert.Equal(150, payment.sum);
        Assert.Equal(200, payment.receivedSum);
        Assert.Equal(50, payment.change);
        Assert.Equal(50, customer.wallet.sum);
        Assert.Single(register.Purchases);
    }

    [Fact]
    public void NextStepUsesCardWhenCashCannotBeChangedAndCardIsAvailable()
    {
        CashRegister register = TestData.CreateOpenRegister();
        TestData.SetPrivateField(register, "nominals", new Dictionary<Nominals, int>());
        Customer customer = TestData.CreateCustomerWithBothPaymentMethods();
        customer.card!.Deposit(1000);
        customer.wallet!.Add(new Dictionary<Nominals, int>
        {
            [Nominals.OneDollar] = 2
        });
        Product product = TestData.CreateProduct(price: 150);
        TestData.AddToCart(customer, product, 1);

        register.Enqueue(customer);
        var (_, receipt) = ProcessUntilFinished(register);

        Assert.NotNull(receipt);
        Assert.IsType<CardPayment>(receipt!.payment);
        Assert.Single(register.Purchases);
    }

    [Fact]
    public void NextStepFailsWithoutFundsAndDoesNotCreatePurchase()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = TestData.CreateCardOnlyCustomer();
        Product product = TestData.CreateProduct(price: 1_000_000);
        TestData.AddToCart(customer, product, 1);
        int before = customer.card!.money;

        register.Enqueue(customer);
        var (served, receipt) = ProcessUntilFinished(register);

        Assert.Same(customer, served);
        Assert.Null(receipt);
        Assert.Empty(register.Purchases);
        Assert.Equal(before, customer.card.money);
    }

    [Fact]
    public void NextStepFailsForEmptyCart()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = TestData.CreateCardOnlyCustomer();
        customer.card!.Deposit(1000);
        register.Enqueue(customer);

        var (served, receipt) = ProcessUntilFinished(register);

        Assert.Same(customer, served);
        Assert.Null(receipt);
        Assert.Empty(register.Purchases);
    }

    [Fact]
    public void NextStepRollsBackCashWhenNoChangeIsAvailable()
    {
        CashRegister register = TestData.CreateOpenRegister();
        TestData.SetPrivateField(register, "nominals", new Dictionary<Nominals, int>());
        Customer customer = TestData.CreateWalletOnlyCustomer();
        customer.wallet!.Add(new Dictionary<Nominals, int>
        {
            [Nominals.OneDollar] = 2
        });
        Product product = TestData.CreateProduct(price: 150);
        TestData.AddToCart(customer, product, 1);
        int walletBefore = customer.wallet.sum;

        register.Enqueue(customer);
        var (_, receipt) = ProcessUntilFinished(register);

        Assert.Null(receipt);
        Assert.Equal(walletBefore, customer.wallet.sum);
        Assert.Empty(register.Purchases);
    }
}

public class ReturnManagerTests
{
    [Fact]
    public void CardReturnDepositsMoneyAndMarksReceipt()
    {
        Product product = TestData.CreateProduct(price: 120);
        var (customer, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 2);
        int before = customer.card!.money;
        ReturnManager manager = new("Return manager");

        Return result = manager.ProcessReturn(
            receipt, product, 1, "Defective", Array.Empty<CashRegister>());

        Assert.Equal(120, result.sum);
        Assert.Equal(before + 120, customer.card.money);
        Assert.Equal(1, receipt.GetReturnableQuantity(product));
    }

    [Fact]
    public void CashReturnUsesRegisterAndAddsNotesToWallet()
    {
        Product product = TestData.CreateProduct(price: 100);
        var (customer, receipt) = TestData.CreateReceipt(product, cardPayment: false, purchasedQuantity: 2);
        CashRegister register = TestData.CreateOpenRegister();
        ReturnManager manager = new("Return manager");
        int before = customer.wallet!.sum;

        Return result = manager.ProcessReturn(
            receipt, product, 1, "Changed mind", new[] { register });

        Assert.Equal(100, result.sum);
        Assert.Equal(before + 100, customer.wallet.sum);
        Assert.Equal(1, receipt.GetReturnableQuantity(product));
    }

    [Fact]
    public void CashReturnTriesNextRegisterWhenFirstCannotPay()
    {
        Product product = TestData.CreateProduct(price: 100);
        var (customer, receipt) = TestData.CreateReceipt(product, cardPayment: false, purchasedQuantity: 1);
        CashRegister empty = TestData.CreateOpenRegister();
        TestData.SetPrivateField(empty, "nominals", new Dictionary<Nominals, int>());
        CashRegister funded = TestData.CreateOpenRegister();
        ReturnManager manager = new("Return manager");
        int before = customer.wallet!.sum;

        manager.ProcessReturn(receipt, product, 1, null, new[] { empty, funded });

        Assert.Equal(before + 100, customer.wallet.sum);
        Assert.Equal(0, receipt.GetReturnableQuantity(product));
    }

    [Fact]
    public void ReturnRejectsProductNotInReceipt()
    {
        Product product = TestData.CreateProduct();
        Product other = TestData.CreateProduct("Other");
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true);
        ReturnManager manager = new("Return manager");

        Assert.Throws<ProductNotFoundException>(() =>
            manager.ProcessReturn(receipt, other, 1, null, Array.Empty<CashRegister>()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(3)]
    public void ReturnRejectsInvalidQuantity(int quantity)
    {
        Product product = TestData.CreateProduct();
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: true, purchasedQuantity: 2);
        ReturnManager manager = new("Return manager");

        Assert.Throws<InvalidReturnException>(() =>
            manager.ProcessReturn(receipt, product, quantity, null, Array.Empty<CashRegister>()));
    }

    [Fact]
    public void CashReturnDoesNotTakeRegisterMoneyWhenCustomerWalletIsMissing()
    {
        Product product = TestData.CreateProduct(price: 100);
        Customer customer = TestData.CreateCardOnlyCustomer();
        Purchase purchase = new(
            customer,
            new Cashier("Receipt cashier"),
            new List<PurchaseItem> { new(product, 1) },
            new CashPayment(100, 100, 0),
            0,
            0);
        Receipt receipt = new(purchase);
        CashRegister register = TestData.CreateOpenRegister();
        Dictionary<Nominals, int> before = (Dictionary<Nominals, int>)typeof(CashRegister)
            .GetField("nominals", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(register)!;
        Dictionary<Nominals, int> snapshot = before.ToDictionary(pair => pair.Key, pair => pair.Value);
        ReturnManager manager = new("Return manager");

        Assert.Throws<InvalidPaymentException>(() =>
            manager.ProcessReturn(receipt, product, 1, null, new[] { register }));

        Assert.Equal(snapshot.Count, before.Count);
        foreach (var pair in snapshot)
            Assert.Equal(pair.Value, before[pair.Key]);
        Assert.Equal(1, receipt.GetReturnableQuantity(product));
    }

    [Fact]
    public void CashReturnThrowsWhenNoRegisterCanIssueMoney()
    {
        Product product = TestData.CreateProduct(price: 100);
        var (_, receipt) = TestData.CreateReceipt(product, cardPayment: false, purchasedQuantity: 1);
        CashRegister empty = TestData.CreateOpenRegister();
        TestData.SetPrivateField(empty, "nominals", new Dictionary<Nominals, int>());
        ReturnManager manager = new("Return manager");

        Assert.Throws<InvalidChangeException>(() =>
            manager.ProcessReturn(receipt, product, 1, null, new[] { empty }));
    }
}

public class CashRegisterQueueAndBonusTests
{
    private static Customer? FinishOne(CashRegister register, out Receipt? receipt)
    {
        receipt = null;
        for (int i = 0; i < 10; i++)
        {
            Customer? completed = register.NextStep(out receipt);
            if (completed != null)
                return completed;
        }
        throw new TimeoutException("Register did not finish queued customer.");
    }

    [Fact]
    public void RegisterProcessesCustomersInQueueOrder()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer first = TestData.CreateCardOnlyCustomer("First");
        Customer second = TestData.CreateCardOnlyCustomer("Second");
        first.card!.Deposit(1000);
        second.card!.Deposit(1000);
        TestData.AddToCart(first, TestData.CreateProduct(price: 100), 1);
        TestData.AddToCart(second, TestData.CreateProduct("Other", price: 100), 1);

        register.Enqueue(first);
        register.Enqueue(second);
        Assert.Equal(2, register.QueueLength);

        // The first tick calls the first customer to the counter.
        Assert.Null(register.NextStep(out _));
        Assert.Equal(Status.Paying, first.status);
        Assert.Equal(Status.InLineAtCheckout, second.status);

        Customer? completed = FinishOne(register, out Receipt? receipt);
        Assert.Same(first, completed);
        Assert.NotNull(receipt);
        Assert.Equal(1, register.QueueLength);

        Assert.Null(register.NextStep(out _));
        Assert.Equal(Status.Paying, second.status);
        completed = FinishOne(register, out receipt);
        Assert.Same(second, completed);
        Assert.NotNull(receipt);
        Assert.Equal(0, register.QueueLength);
        Assert.Equal(2, register.Purchases.Count);
    }

    [Fact]
    public void CheckoutSpendsEligibleBonusesAndAccruesNewBonuses()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = TestData.CreateCardOnlyCustomer();
        customer.card!.Deposit(1000);
        customer.loyalityCard!.bonuses = 30;
        Product product = TestData.CreateProduct(price: 100);
        TestData.AddToCart(customer, product, 1);

        register.Enqueue(customer);
        Assert.Null(register.NextStep(out _));
        Customer? completed = FinishOne(register, out Receipt? receipt);

        Assert.Same(customer, completed);
        Assert.NotNull(receipt);
        Purchase purchase = receipt!.purchase;
        Assert.Equal(30, purchase.bonusesSpent);
        Assert.Equal(1, purchase.bonusesAccrued);
        Assert.Equal(1, customer.loyalityCard.bonuses);
        Assert.Equal(70, purchase.payment.sum);
        Assert.Equal(2, customer.loyalityCard.Transactions.Count);
    }

    [Fact]
    public void CheckoutDoesNotSpendBonusesWhenThresholdIsNotReached()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = TestData.CreateCardOnlyCustomer();
        customer.card!.Deposit(1000);
        customer.loyalityCard!.bonuses = 1;
        Product product = TestData.CreateProduct(price: 100);
        TestData.AddToCart(customer, product, 1);

        register.Enqueue(customer);
        Assert.Null(register.NextStep(out _));
        var completed = FinishOne(register, out Receipt? receipt);

        Assert.Same(customer, completed);
        Assert.NotNull(receipt);
        Assert.Equal(0, receipt!.purchase.bonusesSpent);
        Assert.Equal(1, receipt.purchase.bonusesAccrued);
        Assert.Single(customer.loyalityCard.Transactions);
    }

    [Fact]
    public void CashPaymentWithExactAmountDoesNotNeedChange()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Customer customer = TestData.CreateWalletOnlyCustomer();
        customer.wallet!.Add(new Dictionary<Nominals, int>
        {
            [Nominals.OneDollar] = 2
        });
        Product product = TestData.CreateProduct(price: 200);
        TestData.AddToCart(customer, product, 1);

        register.Enqueue(customer);
        Assert.Null(register.NextStep(out _));
        var completed = FinishOne(register, out Receipt? receipt);

        Assert.Same(customer, completed);
        CashPayment payment = Assert.IsType<CashPayment>(receipt!.payment);
        Assert.Equal(200, payment.receivedSum);
        Assert.Equal(0, payment.change);
    }
}
