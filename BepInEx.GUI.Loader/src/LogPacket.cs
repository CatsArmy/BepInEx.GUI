using LogEventArgs = BepInEx.Logging.LogEventArgs;
using Marshal = System.Runtime.InteropServices.Marshal;

namespace BepInEx.GUI.Loader;

internal struct LogPacket
{
    /// <summary> <code>
    /// |=====================================|<br/>
    /// |Field                        | Offset|<br/>
    /// |Log String Byte Array Length | 0x0000|<br/>
    /// |Log Level                    | 0x0004|<br/>
    /// |Log String Byte Array        | 0x0008|<br/>
    /// |=====================================|<br/>
    /// </code> </summary>
    internal byte[] Bytes;
    private readonly LogEventArgs log;

    internal unsafe LogPacket(LogEventArgs log)
    {
        this.log = log;
        byte[] logStringByteArray = Encoding.UTF8.GetBytes(log.ToString());

        int payloadSize = logStringByteArray.Length;

        const Int32 SizeOfLengthPrefix = sizeof(UInt32);
        const Int32 SizeOfLogLevel = sizeof(Int32);

        Bytes = new byte[SizeOfLengthPrefix + SizeOfLogLevel + payloadSize];

        fixed (byte* byteArrayPtr = Bytes)
        {
            *(UInt32*)byteArrayPtr = (UInt32)payloadSize;

            *(Int32*)(&byteArrayPtr[SizeOfLengthPrefix]) = (Int32)log.Level;

            Marshal.Copy(logStringByteArray, 0, (IntPtr)(&byteArrayPtr[SizeOfLengthPrefix + SizeOfLogLevel]), payloadSize);
        }
    }

    private static readonly string NewLine = Environment.NewLine;

    public override string ToString() => (log is null) ? $"[Log is null]"
            : $"[Log is not null]{NewLine}[Log::{log}]{NewLine}[Data::{log.Data}], [Level::{log.Level}], [Source::{log.Source.SourceName}]";
}
