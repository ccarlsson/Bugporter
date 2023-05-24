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

namespace Bugporter.API.Functions
{
    public class ReportBugFunction
    {
        private readonly ILogger<ReportBugFunction> _logger;
        private readonly CreateGitHubIssueCommand _createGitHubIssueCommand;

        public ReportBugFunction(CreateGitHubIssueCommand createGitHubIssueCommand, ILogger<ReportBugFunction> logger)
        {
            _createGitHubIssueCommand = createGitHubIssueCommand;
            _logger = logger;
        }


        [FunctionName("ReportBugFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = "bugs")] ReportBugRequest req)
        {
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
