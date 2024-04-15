using System.Net;
using System.Net.Sockets;
using System.Threading;
using BepInEx.Logging;
using HarmonyLib;

namespace BepInEx.GUI.Loader;

[HarmonyPatch]
internal class SendLogToClientSocket : ILogListener
{

    private readonly Thread _thread = null;

    private readonly object _queueLock = new();
    private readonly Queue<LogEventArgs> _logQueue = new();

    private bool _isDisposed = false;

    internal static SendLogToClientSocket Instance { get; private set; }
    private const string IP_ADDRESS = "127.0.0.1";
    private const int SLEEP_MILLISECONDS = 17;


    private int _freePort = 0;
    private IPAddress ipAddress = null;
    private TcpListener listener = null;


    internal SendLogToClientSocket(int freePort)
    {
        Instance = this;
        
        _freePort = freePort;

        _thread = new Thread(() =>
        {
            ipAddress = IPAddress.Parse(IP_ADDRESS);

            listener = new TcpListener(ipAddress, _freePort);

            listener.Start();

            while (true)
            {
                Log.Info($"[SendLogToClient] Accepting Socket.");
                Socket clientSocket = listener.AcceptSocket();`

                if (_isDisposed)
                {
                    break;
                }

                SendPacketsToClientUntilConnectionIsClosed(clientSocket);
            }
        });

        _thread.Start();
    }
    [HarmonyPatch]
    [HarmonyPostfix]
    [HarmonyAfter("mattymatty.AsyncLoggers")]
    private static string LogToString(LogEventArgs args)
    {
        return args.ToString();
    }

    private void SendPacketsToClientUntilConnectionIsClosed(Socket clientSocket)
    {
        while (true)
        {
            if (_isDisposed)
            {
                break;
            }

            while (_logQueue.Count > 0)
            {
                LogEventArgs eventArgs;
                lock (_queueLock)
                {
                    eventArgs = _logQueue.Peek();
                }
                string log = GetLogString();
                LogPacket logPacket = new LogPacket(eventArgs, );
                try
                {
                    clientSocket.Send(logPacket.Bytes);
                }
                catch (Exception e)
                {
                    Log.Error($"Error while trying to send log to socket: {e}{Environment.NewLine}Disconnecting socket.");
                    return;
                }

                lock (_queueLock)
                {
                    _ = _logQueue.Dequeue();
                }
            }
            Thread.Sleep(SLEEP_MILLISECONDS);
        }
    }

    public void Dispose()
    {
        _isDisposed = true;
    }

    internal void StoreLog(LogEventArgs eventArgs)
    {
        lock (_queueLock)
        {
            _logQueue.Enqueue(eventArgs);
        }
    }

    private bool _gotFirstLog = false;

    public void LogEvent(object sender, LogEventArgs eventArgs)
    {
        if (_isDisposed)
        {
            return;
        }

        if (eventArgs.Data == null)
        {
            return;
        }

        if (!_gotFirstLog)
        {
            StoreLog(eventArgs);
            return;
        }

        if (eventArgs.Level == LogLevel.Message &&
            eventArgs.Source.SourceName == "BepInEx" &&
            eventArgs.Data.ToString().StartsWith("BepInEx"))
        {
            _gotFirstLog = true;
        }

    }
}
