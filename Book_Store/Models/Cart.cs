using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Book_Store.Data; // Assuming this contains BookStoreContext
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Book_Store.Models
{
    public class Cart
    {
        private readonly BookStoreContext _context;
        private readonly ISession _session;

        private const string CartSessionKey = "CartId";

        public Cart(BookStoreContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _session = httpContextAccessor?.HttpContext.Session ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string Id { get; set; }
        public List<CartItem> CartItems { get; set; }

        public static Cart GetCart(IServiceProvider services)
        {
            var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
            var context = services.GetRequiredService<BookStoreContext>();

            string cartId = httpContextAccessor.HttpContext.Session.GetString(CartSessionKey) ?? Guid.NewGuid().ToString();
            httpContextAccessor.HttpContext.Session.SetString(CartSessionKey, cartId);

            return new Cart(context, httpContextAccessor) { Id = cartId };
        }

        public CartItem GetCartItem(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));

            return _context.CartItems.SingleOrDefault(ci => ci.Book.Id == book.Id && ci.CartId == Id);
        }

        public void AddToCart(Book book, int quantity)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

            var cartItem = GetCartItem(book);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Book = book,
                    Quantity = quantity,
                    CartId = Id
                };

                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            _context.SaveChanges();
        }

        public int ReduceQuantity(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));

            var cartItem = GetCartItem(book);
            var remainingQuantity = 0;

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    remainingQuantity = --cartItem.Quantity;
                }
                else
                {
                    _context.CartItems.Remove(cartItem);
                }
            }

            _context.SaveChanges();
            return remainingQuantity;
        }

        public int IncreaseQuantity(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));

            var cartItem = GetCartItem(book);
            var remainingQuantity = 0;

            if (cartItem != null && cartItem.Quantity > 0)
            {
                remainingQuantity = ++cartItem.Quantity;
            }

            _context.SaveChanges();
            return remainingQuantity;
        }

        public void RemoveFromCart(Book book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));

            var cartItem = GetCartItem(book);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }

        public void ClearCart()
        {
            var cartItems = _context.CartItems.Where(ci => ci.CartId == Id).ToList();

            if (cartItems.Any())
            {
                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();
            }
        }

        public List<CartItem> GetAllCartItems()
        {
            return CartItems ??= _context.CartItems
                .Where(ci => ci.CartId == Id)
                .Include(ci => ci.Book)  // Eager load the related Book entity
                .ToList();
        }

        public int GetCartTotal()
        {
            return _context.CartItems
                .Where(ci => ci.CartId == Id)
                .Select(ci => ci.Book.Price * ci.Quantity)
                .Sum();
        }
    }
}
