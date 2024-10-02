

using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EADProject.Services
    {
        public class CartService
        {
            private readonly IMongoCollection<UserCartModel> _carts;

        public CartService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _carts = database.GetCollection<UserCartModel>("Carts");

        }

        // Create or update a cart for the user
        public async Task<UserCartModel> AddProductToCart(string userId, string productId, int quantity)
            {
            var userCart = await _carts.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();

            if (userCart == null)
            {
                // Create a new cart if none exists
                userCart = new UserCartModel
                {
                    UserId = userId
                };
            }

            // Check if the product is already in the cart
            var existingProduct = userCart.Products.Find(p => p.ProductId == productId);
            if (existingProduct != null)
            {
                // Update the quantity
                existingProduct.Quantity += quantity;

                // If quantity goes to zero or less, remove the product
                if (existingProduct.Quantity <= 0)
                {
                    userCart.Products.Remove(existingProduct);
                }
            }
            else if (quantity > 0) // Only add if quantity is positive
            {
                // Add new product with the specified quantity
                userCart.Products.Add(new CartItemModel
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            // Upsert (insert if not exists, update otherwise) the cart
            var filter = Builders<UserCartModel>.Filter.Eq(cart => cart.UserId, userId);
            await _carts.ReplaceOneAsync(filter, userCart, new ReplaceOptions { IsUpsert = true });

            return userCart;
        }

            // Get the cart for a specific user
            public async Task<UserCartModel> GetUserCart(string userId)
            {
                return await _carts.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();
            }


        // Decrement a product quantity in the cart
        public async Task<UserCartModel> DecrementProductQuantity(string userId, string productId)
        {
            // Find the user's cart
            var userCart = await _carts.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();

            if (userCart != null)
            {
                // Find the product in the cart
                var existingProduct = userCart.Products.Find(p => p.ProductId == productId);

                if (existingProduct != null)
                {
                    // Decrement the quantity
                    existingProduct.Quantity--;

                    // If quantity goes to zero or less, remove the product from the cart
                    if (existingProduct.Quantity <= 0)
                    {
                        userCart.Products.Remove(existingProduct);
                    }

                    // Update the cart in the database
                    var filter = Builders<UserCartModel>.Filter.Eq(cart => cart.UserId, userId);
                    await _carts.ReplaceOneAsync(filter, userCart);
                }
            }

            return userCart; // Return the updated cart
        }

        // Remove an item from the cart
        public async Task RemoveProductFromCart(string userId, string productId)
            {
                var userCart = await _carts.Find(cart => cart.UserId == userId).FirstOrDefaultAsync();

                if (userCart != null)
                {
                    userCart.Products.RemoveAll(p => p.ProductId == productId);

                    // Update the cart in the database
                    var filter = Builders<UserCartModel>.Filter.Eq(cart => cart.UserId, userId);
                    await _carts.ReplaceOneAsync(filter, userCart);
                }
            }
        }
    }
