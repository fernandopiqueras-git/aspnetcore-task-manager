using AspNetCoreTaskManager.Dtos;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Tests;

public class TaskScheduleRequestTests
{
    [Fact]
    public void Validate_AcceptsAValidSchedule()
    {
        var request = new TaskScheduleRequest
        {
            Title = "Preparar entrega",
            Start = new DateTime(2026, 9, 15, 10, 0, 0),
            End = new DateTime(2026, 9, 15, 11, 0, 0)
        };

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Fact]
    public void Validate_RejectsAnEndBeforeTheStart()
    {
        var request = new TaskScheduleRequest
        {
            Title = "Preparar entrega",
            Start = new DateTime(2026, 9, 15, 11, 0, 0),
            End = new DateTime(2026, 9, 15, 10, 0, 0)
        };

        var results = Validate(request);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(TaskScheduleRequest.End)));
    }

    [Fact]
    public void Validate_RejectsAnEmptyTitleAndMissingStart()
    {
        var results = Validate(new TaskScheduleRequest());

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(TaskScheduleRequest.Title)));
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(TaskScheduleRequest.Start)));
    }

    private static IReadOnlyCollection<ValidationResult> Validate(TaskScheduleRequest request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, true);
        return results;
    }
}
