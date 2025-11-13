namespace LibraryManagementSystem.Models
{
    public class BookDto
    {
        public int BookId { get; set; }   // ✅ Must be BookId (not Id)
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Genre { get; set; }
        public int Quantity { get; set; }
    }
}
