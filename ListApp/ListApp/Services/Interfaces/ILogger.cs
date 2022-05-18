using System;
using System.Collections.Generic;

namespace ListApp.Services.Interfaces
{
    public interface ILogger
    {
        void TrackError(Exception exception, Dictionary<string, string> properties = null);
        void TrackEvent(string @event, IDictionary<string, string> properties = null);
    }
}
