using Caliburn.Micro;
using System;

namespace windows_party.Helpers
{
    /**
     * Framework-level logging idea from
     * https://buksbaum.us/2010/08/08/how-to-do-logging-with-caliburn.micro/
     */

    class FrameworkLogger : ILog
    {
        #region Logger
        private readonly NLog.Logger Logger;
        #endregion

        #region constructor/destructor
        public FrameworkLogger(Type type)
        {
            Logger = NLog.LogManager.GetLogger(type.Name);
        }
        #endregion

        #region logging handlers
        public void Error(Exception exception) => Logger.Error(exception, exception.Message);

        public void Warn(string format, params object[] args) => Logger.Warn(format, args);

        // ILog does not have "Debug" level messages, so everything non-essential gets dumped into Info, redirect it properly
        public void Info(string format, params object[] args) => Logger.Debug(format, args);
        #endregion
    }
}
