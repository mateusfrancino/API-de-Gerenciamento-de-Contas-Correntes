using MediatR;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/movimento")]
public class MovimentoController : ControllerBase
{
    private readonly IMediator _mediator;

    public MovimentoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMovement([FromBody] CriarMovimentoRequest request)
    {
        var response = await _mediator.Send(request);
        return response.Success ? Ok(response.MovementId) : BadRequest(response.ErrorMessage);
    }
}

