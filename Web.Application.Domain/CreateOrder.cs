namespace Store.Books.Domain
{
    public class CreateOrder
    {
        public int KatanaId { get; set; }
        public string Title { get; set; }
        public int PriceId { get; set; }
        public decimal Price { get; set; }
        public string UserName { get; set; }
    }
}
