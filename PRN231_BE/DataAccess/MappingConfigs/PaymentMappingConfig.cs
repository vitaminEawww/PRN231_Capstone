

using DataAccess.Entities;
using DataAccess.Enums;
using DataAccess.Models.Payment;
using Mapster;

namespace DataAccess.MappingConfigs;

public class PaymentMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Payment, PaymentResponseDTO>()
            .Map(dest => dest.OrderDescription, src => src.Description)
            .Map(dest => dest.TransactionId, src => src.TransactionId)
            .Map(dest => dest.OrderId, src => src.Id.ToString())
            .Map(dest => dest.PaymentMethod, src => src.Method.ToString())
            .Map(dest => dest.PaymentId, src => src.Id.ToString())
            .Map(dest => dest.Success, src => src.Status == PaymentStatus.Completed)
            .Map(dest => dest.Token, src => src.TransactionId)
            .Map(dest => dest.VnPayResponseCode, src => src.Status.ToString());
    }
}
