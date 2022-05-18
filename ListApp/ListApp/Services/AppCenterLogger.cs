using ListApp.Services.Interfaces;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;

namespace ListApp.Services
{
    public class AppCenterLogger : ILogger
    {
        public void TrackError(Exception exception, Dictionary<string, string> properties = null) => Crashes.TrackError(exception, properties);
        public void TrackEvent(string @event, IDictionary<string, string> properties = null) => Analytics.TrackEvent(@event, properties);
    }
}
