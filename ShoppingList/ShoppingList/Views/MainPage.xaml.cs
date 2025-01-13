using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using ShoppingList.Models;
using Microsoft.Maui.Controls;

namespace ShoppingList.Views
{
    public partial class MainPage : ContentPage
    {
        private List<Product> _products;
        private string FileName => Path.Combine(FileSystem.AppDataDirectory, "products.json");

        public MainPage()
        {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            _products = await GetProductsAsync();
            ProductsListView.ItemsSource = _products;
        }

        private async Task<List<Product>> GetProductsAsync()
        {
            if (!File.Exists(FileName))
                return new List<Product>();

            var json = await File.ReadAllTextAsync(FileName);
            return JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
        }

        private async Task SaveProductsAsync()
        {
            var json = JsonSerializer.Serialize(_products);
            await File.WriteAllTextAsync(FileName, json);
        }

        private async void OnAddProductClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(UnitEntry.Text))
            {
                await DisplayAlert("Error", "Please enter the product name and unit.", "OK");
                return;
            }

            var newProduct = new Product
            {
                Name = NameEntry.Text,
                Unit = UnitEntry.Text,
                Quantity = 1,
                IsPurchased = false
            };

            _products.Add(newProduct);
            await SaveProductsAsync();

            NameEntry.Text = string.Empty;
            UnitEntry.Text = string.Empty;

            ProductsListView.ItemsSource = null;
            ProductsListView.ItemsSource = _products;
        }

        private async void OnIncreaseClicked(object sender, EventArgs e)
        {
            var product = (sender as Button)?.BindingContext as Product;
            if (product != null)
            {
                product.Quantity++;
                await SaveProductsAsync();
                ProductsListView.ItemsSource = null;
                ProductsListView.ItemsSource = _products;
            }
        }

        private async void OnDecreaseClicked(object sender, EventArgs e)
        {
            var product = (sender as Button)?.BindingContext as Product;
            if (product != null && product.Quantity > 0)
            {
                product.Quantity--;
                await SaveProductsAsync();
                ProductsListView.ItemsSource = null;
                ProductsListView.ItemsSource = _products;
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            var product = (sender as Button)?.BindingContext as Product;
            if (product != null)
            {
                _products.Remove(product);
                await SaveProductsAsync();
                ProductsListView.ItemsSource = null;
                ProductsListView.ItemsSource = _products;
            }
        }

        private async void OnProductTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item != null)
            {
                var product = e.Item as Product;
                if (product != null)
                {
                    product.IsPurchased = !product.IsPurchased;

                    if (product.IsPurchased)
                    {
                        _products.Remove(product);
                        _products.Add(product);
                    }

                    await SaveProductsAsync();
                    ProductsListView.ItemsSource = null;
                    ProductsListView.ItemsSource = _products;
                }
            }
        }
    }
} 