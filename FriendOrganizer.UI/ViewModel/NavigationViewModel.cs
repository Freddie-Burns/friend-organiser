﻿using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupService;
        private IEventAggregator _eventAggregator;

        public NavigationViewModel(
            IFriendLookupDataService friendLookupService,
            IEventAggregator eventAggregator)
        {
            _friendLookupService = friendLookupService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);

            Friends = new ObservableCollection<NavigationItemViewModel>();
        }

        private void AfterFriendSaved(AfterFriendSavedEventArgs obj)
        {
            var lookupItem = Friends.SingleOrDefault(l => l.Id == obj.Id);
            if (lookupItem == null)
            {
                Friends.Add(new NavigationItemViewModel(
                    obj.Id,
                    obj.DisplayMember,
                    _eventAggregator));
            }
            else
            {
                lookupItem.DisplayMember = obj.DisplayMember;
            }
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var item in lookup)
            {
                Friends.Add(new NavigationItemViewModel(item.Id, item.DisplayMember, _eventAggregator));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; }
    }
}
