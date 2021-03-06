﻿namespace OpenNETCF.IO.Serial
{
    using System;
    using System.Runtime.InteropServices;

    internal abstract class CommAPI
    {
        internal static bool bFullFramework = (Environment.OSVersion.Platform != PlatformID.WinCE);
        internal const uint CreateAccess = 0xc0000000;
        internal const uint ERROR_IO_PENDING = 0x3e5;
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const int INVALID_HANDLE_VALUE = -1;
        internal const uint OPEN_EXISTING = 3;
        internal const int PURGE_RXABORT = 2;
        internal const int PURGE_RXCLEAR = 8;
        internal const int PURGE_TXABORT = 1;
        internal const int PURGE_TXCLEAR = 4;

        protected CommAPI()
        {
        }

        internal virtual bool ClearCommError(IntPtr hPort, ref CommErrorFlags flags, CommStat stat)
        {
            return false;
        }

        internal virtual bool CloseHandle(IntPtr hPort)
        {
            return false;
        }

        internal virtual IntPtr CreateEvent(bool bManualReset, bool bInitialState, string lpName)
        {
            return (IntPtr) 0L;
        }

        internal virtual IntPtr CreateFile(string FileName)
        {
            return (IntPtr) 0L;
        }

        internal virtual bool EscapeCommFunction(IntPtr hPort, CommEscapes escape)
        {
            return false;
        }

        internal virtual bool GetCommModemStatus(IntPtr hPort, ref uint lpModemStat)
        {
            return false;
        }

        internal virtual bool GetCommProperties(IntPtr hPort, CommCapabilities commcap)
        {
            return false;
        }

        internal virtual bool GetCommState(IntPtr hPort, DCB dcb)
        {
            return false;
        }

        internal virtual bool GetOverlappedResult(IntPtr hPort, IntPtr lpOverlapped, out int lpNumberOfBytesTransferred, bool bWait)
        {
            lpNumberOfBytesTransferred = 0;
            return false;
        }

        internal virtual bool PulseEvent(IntPtr hEvent)
        {
            return false;
        }

        internal virtual int PurgeComm(IntPtr hPort, int dwFlags)
        {
            return 0;
        }

        internal virtual IntPtr QueryFile(string FileName)
        {
            return (IntPtr) 0L;
        }

        internal virtual bool ReadFile(IntPtr hPort, byte[] buffer, int cbToRead, ref int cbRead, IntPtr lpOverlapped)
        {
            return false;
        }

        internal virtual bool ResetEvent(IntPtr hEvent)
        {
            return false;
        }

        internal virtual bool SetCommMask(IntPtr hPort, CommEventFlags dwEvtMask)
        {
            return false;
        }

        internal virtual bool SetCommState(IntPtr hPort, DCB dcb)
        {
            return false;
        }

        internal virtual bool SetCommTimeouts(IntPtr hPort, CommTimeouts timeouts)
        {
            return false;
        }

        internal virtual bool SetEvent(IntPtr hEvent)
        {
            return false;
        }

        internal virtual bool SetupComm(IntPtr hPort, int dwInQueue, int dwOutQueue)
        {
            return false;
        }

        internal virtual bool WaitCommEvent(IntPtr hPort, ref CommEventFlags flags)
        {
            return false;
        }

        internal virtual int WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds)
        {
            return 0;
        }

        internal virtual bool WriteFile(IntPtr hPort, byte[] buffer, int cbToWrite, ref int cbWritten, IntPtr lpOverlapped)
        {
            return false;
        }

        internal static bool FullFramework
        {
            get
            {
                return bFullFramework;
            }
        }
    }
}

