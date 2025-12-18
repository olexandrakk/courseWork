namespace courseWork.BLL.Common.DTO
{
    public class BookDto
    {
        public int BookID { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int NumberOfPages { get; set; }
        public decimal Price { get; set; }

        public string PublisherName { get; set; } = string.Empty;
        public List<AuthorDto> Authors { get; set; } = new List<AuthorDto>();

    }
}
