using AspNetCoreTaskManager.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Tests;

public class FutureOrTodayAttributeTests
{
    private readonly FutureOrTodayAttribute attribute = new();

    [Fact]
    public void IsValid_WithNull_ReturnsSuccess()
    {
        Assert.Equal(ValidationResult.Success, attribute.GetValidationResult(null, Context()));
    }

    [Fact]
    public void IsValid_WithToday_ReturnsSuccess()
    {
        Assert.Equal(ValidationResult.Success, attribute.GetValidationResult(DateTime.Today, Context()));
    }

    [Fact]
    public void IsValid_WithFutureDate_ReturnsSuccess()
    {
        Assert.Equal(ValidationResult.Success, attribute.GetValidationResult(DateTime.Today.AddDays(1), Context()));
    }

    [Fact]
    public void IsValid_WithPastDate_ReturnsError()
    {
        var result = attribute.GetValidationResult(DateTime.Today.AddDays(-1), Context());
        Assert.NotEqual(ValidationResult.Success, result);
    }

    private static ValidationContext Context() => new(new TaskItem());
}
