using AwesomeAssertions;
using Crosscutting.Result;

namespace Unit.Crosscutting;

public class ResultCollectionTests
{
    [Fact]
    public void Empty_ShouldHaveNoError()
    {
        var collection = ResultCollection.Empty();

        collection.Error.Should().BeNull();
    }

    [Fact]
    public void Empty_HasFailed_ShouldReturnFalse()
    {
        var collection = ResultCollection.Empty();

        collection.HasFailed().Should().BeFalse();
    }

    [Fact]
    public void Of_WhenAllOk_ShouldHaveNullError()
    {
        var collection = ResultCollection.Of(Result.Ok(), Result.Ok());

        collection.Error.Should().BeNull();
    }

    [Fact]
    public void Of_WhenAllOk_HasFailed_ShouldReturnFalse()
    {
        var collection = ResultCollection.Of(Result.Ok(), Result.Ok());

        collection.HasFailed().Should().BeFalse();
    }

    [Fact]
    public void Of_WhenOneFailed_HasFailed_ShouldReturnTrue()
    {
        var collection = ResultCollection.Of(Result.Ok(), Result.Err("first error"));

        collection.HasFailed().Should().BeTrue();
    }

    [Fact]
    public void Of_WhenOneFailed_ShouldHaveThatErrorMessage()
    {
        var collection = ResultCollection.Of(Result.Ok(), Result.Err("first error"));

        collection.Error.Should().Be("first error");
    }

    [Fact]
    public void Of_WhenMultipleFailed_ShouldHaveCommaSeparatedErrorMessages()
    {
        var collection = ResultCollection.Of(Result.Err("first error"), Result.Ok(), Result.Err("second error"));

        collection.Error.Should().Be("first error, second error");
    }

    [Fact]
    public void Add_ShouldReturnNewCollectionWithResultAppended()
    {
        var original = ResultCollection.Of(Result.Err("first error"));

        var extended = original.Add(Result.Err("second error"));

        extended.Error.Should().Be("first error, second error");
    }

    [Fact]
    public void Add_ShouldNotMutateOriginalCollection()
    {
        var original = ResultCollection.Of(Result.Err("first error"));

        original.Add(Result.Err("second error"));

        original.Error.Should().Be("first error");
    }

    [Fact]
    public void Add_CanBeChainedFluently()
    {
        var collection = ResultCollection.Empty()
            .Add(Result.Err("first error"))
            .Add(Result.Ok())
            .Add(Result.Err("second error"));

        collection.Error.Should().Be("first error, second error");
    }

    [Fact]
    public void ToResult_WhenAllOk_ShouldReturnOk()
    {
        var collection = ResultCollection.Of(Result.Ok(), Result.Ok());

        var result = collection.ToResult();

        result.HasFailed().Should().BeFalse();
    }

    [Fact]
    public void ToResult_WhenAnyFailed_ShouldReturnErrWithCommaSeparatedMessages()
    {
        var collection = ResultCollection.Of(Result.Err("first error"), Result.Ok(), Result.Err("second error"));

        var result = collection.ToResult();

        result.HasFailed().Should().BeTrue();
        result.Error.Should().Be("first error, second error");
    }
}
