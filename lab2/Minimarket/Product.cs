using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Minimarket
{
    class Product
    {
        string name;
        double price;
        ProductCategory category;
        Manufacturer manufacturer;
        int barcode;
        void ChangePrice(double price)
        {
            this.price = price;
        }
    }

    class ProductCategory
    {
        int id;
        string categoryName;
        string? categoryDescription;
        ProductCategory? parent;
    }

    class Manufacturer
    {
        int id;
        string manufacturerName;
        Country country;
    }

    class Label
    {
        int barcode;
        DateOnly expDate;
        ProductBatch batch;
    }

    class ProductBatch {
        int id;
        Product product;
        int number;
        DateOnly dateOfProduction;
        DateOnly expDate;
    }

    class Location
    {
        Department department;
        Shelf shelf;
    }

    class Department
    {
        int id;
        string name;
        string? description;
    }

    class Shelf
    {
        int id;
        int number;
        int capacity;
        Department department;
    }
}
