using System.Globalization;

namespace SalonSync.Core.DTOs;

public class AvailableDateDto
{
    public DateTime Date { get; set; }
    public DayOfWeek DayOfWeek => Date.DayOfWeek;
    public string Day => Date.DayOfWeek.ToString();
}