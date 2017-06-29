using System;
using System.ComponentModel;

namespace NetStats.UI
{
    public class StatsViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public StatsViewModel()
        {

        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}

