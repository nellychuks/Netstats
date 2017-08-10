using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Netstats.UI;
using System.Threading;
using ReactiveUI.Legacy;

namespace Netstats.UI
{
    public class ShellViewModel:ReactiveObject
    {
        private bool showAbout;
        private SemaphoreSlim gate;

        public ShellViewModel()
        {
            gate = new SemaphoreSlim(1, 1);
            ShowAboutViewCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ShowAboutViewCommand.Subscribe(_ => ShowAbout = true);
            LockAppCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            LockAppCommand.Subscribe(async _ => await LockApp());
        }

        public bool ShowAbout { get { return showAbout; } set { this.RaiseAndSetIfChanged(ref showAbout, value); } }
        public ReactiveCommand<object> ShowAboutViewCommand { get; private set; }
        public ReactiveCommand<object> LockAppCommand { get; private set; }

        public async Task LockApp()
        {
            await gate.WaitAsync();
            if (!Global.IsAppLocked)
            {
                Global.IsAppLocked = true;
                NavigationHelper.NavigateTo(ViewType.PinEntryView, null);
            }
            gate.Release();
        }

    }
}
