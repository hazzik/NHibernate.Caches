﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Cache;
using NHibernate.Caches.Common.Tests;
using NUnit.Framework;

namespace NHibernate.Caches.StackExRedis.Tests
{
	using System.Threading;
	public partial class RedisCachePerformanceFixture : Fixture
	{

		[Test]
		public Task TestGetOperationAsync()
		{
			return TestOperationAsync("Get", true, (cache, key, _) => cache.GetAsync(key, CancellationToken.None));
		}

		[Test]
		public Task TestGetManyOperationAsync()
		{
			return TestBatchOperationAsync("GetMany", true, (cache, keys, _) => cache.GetManyAsync(keys, CancellationToken.None));
		}

		[Test]
		public Task TestGetOperationWithSlidingExpirationAsync()
		{
			var props = new Dictionary<string, string> {{"sliding", "true"}};
			return TestOperationAsync("Get", true, (cache, key, _) => cache.GetAsync(key, CancellationToken.None),
				caches: new List<RedisCache> {GetDefaultRedisCache(props), GetFastRedisCache(props)});
		}

		[Test]
		public Task TestGetManyOperationWithSlidingExpirationAsync()
		{
			var props = new Dictionary<string, string> {{"sliding", "true"}};
			return TestBatchOperationAsync("GetMany", true, (cache, keys, _) => cache.GetManyAsync(keys, CancellationToken.None),
				caches: new List<RedisCache> {GetDefaultRedisCache(props), GetFastRedisCache(props)});
		}

		[Test]
		public Task TestPutOperationAsync()
		{
			var props = new Dictionary<string, string> {{"expiration", "0"}};
			return TestOperationAsync("Put", false, (cache, key, value) => cache.PutAsync(key, value, CancellationToken.None),
				caches: new List<RedisCache> {GetFastRedisCache(props)});
		}

		[Test]
		public Task TestPutManyOperationAsync()
		{
			var props = new Dictionary<string, string> {{"expiration", "0"}};
			return TestBatchOperationAsync("PutMany", false, (cache, keys, values) => cache.PutManyAsync(keys, values, CancellationToken.None),
				caches: new List<RedisCache> {GetFastRedisCache(props)});
		}

		[Test]
		public Task TestPutOperationWithExpirationAsync()
		{
			return TestOperationAsync("Put", false, (cache, key, value) => cache.PutAsync(key, value, CancellationToken.None));
		}

		[Test]
		public Task TestPutManyOperationWithExpirationAsync()
		{
			return TestBatchOperationAsync("PutMany", false, (cache, keys, values) => cache.PutManyAsync(keys, values, CancellationToken.None));
		}

		[Test]
		public Task TestLockUnlockOperationAsync()
		{
			return TestOperationAsync("Lock/Unlock", true, async (cache, key, _) =>
			{
				var value = await (cache.LockAsync(key, CancellationToken.None));
				await (cache.UnlockAsync(key, value, CancellationToken.None));
			});
		}

		[Test]
		public Task TestLockUnlockManyOperationAsync()
		{
			return TestBatchOperationAsync("LockMany/UnlockMany", true, async (cache, keys, _) =>
			{
				var value = await (cache.LockManyAsync(keys, CancellationToken.None));
				await (cache.UnlockManyAsync(keys, value, CancellationToken.None));
			});
		}

		private async Task PutCacheDataAsync(CacheBase cache, Dictionary<CacheKey, List<object>> cacheData, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var pair in cacheData)
			{
				await (cache.PutAsync(pair.Key, pair.Value, cancellationToken));
			}
		}

		private async Task RemoveCacheDataAsync(CacheBase cache, Dictionary<CacheKey, List<object>> cacheData, CancellationToken cancellationToken = default(CancellationToken))
		{
			foreach (var pair in cacheData)
			{
				await (cache.RemoveAsync(pair.Key, cancellationToken));
			}
		}
	}
}
