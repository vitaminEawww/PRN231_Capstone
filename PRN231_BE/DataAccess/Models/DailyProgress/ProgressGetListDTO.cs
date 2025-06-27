using System;
using DataAccess.Common;

namespace DataAccess.Models.DailyProgress;

public class ProgressGetListDTO : PagingDTO
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
