﻿namespace SiRFLive.Utilities
{
    using CommonClassLibrary;
    using SiRFLive.Communication;
    using SiRFLive.General;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;

    public class ListenerManager
    {
        private string _aidingProtocol = "AI3";
        private Hashtable _listenerList = new Hashtable();
        private object _listenerLock = new object();
        protected CommunicationManager.ReceiverType _rxType = CommunicationManager.ReceiverType.SLC;

        public void Cleanup()
        {
            this.DeleteAllListeners();
        }

        public void Cleanup(string listenerName)
        {
            if ((listenerName != null) && (listenerName.Length != 0))
            {
                this.DeleteListener(listenerName);
            }
        }

        public void Cleanup(string listenerName, string comport)
        {
            string str = string.Empty;
            if (((listenerName != null) && (listenerName.Length != 0)) && ((comport != null) && (comport.Length != 0)))
            {
                this.DeleteListener(str);
            }
        }

        public virtual ListenerContent Create(string listenerName, string comport)
        {
            if (((listenerName == null) || (listenerName.Length == 0)) || ((comport == null) || (comport.Length == 0)))
            {
                return null;
            }
            switch (listenerName)
            {
                case "HWConfig":
                case "HWConfig_GUI":
                    return this.Create(listenerName, 0, 2, 0x10, -1, comport);

                case "TimeAiding":
                case "TimeAiding_GUI":
                    return this.Create(listenerName, 0, 2, 0x11, -1, comport);

                case "FreqAiding":
                case "FreqAiding_GUI":
                    return this.Create(listenerName, 0, 2, 0x12, -1, comport);

                case "ApproxPosition":
                case "ApproxPosition_GUI":
                    return this.Create(listenerName, 0, 2, 0x13, -1, comport);

                case "MPM_navState1":
                case "MPM_navState1_GUI":
                    return this.Create(listenerName, 0, -1, 0x44, 0xff, comport);

                case "MPM_navState_V2":
                case "MPM_navState_V2_GUI":
                    return this.Create(listenerName, 0, -1, 0x44, 0xe1, comport);

                case "MPM_SVD1":
                case "MPM_SVD1_GUI":
                    return this.Create(listenerName, 0, -1, 0xff, -1, comport);

                case "MPM_navState1:status":
                case "MPM_navState1:status_GUI":
                    return this.Create(listenerName, 0, -1, 0xff, -1, comport);

                case "valid sats":
                case "valid sats_GUI":
                    return this.Create(listenerName, 0, -1, 0xff, -1, comport);

                case "Nav":
                case "Nav_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 2, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 2, -1, comport);

                case "TTFF":
                case "TTFF_GUI":
                case "TTFF_MSA":
                case "TTFF_MSA_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 0xe1, 6, comport);
                    }
                    return this.Create(listenerName, 0, 0xbb, 6, -1, comport);

                case "ACK":
                case "ACK_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 11, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 11, -1, comport);

                case "N'Ack":
                case "N'Ack_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 12, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 12, -1, comport);

                case "ClockStatus":
                case "ClockStatus_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 7, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 7, -1, comport);

                case "XOLearning_CurrentTemp":
                case "XOLearning_CurrentTemp_GUI":
                    return this.Create(listenerName, 0, -1, 0x5d, 12, comport);

                case "MeasuredNavigationData":
                case "MeasuredNavigationData_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 2, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 2, -1, comport);

                case "GeodeticNavigationData":
                case "GeodeticNavigationData_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 0x29, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 0x29, -1, comport);

                case "NavLibMeasurement":
                case "NavLibMeasurement_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 0x1c, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 0x1c, -1, comport);

                case "PostionResponse":
                case "PostionResponse_GUI":
                case "MeasurementResponse":
                case "MeasurementResponse_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 0x29, -1, comport);
                    }
                    if (!(this._aidingProtocol == "AI3"))
                    {
                        return this.Create(listenerName, 0, 0xee, 0x29, -1, comport);
                    }
                    return this.Create(listenerName, 0, 1, 2, -1, comport);

                case "SWVersion":
                case "SWVersion_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 6, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 6, -1, comport);

                case "CNO":
                case "CNO_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 4, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 4, -1, comport);

                case "SVS":
                case "SVS_GUI":
                    if (this._rxType != CommunicationManager.ReceiverType.SLC)
                    {
                        return this.Create(listenerName, 0, -1, 4, -1, comport);
                    }
                    return this.Create(listenerName, 0, 0xee, 4, -1, comport);

                case "PollAlm_GUI":
                    return this.Create(listenerName, 0, -1, 14, -1, comport);

                case "PollEph_GUI":
                    return this.Create(listenerName, 0, -1, 15, -1, comport);
            }
            return null;
        }

        public virtual ListenerContent Create(string listenerName, int source, int chan, int msgId, int subId, string thisCom)
        {
            if (((listenerName == null) || (listenerName.Length == 0)) || ((thisCom == null) || (thisCom.Length == 0)))
            {
                return null;
            }
            string key = @".\private$\" + listenerName + "_" + thisCom;
            ListenerContent content = null;
            lock (this.ListenerLock)
            {
                if (this.ListenerList.ContainsKey(key))
                {
                    return (ListenerContent) this.ListenerList[key];
                }
                BackgroundWorker worker = new BackgroundWorker();
                BackgroundWorker worker2 = new BackgroundWorker();
                worker.WorkerSupportsCancellation = true;
                worker2.WorkerSupportsCancellation = true;
                worker2.DoWork += new DoWorkEventHandler(this.doMainWork);
                content = new ListenerContent();
                content.ListenerName = key;
                content.Source = source;
                content.Chan = chan;
                content.MsgId = msgId;
                content.SubId = subId;
                content.ComPort = thisCom;
                content.DoUserWork = worker;
                content.DoMainWork = worker2;
                this.ListenerList.Add(key, content);
            }
            return content;
        }

        public void Delete(string queueName)
        {
            this.DeleteListener(queueName);
        }

        public void DeleteAllListeners()
        {
            lock (this.ListenerLock)
            {
                foreach (string str in this.ListenerList.Keys)
                {
                    ListenerContent content = (ListenerContent) this.ListenerList[str];
                    if (content.DoUserWork != null)
                    {
                        content.DoUserWork.CancelAsync();
                    }
                    if (content.DoMainWork != null)
                    {
                        content.DoMainWork.CancelAsync();
                    }
                }
                this.ListenerList.Clear();
            }
        }

        public void DeleteListener(string listenerName)
        {
            string str = string.Empty;
            if ((listenerName != null) && (listenerName.Length != 0))
            {
                Hashtable hashtable = new Hashtable();
                lock (this.ListenerLock)
                {
                    foreach (string str2 in this.ListenerList.Keys)
                    {
                        ListenerContent content = (ListenerContent) this.ListenerList[str2];
                        if (listenerName.Contains(@".\private$\"))
                        {
                            str = listenerName;
                        }
                        else
                        {
                            str = @".\private$\" + listenerName + "_" + content.ComPort;
                        }
                        if (content.ListenerName == str)
                        {
                            if (content.DoUserWork != null)
                            {
                                content.DoUserWork.CancelAsync();
                            }
                            if (content.DoMainWork != null)
                            {
                                content.DoMainWork.CancelAsync();
                            }
                        }
                        else
                        {
                            hashtable.Add(str2, content);
                        }
                    }
                    this.ListenerList.Clear();
                    this.ListenerList = hashtable;
                }
            }
        }

        private void doMainWork(object sender, DoWorkEventArgs e)
        {
            ListenerContent argument = (ListenerContent) e.Argument;
            Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
        Label_001B:
            if (argument.DoMainWork.CancellationPending)
            {
                argument.DoUserWork.CancelAsync();
                e.Cancel = true;
            }
            else
            {
                if (!argument.DoUserWork.IsBusy && (argument.QueueData.Count > 0))
                {
                    MessageQData data = (MessageQData) argument.QueueData.Dequeue();
                    argument.DoUserWork.RunWorkerAsync(data);
                }
                if (argument.QueueData.Count > 20)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    Thread.Sleep(20);
                }
                goto Label_001B;
            }
        }

        public bool Exists(string listenerName, string comport)
        {
            string str = string.Empty;
            if (((listenerName != null) && (listenerName.Length != 0)) && ((comport != null) && (comport.Length != 0)))
            {
                if (listenerName.Contains(@".\private$\"))
                {
                    str = listenerName;
                }
                else
                {
                    str = @".\private$\" + listenerName + "_" + comport;
                }
                foreach (string str2 in this.ListenerList.Keys)
                {
                    if (str == str2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public ListenerContent GetListenerQRef(string m_key)
        {
            if (((m_key != null) && (m_key.Length != 0)) && this.ListenerList.ContainsKey(m_key))
            {
                return (ListenerContent) this.ListenerList[m_key];
            }
            return null;
        }

        public ListenerContent GetListenerQRef(string listenerName, string comPort)
        {
            string key = string.Empty;
            if (((listenerName != null) && (listenerName.Length != 0)) && ((comPort != null) && (comPort.Length != 0)))
            {
                if (listenerName.Contains(@".\private$\"))
                {
                    key = listenerName;
                }
                else
                {
                    key = @".\private$\" + listenerName + "_" + comPort;
                }
                if (this.ListenerList.ContainsKey(key))
                {
                    return (ListenerContent) this.ListenerList[key];
                }
            }
            return null;
        }

        public ListenerContent GetListenerRef(string m_key)
        {
            if (((m_key != null) && (m_key.Length != 0)) && this.ListenerList.ContainsKey(m_key))
            {
                return (ListenerContent) this.ListenerList[m_key];
            }
            return null;
        }

        public static MessageQData Marshall(DoWorkEventArgs myArgs)
        {
            return (MessageQData) myArgs.Argument;
        }

        public void Start()
        {
            lock (this.ListenerLock)
            {
                foreach (string str in this.ListenerList.Keys)
                {
                    ListenerContent content = (ListenerContent) this.ListenerList[str];
                    this.Start(content.ListenerName);
                }
            }
        }

        public void Start(string listenerName)
        {
            if (((listenerName != null) && (listenerName.Length != 0)) && this.ListenerList.ContainsKey(listenerName))
            {
                ListenerContent argument = (ListenerContent) this.ListenerList[listenerName];
                if (!argument.State)
                {
                    argument.State = true;
                    if (!argument.DoMainWork.IsBusy)
                    {
                        argument.DoMainWork.RunWorkerAsync(argument);
                    }
                }
            }
        }

        public void Start(string listenerName, string comPort)
        {
            string key = string.Empty;
            if (((listenerName != null) && (listenerName.Length != 0)) && ((comPort != null) && (comPort.Length != 0)))
            {
                if (listenerName.Contains(@".\private$\"))
                {
                    key = listenerName;
                }
                else
                {
                    key = @".\private$\" + listenerName + "_" + comPort;
                }
                if (this.ListenerList.ContainsKey(key))
                {
                    ListenerContent argument = (ListenerContent) this.ListenerList[key];
                    if (!argument.State)
                    {
                        argument.State = true;
                        if (!argument.DoMainWork.IsBusy)
                        {
                            argument.DoMainWork.RunWorkerAsync(argument);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
            foreach (string str in this.ListenerList.Keys)
            {
                ListenerContent content = (ListenerContent) this.ListenerList[str];
                this.Stop(content.ListenerName);
            }
        }

        public void Stop(string listenerName)
        {
            if (((listenerName != null) && (listenerName.Length != 0)) && this.ListenerList.ContainsKey(listenerName))
            {
                ListenerContent content = (ListenerContent) this.ListenerList[listenerName];
                if (content.State)
                {
                    lock (this.ListenerLock)
                    {
                        content.State = false;
                        content.QueueData.Clear();
                    }
                }
            }
        }

        public void Stop(string listenerName, string comPort)
        {
            string key = string.Empty;
            if (((listenerName != null) && (listenerName.Length != 0)) && ((comPort != null) && (comPort.Length != 0)))
            {
                if (listenerName.Contains(@".\private$\"))
                {
                    key = listenerName;
                }
                else
                {
                    key = @".\private$\" + listenerName + "_" + comPort;
                }
                if (this.ListenerList.ContainsKey(key))
                {
                    ListenerContent content = (ListenerContent) this.ListenerList[key];
                    if (content.State)
                    {
                        lock (this.ListenerLock)
                        {
                            content.State = false;
                            content.QueueData.Clear();
                        }
                    }
                }
            }
        }

        public string AidingProtocol
        {
            get
            {
                return this._aidingProtocol;
            }
            set
            {
                this._aidingProtocol = value;
            }
        }

        public Hashtable ListenerList
        {
            get
            {
                return this._listenerList;
            }
            set
            {
                this._listenerList = value;
            }
        }

        public object ListenerLock
        {
            get
            {
                return this._listenerLock;
            }
            set
            {
                this._listenerLock = value;
            }
        }

        public CommunicationManager.ReceiverType RxType
        {
            get
            {
                return this._rxType;
            }
            set
            {
                this._rxType = value;
            }
        }
    }
}

