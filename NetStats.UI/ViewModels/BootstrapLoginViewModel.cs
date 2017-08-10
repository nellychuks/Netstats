using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Netstats.Core.Management;
using System.Collections.Generic;
using ReactiveUI.Legacy;

namespace Netstats.UI.ViewModels
{
    public class BootstrapLoginViewModel :ReactiveObject
    {
        public ReactiveCommand<object> ProccedToLoginCommand { get; }
        public ReactiveList<User> Users { get; set; } = new ReactiveList<User>();

        public BootstrapLoginViewModel()
        {
            ProccedToLoginCommand = ReactiveUI.Legacy.ReactiveCommand.Create();
            ProccedToLoginCommand.SubscribeOnDispatcher()
                                 .ObserveOnDispatcher()
                                 .Subscribe(x => ProceedToLogin(x as string));
            /*Load up saved users*/
            UserCredentialsStore.Instance.GetUsers().SubscribeOnDispatcher()
                                                    .ObserveOnDispatcher()
                                                    .Subscribe(UpdateUsers);
        }

        public void UpdateUsers(IEnumerable<User> users)
        {
            //Don't bother adding a user, if it's already there
            foreach (var user in users)
            {
                if (!Users.Contains(user))
                    Users.Add(user);
            }
        }

        public void ProceedToLogin(string alias)
        {
            if (alias != null)
                NavigationHelper.NavigateTo(ViewType.LoginView, Users.First(x => x.Alias == alias));
            else
                NavigationHelper.NavigateTo(ViewType.LoginView, null);
        }
    }
}
