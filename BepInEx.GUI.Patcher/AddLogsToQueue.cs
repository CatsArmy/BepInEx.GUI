﻿using BepInEx.Logging;

namespace BepInEx.GUI.Patcher
{
    public class AddLogsToQueue : ILogListener
    {
        public void Dispose()
        {

        }

        public void LogEvent(object sender, LogEventArgs e)
        {
            var l = new LogEntry(e.Source.SourceName, e.Level.ToString(), e.Level, e.Data.ToString());
            Patcher.SocketServer.LogQueue.Enqueue(l);
        }
    }
}
