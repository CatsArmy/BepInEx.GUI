using System.Runtime.InteropServices;
using BepInEx.Logging;

namespace BepInEx.GUI.Loader;

internal unsafe struct LogPacket
{
    /// |=============================|=|=======|
    /// |Field                        |-| Offset|
    /// |_____________________________| |_______|
    /// |Log String Byte Array Length |-| 0x0000|
    /// |Log Level                    |-| 0x0004|
    /// |Log String Byte Array        |-| 0x0008|
    /// |=============================|=|=======|
    internal byte[] Bytes;

    internal unsafe LogPacket(LogEventArgs logArgs, string log = null)
    {
        byte[] logStringByteArray = Encoding.UTF8.GetBytes(log == null ? log : $"{logArgs}");
        
        int payloadSize = logStringByteArray.Length;
        const Int32 SizeOfLengthPrefix = sizeof(UInt32);
        const Int32 SizeOfLogLevel = sizeof(Int32);

        const int LengthOfPacket = SizeOfLengthPrefix + SizeOfLogLevel + payloadSize;
        Bytes = new byte[LengthOfPacket];

        fixed (byte* byteArrayPtr = Bytes)
        {
            *(UInt32*)byteArrayPtr = (UInt32)payloadSize;

            *(Int32*)(&byteArrayPtr[SizeOfLengthPrefix]) = (Int32)logArgs.Level;

            Marshal.Copy(logStringByteArray, startIndex: 0,
                (IntPtr)(&byteArrayPtr[SizeOfLengthPrefix + SizeOfLogLevel]), payloadSize);
        }
    }
}
