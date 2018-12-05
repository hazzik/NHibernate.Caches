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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using NHibernate.Cache;
using NHibernate.Cache.Entry;
using NHibernate.Caches.Common.Tests;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Caches.StackExRedis.Tests
{
	using System.Threading.Tasks;
	public abstract partial class RedisCacheFixture : CacheFixture
	{

		[Test]
		public async Task TestNHibernateAnyTypeSerializationAsync()
		{
			var objectTypeCacheEntryType = typeof(AnyType.ObjectTypeCacheEntry);
			var entityNameField = objectTypeCacheEntryType.GetField("entityName", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(entityNameField, Is.Not.Null, "field entityName in NHibernate.Type.AnyType.ObjectTypeCacheEntry was not found");
			var idField = objectTypeCacheEntryType.GetField("id", BindingFlags.Instance | BindingFlags.NonPublic);
			Assert.That(idField, Is.Not.Null, "field id in NHibernate.Type.AnyType.ObjectTypeCacheEntry was not found");

			var entityName = nameof(MyEntity);
			var propertyValues = new Dictionary<IType, object>
			{
				{NHibernateUtil.Object, new MyEntity{Id = 2}}
			};

			var sfImpl = Substitute.For<ISessionFactoryImplementor>();
			var sessionImpl = Substitute.For<ISessionImplementor>();
			sessionImpl.BestGuessEntityName(Arg.Any<object>()).Returns(o => o[0].GetType().Name);
			sessionImpl.GetContextEntityIdentifier(Arg.Is<object>(o => o is MyEntity)).Returns(o => ((MyEntity) o[0]).Id);
			var entityPersister = Substitute.For<IEntityPersister>();
			entityPersister.EntityName.Returns(entityName);
			entityPersister.IsLazyPropertiesCacheable.Returns(false);
			entityPersister.PropertyTypes.Returns(propertyValues.Keys.ToArray());

			var cacheKey = new CacheKey(1, NHibernateUtil.Int32, entityName, sfImpl);
			var cacheEntry = await (CacheEntry.CreateAsync(propertyValues.Values.ToArray(), entityPersister, false, null, sessionImpl, null, CancellationToken.None));

			Assert.That(cacheEntry.DisassembledState, Has.Length.EqualTo(1));
			var anyObject = cacheEntry.DisassembledState[0];
			Assert.That(anyObject, Is.TypeOf(objectTypeCacheEntryType));
			Assert.That(entityNameField.GetValue(anyObject), Is.EqualTo(nameof(MyEntity)));
			Assert.That(idField.GetValue(anyObject), Is.EqualTo(2));

			var cache = GetDefaultCache();
			await (cache.PutAsync(cacheKey, cacheEntry, CancellationToken.None));
			var value = await (cache.GetAsync(cacheKey, CancellationToken.None));

			Assert.That(value, Is.TypeOf<CacheEntry>());
			var retrievedCacheEntry = (CacheEntry) value;
			Assert.That(retrievedCacheEntry.DisassembledState, Has.Length.EqualTo(1));
			var retrievedAnyObject = retrievedCacheEntry.DisassembledState[0];
			Assert.That(retrievedAnyObject, Is.TypeOf(objectTypeCacheEntryType));
			Assert.That(entityNameField.GetValue(retrievedAnyObject), Is.EqualTo(nameof(MyEntity)),
				"entityName is different from the original AnyType.ObjectTypeCacheEntry");
			Assert.That(idField.GetValue(retrievedAnyObject), Is.EqualTo(2),
				"id is different from the original AnyType.ObjectTypeCacheEntry");
		}

		[Test]
		public async Task TestNHibernateStandardTypesSerializationAsync()
		{
			var entityName = nameof(MyEntity);
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<Root>XmlDoc</Root>");
			var propertyValues = new Dictionary<IType, object>
			{
				{NHibernateUtil.AnsiString, "test"},
				{NHibernateUtil.Binary, new byte[] {1, 2, 3, 4}},
				{NHibernateUtil.BinaryBlob, new byte[] {1, 2, 3, 4}},
				{NHibernateUtil.Boolean, true},
				{NHibernateUtil.Byte, (byte) 1},
				{NHibernateUtil.Character, 'a'},
				{NHibernateUtil.CultureInfo, CultureInfo.CurrentCulture},
				{NHibernateUtil.DateTime, DateTime.Now},
				{NHibernateUtil.DateTimeNoMs, DateTime.Now},
				{NHibernateUtil.LocalDateTime, DateTime.Now},
				{NHibernateUtil.UtcDateTime, DateTime.UtcNow},
				{NHibernateUtil.LocalDateTimeNoMs, DateTime.Now},
				{NHibernateUtil.UtcDateTimeNoMs, DateTime.UtcNow},
				{NHibernateUtil.DateTimeOffset, DateTimeOffset.Now},
				{NHibernateUtil.Date, DateTime.Today},
				{NHibernateUtil.Decimal, 2.5m},
				{NHibernateUtil.Double, 2.5d},
				{NHibernateUtil.Currency, 2.5m},
				{NHibernateUtil.Guid, Guid.NewGuid()},
				{NHibernateUtil.Int16, (short) 1},
				{NHibernateUtil.Int32, 3},
				{NHibernateUtil.Int64, 3L},
				{NHibernateUtil.SByte, (sbyte) 1},
				{NHibernateUtil.UInt16, (ushort) 1},
				{NHibernateUtil.UInt32, (uint) 1},
				{NHibernateUtil.UInt64, (ulong) 1},
				{NHibernateUtil.Single, 1.1f},
				{NHibernateUtil.String, "test"},
				{NHibernateUtil.StringClob, "test"},
				{NHibernateUtil.Time, DateTime.Now},
				{NHibernateUtil.Ticks, DateTime.Now},
				{NHibernateUtil.TimeAsTimeSpan, TimeSpan.FromMilliseconds(15)},
				{NHibernateUtil.TimeSpan, TimeSpan.FromMilliseconds(1234)},
				{NHibernateUtil.DbTimestamp, DateTime.Now},
				{NHibernateUtil.TrueFalse, false},
				{NHibernateUtil.YesNo, true},
				{NHibernateUtil.Class, typeof(IType)},
				{NHibernateUtil.MetaType, entityName},
				{NHibernateUtil.Serializable, new MyEntity {Id = 1}},
				{NHibernateUtil.AnsiChar, 'a'},
				{NHibernateUtil.XmlDoc, xmlDoc},
				{NHibernateUtil.XDoc, XDocument.Parse("<Root>XDoc</Root>")},
				{NHibernateUtil.Uri, new Uri("http://test.com")}
			};

			var sfImpl = Substitute.For<ISessionFactoryImplementor>();
			var sessionImpl = Substitute.For<ISessionImplementor>();
			var entityPersister = Substitute.For<IEntityPersister>();
			entityPersister.EntityName.Returns(entityName);
			entityPersister.IsLazyPropertiesCacheable.Returns(false);
			entityPersister.PropertyTypes.Returns(propertyValues.Keys.ToArray());

			var cacheKey = new CacheKey(1, NHibernateUtil.Int32, entityName, sfImpl);
			var cacheEntry = await (CacheEntry.CreateAsync(propertyValues.Values.ToArray(), entityPersister, false, null, sessionImpl, null, CancellationToken.None));

			var cache = GetDefaultCache();
			await (cache.PutAsync(cacheKey, cacheEntry, CancellationToken.None));
			var value = await (cache.GetAsync(cacheKey, CancellationToken.None));

			Assert.That(value, Is.TypeOf<CacheEntry>());
			var retrievedCacheEntry = (CacheEntry) value;
			Assert.That(retrievedCacheEntry.DisassembledState, Is.EquivalentTo(cacheEntry.DisassembledState),
				"DisassembledState is different from the original CacheEntry");
		}

		[Test]
		public async Task TestNHibernateCacheEntrySerializationAsync()
		{
			var entityName = nameof(MyEntity);
			var propertyValues = new Dictionary<IType, object>
			{
				{NHibernateUtil.String, "test"}
			};

			var sfImpl = Substitute.For<ISessionFactoryImplementor>();
			var sessionImpl = Substitute.For<ISessionImplementor>();
			var entityPersister = Substitute.For<IEntityPersister>();
			entityPersister.EntityName.Returns(entityName);
			entityPersister.IsLazyPropertiesCacheable.Returns(false);
			entityPersister.PropertyTypes.Returns(propertyValues.Keys.ToArray());

			var cacheKey = new CacheKey(1, NHibernateUtil.Int32, entityName, sfImpl);
			var cacheEntry = await (CacheEntry.CreateAsync(propertyValues.Values.ToArray(), entityPersister, true, 4, sessionImpl, null, CancellationToken.None));

			var cache = GetDefaultCache();
			await (cache.PutAsync(cacheKey, cacheEntry, CancellationToken.None));
			var value = await (cache.GetAsync(cacheKey, CancellationToken.None));

			Assert.That(value, Is.TypeOf<CacheEntry>());
			var retrievedCacheEntry = (CacheEntry) value;
			Assert.That(retrievedCacheEntry.AreLazyPropertiesUnfetched, Is.EqualTo(cacheEntry.AreLazyPropertiesUnfetched),
				"AreLazyPropertiesUnfetched is different from the original CacheEntry");
			Assert.That(retrievedCacheEntry.DisassembledState, Is.EquivalentTo(cacheEntry.DisassembledState),
				"DisassembledState is different from the original CacheEntry");
			Assert.That(retrievedCacheEntry.Subclass, Is.EqualTo(cacheEntry.Subclass),
				"Subclass is different from the original CacheEntry");
			Assert.That(retrievedCacheEntry.Version, Is.EqualTo(cacheEntry.Version),
				"Version is different from the original CacheEntry");
		}

		[Test]
		public async Task TestNHibernateCollectionCacheEntrySerializationAsync()
		{
			var sfImpl = Substitute.For<ISessionFactoryImplementor>();
			var collection = Substitute.For<IPersistentCollection>();
			collection.Disassemble(null).Returns(o => new object[] {"test"});

			var cacheKey = new CacheKey(1, NHibernateUtil.Int32, "MyCollection", sfImpl);
			var cacheEntry = await (CollectionCacheEntry.CreateAsync(collection, null, CancellationToken.None));
			Assert.That(cacheEntry.State, Has.Length.EqualTo(1));

			var cache = GetDefaultCache();
			await (cache.PutAsync(cacheKey, cacheEntry, CancellationToken.None));
			var value = await (cache.GetAsync(cacheKey, CancellationToken.None));

			Assert.That(value, Is.TypeOf<CollectionCacheEntry>());
			var retrievedCacheEntry = (CollectionCacheEntry) value;
			Assert.That(retrievedCacheEntry.State, Has.Length.EqualTo(1));
			Assert.That(retrievedCacheEntry.State[0], Is.EquivalentTo("test"),
				"State is different from the original CollectionCacheEntry");
		}

		[Test]
		public async Task TestNHibernateCacheLockSerializationAsync()
		{
			var sfImpl = Substitute.For<ISessionFactoryImplementor>();
			var cacheKey = new CacheKey(1, NHibernateUtil.Int32, "CacheLock", sfImpl);
			var cacheEntry = new CacheLock
			{
				Timeout = 1234, Id = 1, Version = 5
			};
			cacheEntry.Lock(123, 2);

			var cache = GetDefaultCache();
			await (cache.PutAsync(cacheKey, cacheEntry, CancellationToken.None));
			var value = await (cache.GetAsync(cacheKey, CancellationToken.None));

			Assert.That(value, Is.TypeOf<CacheLock>());
			var retrievedCacheEntry = (CacheLock) value;
			Assert.That(retrievedCacheEntry.Id, Is.EqualTo(cacheEntry.Id),
				"Id is different from the original CacheLock");
			Assert.That(retrievedCacheEntry.IsLock, Is.EqualTo(cacheEntry.IsLock),
				"IsLock is different from the original CacheLock");
			Assert.That(retrievedCacheEntry.WasLockedConcurrently, Is.EqualTo(cacheEntry.WasLockedConcurrently),
				"WasLockedConcurrently is different from the original CacheLock");
			Assert.That(retrievedCacheEntry.ToString(), Is.EqualTo(cacheEntry.ToString()),
				"ToString() is different from the original CacheLock");
		}

		[Test]
		public async Task TestNHibernateCachedItemSerializationAsync()
		{
			var sfImpl = Substitute.For<ISessionFactoryImplementor>();
			var cacheKey = new CacheKey(1, NHibernateUtil.Int32, "CachedItem", sfImpl);
			var cacheEntry = new CachedItem
			{
				Value = "test", FreshTimestamp = 111, Version = 5
			};
			cacheEntry.Lock(123, 2);

			var cache = GetDefaultCache();
			await (cache.PutAsync(cacheKey, cacheEntry, CancellationToken.None));
			var value = await (cache.GetAsync(cacheKey, CancellationToken.None));

			Assert.That(value, Is.TypeOf<CachedItem>());
			var retrievedCacheEntry = (CachedItem) value;
			Assert.That(retrievedCacheEntry.FreshTimestamp, Is.EqualTo(cacheEntry.FreshTimestamp),
				"FreshTimestamp is different from the original CachedItem");
			Assert.That(retrievedCacheEntry.IsLock, Is.EqualTo(cacheEntry.IsLock),
				"IsLock is different from the original CachedItem");
			Assert.That(retrievedCacheEntry.Value, Is.EqualTo(cacheEntry.Value),
				"Value is different from the original CachedItem");
			Assert.That(retrievedCacheEntry.ToString(), Is.EqualTo(cacheEntry.ToString()),
				"ToString() is different from the original CachedItem");
		}

		[Test]
		public async Task TestEqualObjectsWithDifferentHashCodeAsync()
		{
			var value = "value";
			var obj1 = new CustomCacheKey(1, "test", false);
			var obj2 = new CustomCacheKey(1, "test", false);

			var cache = GetDefaultCache();

			await (cache.PutAsync(obj1, value, CancellationToken.None));
			Assert.That(await (cache.GetAsync(obj1, CancellationToken.None)), Is.EqualTo(value), "Unable to retrieved cached object for key obj1");
			Assert.That(await (cache.GetAsync(obj2, CancellationToken.None)), Is.EqualTo(value), "Unable to retrieved cached object for key obj2");
			await (cache.RemoveAsync(obj1, CancellationToken.None));
		}

		[Test]
		public async Task TestEqualObjectsWithDifferentHashCodeAndUseHashCodeGlobalConfigurationAsync()
		{
			var value = "value";
			var obj1 = new CustomCacheKey(1, "test", false);
			var obj2 = new CustomCacheKey(1, "test", false);

			var props = GetDefaultProperties();
			var cacheProvider = ProviderBuilder();
			props[RedisEnvironment.AppendHashcode] = "true";
			cacheProvider.Start(props);
			var cache = cacheProvider.BuildCache(DefaultRegion, props);

			await (cache.PutAsync(obj1, value, CancellationToken.None));
			Assert.That(await (cache.GetAsync(obj1, CancellationToken.None)), Is.EqualTo(value), "Unable to retrieved cached object for key obj1");
			Assert.That(await (cache.GetAsync(obj2, CancellationToken.None)), Is.Null, "The hash code should be used in the cache key");
			await (cache.RemoveAsync(obj1, CancellationToken.None));
		}

		[Test]
		public async Task TestEqualObjectsWithDifferentHashCodeAndUseHashCodeRegionConfigurationAsync()
		{
			var value = "value";
			var obj1 = new CustomCacheKey(1, "test", false);
			var obj2 = new CustomCacheKey(1, "test", false);

			var props = GetDefaultProperties();
			var cacheProvider = ProviderBuilder();
			cacheProvider.Start(props);
			props["append-hashcode"] = "true";
			var cache = cacheProvider.BuildCache(DefaultRegion, props);

			await (cache.PutAsync(obj1, value, CancellationToken.None));
			Assert.That(await (cache.GetAsync(obj1, CancellationToken.None)), Is.EqualTo(value), "Unable to retrieved cached object for key obj1");
			Assert.That(await (cache.GetAsync(obj2, CancellationToken.None)), Is.Null, "The hash code should be used in the cache key");
			await (cache.RemoveAsync(obj1, CancellationToken.None));
		}

		[Test]
		public async Task TestNonEqualObjectsWithEqualToStringAsync()
		{
			var value = "value";
			var obj1 = new CustomCacheKey(new ObjectEqualToString(1), "test", true);
			var obj2 = new CustomCacheKey(new ObjectEqualToString(2), "test", true);

			var cache = GetDefaultCache();

			await (cache.PutAsync(obj1, value, CancellationToken.None));
			Assert.That(await (cache.GetAsync(obj1, CancellationToken.None)), Is.EqualTo(value), "Unable to retrieved cached object for key obj1");
			Assert.That(await (cache.GetAsync(obj2, CancellationToken.None)), Is.EqualTo(value), "Unable to retrieved cached object for key obj2");
			await (cache.RemoveAsync(obj1, CancellationToken.None));
		}

		[Test]
		public async Task TestNonEqualObjectsWithEqualToStringUseHashCodeAsync()
		{
			var value = "value";
			var obj1 = new CustomCacheKey(new ObjectEqualToString(1), "test", true);
			var obj2 = new CustomCacheKey(new ObjectEqualToString(2), "test", true);

			var props = GetDefaultProperties();
			var cacheProvider = ProviderBuilder();
			props[RedisEnvironment.AppendHashcode] = "true";
			cacheProvider.Start(props);
			var cache = cacheProvider.BuildCache(DefaultRegion, props);

			await (cache.PutAsync(obj1, value, CancellationToken.None));
			Assert.That(await (cache.GetAsync(obj1, CancellationToken.None)), Is.EqualTo(value), "Unable to retrieved cached object for key obj1");
			Assert.That(await (cache.GetAsync(obj2, CancellationToken.None)), Is.Null, "Unexpectedly found a cache entry for key obj2 after obj1 put");
			await (cache.RemoveAsync(obj1, CancellationToken.None));
		}

		[Test]
		public async Task TestEnvironmentNameAsync()
		{
			var props = GetDefaultProperties();

			var developProvider = ProviderBuilder();
			props[RedisEnvironment.EnvironmentName] = "develop";
			developProvider.Start(props);
			var developCache = developProvider.BuildCache(DefaultRegion, props);

			var releaseProvider = ProviderBuilder();
			props[RedisEnvironment.EnvironmentName] = "release";
			releaseProvider.Start(props);
			var releaseCache = releaseProvider.BuildCache(DefaultRegion, props);

			const string key = "testKey";
			const string value = "testValue";

			await (developCache.PutAsync(key, value, CancellationToken.None));

			Assert.That(await (releaseCache.GetAsync(key, CancellationToken.None)), Is.Null, "release environment should be separate from develop");

			await (developCache.RemoveAsync(key, CancellationToken.None));
			await (releaseCache.PutAsync(key, value, CancellationToken.None));

			Assert.That(await (developCache.GetAsync(key, CancellationToken.None)), Is.Null, "develop environment should be separate from release");

			await (releaseCache.RemoveAsync(key, CancellationToken.None));

			developProvider.Stop();
			releaseProvider.Stop();
		}

		[Test]
		public async Task TestPutManyAsync()
		{
			var keys = new object[10];
			var values = new object[10];
			for (var i = 0; i < keys.Length; i++)
			{
				keys[i] = $"keyTestPut{i}";
				values[i] = $"valuePut{i}";
			}

			var cache = (RedisCache) GetDefaultCache();
			// Due to async version, it may already be there.
			foreach (var key in keys)
				await (cache.RemoveAsync(key, CancellationToken.None));

			Assert.That(await (cache.GetManyAsync(keys, CancellationToken.None)), Is.EquivalentTo(new object[10]), "cache returned items we didn't add !?!");

			await (cache.PutManyAsync(keys, values, CancellationToken.None));
			var items = await (cache.GetManyAsync(keys, CancellationToken.None));

			for (var i = 0; i < items.Length; i++)
			{
				var item = items[i];
				Assert.That(item, Is.Not.Null, "unable to retrieve cached item");
				Assert.That(item, Is.EqualTo(values[i]), "didn't return the item we added");
			}
		}

		[Test]
		public async Task TestRemoveManyAsync()
		{
			var keys = new object[10];
			var values = new object[10];
			for (var i = 0; i < keys.Length; i++)
			{
				keys[i] = $"keyTestRemove{i}";
				values[i] = $"valueRemove{i}";
			}

			var cache = (RedisCache) GetDefaultCache();

			// add the item
			await (cache.PutManyAsync(keys, values, CancellationToken.None));

			// make sure it's there
			var items = await (cache.GetManyAsync(keys, CancellationToken.None));
			Assert.That(items, Is.EquivalentTo(values), "items just added are not there");

			// remove it
			foreach (var key in keys)
				await (cache.RemoveAsync(key, CancellationToken.None));

			// make sure it's not there
			items = await (cache.GetManyAsync(keys, CancellationToken.None));
			Assert.That(items, Is.EquivalentTo(new object[10]), "items still exists in cache after remove");
		}

		[Test]
		public async Task TestLockUnlockManyAsync()
		{
			if (!SupportsLocking)
				Assert.Ignore("Test not supported by provider");

			var keys = new object[10];
			var values = new object[10];
			for (var i = 0; i < keys.Length; i++)
			{
				keys[i] = $"keyTestLock{i}";
				values[i] = $"valueLock{i}";
			}

			var cache = (RedisCache)GetDefaultCache();

			// add the item
			await (cache.PutManyAsync(keys, values, CancellationToken.None));
			await (cache.LockManyAsync(keys, CancellationToken.None));
			Assert.ThrowsAsync<CacheException>(() => cache.LockManyAsync(keys, CancellationToken.None), "all items should be locked");

			await (Task.Delay(cache.Timeout / Timestamper.OneMs));

			for (var i = 0; i < 2; i++)
			{
				Assert.DoesNotThrowAsync(async () =>
				{
					await (cache.UnlockManyAsync(keys, await (cache.LockManyAsync(keys, CancellationToken.None)), CancellationToken.None));
				}, "the items should be unlocked");
			}

			// Test partial locks by locking the first 5 keys and afterwards try to lock last 6 keys.
			var lockValue = await (cache.LockManyAsync(keys.Take(5).ToArray(), CancellationToken.None));

			Assert.ThrowsAsync<CacheException>(() => cache.LockManyAsync(keys.Skip(4).ToArray(), CancellationToken.None), "the fifth key should be locked");

			Assert.DoesNotThrowAsync(async () =>
			{
				await (cache.UnlockManyAsync(keys, await (cache.LockManyAsync(keys.Skip(5).ToArray(), CancellationToken.None)), CancellationToken.None));
			}, "the last 5 keys should not be locked.");

			// Unlock the first 5 keys
			await (cache.UnlockManyAsync(keys, lockValue, CancellationToken.None));

			Assert.DoesNotThrowAsync(async () =>
			{
				lockValue = await (cache.LockManyAsync(keys, CancellationToken.None));
				await (cache.UnlockManyAsync(keys, lockValue, CancellationToken.None));
			}, "the first 5 keys should not be locked.");
		}

		[Test]
		public void TestNullKeyPutManyAsync()
		{
			var cache = (RedisCache) GetDefaultCache();
			Assert.ThrowsAsync<ArgumentNullException>(() => cache.PutManyAsync(null, null, CancellationToken.None));
		}

		[Test]
		public void TestNullValuePutManyAsync()
		{
			var cache = (RedisCache) GetDefaultCache();
			Assert.ThrowsAsync<ArgumentNullException>(() => cache.PutManyAsync(new object[] { "keyTestNullValuePut" }, null, CancellationToken.None));
		}

		[Test]
		public async Task TestNullKeyGetManyAsync()
		{
			var cache = (RedisCache) GetDefaultCache();
			await (cache.PutAsync("keyTestNullKeyGet", "value", CancellationToken.None));
			var items = await (cache.GetManyAsync(null, CancellationToken.None));
			Assert.IsNull(items);
		}
	}
}
