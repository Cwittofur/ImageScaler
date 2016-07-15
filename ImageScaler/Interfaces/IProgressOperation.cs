using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageScaler.Interfaces
{
    public interface IProgressOperation
    {
        int Total { get; }
        int Current { get; }
        void Start();
        void CancelAsync();
        event EventHandler ProgressChanged;
        event EventHandler ProgressTotalChanged;
        event EventHandler Complete;
    }
}
