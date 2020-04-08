using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;
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

        public async void SaveUser(Player user)
        {
            await LocalStorage.SetItemAsync("skribblUser_name", user.Name);
            await LocalStorage.SetItemAsync("skribblUser_id", user.Id);
        }

        public async Task<Player> GetUser()
        {
            var name = await LocalStorage.GetItemAsync("skribblUser_name");
            var id = await LocalStorage.GetItemAsync("skribblUser_id");
            if (name == null || id == null)
            {
                throw new UserNotInLocalStorageException("User not found! Or you do not have access to this lobby.");
            }
            var user = new Player(name)
            {
                Id = id
            };
            return user;
        }
    }
}
