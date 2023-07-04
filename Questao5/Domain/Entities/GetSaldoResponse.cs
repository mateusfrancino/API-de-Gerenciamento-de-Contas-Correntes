public class GetSaldoResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? NumeroConta { get; set; }
    public string? TitularConta { get; set; }
    public DateTime ResponseDateTime { get; set; }
    public decimal Saldo { get; set; }
}