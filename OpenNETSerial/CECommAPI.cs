﻿namespace OpenNETCF.IO.Serial
{
    using System;
    using System.Runtime.InteropServices;

    internal class CECommAPI : CommAPI
    {
        [DllImport("coredll.dll", EntryPoint="ClearCommError", SetLastError=true)]
        private static extern int CEClearCommError(IntPtr hFile, ref CommErrorFlags lpErrors, CommStat lpStat);
        [DllImport("coredll.dll", EntryPoint="CloseHandle", SetLastError=true)]
        private static extern int CECloseHandle(IntPtr hObject);
        [DllImport("coredll.dll", EntryPoint="CreateEvent", SetLastError=true)]
        private static extern IntPtr CECreateEvent(IntPtr lpEventAttributes, int bManualReset, int bInitialState, string lpName);
        [DllImport("coredll.dll", EntryPoint="CreateFileW", SetLastError=true)]
        private static extern IntPtr CECreateFileW(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("coredll.dll", EntryPoint="EscapeCommFunction", SetLastError=true)]
        private static extern int CEEscapeCommFunction(IntPtr hFile, uint dwFunc);
        [DllImport("coredll.dll", EntryPoint="EventModify", SetLastError=true)]
        private static extern int CEEventModify(IntPtr hEvent, uint function);
        [DllImport("coredll.dll", EntryPoint="GetCommModemStatus", SetLastError=true)]
        private static extern int CEGetCommModemStatus(IntPtr hFile, ref uint lpModemStat);
        [DllImport("coredll.dll", EntryPoint="GetCommProperties", SetLastError=true)]
        private static extern int CEGetCommProperties(IntPtr hFile, CommCapabilities commcap);
        [DllImport("coredll.dll", EntryPoint="GetCommState", SetLastError=true)]
        private static extern int CEGetCommState(IntPtr hFile, DCB dcb);
        [DllImport("coredll.dll", EntryPoint="PurgeComm", SetLastError=true)]
        private static extern int CEPurgeComm(IntPtr hPort, int dwFlags);
        [DllImport("coredll.dll", EntryPoint="ReadFile", SetLastError=true)]
        private static extern int CEReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);
        [DllImport("coredll.dll", EntryPoint="SetCommMask", SetLastError=true)]
        private static extern int CESetCommMask(IntPtr handle, CommEventFlags dwEvtMask);
        [DllImport("coredll.dll", EntryPoint="SetCommState", SetLastError=true)]
        private static extern int CESetCommState(IntPtr hFile, DCB dcb);
        [DllImport("coredll.dll", EntryPoint="SetCommTimeouts", SetLastError=true)]
        private static extern int CESetCommTimeouts(IntPtr hFile, CommTimeouts timeouts);
        [DllImport("coredll.dll", EntryPoint="SetupComm", SetLastError=true)]
        private static extern int CESetupComm(IntPtr hFile, int dwInQueue, int dwOutQueue);
        [DllImport("coredll.dll", EntryPoint="WaitCommEvent", SetLastError=true)]
        private static extern int CEWaitCommEvent(IntPtr hFile, ref CommEventFlags lpEvtMask, IntPtr lpOverlapped);
        [DllImport("coredll.dll", EntryPoint="WaitForSingleObject", SetLastError=true)]
        private static extern int CEWaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);
        [DllImport("coredll.dll", EntryPoint="WriteFile", SetLastError=true)]
        private static extern int CEWriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr lpOverlapped);
        internal override bool ClearCommError(IntPtr hPort, ref CommErrorFlags flags, CommStat stat)
        {
            return Convert.ToBoolean(CEClearCommError(hPort, ref flags, stat));
        }

        internal override bool CloseHandle(IntPtr hPort)
        {
            return Convert.ToBoolean(CECloseHandle(hPort));
        }

        internal override IntPtr CreateEvent(bool bManualReset, bool bInitialState, string lpName)
        {
            return CECreateEvent(IntPtr.Zero, Convert.ToInt32(bManualReset), Convert.ToInt32(bInitialState), lpName);
        }

        internal override IntPtr CreateFile(string FileName)
        {
            return CECreateFileW(FileName, 0xc0000000, 0, IntPtr.Zero, 3, 0, IntPtr.Zero);
        }

        internal override bool EscapeCommFunction(IntPtr hPort, CommEscapes escape)
        {
            return Convert.ToBoolean(CEEscapeCommFunction(hPort, (uint) escape));
        }

        internal override bool GetCommModemStatus(IntPtr hPort, ref uint lpModemStat)
        {
            return Convert.ToBoolean(CEGetCommModemStatus(hPort, ref lpModemStat));
        }

        internal override bool GetCommProperties(IntPtr hPort, CommCapabilities commcap)
        {
            return Convert.ToBoolean(CEGetCommProperties(hPort, commcap));
        }

        internal override bool GetCommState(IntPtr hPort, DCB dcb)
        {
            return Convert.ToBoolean(CEGetCommState(hPort, dcb));
        }

        internal override bool PulseEvent(IntPtr hEvent)
        {
            return Convert.ToBoolean(CEEventModify(hEvent, 1));
        }

        internal override int PurgeComm(IntPtr hPort, int dwFlags)
        {
            return CEPurgeComm(hPort, dwFlags);
        }

        internal override IntPtr QueryFile(string FileName)
        {
            return CECreateFileW(FileName, 0, 0, IntPtr.Zero, 3, 0, IntPtr.Zero);
        }

        internal override bool ReadFile(IntPtr hPort, byte[] buffer, int cbToRead, ref int cbRead, IntPtr lpOverlapped)
        {
            return Convert.ToBoolean(CEReadFile(hPort, buffer, cbToRead, ref cbRead, IntPtr.Zero));
        }

        internal override bool ResetEvent(IntPtr hEvent)
        {
            return Convert.ToBoolean(CEEventModify(hEvent, 2));
        }

        internal override bool SetCommMask(IntPtr hPort, CommEventFlags dwEvtMask)
        {
            return Convert.ToBoolean(CESetCommMask(hPort, dwEvtMask));
        }

        internal override bool SetCommState(IntPtr hPort, DCB dcb)
        {
            return Convert.ToBoolean(CESetCommState(hPort, dcb));
        }

        internal override bool SetCommTimeouts(IntPtr hPort, CommTimeouts timeouts)
        {
            return Convert.ToBoolean(CESetCommTimeouts(hPort, timeouts));
        }

        internal override bool SetEvent(IntPtr hEvent)
        {
            return Convert.ToBoolean(CEEventModify(hEvent, 3));
        }

        internal override bool SetupComm(IntPtr hPort, int dwInQueue, int dwOutQueue)
        {
            return Convert.ToBoolean(CESetupComm(hPort, dwInQueue, dwOutQueue));
        }

        internal override bool WaitCommEvent(IntPtr hPort, ref CommEventFlags flags)
        {
            return Convert.ToBoolean(CEWaitCommEvent(hPort, ref flags, IntPtr.Zero));
        }

        internal override int WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds)
        {
            return CEWaitForSingleObject(hHandle, dwMilliseconds);
        }

        internal override bool WriteFile(IntPtr hPort, byte[] buffer, int cbToWrite, ref int cbWritten, IntPtr lpOverlapped)
        {
            return Convert.ToBoolean(CEWriteFile(hPort, buffer, cbToWrite, out cbWritten, IntPtr.Zero));
        }
    }
}

