using System.Collections.Generic;
using System.Text;

namespace Game.Services.Analytics
{
    public sealed class LoggedAnalyticsEvent
    {
        public LoggedAnalyticsEvent(string name, IReadOnlyList<(string Key, object Value)> parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public string Name { get; }
        public IReadOnlyList<(string Key, object Value)> Parameters { get; }

        public override string ToString()
        {
            var builder = new StringBuilder(Name);
            for (var i = 0; i < Parameters.Count; i++)
            {
                builder.Append(i == 0 ? " " : ", ");
                builder.Append(Parameters[i].Key).Append('=').Append(Parameters[i].Value);
            }

            return builder.ToString();
        }
    }
}
