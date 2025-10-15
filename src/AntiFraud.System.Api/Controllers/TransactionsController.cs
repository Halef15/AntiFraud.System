using AntiFraud.System.Application.Cqrs.Commands;
using AntiFraud.System.Application.Cqrs.Queries;
using AntiFraud.System.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static AntiFraud.System.Application.Cqrs.Queries.GetTransactionQuery;

namespace AntiFraud.System.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = new[] { result.Error } });
            }

            return CreatedAtAction(nameof(GetTransaction), new { transactionId = result.Value }, result.Value);
        }

        [HttpGet("{transactionId:guid}")]
        [ProducesResponseType(typeof(TransactionViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTransaction(Guid transactionId)
        {
            var query = new GetTransactionQuery(transactionId);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess || result.Value is null)
            {
                return NotFound(new { errors = new[] { result.Error } });
            }

            return Ok(result.Value);
        }

        // NOVO ENDPOINT: GetAll
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransactionViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTransactions()
        {
            var query = new GetAllTransactionQuery();
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
            {
                // Embora improvável, é uma boa prática tratar falhas
                return BadRequest(new { errors = new[] { result.Error } });
            }

            return Ok(result.Value);
        }

        // NOVO ENDPOINT: Update
        [HttpPut("{transactionId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTransaction(Guid transactionId, [FromBody] UpdateTransactionCommand command)
        {
            command.TransactionId = transactionId;
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                // Erros de validação ou "não encontrado" serão tratados aqui
                return BadRequest(new { errors = new[] { result.Error } });
            }

            return NoContent(); // Retorno padrão para PUT/PATCH bem-sucedido
        }
    }
}