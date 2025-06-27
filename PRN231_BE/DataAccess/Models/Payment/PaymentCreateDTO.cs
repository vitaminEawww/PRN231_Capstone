using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Payment;

public class PaymentCreateDTO
{
    public string PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}
