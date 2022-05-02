namespace CoreDiffApi.Models
{
    /// <summary>
    /// DiffInput defines the data or DbSet to be saved in database
    /// DataLeft & DataRight are the Based64 encodes from left & right POST
    /// </summary>
    public class DiffInput
    {
        public string Id { get; set; }    
        public string DataLeft { get; set; }
        public string DataRight { get; set; }
    }
}
