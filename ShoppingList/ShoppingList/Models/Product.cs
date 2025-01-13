using System;

namespace ShoppingList.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public bool IsPurchased { get; set; }
    }
}