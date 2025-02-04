using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests
{
    public abstract class Request
    {
        public string UserId { get; set; } = string.Empty;
    }
}
