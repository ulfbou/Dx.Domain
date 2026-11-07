using Dx.Domain;
using Dx.Domain.Errors;
using Dx.Domain.Primitives;
using DxFacade = Dx.Domain.Dx;

static Result<UserId> ParseUserId(string text) =>
    UserId.TryParse(text, null, out var id)
        ? DxFacade.Result.Success(id)
        : DxFacade.Result.Failure<UserId>(
            DomainError.Create("user.id.invalid", "Expected a non-empty GUID in N format."));

var result = ParseUserId("d3c9f1a2b3c4d5e6f7a8b9c0d1e2f3a4");
var message = result.Match(
    id => $"user:{id}",
    error => $"error:{error.Code}");
Console.WriteLine(message);
