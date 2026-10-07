using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Services.Analytics
{
    public sealed class FakeAnalyticsService : IAnalyticsService
    {
        public const int MaxParameters = 25;

        private static readonly Regex SnakeCase = new("^[a-z][a-z0-9]*(_[a-z0-9]+)*$", RegexOptions.CultureInvariant);

        private readonly List<LoggedAnalyticsEvent> loggedEvents = new();

        public IReadOnlyList<LoggedAnalyticsEvent> LoggedEvents => loggedEvents;

        public void Log(string eventName, params (string Key, object Value)[] parameters)
        {
            parameters ??= Array.Empty<(string Key, object Value)>();
            RequireSnakeCase(eventName, nameof(eventName));
            if (parameters.Length > MaxParameters)
                throw new ArgumentException($"Event '{eventName}' has more than {MaxParameters} parameters.", nameof(parameters));

            foreach (var parameter in parameters) RequireSnakeCase(parameter.Key, nameof(parameters));

            var loggedEvent = new LoggedAnalyticsEvent(eventName, ((string Key, object Value)[])parameters.Clone());
            loggedEvents.Add(loggedEvent);
            Debug.Log($"[FakeAnalytics] {loggedEvent}");
        }

        private static void RequireSnakeCase(string name, string parameterName)
        {
            if (name == null || !SnakeCase.IsMatch(name))
                throw new ArgumentException($"Analytics name '{name}' is not snake_case.", parameterName);
        }
    }
}
