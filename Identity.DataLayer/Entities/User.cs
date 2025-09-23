namespace Identity.DataLayer.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ICollection<Client> Clients { get; set; }
    }
}
