using System.Collections.Generic;
using System.Linq;

namespace Implementation
{
  public class Sequencer
  {
    private const string JobDelimiter = ",";
    private const string DependencySignature = "=>";

    public string Sequence(string jobsToSequence)
    {
      if (string.IsNullOrWhiteSpace(jobsToSequence))
      {
        return string.Empty;
      }

      return GetSequencedJobs(RemoveWhitespace(jobsToSequence));
    }

    private static string RemoveWhitespace(string jobsToSequence)
    {
      return jobsToSequence.Replace(" ", string.Empty);
    }

    private static string GetSequencedJobs(string jobsToSequence)
    {
      var jobSequence = new List<string>();
      var jobs = GetJobs(jobsToSequence);

      ProcessJobs(jobs, jobSequence);

      return string.Concat(jobSequence);
    }

    private static List<string> GetJobs(string jobsToSequence)
    {
      return jobsToSequence.Split(JobDelimiter).ToList();
    }

    private static void ProcessJobs(List<string> jobs, ICollection<string> jobSequence)
    {
      for (var repetition = 0; repetition < jobs.Count; repetition++)
      {
        jobs.ForEach(job => ProcessJob(jobSequence, job));

        if (jobSequence.Count == jobs.Count)
        {
          return;
        }
      }

      throw new CircularJobDependencyChainException();
    }

    private static void ProcessJob(ICollection<string> jobSequence, string job)
    {
      var operands = job.Split(DependencySignature);
      var leftjob = operands.First();
      var rightJob = operands.Last();

      AssertNotSelfReferencingJob(leftjob, rightJob);

      if (string.IsNullOrWhiteSpace(rightJob))
      {
        jobSequence.AddIfNotAlreadyInCollection(leftjob);
        return;
      }

      AddJobIfDependencyInCollection(jobSequence, rightJob, leftjob);
    }

    private static void AddJobIfDependencyInCollection(ICollection<string> jobSequence, string rightJob, string leftjob)
    {
      if (jobSequence.Contains(rightJob))
      {
        jobSequence.AddIfNotAlreadyInCollection(leftjob);
      }
    }

    private static void AssertNotSelfReferencingJob(string leftjob, string rightJob)
    {
      if (leftjob == rightJob)
      {
        throw new SelfReferencingJobDependencyException();
      }
    }
  }
}