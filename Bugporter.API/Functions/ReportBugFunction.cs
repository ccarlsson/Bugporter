using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Bugporter.API.Features.ReportBug.GitHub;
using Bugporter.API.Features.ReportBug;
using FirebaseAdminAuthentication.DependencyInjection.Services;
using Octokit;
using FirebaseAdminAuthentication.DependencyInjection.Models;

namespace Bugporter.API.Functions
{
    public class ReportBugFunction
    {
        private readonly ILogger<ReportBugFunction> _logger;
        private readonly CreateGitHubIssueCommand _createGitHubIssueCommand;
        private readonly FirebaseAuthenticationFunctionHandler _authenticationHandler;

        public ReportBugFunction(CreateGitHubIssueCommand createGitHubIssueCommand, ILogger<ReportBugFunction> logger, FirebaseAuthenticationFunctionHandler firebaseAuthenticationFunctionHandler)
        {
            _createGitHubIssueCommand = createGitHubIssueCommand;
            _logger = logger;
            _authenticationHandler = firebaseAuthenticationFunctionHandler;
        }


        [FunctionName("ReportBugFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = "bugs")] ReportBugRequest req, HttpRequest httpRequest)
        {
            var authResult = await _authenticationHandler.HandleAuthenticateAsync(httpRequest);
            if (!authResult.Succeeded)
            {
                return new UnauthorizedResult();
            }

            var userId = authResult.Principal.FindFirst(FirebaseUserClaimType.ID).Value;

            _logger.LogInformation($"Bug reported by {userId}");

            NewBug newBug = new NewBug(req.Summary, req.Description);
            ReportedBug reportedBug = await _createGitHubIssueCommand.Execute(newBug);

            return new OkObjectResult(new ReportBugResponse
            {
                Id = reportedBug.Id,
                Summary = reportedBug.Summary,
                Description = reportedBug.Description,
            });
        }
    }
}
