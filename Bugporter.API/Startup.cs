using Bugporter.API.Features.ReportBug.GitHub;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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
        var services = builder.Services;

        services.Configure<GitHubRepositoryOptions>(options =>
        {
            options.Owner = config.GetValue<string>("GITHUB_REPOSITORY_OWNER");
            options.Name = config.GetValue<string>("GITHUB_REPOSITORY_NAME");
        });

        var firebaseConfig = config.GetValue<string>("FIREBASE_CONFIG");
          
        var firebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseConfig)
        });


        services.AddSingleton(firebaseApp);

        var githubToken = config.GetValue<string>("GITHUB_TOKEN");

        services.AddSingleton(new GitHubClient(new ProductHeaderValue("bugporter-api"))
        {
            Credentials = new Credentials(githubToken)
        });

        services.AddSingleton<CreateGitHubIssueCommand>();
    }
}
