using Telegram.Common.Firebase;
using Telegram.Common.Pages;
using Telegram.Common.User;

namespace Telegram.Common.Storage
{
    public class UserStateStorage(FirebaseProvider firebaseProvider, PagesFactory pagesFactory)
    {
        public async Task AddOrUpdateAsync(long telegramUserId, UserState userState)
        {
            var userStateFirebase = ToUserStateFirebase(userState);
            await firebaseProvider.AddOrUpdateAsync($"userStates/{telegramUserId}", userStateFirebase);
        }

        private UserStateFirebase ToUserStateFirebase(UserState userState)
        {
            return new UserStateFirebase
            {
                UserData = userState.UserData,
                PagesNames = userState.Pages.Select(f => f.GetType().Name).Reverse().ToList()
            };
        }

        public async Task<UserState> TryGetAsync(long telegramUserId)
        {
            var userStateFirebase = await firebaseProvider.TryGetAsync<UserStateFirebase>($"userStates/{telegramUserId}");
            if (userStateFirebase == null)
            {
                return null;
            }
            return ToUserState(userStateFirebase);
        }

        private UserState? ToUserState(UserStateFirebase userStateFirebase)
        {
            var pages = userStateFirebase.PagesNames.Select(f => pagesFactory.GetPage(f)).Reverse();
            return new UserState(new Stack<IPage>(pages), userStateFirebase.UserData);
        }
    }
}
