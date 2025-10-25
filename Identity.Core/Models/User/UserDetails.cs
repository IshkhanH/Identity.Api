namespace Identity.Core.Models.User
{
    public class UserDetails
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int DeletedBy { get; set; }
    }
}
