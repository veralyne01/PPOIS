namespace Minimarket.Tests;

public class CashManagerTests
{
    [Fact]
    public void CashRegisterExposesComputedFreeStateAndCashBalance()
    {
        CashRegister register = TestData.CreateOpenRegister();
        Assert.True(register.IsFree);

        long before = register.Balance;
        register.GiveChange(100);
        Assert.Equal(before - 100, register.Balance);

        Customer customer = TestData.CreateCardOnlyCustomer();
        register.Enqueue(customer);
        Assert.False(register.IsFree);
    }

    [Fact]
    public void AddAndGetCountWorkForNewAndExistingNominals()
    {
        Dictionary<Nominals, int> money = new();

        CashManager.Add(money, Nominals.TenPounds, 2);
        CashManager.Add(money, Nominals.TenPounds, 3);

        Assert.Equal(5, CashManager.GetCount(money, Nominals.TenPounds));
        Assert.Equal(0, CashManager.GetCount(money, Nominals.OnePound));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddRejectsNonPositiveCount(int count)
    {
        Assert.Throws<ArgumentException>(() =>
            CashManager.Add(new Dictionary<Nominals, int>(), Nominals.OnePound, count));
    }

    [Fact]
    public void RemoveReturnsFalseWhenDenominationMissingOrInsufficient()
    {
        Dictionary<Nominals, int> money = new() { [Nominals.TenPounds] = 2 };

        Assert.False(CashManager.Remove(money, Nominals.FivePounds, 1));
        Assert.False(CashManager.Remove(money, Nominals.TenPounds, 3));
        Assert.Equal(2, money[Nominals.TenPounds]);
    }

    [Fact]
    public void RemoveDecreasesCountAndRemovesZeroEntry()
    {
        Dictionary<Nominals, int> money = new() { [Nominals.TenPounds] = 2 };

        Assert.True(CashManager.Remove(money, Nominals.TenPounds, 1));
        Assert.Equal(1, CashManager.GetCount(money, Nominals.TenPounds));
        Assert.True(CashManager.Remove(money, Nominals.TenPounds, 1));
        Assert.False(money.ContainsKey(Nominals.TenPounds));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RemoveRejectsNonPositiveCount(int count)
    {
        Assert.Throws<ArgumentException>(() =>
            CashManager.Remove(new Dictionary<Nominals, int>(), Nominals.OnePound, count));
    }

    [Fact]
    public void SumCalculatesTotalFromAllDenominations()
    {
        Dictionary<Nominals, int> money = new()
        {
            [Nominals.OnePound] = 2,
            [Nominals.FivePounds] = 3,
            [Nominals.TenPounds] = 1
        };

        Assert.Equal(27, CashManager.Sum(money));
        Assert.Equal(0, CashManager.Sum(new Dictionary<Nominals, int>()));
    }

    [Fact]
    public void ManageNominalsReturnsEmptyForZero()
    {
        Dictionary<Nominals, int> money = new() { [Nominals.OnePound] = 1 };
        Assert.Empty(CashManager.ManageNominals(money, 0));
    }

    [Fact]
    public void ManageNominalsFindsBoundedCombinationWhenGreedyWouldFail()
    {
        Dictionary<Nominals, int> money = new()
        {
            [Nominals.FivePounds] = 1,
            [Nominals.TwoPounds] = 3
        };

        Dictionary<Nominals, int> result = CashManager.ManageNominals(money, 6);

        Assert.Equal(3, result[Nominals.TwoPounds]);
        Assert.False(result.ContainsKey(Nominals.FivePounds));
        Assert.Equal(6, CashManager.Sum(result));
    }

    [Fact]
    public void ManageNominalsUsesAvailableCountsOnly()
    {
        Dictionary<Nominals, int> money = new()
        {
            [Nominals.TenPounds] = 1,
            [Nominals.FivePounds] = 1
        };

        Assert.Throws<InvalidChangeException>(() => CashManager.ManageNominals(money, 20));
    }

    [Fact]
    public void ManageNominalsThrowsForImpossibleChange()
    {
        Dictionary<Nominals, int> money = new()
        {
            [Nominals.FivePounds] = 1,
            [Nominals.TenPounds] = 1
        };

        Assert.Throws<InvalidChangeException>(() => CashManager.ManageNominals(money, 3));
    }

    [Fact]
    public void ManageNominalsRejectsNegativeChange()
    {
        Assert.Throws<ArgumentException>(() =>
            CashManager.ManageNominals(new Dictionary<Nominals, int>(), -1));
    }
}

public class BankCardTests
{
    [Fact]
    public void DepositAndWithdrawChangeBalance()
    {
        BankCard card = new();
        int initial = card.money;

        card.Deposit(500);
        Assert.Equal(initial + 500, card.money);

        card.Withdraw(200);
        Assert.Equal(initial + 300, card.money);
    }

    [Theory]
    [InlineData(-1)]
    public void DepositRejectsNegativeAmount(int amount)
    {
        BankCard card = new();
        Assert.Throws<ArgumentException>(() => card.Deposit(amount));
    }

    [Fact]
    public void WithdrawRejectsNegativeAmount()
    {
        BankCard card = new();
        Assert.Throws<ArgumentException>(() => card.Withdraw(-1));
    }

    [Fact]
    public void WithdrawRejectsAmountAboveBalance()
    {
        BankCard card = new();
        Assert.Throws<InvalidPaymentException>(() => card.Withdraw(card.money + 1));
    }
}

public class CustomerWalletTests
{
    [Fact]
    public void ConstructorInitializesConsistentSum()
    {
        CustomerWallet wallet = new();
        Assert.Equal((int)CashManager.Sum(wallet.nominals), wallet.sum);
        Assert.DoesNotContain(wallet.nominals.Keys, key => (int)key > 2000);
    }

    [Fact]
    public void AddAndRemoveUpdateSum()
    {
        CustomerWallet wallet = new();
        TestData.ResetWallet(wallet);
        var notes = new Dictionary<Nominals, int>
        {
            [Nominals.FivePounds] = 2,
            [Nominals.TenPounds] = 1
        };

        wallet.Add(notes);
        Assert.Equal(20, wallet.sum);

        wallet.Remove(new Dictionary<Nominals, int> { [Nominals.FivePounds] = 1 });
        Assert.Equal(15, wallet.sum);
    }

    [Fact]
    public void PickNominalsReturnsExactPaymentWhenPossible()
    {
        CustomerWallet wallet = new();
        TestData.ResetWallet(wallet);
        wallet.Add(new Dictionary<Nominals, int>
        {
            [Nominals.OneDollar] = 2
        });

        var notes = wallet.PickNominals(200);

        Assert.Equal(200, CashManager.Sum(notes));
        Assert.Equal(2, notes[Nominals.OneDollar]);
    }

    [Fact]
    public void PickNominalsCanOverpayWithOneAvailableLargerNote()
    {
        CustomerWallet wallet = new();
        TestData.ResetWallet(wallet);
        wallet.Add(new Dictionary<Nominals, int>
        {
            [Nominals.OneDollar] = 2
        });

        var notes = wallet.PickNominals(150);

        Assert.Equal(200, CashManager.Sum(notes));
    }

    [Fact]
    public void PickNominalsAcceptsZero()
    {
        CustomerWallet wallet = new();
        Assert.Empty(wallet.PickNominals(0));
    }

    [Fact]
    public void PickNominalsRejectsNegativeAndInsufficientAmounts()
    {
        CustomerWallet wallet = new();
        Assert.Throws<ArgumentException>(() => wallet.PickNominals(-1));
        Assert.Throws<InsufficientFundsException>(() => wallet.PickNominals(wallet.sum + 1));
    }

    [Fact]
    public void RemoveRejectsNotesThatWalletDoesNotHave()
    {
        CustomerWallet wallet = new();
        TestData.ResetWallet(wallet);

        Assert.Throws<InvalidOperationException>(() =>
            wallet.Remove(new Dictionary<Nominals, int>
            {
                [Nominals.OnePound] = 1
            }));
    }
}

public class LoyaltyCardTests
{
    [Fact]
    public void SpendBonusesDeductsAndRecordsTransaction()
    {
        LoyalityCard card = new() { bonuses = 30 };

        card.SpendBonuses(10);

        Assert.Equal(20, card.bonuses);
        Assert.Single(card.Transactions);
        Assert.Equal(OperationType.Purchase, card.Transactions[0].operation);
        Assert.Equal(10, card.Transactions[0].bonuses);
        Assert.Same(card, card.Transactions[0].card);
    }

    [Fact]
    public void AccrueBonusesAddsAndRecordsTransaction()
    {
        LoyalityCard card = new() { bonuses = 5 };

        card.AccrueBonuses(12);

        Assert.Equal(17, card.bonuses);
        Assert.Single(card.Transactions);
        Assert.Equal(OperationType.Accrual, card.Transactions[0].operation);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(31)]
    public void SpendBonusesRejectsInvalidAmount(int amount)
    {
        LoyalityCard card = new() { bonuses = 30 };
        Assert.Throws<ArgumentException>(() => card.SpendBonuses(amount));
        Assert.Equal(30, card.bonuses);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AccrueBonusesRejectsNonPositiveAmount(int amount)
    {
        LoyalityCard card = new();
        int before = card.bonuses;
        Assert.Throws<ArgumentException>(() => card.AccrueBonuses(amount));
        Assert.Equal(before, card.bonuses);
    }
}

public class CustomerTests
{
    [Fact]
    public void ConstructorCreatesIdentityCartLoyaltyCardAndAtLeastOnePaymentMethod()
    {
        Customer customer = new("Alice");

        Assert.True(customer.id > 1000);
        Assert.Equal("Alice", customer.name);
        Assert.Equal(Status.LookingForProducts, customer.status);
        Assert.NotNull(customer.cart);
        Assert.NotNull(customer.loyalityCard);
        Assert.True(customer.hasBankCard || customer.hasWallet);
        Assert.Equal(customer.hasBankCard, customer.card != null);
        Assert.Equal(customer.hasWallet, customer.wallet != null);
        Assert.InRange(customer.timer, 1, 3);
    }

    [Fact]
    public void SetStatusChangesStatus()
    {
        Customer customer = new("Alice");
        customer.SetStatus(Status.Paying);
        Assert.Equal(Status.Paying, customer.status);
    }

    [Fact]
    public void AddWishAggregatesDuplicateProducts()
    {
        Customer customer = new("Alice");
        Product product = TestData.CreateProduct();

        customer.AddWish(product, 2);
        customer.AddWish(product, 3);

        Assert.True(customer.HasWishes);
        Assert.Equal(5, customer.WishList[product]);
    }

    [Fact]
    public void AddWishRejectsNullAndNonPositiveQuantity()
    {
        Customer customer = new("Alice");
        Assert.Throws<ArgumentNullException>(() => customer.AddWish(null!, 1));
        Assert.Throws<ArgumentException>(() => customer.AddWish(TestData.CreateProduct(), 0));
        Assert.Throws<ArgumentException>(() => customer.AddWish(TestData.CreateProduct(), -1));
    }

    [Fact]
    public void GenerateWishListDoesNothingForEmptyCatalog()
    {
        Customer customer = new("Alice");
        customer.GenerateWishList(new ProductCatalog());
        Assert.False(customer.HasWishes);
    }

    [Fact]
    public void TakeNextWishReturnsAndRemovesFirstWish()
    {
        Customer customer = new("Alice");
        Product product = TestData.CreateProduct();
        customer.AddWish(product, 2);

        var wish = customer.TakeNextWish();

        Assert.Same(product, wish.Key);
        Assert.Equal(2, wish.Value);
        Assert.False(customer.HasWishes);
    }

    [Fact]
    public void TakeNextWishThrowsForEmptyList()
    {
        Customer customer = new("Alice");
        Assert.Throws<InvalidOperationException>(() => customer.TakeNextWish());
    }
}

public class ShoppingCartTests
{
    [Fact]
    public void AddBatchGroupsSameProductAndCalculatesTotal()
    {
        Product product = TestData.CreateProduct(price: 120);
        ShoppingCart cart = new();
        cart.AddBatch(TestData.CreateBatch(product, 2));
        cart.AddBatch(TestData.CreateBatch(product, 3));

        Assert.False(cart.IsEmpty);
        Assert.Single(cart.ProductList);
        Assert.Equal(5, cart.ProductList[0].number);
        Assert.Equal(600, cart.CalculateSum());
        Assert.Equal(2, cart.ProductList[0].Batches.Count);
    }

    [Fact]
    public void ClearEmptiesCartAndResetsTotal()
    {
        ShoppingCart cart = new();
        cart.AddBatch(TestData.CreateBatch(quantity: 2));

        cart.Clear();

        Assert.True(cart.IsEmpty);
        Assert.Empty(cart.ProductList);
        Assert.Equal(0, cart.CalculateSum());
    }

    [Fact]
    public void CartItemIncreaseAndDecreaseReturnPreviousNumber()
    {
        CartItem item = new(TestData.CreateProduct());
        item.IncreaseNumber();
        item.IncreaseNumber();

        int beforeIncrease = item.IncreaseNumber();
        int beforeDecrease = item.DecreaseNumber();

        Assert.Equal(2, beforeIncrease);
        Assert.Equal(3, beforeDecrease);
        Assert.Equal(2, item.number);
    }
}

public class DateAndCardTests
{
    [Fact]
    public void GenerateRandomDateReturnsDateWithinSupportedRange()
    {
        for (int i = 0; i < 100; i++)
        {
            DateOnly date = DateManager.GenerateRandomDate();
            Assert.InRange(date, new DateOnly(2010, 1, 1), DateOnly.FromDateTime(DateTime.Now));
        }
    }

    [Fact]
    public void CardNumbersAreGeneratedAndRegistrationDateIsPresent()
    {
        BankCard first = new();
        BankCard second = new();

        Assert.True(first.number > 0);
        Assert.NotEqual(first.number, second.number);
        Assert.InRange(first.dateOfRegistration,
            new DateOnly(2010, 1, 1), DateOnly.FromDateTime(DateTime.Now));
    }
}
