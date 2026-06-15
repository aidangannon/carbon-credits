using AwesomeAssertions;
using Crosscutting.Result;

namespace Unit.Crosscutting;

public class ResultTests
{
    [Fact]
    public void Ok_ShouldHaveNullError()
    {
        var result = Result.Ok();

        result.Error.Should().BeNull();
    }

    [Fact]
    public void Err_ShouldHaveErrorMessage()
    {
        var result = Result.Err("something went wrong");

        result.Error.Should().Be("something went wrong");
    }

    [Fact]
    public void HasFailed_WhenOk_ShouldReturnFalse()
    {
        var result = Result.Ok();

        result.HasFailed().Should().BeFalse();
    }

    [Fact]
    public void HasFailed_WhenErr_ShouldReturnTrue()
    {
        var result = Result.Err("something went wrong");

        result.HasFailed().Should().BeTrue();
    }
}
