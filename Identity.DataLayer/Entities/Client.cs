using Identity.DataLayer.Entities.Documents;
using Identity.DataLayer.Entities.Emails;
using Identity.DataLayer.Entities.Phones;

namespace Identity.DataLayer.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; } 
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
        public int? DeletedBy {  get; set; }
        public int? ModifiedBy { get; set; }
        public List<ClientEmail> Emails { get; set; } = [];
        public List<ClientPhone> Phones { get; set; } = [];
        public List<ClientDocument> Documents { get; set; } = [];
    }
}