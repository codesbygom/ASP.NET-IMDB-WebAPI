using IMDB.Application.Common.Exceptions;
using IMDB.Application.Common.Models;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.People;

public sealed record PersonData(string ImdbId, string FullName, DateTime BirthDate, string Bio, string PhotoUrl);

public sealed record GetPeopleQuery(int? Page, int? PageSize) : IRequest<PagedResult<PersonDto>>;

public sealed class GetPeopleQueryHandler(IPersonRepository people)
    : IRequestHandler<GetPeopleQuery, PagedResult<PersonDto>>
{
    public Task<PagedResult<PersonDto>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
    {
        return people.GetResultAsync(request.Page, request.PageSize, p => p.ToDisplayDto(), cancellationToken);
    }
}

public sealed record GetPersonQuery(int Id) : IRequest<PersonDto>;

public sealed class GetPersonQueryHandler(IPersonRepository people) : IRequestHandler<GetPersonQuery, PersonDto>
{
    public async Task<PersonDto> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var person = await people.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Person not found");

        return person.ToDetailDto();
    }
}

public sealed record CreatePersonCommand(PersonData Data) : IRequest<PersonDto>;

public sealed class CreatePersonCommandHandler(IPersonRepository people, IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePersonCommand, PersonDto>
{
    public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        var person = Person.Create(data.ImdbId, data.FullName, data.BirthDate, data.Bio, data.PhotoUrl);

        people.Add(person);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return person.ToDisplayDto();
    }
}

public sealed record UpdatePersonCommand(int Id, PersonData Data) : IRequest<PersonDto>;

public sealed class UpdatePersonCommandHandler(IPersonRepository people, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePersonCommand, PersonDto>
{
    public async Task<PersonDto> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var person = await people.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Person not found");

        person.Update(data.ImdbId, data.FullName, data.BirthDate, data.Bio, data.PhotoUrl);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return person.ToDetailDto();
    }
}

public sealed record DeletePersonCommand(int Id) : IRequest;

public sealed class DeletePersonCommandHandler(IPersonRepository people, IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePersonCommand>
{
    public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await people.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Person not found or could not be deleted");

        people.Remove(person);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
