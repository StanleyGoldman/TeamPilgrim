using System;
using JustAProgrammer.TeamPilgrim.VisualStudio.Common.Enums;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common.Extensions
{
    public static class SelectedWorkItemCheckinActionEnumExtensions
    {
        public static WorkItemCheckinAction ToWorkItemCheckinAction(this SelectedWorkItemCheckinActionEnum selectedWorkItemCheckinActionEnum)
        {
            switch (selectedWorkItemCheckinActionEnum)
            {
                case SelectedWorkItemCheckinActionEnum.Resolve:
                    return WorkItemCheckinAction.Resolve;

                case SelectedWorkItemCheckinActionEnum.Associate:
                    return WorkItemCheckinAction.Associate;
            }

            throw new ArgumentException();
        }
    }
}