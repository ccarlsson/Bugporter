using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Octokit;
using System.Threading.Tasks;

namespace Bugporter.API.Features.ReportBug.GitHub;
public class CreateGitHubIssueCommand
{
    private readonly ILogger<CreateGitHubIssueCommand> _logger;
    private readonly GitHubClient _gitHubClient;
    private readonly GitHubRepositoryOptions _githubRepositoryOptions;
    public CreateGitHubIssueCommand(
        ILogger<CreateGitHubIssueCommand> logger,
        IOptions<GitHubRepositoryOptions> githubRepositoryOptions,
        GitHubClient gitHubClient)
    {
        _logger = logger;
        _gitHubClient = gitHubClient;
        _githubRepositoryOptions = githubRepositoryOptions.Value;
    }

    public async Task<ReportedBug> Execute(NewBug newBug)
    {
        _logger.LogInformation("Creating GitHub issue");

        //! Create GitHub Issue
        var newIssue = new NewIssue(newBug.Summary) { Body = newBug.Description };

        Issue createdIssue = await _gitHubClient.Issue.Create(
            _githubRepositoryOptions.Owner,
            _githubRepositoryOptions.Name, newIssue);

        _logger.LogInformation($"Successfully created GitHub issue {createdIssue.Number}");

        return new ReportedBug(
            createdIssue.Number.ToString(),
            createdIssue.Title,
            createdIssue.Body);
    }
}
