namespace Bugporter.API.Features.ReportBug;
public class NewBug
{
    public string Summary { get; }
    public string Description { get; }

    public NewBug(string summary, string description)
    {
        Summary = summary;
        Description = description;
    }
}
