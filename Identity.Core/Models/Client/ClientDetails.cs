namespace Identity.Core.Models.Client
{
    public class ClientDetails
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }
        public string Passport { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
