﻿using System.Threading.Tasks;

namespace Olive
{
    class GetCache<T> : GetImplementation<T>
    {
        bool Silent;

        public GetCache(ApiClient client, bool silent) : base(client) { Silent = silent; }

        public override async Task<bool> Attempt(string url)
        {
            var cache = ApiResponseCache<T>.Create(url);

            if (!await cache.HasValidValue(CacheAge)) return false;

            Result = cache.Data;

            if (!Silent && FallBackEventPolicy == FallBackEventPolicy.Raise)
            {
                await FallBackEvent.Raise(new FallBackEvent
                {
                    Url = url,
                    CacheAge = cache.Age,
                    FriendlyMessage = $"Failed to get fresh results from {url.AsUri().Host}. Using the latest cache from {cache.Age.ToNaturalTime(1)} ago."
                });
            }

            return true;
        }
    }
}