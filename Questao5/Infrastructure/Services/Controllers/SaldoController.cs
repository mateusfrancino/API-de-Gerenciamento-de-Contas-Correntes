using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


[ApiController]
[Route("api/saldo")]
public class SaldoController : ControllerBase
{
    private readonly IMediator _mediator;

    public SaldoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSaldo([FromQuery(Name = "idContaCorrente")] string idContaCorrente)
    {
        var request = new GetSaldoRequest { IdContaCorrente = idContaCorrente };
        var response = await _mediator.Send(request);

        if (response.Success)
        {
            var result = new
            {
                NumeroConta = response.NumeroConta,
                TitularConta = response.TitularConta,
                ResponseDateTime = response.ResponseDateTime,
                Saldo = response.Saldo
            };

            return Ok(result);
        }
        else
        {
            return BadRequest(response.ErrorMessage);
        }
    }
}

