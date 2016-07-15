using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using ImageScaler.Interfaces;

namespace ImageScaler.Resources
{
    /// <summary>
    /// Exports event logs on a worker thread with progress reporting
    /// </summary>
    public class EventLogExporter : IProgressOperation
    {
        private int _total;
        private int _current;
        private bool _isCancelationPending;

        public EventLogExporter()
        {
            this._total = 0;
            this._current = 0;
            this._isCancelationPending = false;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            EventLog[] eventLogs = EventLog.GetEventLogs();

            //get the total number of lines in the event logs
            //so a progress bar maximum can be updated
            int numRows = 0;
            foreach (EventLog eventLog in eventLogs)
            {
                numRows += eventLog.Entries.Count;
            }
            this.Total = numRows;

            foreach (EventLog eventLog in eventLogs)
            {
                //only write the log if entries exist
                if (eventLog.Entries.Count > 0)
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        DateTime now = DateTime.Now;
                        string fileName = String.Format("{0}-{1:00}-{2:00}_{3:00}{4:00}{5:00}_EventLog_{6}.log", new object[] { now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, eventLog.LogDisplayName });

                        foreach (EventLogEntry eventLogEntry in eventLog.Entries)
                        {
                            //exit if the user cancels
                            if (this._isCancelationPending == true)
                            {
                                return;
                            }

                            string row = String.Format("{0} {1} {2} {3}", eventLogEntry.EntryType, eventLogEntry.TimeWritten, eventLogEntry.Source, eventLogEntry.Message);

                            row = row.Replace("\r", String.Empty);
                            row = row.Replace("\n", String.Empty);
                            row += "\r\n";
                            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                            byte[] rowData = asciiEncoding.GetBytes(row);
                            memStream.Write(rowData, 0, rowData.Length);

                            //notify that the current event log line has changed
                            //(Updates progress bar)
                            this.Current++;
                        }

                        //write the log
                        using (FileStream fs = File.Create(fileName))
                        {
                            memStream.WriteTo(fs);
                        }
                    }
                }
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OnComplete(EventArgs.Empty);
        }

        protected virtual void OnProgressChanged(EventArgs e)
        {
            if (this.ProgressChanged != null)
            {
                this.ProgressChanged(this, e);
            }
        }

        protected virtual void OnProgressTotalChanged(EventArgs e)
        {
            if (this.ProgressTotalChanged != null)
            {
                this.ProgressTotalChanged(this, e);
            }
        }

        protected virtual void OnComplete(EventArgs e)
        {
            if (this.Complete != null)
            {
                this.Complete(this, e);
            }
        }

        #region IProgressOperation Members

        public int Total
        {
            get
            {
                return this._total;
            }
            private set
            {
                this._total = value;
                OnProgressTotalChanged(EventArgs.Empty);
            }
        }

        public int Current
        {
            get
            {
                return this._current;
            }
            private set
            {
                this._current = value;
                OnProgressChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Starts the background operation that will export the event logs
        /// </summary>
        public void Start()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Requests cancelation of the event log exporting
        /// </summary>
        public void CancelAsync()
        {
            this._isCancelationPending = true;
        }

        public event EventHandler ProgressChanged;
        public event EventHandler ProgressTotalChanged;
        public event EventHandler Complete;

        #endregion
    }
}
