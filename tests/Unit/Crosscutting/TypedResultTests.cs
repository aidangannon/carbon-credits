using AwesomeAssertions;
using Crosscutting.Result;

namespace Unit.Crosscutting;

public class TypedResultTests
{
    [Fact]
    public void Ok_ShouldHaveNullError()
    {
        var result = Result<int>.Ok(42);

        result.Error.Should().BeNull();
    }

    [Fact]
    public void Ok_ShouldHaveValue()
    {
        var result = Result<int>.Ok(42);

        result.Value.Should().Be(42);
    }

    [Fact]
    public void Err_ShouldHaveErrorMessage()
    {
        var result = Result<int>.Err("something went wrong");

        result.Error.Should().Be("something went wrong");
    }

    [Fact]
    public void Err_ShouldHaveDefaultValue()
    {
        var result = Result<int>.Err("something went wrong");

        result.Value.Should().Be(default);
    }

    [Fact]
    public void HasFailed_WhenOk_ShouldReturnFalse()
    {
        var result = Result<int>.Ok(42);

        result.HasFailed().Should().BeFalse();
    }

    [Fact]
    public void HasFailed_WhenErr_ShouldReturnTrue()
    {
        var result = Result<int>.Err("something went wrong");

        result.HasFailed().Should().BeTrue();
    }

    [Fact]
    public void Unwrap_WhenOk_ShouldReturnValue()
    {
        var result = Result<int>.Ok(42);

        result.Unwrap().Should().Be(42);
    }

    [Fact]
    public void Unwrap_WhenErr_ShouldThrow()
    {
        var result = Result<int>.Err("something went wrong");

        var act = () => result.Unwrap();

        act.Should().Throw<ArgumentNullException>();
    }
}
