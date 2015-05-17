using Implementation;
using NUnit.Framework;

namespace Tests
{
  [TestFixture]
  public class SequencerTests
  {
    private Sequencer _sequencer;

    [SetUp]
    public void SetUp()
    {
      _sequencer = new Sequencer();
    }

    [Test]
    public void Sequence_GivenAnEmptyString_ShouldReturnAnEmptySequence()
    {
      var result = _sequencer.Sequence(string.Empty);

      Assert.That(result, Is.Empty);
    }

    [TestCase("a =>", "a")]
    [TestCase("b =>", "b")]
    public void Sequence_GivenASingleJob_ShouldReturnASequenceContainingOnlyTheSingleJob(string job, string expectedSequence)
    {
      var result = _sequencer.Sequence(job);

      Assert.That(result, Is.EqualTo(expectedSequence));
    }

    [Test]
    public void Sequence_GivenMultipleJobsWithNoDependencies_ShouldReturnTheJobsInNoSignificantOrder()
    {
      var result = _sequencer.Sequence("a =>, b =>, c =>");

      Assert.That(result.Contains("a"));
      Assert.That(result.Contains("b"));
      Assert.That(result.Contains("c"));
      Assert.That(result.Length, Is.EqualTo(3));
    }

    [Test]
    public void Sequence_GivenMultipleJobsWithASingleDependency_ShouldReturnThreeJobs()
    {
      var result = _sequencer.Sequence("a =>, b => c, c =>");

      Assert.That(result.Contains("a"));
      Assert.That(result.Contains("b"));
      Assert.That(result.Contains("c"));
      Assert.That(result.Length, Is.EqualTo(3));
    }

    [Test]
    public void Sequence_GivenMultipleJobsWithASingleDependency_ShouldHonourTheJobDependency()
    {
      var result = _sequencer.Sequence("a =>, b => c, c =>");

      Assert.That(result.IndexOf('c'), Is.LessThan(result.IndexOf('b')));
    }

    [Test]
    public void Sequence_GivenMultipleJobsWithMultipleDependencies_ShouldReturnTheCorrectSequence()
    {
      var result = _sequencer.Sequence("a =>, b => c, c => f, d => a, e => b, f =>");

      Assert.That(result.IndexOf('c'), Is.LessThan(result.IndexOf('b')));
      Assert.That(result.IndexOf('f'), Is.LessThan(result.IndexOf('c')));
      Assert.That(result.IndexOf('a'), Is.LessThan(result.IndexOf('d')));
      Assert.That(result.IndexOf('b'), Is.LessThan(result.IndexOf('e')));
      Assert.That(result.Length, Is.EqualTo(6));
    }

    [Test]
    [ExpectedException(typeof(SelfReferencingJobDependencyException))]
    public void Sequence_GivenSelfReferencingJob_ShouldThrowException()
    {
      _sequencer.Sequence("a =>, b =>, c => c");
    }

    [Test]
    [ExpectedException(typeof(CircularJobDependencyChainException))]
    public void Sequence_GivenSelfCircularReferenceChain_ShouldThrowException()
    {
      _sequencer.Sequence("a =>, b => c, c => f, d => a, e =>, f => b");
    }
  }
}