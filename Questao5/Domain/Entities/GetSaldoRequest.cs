using MediatR;

public class GetSaldoRequest : IRequest<GetSaldoResponse>
{
    public string? IdContaCorrente { get; set; }
}