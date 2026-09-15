using AwesomeAssertions;
using Core.Errors;
using Host.Mappers;
using Microsoft.AspNetCore.Http;

namespace Unit.Host.Mappers;

public class ErrorCodeMapperTests
{
    [Fact]
    public void ToErrorDetails_WhenAccountNotFound_ReturnsWith404()
    {
        var result = ErrorCodeMapper.ToErrorDetails(AccountErrors.NotFound);

        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void ToErrorDetails_WhenAccountNotFound_ReturnsWithTitle()
    {
        var result = ErrorCodeMapper.ToErrorDetails(AccountErrors.NotFound);

        result.Title.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ToErrorDetails_WhenAccountNotFound_ReturnsWithDetails()
    {
        var result = ErrorCodeMapper.ToErrorDetails(AccountErrors.NotFound);

        result.Title.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ToErrorDetails_WhenUnknownErrorCode_Throws()
    {
        var act = () => ErrorCodeMapper.ToErrorDetails("unknown.error");

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ToErrorDetails_WhenCreditNotFound_ReturnsWith404()
    {
        var result = ErrorCodeMapper.ToErrorDetails(CreditErrors.NotFound);

        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Fact]
    public void ToErrorDetails_WhenCreditProjectNotFoundMustRetire_ReturnsWith422()
    {
        var result = ErrorCodeMapper.ToErrorDetails(CreditErrors.ProjectNotFoundMustRetire);

        result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public void ToErrorDetails_WhenAccountCreatedInFuture_ReturnsWith422()
    {
        var result = ErrorCodeMapper.ToErrorDetails(AccountErrors.CreatedInFuture);

        result.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public void ToErrorDetails_WhenAccountLeftInPartialTransferState_ReturnsWith500()
    {
        var result = ErrorCodeMapper.ToErrorDetails($"{AccountErrors.PartialTransferState}: account id: {Guid.NewGuid()} and account id: {Guid.NewGuid()} have been left in a partial state: some inner error");

        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
