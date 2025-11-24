using MediatR;

namespace BuildingBlocks.CQRS;


public interface IQuery : IQuery<Unit>
{

}

public interface IQuery<out IResponse> : IRequest<IResponse>
{
}

