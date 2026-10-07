using System;
using System.Collections.Generic;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class TutorialState
    {
        public bool TutorialRunCompleted;
        public HashSet<string> CompletedSteps = new();
    }
}
