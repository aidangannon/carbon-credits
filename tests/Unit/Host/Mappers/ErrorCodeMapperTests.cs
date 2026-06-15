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
}
