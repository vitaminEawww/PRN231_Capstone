using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

public partial class SystemReport
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public ReportType Type { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Data { get; set; } = null!; // JSON data
    public DateTime GeneratedAt { get; set; }
}
