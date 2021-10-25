using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    public class ComplianceDemos
    {
        internal static async Task RunAsync(TwitterContext twitterCtx)
        {
            char key;

            do
            {
                ShowMenu();

                key = Console.ReadKey(true).KeyChar;

                switch (key)
                {
                    case '0':
                        Console.WriteLine("\n\tFinding compliance job...\n");
                        await FindComplianceJobAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tGetting all compliance jobs...\n");
                        await GetMultipleComplianceJobsAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tCreating a compliance job...\n");
                        await CreateComplianceJobAsync(twitterCtx);
                        break;
                    case 'q':
                    case 'Q':
                        Console.WriteLine("\nReturning...\n");
                        break;
                    default:
                        Console.WriteLine(key + " is unknown");
                        break;
                }

            } while (char.ToUpper(key) != 'Q');
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nSearch Demos - Please select:\n");

            Console.WriteLine("\t 0. Single Compliance Job");
            Console.WriteLine("\t 1. Multiple Compliance Jobs");
            Console.WriteLine("\t 2. Create a Compliance Job");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task FindComplianceJobAsync(TwitterContext twitterCtx)
        {
            Console.Write("What is the Job ID? ");
            string? jobID = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(jobID))
            {
                Console.WriteLine("You didn't enter a job ID.");
                return;
            }

            ComplianceQuery? response =
                await
                (from job in twitterCtx.Compliance
                 where job.Type == ComplianceType.SingleJob &&
                       job.ID == jobID
                 select job)
                .SingleOrDefaultAsync();

            if (response?.Jobs?.Any() ?? false)
                response.Jobs.ForEach(job =>
                    Console.WriteLine(
                        $"\nID: {job.ID}" +
                        $"\nName: {job.Name}" +
                        $"\nStatus: {job.Status}"));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task GetMultipleComplianceJobsAsync(TwitterContext twitterCtx)
        {
            ComplianceQuery? response =
                await
                (from job in twitterCtx.Compliance
                 where job.Type == ComplianceType.MultipleJobs &&
                       job.JobType == ComplianceJobType.Tweets //&&
                       //job.Status == ComplianceStatus.InProgress
                 select job)
                .SingleOrDefaultAsync();

            if (response?.Jobs?.Any() ?? false)
                response.Jobs.ForEach(job =>
                    Console.WriteLine(
                        $"\nID: {job.ID}" +
                        $"\nName: {job.Name}" +
                        $"\nStatus: {job.Status}"));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task CreateComplianceJobAsync(TwitterContext twitterCtx)
        {
            string jobName = $"test-{DateTime.Now.ToString("yyyyMMddhhmm")}";

            ComplianceQuerySingle? response = 
                await twitterCtx.CreateComplianceJobAsync(ComplianceJobType.Tweets, jobName, true);

            ComplianceJob? job = response?.Job;

            if (job is not null)
                Console.WriteLine(
                    $"\nID: {job.ID}" +
                    $"\nName: {job.Name}" +
                    $"\nStatus: {job.Status}");
            else
                Console.WriteLine("Job not returned");

        }
    }
}
