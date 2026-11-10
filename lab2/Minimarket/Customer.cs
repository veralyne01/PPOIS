using System;
using System.Collections.Generic;
using System.Text;

namespace Minimarket
{
    class Customer
    {
        int id;
        string name;
        PhoneNumber number;
        DateOnly dateOfRegistration;
    }

    class ShoppingCart
    {
        int id;
        Customer customer;
        List<CartItem>? productList;
    }

    class CartItem
    {
        Product product;
        int number;
    }
    
    class LoyalityCard
    {
        int number;
        Customer owner;
        double bonuses;
        DateOnly dateOfRegistration;
    }

    class BonusTransaction
    {
        int id;
        double bonuses;
        DateOnly date;
        LoyalityCard card;
        OperationType operation;
    }

    class CustomerAccount
    {
        int id;
        Customer owner;
        double balance;
        DateOnly dateOfOpening;
    }

    class BankCard
    {
        int token;
        DateOnly expDate;
        CustomerAccount account;
    }

    class CustomerWallet
    {
        Customer owner;
        int sum;
        Dictionary<Nominal, int> nominals;
    }
}
