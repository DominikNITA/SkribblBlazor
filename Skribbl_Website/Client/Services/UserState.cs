using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Client.Services
{
    public class UserState
    {
        public LocalStorage LocalStorage { get; set; }

        public UserState(LocalStorage localStorage)
        {
            LocalStorage = localStorage;
        }

        public async void SaveUser(UserDto user)
        {
            //TODO: save User instance instead of particular class members
            await LocalStorage.SetItemAsync("skribblUser_name", user.Name);
            await LocalStorage.SetItemAsync("skribblUser_id", user.Id);
            await LocalStorage.SetItemAsync("skribblUser_isHost", user.IsHost);
        }

        public async Task<UserDto> GetUser()
        {
            var name = await LocalStorage.GetItemAsync("skribblUser_name");
            var id = await LocalStorage.GetItemAsync("skribblUser_id");
            var isHost = await LocalStorage.GetItemAsync<bool?>("skribblUser_isHost");
            if (name == null || id == null || isHost == null)
            {
                throw new UserNotInLocalStorageException("User not found! Or you do not have access to this lobby.");
            }
            var user = new UserDto(name);
            user.Id = id;
            user.IsHost = isHost.Value;
            return user;
        }
    }
}
