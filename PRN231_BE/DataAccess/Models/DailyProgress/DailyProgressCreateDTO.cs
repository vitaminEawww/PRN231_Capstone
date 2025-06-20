using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.DailyProgress;

public class DailyProgressCreateDTO
{
    [Required(ErrorMessage = "Ngày ghi chép là bắt buộc")]
    public DateTime Date { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số điếu hút phải >= 0")]
    public int CigarettesSmoked { get; set; } = 0;

    [Range(0, double.MaxValue, ErrorMessage = "Tiền chi tiêu phải >= 0")]
    public decimal MoneySpent { get; set; } = 0;

    [Range(0, 10, ErrorMessage = "Mức độ thèm muốn phải từ 0-10")]
    public int CravingLevel { get; set; } = 0;

    [Range(0, 10, ErrorMessage = "Mức độ tâm trạng phải từ 0-10")]
    public int MoodLevel { get; set; } = 0;

    [Range(0, 10, ErrorMessage = "Mức độ năng lượng phải từ 0-10")]
    public int EnergyLevel { get; set; } = 0;

    [StringLength(1000, ErrorMessage = "Ghi chú không được vượt quá 1000 ký tự")]
    public string? Notes { get; set; }

    [StringLength(500, ErrorMessage = "Tác nhân kích hoạt không được vượt quá 500 ký tự")]
    public string? Triggers { get; set; }

    public bool IsSmokeFree { get; set; } = false;
}