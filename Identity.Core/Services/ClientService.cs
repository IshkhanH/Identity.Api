using FluentValidation;
using Identity.Core.Interfaces;
using Identity.Core.Models.Client;
using Newtonsoft.Json;
using Identity.Core.Enums;
using System.Security.Claims;
using ClosedXML.Excel;

namespace Identity.Core.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IValidator<ClientCreate> _createValidator;
        private readonly IValidator<ClientUpdate> _updateValidator;

        public ClientService(
            IClientRepository clientRepository,
            IValidator<ClientCreate> createValidator,
            IValidator<ClientUpdate> updateValidator)
        {
            _clientRepository = clientRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<int> CreateAsync(ClientCreate model)
        {
            var validationResult = await _createValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new InvalidDataException(string.Join("; ", validationResult.Errors.Select (e => e.ErrorMessage)));

            var id = await _clientRepository.CreateAsync(new ClientCreate
            {
                Address = model.Address,
                SurName = model.SurName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserId = model.UserId,
                Emails = model.Emails,
                Phones = model.Phones,
                Documents = model.Documents
            });

            return id;
        }

        public async Task<UpdateStatus> UpdateAsync(int id, ClientUpdate model, ClaimsPrincipal user)
        {
            var client = await _clientRepository.GetAsync(id);

            if (client == null)
                return UpdateStatus.NotFound;

            if (client.IsDeleted)
                return UpdateStatus.Deleted;

            var validationResult = await _updateValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
                throw new InvalidDataException(string.Join("; ", validationResult.Errors.Select (e => e.ErrorMessage)));

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("Օգտատերը գտնված չէ:");

            var userId = int.Parse(userIdClaim);

            await _clientRepository.UpdateAsync(new ClientUpdate
            {
                Id = id,
                Address = model.Address,
                SurName = model.SurName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserId = client.UserId,
                Emails = model.Emails,
                Phones = model.Phones,
                Documents = model.Documents,

            });
            return UpdateStatus.Updated;
        }

        public async Task<DeleteStatus> DeleteAsync(int id, int userId)
        {
            return await _clientRepository.DeleteAsync(id, userId);
        }
        public async Task<ClientDetails> GetAsync(int id)
        {
            var data = await _clientRepository.GetAsync(id);

            if (data == null)
                return null;

            return new ClientDetails
            {
                Address = data.Address,
                CreatedDate = data.CreatedDate,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Id = id,
                UserId = data.UserId,
                ModifiedDate = data.ModifiedDate,
                SurName = data.SurName,
                Emails = data.Emails,
                Phones = data.Phones,
                Documents = data.Documents,
                IsDeleted = data.IsDeleted,
                DeletedBy = data.DeletedBy,
                ModifiedBy = data.ModifiedBy,

            };
        }

        public async Task<IEnumerable<ClientDetails>> GetListAsync()
        {
            var list = await _clientRepository.GetListAsync();
            return list.Select(data => new ClientDetails
            {
                Address = data.Address,
                CreatedDate = data.CreatedDate,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Id = data.Id,
                UserId = data.UserId,
                ModifiedDate = data.ModifiedDate,
                SurName = data.SurName,
                Emails = data.Emails,
                Phones = data.Phones,
                Documents = data.Documents,
                DeletedBy = data.DeletedBy,
                IsDeleted = data.IsDeleted,
                ModifiedBy = data.ModifiedBy,

            }).ToList();
        }
        public async Task<IEnumerable<ClientDetails>> GetByUserIdAsync(int userId)
        {
            return (List<ClientDetails>)await _clientRepository.GetByUserIdAsync(userId);
        }
        public async Task<byte[]> ExportExcelClientsAsync()
        {
            var clients = await GetListAsync();
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ClientsFull");

            ws.Cell(1, 1).Value = "Id";
            ws.Cell(1, 2).Value = "UserId";
            ws.Cell(1, 3).Value = "FirstName";
            ws.Cell(1, 4).Value = "LastName";
            ws.Cell(1, 5).Value = "SurName";
            ws.Cell(1, 6).Value = "Address";
            ws.Cell(1, 7).Value = "Emails";
            ws.Cell(1, 8).Value = "Phones";
            ws.Cell(1, 9).Value = "Passport";
            ws.Cell(1, 10).Value = "IdCard";
            ws.Cell(1, 11).Value = "CreatedDate";
            ws.Cell(1, 12).Value = "ModifiedDate";
            ws.Cell(1, 13).Value = "IsDeleted";
            ws.Cell(1, 14).Value = "DeletedBy";
            ws.Cell(1, 15).Value = "ModifiedBy";

            ws.Range(1, 1, 1, 11).Style.Font.Bold = true;
            ws.Range(1, 1, 1, 11).Style.Fill.BackgroundColor = XLColor.LightGray;

            int row = 2;
            foreach (var c in clients)
            {
                ws.Cell(row, 1).Value = c.Id;
                ws.Cell(row, 2).Value = c.UserId;
                ws.Cell(row, 3).Value = c.FirstName;
                ws.Cell(row, 4).Value = c.LastName;
                ws.Cell(row, 5).Value = c.SurName;
                ws.Cell(row, 6).Value = c.Address;
                ws.Cell(row, 7).Value = c.Emails != null ? string.Join(", ", c.Emails.Select(e => e.Email)) : "";
                ws.Cell(row, 8).Value = c.Phones != null ? string.Join(", ", c.Phones.Select(p => p.PhoneNumber)) : "";
                ws.Cell(row, 9).Value = c.Documents != null ? string.Join(", ", c.Documents.Select(d => d.Passport)) : "";
                ws.Cell(row, 10).Value = c.Documents != null ? string.Join(", ", c.Documents.Select(d => d.IdCard)) : "";
                ws.Cell(row, 11).Value = c.CreatedDate;
                ws.Cell(row, 12).Value = c.ModifiedDate;
                ws.Cell(row, 13).Value = c.IsDeleted;
                ws.Cell(row, 14).Value = c.DeletedBy;
                ws.Cell(row, 15).Value = c.ModifiedBy;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }
    }
}
