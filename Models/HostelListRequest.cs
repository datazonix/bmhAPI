namespace bmhAPI.Models
{
    public class HostelListRequest
    {
        public required string HostelCode { get; set; }  // Friendly API field
    }

    public class HostelCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public int Id { get; set; }
    }

}
