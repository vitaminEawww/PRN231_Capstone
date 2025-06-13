using DataAccess.Enums;

namespace DataAccess.Entities;

public class SmokingRecord
{
    public int Id { get; set; }
    public int CustomerId { get; set; }

    /// <summary>
    /// Số điếu hút trong 1 ngày
    /// </summary>
    public int CigarettesPerDay { get; set; }

    /// <summary>
    /// Giá tiền mỗi gói hay hút
    /// </summary>
    public decimal CostPerPack { get; set; }

    /// <summary>
    /// Số điếu mỗi gói - mặc định là 20 rồi
    /// </summary>
    public int CigarettesPerPack { get; set; } = 20;

    /// <summary>
    /// Tần suất hút thuốc
    /// </summary>
    public SmokingFrequency Frequency { get; set; }

    /// <summary>
    /// Thương hiệu thuốc
    /// </summary>
    public string? Brands { get; set; }

    /// <summary>
    /// Tác nhân kích hoạt việc hút thuốc
    /// </summary>
    public string? Triggers { get; set; }

    public DateTime? SmokingStartDate { get; set; }
    public DateTime? QuitSmokingStartDate { get; set; }
    public int? SmokingYears { get; set; }

    /// <summary>
    /// Ngày hút thuốc
    /// </summary>
    public DateTime RecordDate { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
}
