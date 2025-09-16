namespace Identity.DataLayer.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<Client> Clients { get; set; }
    }
}
