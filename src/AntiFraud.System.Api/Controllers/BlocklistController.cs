using AntiFraud.System.Application.Cqrs.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AntiFraud.System.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlocklistController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlocklistController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> BlockCard([FromBody] BlockCardCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                // Trata tanto erros de validação quanto o conflito de "cartão já existe"
                return BadRequest(new { errors = new[] { result.Error } });
            }

            return CreatedAtAction(nameof(BlockCard), new { id = result.Value }, result.Value);
        }

        // TODO: Implementar os endpoints GET e DELETE usando Queries e Commands do CQRS
        // para manter a consistência total.
    }
}