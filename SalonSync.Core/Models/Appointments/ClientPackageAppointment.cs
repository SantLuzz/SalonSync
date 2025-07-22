using SalonSync.Core.Models.Packages;

namespace SalonSync.Core.Models.Appointments;

public class ClientPackageAppointment
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long ClientPackageId { get; set; }
    public long AppointmentId { get; set; }
    public ClientPackage? ClientPackages { get; set; }
    //public Appointment? Appointment { get; set; }
}