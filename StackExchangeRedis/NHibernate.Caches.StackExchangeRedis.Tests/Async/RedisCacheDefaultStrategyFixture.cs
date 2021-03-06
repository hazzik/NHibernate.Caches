﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Threading;
using NHibernate.Cache;
using NUnit.Framework;

namespace NHibernate.Caches.StackExchangeRedis.Tests
{
	using System.Threading.Tasks;
	public partial class RedisCacheDefaultStrategyFixture : RedisCacheFixture<DefaultRegionStrategy>
	{

		[Test]
		public async Task TestMaxAllowedVersionAsync()
		{
			var cache = (RedisCache) GetDefaultCache();
			var strategy = (DefaultRegionStrategy)cache.RegionStrategy;
			var version = strategy.CurrentVersion;

			var props = GetDefaultProperties();
			props.Add("cache.region_strategy.default.max_allowed_version", version.ToString());
			cache = (RedisCache) DefaultProvider.BuildCache(DefaultRegion, props);
			strategy = (DefaultRegionStrategy) cache.RegionStrategy;

			await (cache.ClearAsync(CancellationToken.None));

			Assert.That(strategy.CurrentVersion, Is.EqualTo(1L), "the version was not reset to 1");
		}

		[Test]
		public async Task TestClearWithMultipleClientsAndPubSubAsync()
		{
			const string key = "keyTestClear";
			const string value = "valueClear";

			var cache = (RedisCache)GetDefaultCache();
			var strategy = (DefaultRegionStrategy)cache.RegionStrategy;
			var cache2 = (RedisCache) GetDefaultCache();
			var strategy2 = (DefaultRegionStrategy) cache2.RegionStrategy;

			// add the item
			await (cache.PutAsync(key, value, CancellationToken.None));

			// make sure it's there
			var item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Not.Null, "couldn't find item in cache");

			item = await (cache2.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Not.Null, "couldn't find item in second cache");

			var version = strategy.CurrentVersion;

			// clear the cache
			await (cache.ClearAsync(CancellationToken.None));

			Assert.That(strategy.CurrentVersion, Is.EqualTo(version + 1), "the version has not been updated");
			await (Task.Delay(TimeSpan.FromSeconds(2)));
			Assert.That(strategy2.CurrentVersion, Is.EqualTo(version + 1), "the version should be updated with the pub/sub api");

			// make sure we don't get an item
			item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Null, "item still exists in cache after clear");

			item = await (cache2.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Null, "item still exists in the second cache after clear");
		}

		[Test]
		public async Task TestClearWithMultipleClientsAndNoPubSubAsync()
		{
			const string key = "keyTestClear";
			const string value = "valueClear";

			var props = GetDefaultProperties();
			props.Add("cache.region_strategy.default.use_pubsub", "false");

			var cache = (RedisCache) DefaultProvider.BuildCache(DefaultRegion, props);
			var strategy = (DefaultRegionStrategy) cache.RegionStrategy;
			var cache2 = (RedisCache) DefaultProvider.BuildCache(DefaultRegion, props);
			var strategy2 = (DefaultRegionStrategy) cache2.RegionStrategy;

			// add the item
			await (cache.PutAsync(key, value, CancellationToken.None));

			// make sure it's there
			var item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Not.Null, "couldn't find item in cache");

			item = await (cache2.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Not.Null, "couldn't find item in second cache");

			var version = strategy.CurrentVersion;

			// clear the cache
			await (cache.ClearAsync(CancellationToken.None));

			Assert.That(strategy.CurrentVersion, Is.EqualTo(version + 1), "the version has not been updated");
			await (Task.Delay(TimeSpan.FromSeconds(2)));
			Assert.That(strategy2.CurrentVersion, Is.EqualTo(version), "the version should not be updated");

			// make sure we don't get an item
			item = await (cache.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Null, "item still exists in cache after clear");

			item = await (cache2.GetAsync(key, CancellationToken.None));
			Assert.That(item, Is.Null, "item still exists in the second cache after clear");
		}
	}
}
