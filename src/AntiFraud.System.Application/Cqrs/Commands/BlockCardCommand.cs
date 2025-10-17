using AntiFraud.System.BuildingBlocks.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace AntiFraud.System.Application.Cqrs.Commands
{
    public class BlockCardCommand : IRequest<Result<Guid>>
    {
        public required string CardNumber { get; set; }
        public string? Reason { get; set; }
    }
}