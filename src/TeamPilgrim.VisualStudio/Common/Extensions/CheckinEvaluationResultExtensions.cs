using System.Linq;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions
{
    public static class CheckinEvaluationResultExtensions
    {
        public static bool IsValid(this CheckinEvaluationResult checkinEvaluationResult)
        {
            return !checkinEvaluationResult.Conflicts.Any()
                   && !checkinEvaluationResult.NoteFailures.Any()
                   && !checkinEvaluationResult.PolicyFailures.Any()
                   && checkinEvaluationResult.PolicyEvaluationException == null;
        }
    }
}