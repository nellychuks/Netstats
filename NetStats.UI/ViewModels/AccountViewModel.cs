using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Netstats.Core;

namespace Netstats.UI.ViewModels
{
    public class AccountViewModel : ReactiveObject
    {
        public ReactiveCommand ProccedToLoginCommand { get; }
        public ReactiveList<User> Users { get; set; } = new ReactiveList<User>();

        public AccountViewModel()
        {
            ProccedToLoginCommand = ReactiveCommand.Create<string>(x => ProceedToLogin(x));
            /*Load up saved users*/
            UserCredentialsStore.Instance.GetUsers().ObserveOnDispatcher()
                                                    .Subscribe(users =>
                                                    {
                                                        //Don't bother adding a user, if it's already there
                                                        foreach (var user in users)
                                                        {
                                                            if (!Users.Contains(user))
                                                                Users.Add(user);
                                                        }
                                                    });
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
