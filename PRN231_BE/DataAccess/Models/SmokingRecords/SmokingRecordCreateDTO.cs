using System;
using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Models.SmokingRecords;

public class SmokingRecordCreateDTO
{
    [Required]
    public int CustomerId { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Số điếu hút trong ngày phải từ 1-100")]
    public int CigarettesPerDay { get; set; }

    [Required]
    [Range(0.01, 1000000, ErrorMessage = "Giá tiền phải lớn hơn 0")]
    public decimal CostPerPack { get; set; }

    [Range(1, 50, ErrorMessage = "Số điếu mỗi gói phải từ 1-50")]
    public int CigarettesPerPack { get; set; } = 20;

    [Required]
    public SmokingFrequency Frequency { get; set; }

    [MaxLength(200)]
    public string? Brands { get; set; }

    [MaxLength(500)]
    public string? Triggers { get; set; }

    public DateTime? SmokingStartDate { get; set; }

    [Range(0, 100, ErrorMessage = "Số năm hút thuốc phải từ 0-100")]
    public int? SmokingYears { get; set; }
}
