using System;
using DataAccess.Enums;

namespace DataAccess.Models.SmokingRecords;

public class SmokingRecordResponseDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CigarettesPerDay { get; set; }
    public decimal CostPerPack { get; set; }
    public int CigarettesPerPack { get; set; }
    public SmokingFrequency Frequency { get; set; }
    public string FrequencyDisplay => GetFrequencyDisplay(Frequency);
    public string? Brands { get; set; }
    public string? Triggers { get; set; }
    public DateTime? SmokingStartDate { get; set; }
    public DateTime? QuitSmokingStartDate { get; set; }
    public int? SmokingYears { get; set; }
    public DateTime RecordDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Calculated fields
    public decimal DailyCost => Math.Round((decimal)CigarettesPerDay / CigarettesPerPack * CostPerPack, 0);
    public decimal MonthlyCost => Math.Round(DailyCost * 30, 0);
    public decimal YearlyCost => Math.Round(DailyCost * 365, 0);

    private string GetFrequencyDisplay(SmokingFrequency frequency)
    {
        return frequency switch
        {
            SmokingFrequency.VeryLight => "Rất ít (< 5 điếu/ngày)",
            SmokingFrequency.Light => "Ít (5-10 điếu/ngày)",
            SmokingFrequency.Moderate => "Trung bình (11-20 điếu/ngày)",
            SmokingFrequency.Heavy => "Nhiều (21-30 điếu/ngày)",
            SmokingFrequency.VeryHeavy => "Rất nhiều (> 30 điếu/ngày)",
            _ => "Chưa xác định"
        };
    }
}
