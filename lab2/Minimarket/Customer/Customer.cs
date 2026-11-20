using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    public class Customer
    {
        public int id { get; private set; }
        private static int nextCustomerId = 1000;
        public string name;
        PhoneNumber number;
        DateOnly dateOfRegistration;
        public ShoppingCart cart { get; private set; }
        public CustomerWallet? wallet { get; private set; }
        public BankCard? card { get; private set; }
        public LoyalityCard? loyalityCard { get; private set; }
        public bool hasBankCard { get; private set; }
        public bool hasWallet { get; private set; }
        public Status status { get; private set; }
        private readonly Dictionary<Product, int> wishList = new();
        public IReadOnlyDictionary<Product, int> WishList => wishList;
        public bool HasWishes => wishList.Count > 0;

        public int timer { get; set; }

        public Customer(string name)
        {
            id = ++nextCustomerId;
            this.name = name;
            status = Status.LookingForProducts;
            cart = new ShoppingCart();
            timer = Random.Shared.Next(1, 4);
            loyalityCard = new();
            int randomizer = Random.Shared.Next(0,3);
            switch (randomizer)
            {
                case 0:
                    wallet = new();
                    hasWallet = true;
                    break;
                case 1:
                    card = new();
                    hasBankCard = true;
                    break;
                case 2:
                    wallet = new();
                    hasWallet = true;
                    card = new();
                    hasBankCard = true;
                    break;
                default:
                    throw new InsufficientFundsException();
            }
        }

        public void SetStatus(Status status)
        {
            this.status = status;
        }

        public void AddWish(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive!");
            wishList[product] = wishList.GetValueOrDefault(product) + quantity;
        }

        public void GenerateWishList(ProductCatalog catalog)
        {
            if (catalog.Products.Count == 0)
                return;
            int count = Random.Shared.Next(1, Math.Min(5, catalog.Products.Count) + 1);
            var products = catalog.Products
                .OrderBy(product => Random.Shared.Next())
                .Take(count);
            foreach (Product product in products)
            {
                AddWish(product, Random.Shared.Next(1, 4));
            }
        }

        public KeyValuePair<Product, int> TakeNextWish()
        {
            if (!HasWishes)
                throw new InvalidOperationException("The wish list is empty!");
            var wish = wishList.First();
            wishList.Remove(wish.Key);
            return wish;
        }
    }
}
