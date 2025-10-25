namespace Identity.Core.Models.Client
{
    public class ClientDetails
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public List<ClientEmailDto> Emails { get; set; } = [];
        public List<ClientPhoneDto> Phones { get; set; } = [];
        public List<ClientDocumentDto> Documents { get; set; } = [];
        
    }
}
