using System.Text.Json.Serialization;

namespace Identity.Core.Models.Client
{
    public class ClientCreate
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SurName { get; set; }        
        public string Address { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }        
        public List<ClientEmailDto> Emails { get; set; } = [];
        public List<ClientPhoneDto> Phones { get; set; } = [];
        public List<ClientDocumentDto> Documents { get; set; } = [];
    }
}
