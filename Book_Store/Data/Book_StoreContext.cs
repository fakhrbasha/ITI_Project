using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Book_Store.Models;

namespace Book_Store.Data
{
	public class BookStoreContext : IdentityDbContext<IdentityUser , IdentityRole , string>
	{
		public BookStoreContext(DbContextOptions<BookStoreContext> options)
			: base(options)
		{
		}

		public DbSet<Book> Books { get; set; }
		public DbSet<CartItem> CartItems { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
	}
}
