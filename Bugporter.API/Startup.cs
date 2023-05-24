using Bugporter.API.Features.ReportBug.GitHub;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

[assembly: FunctionsStartup(typeof(Bugporter.API.Startup))]

namespace Bugporter.API;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = builder.GetContext().Configuration;
        builder.Services.Configure<GitHubRepositoryOptions>(options =>
        {
            options.Owner = config.GetValue<string>("GITHUB_REPOSITORY_OWNER");
            options.Name = config.GetValue<string>("GITHUB_REPOSITORY_NAME");
        });

        var githubToken = config.GetValue<string>("GITHUB_TOKEN");

        builder.Services.AddSingleton(new GitHubClient(new ProductHeaderValue("bugporter-api"))
        {
            Credentials = new Credentials(githubToken)
        });

        builder.Services.AddSingleton<CreateGitHubIssueCommand>();
    }
}
