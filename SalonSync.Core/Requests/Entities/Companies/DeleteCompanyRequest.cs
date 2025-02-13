using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Companies;

public class DeleteCompanyRequest : Request
{
    public long Id { get; set; }
}