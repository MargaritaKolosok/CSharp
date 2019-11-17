namespace Models.Accounts.Models
{
    public class Model
    {
        public string Schema { get; set; }
        public int Id { get; set; }
        public bool Followed { get; set; }
        public int Parent { get; set; }
    }
}
