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
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Caches.Common;
using NHibernate.Caches.StackExchangeRedis.Messages;
using StackExchange.Redis;

namespace NHibernate.Caches.StackExchangeRedis
{
	internal partial class TwoLayerCache
	{

		public Task PutAsync(RedisKey cacheKey, object value, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return PutLocalAsync(cacheKey, value, true, cancellationToken);
		}

		public async Task<object> GetAsync(string cacheKey, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var value = GetLocal(cacheKey);
			if (value != null)
			{
				return value;
			}

			var version = _version;
			_log.Debug("Object was not found in local cache, fetching it from Redis.");
			value = await (ExecuteGetAsync(cacheKey, cancellationToken)).ConfigureAwait(false);
			if (value == null)
			{
				return null;
			}

			TimeSpan? timeToLive = null;
			if (_expirationEnabled && !_useSlidingExpiration)
			{
				cancellationToken.ThrowIfCancellationRequested();
				// We have to be careful as the key may be removed between base.Get and Database.KeyTimeToLive calls.
				// In that case, timeToLive will be null
				timeToLive = await (_database.KeyTimeToLiveAsync(cacheKey)).ConfigureAwait(false);
				if (!timeToLive.HasValue)
				{
					return null;
				}
			}

			return AddAndGet(cacheKey, value, version, timeToLive);
		}

		public async Task<object[]> GetManyAsync(RedisKey[] cacheKeys, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int i;
			var values = new object[cacheKeys.Length];
			List<KeyValuePair<int, RedisKey>> missingCacheKeys = null;
			for (i = 0; i < cacheKeys.Length; i++)
			{
				values[i] = GetLocal(cacheKeys[i]);
				if (values[i] != null)
				{
					continue;
				}

				if (missingCacheKeys == null)
				{
					missingCacheKeys = new List<KeyValuePair<int, RedisKey>>();
				}

				missingCacheKeys.Add(new KeyValuePair<int, RedisKey>(i, cacheKeys[i]));
			}

			if (missingCacheKeys == null)
			{
				return values;
			}

			RedisKey[] missingKeys;
			if (missingCacheKeys.Count == cacheKeys.Length)
			{
				missingKeys = cacheKeys;
			}
			else
			{
				missingKeys = new RedisKey[missingCacheKeys.Count];
				for (i = 0; i < missingCacheKeys.Count; i++)
				{
					missingKeys[i] = cacheKeys[missingCacheKeys[i].Key];
				}
			}

			var version = _version;
			RedisValue[] timesToLive = null;
			var redisValues = await (ExecuteGetManyAsync(missingKeys, cancellationToken)).ConfigureAwait(false);
			if (_expirationEnabled && !_useSlidingExpiration)
			{
				cancellationToken.ThrowIfCancellationRequested();
				timesToLive = (RedisValue[]) await (_database.ScriptEvaluateAsync(GetManyTimeToLiveLuaScript, missingKeys)).ConfigureAwait(false);
			}

			for (i = 0; i < missingKeys.Length; i++)
			{
				if (redisValues[i] == null)
				{
					continue;
				}

				TimeSpan? timeToLive = null;
				if (timesToLive != null)
				{
					if (!timesToLive[i].HasValue)
					{
						// The key was removed in the meantime
						values[missingCacheKeys[i].Key] = null;
						continue;
					}

					timeToLive = TimeSpan.FromMilliseconds((long) timesToLive[i]);
				}

				values[missingCacheKeys[i].Key] = AddAndGet(missingKeys[i], redisValues[i], version, timeToLive) ?? redisValues[i];
			}

			return values;
		}

		public Task<bool> RemoveAsync(string cacheKey, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			return RemoveLocalAsync(cacheKey, true, cancellationToken);
		}

		private Task<bool> RemoveLocalAsync(string cacheKey, bool publish, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<bool>(cancellationToken);
			}
			return ExecuteOperationAsync(cacheKey, null, publish, true, cancellationToken);
		}

		private Task PutLocalAsync(string cacheKey, object value, bool publish, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return ExecuteOperationAsync(cacheKey, value, publish, false, cancellationToken);
		}

		private async Task<bool> ExecuteOperationAsync(string cacheKey, object value, bool publish, bool remove, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var invalidationMessage = publish
				? _serializer.Serialize(new CacheKeyInvalidationMessage
				{
					ClientId = _clientId,
					Key = cacheKey
				})
				: null;

			var cacheValue = (CacheValue) _memoryCache.Get(cacheKey);
			cancellationToken.ThrowIfCancellationRequested();
			if (cacheValue == null && await (TryPutLocalAsync()).ConfigureAwait(false))
			{
				return !remove;
			}

			var lockValue = cacheValue.Lock;
			await (lockValue.WaitAsync(cancellationToken)).ConfigureAwait(false);
			try
			{
				cacheValue = (CacheValue) _memoryCache.Get(cacheKey);
				cancellationToken.ThrowIfCancellationRequested();

				// When a different thread calls TryPutLocal at the begining of the method
				// and gets the lock before the current thread we have to retry to put as the lock
				// value is not valid anymore
				if (cacheValue == null && await (TryPutLocalAsync()).ConfigureAwait(false)) // The key expired in the meantime
				{
					return !remove;
				}

				if (lockValue != cacheValue.Lock)
				{
					return false;
				}

				if (cacheValue is RemovedCacheValue)
				{
					if (remove)
					{
						cacheValue.Version = _version;
					}
					else
					{
						cacheValue = new CacheValue(value, _version, cacheValue.Lock);
					}
				}
				else
				{
					if (remove)
					{
						cacheValue = new RemovedCacheValue(_version, cacheValue.Lock);
					}
					else
					{
						cacheValue.Value = value;
						cacheValue.Version = _version;
					}
				}
				cancellationToken.ThrowIfCancellationRequested();

				return await (PutAndPublishAsync()).ConfigureAwait(false);
			}
			finally
			{
				lockValue.Release();
			}

			async Task<bool> TryPutLocalAsync()
			{
				await (_writeLock.WaitAsync(cancellationToken)).ConfigureAwait(false);
				try
				{
					cacheValue = (CacheValue) _memoryCache.Get(cacheKey);
					if (cacheValue != null)
					{
						return false;
					}

					return await (PutAndPublishAsync()).ConfigureAwait(false);
				}
				finally
				{
					_writeLock.Release();
				}
			}

			async Task<bool> PutAndPublishAsync()
			{
				if (remove)
				{
					RemoveLocal(cacheKey, new RemovedCacheValue(_version));
				}
				else
				{
					PutLocal(cacheKey, new CacheValue(value, _version));
				}

				if (publish)
				{
					await (ExecuteOperationAsync(cacheKey, value, invalidationMessage, remove)).ConfigureAwait(false);
				}

				return true;
			}
		}

		private Task ExecuteOperationAsync(string cacheKey, object value, byte[] message, bool remove)
		{
			try
			{
				if (remove)
				{
					return ExecuteRemoveAsync(cacheKey, message);
				}
				else
				{
					return ExecutePutAsync(cacheKey, value, message);
				}
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
