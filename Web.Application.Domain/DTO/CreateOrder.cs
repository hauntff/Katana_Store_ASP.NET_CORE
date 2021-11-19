namespace Web.Application.Domain.DTO
{
    public class CreateOrder
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int PriceId { get; set; }
        public decimal Price { get; set; }
        public string UserName { get; set; }
    }
}
