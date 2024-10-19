namespace Book_Store.Models
{
	public class Order
	{
		public int Id { get; set; }
		public List<OrderItem> OrderItems { get; set; } 
		public int OrderTotal { get; set; }
		public DateTime OrderPlaced { get; set; }
	}
}
