using Firebase.Database;
using Firebase.Database.Query;

namespace Telegram.Common.Firebase
{
    public class FirebaseProvider
    {
        private FirebaseClient client;

        public FirebaseProvider(FirebaseClient client)
        {
            this.client = client;
        }

        public async Task<T> TryGetAsync<T>(string key)
        {
            return await client.Child(key).OnceSingleAsync<T>();
        }

        public async Task AddOrUpdateAsync<T>(string key, T item)
        {
            await client
                .Child(key)
                .PutAsync(item);
        }
    }
}
