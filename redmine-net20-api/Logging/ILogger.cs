using System.Collections.Generic;
using System.Text;

namespace Redmine.Net.Api.Logging
{
    public interface ILogger
    {
        void Log(LogEntry entry);
    }
}