#region License

//
//  CoreMemoryCache - A cache provider for NHibernate using Microsoft.Extensions.Caching.Memory.
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//

#endregion

using System;
using NHibernate.Cache;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using NHibernate.Util;

namespace NHibernate.Caches.CoreMemoryCache
{
	// 6.0 TODO: replace that class by its base
	/// <summary>
	/// Pluggable cache implementation using the Microsoft.Extensions.Caching.Memory classes.
	/// </summary>
	/// <remarks>
	/// Priority is not configurable because it is un-usable: the compaction on memory pressure feature has been
	/// removed from MemoryCache, only explicit compaction or size limit compaction may use priorities. But
	/// <see cref="CacheBase" /> API does not have a suitable method for triggering compaction, and size of each
	/// cached entry has to be user provided, which <see cref="CacheBase" /> API does not support.
	/// </remarks>
	public class CoreMemoryCache : CoreMemoryCacheBase,
#pragma warning disable 618
		ICache
#pragma warning restore 618
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CoreMemoryCache()
		{
		}

		/// <summary>
		/// Constructor with no properties.
		/// </summary>
		/// <param name="region">The region of the cache.</param>
		public CoreMemoryCache(string region)
			: base(region)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="region">The region of the cache.</param>
		/// <param name="properties">Cache configuration properties.</param>
		/// <remarks>
		/// There are three (3) configurable parameters:
		/// <ul>
		///		<li>expiration (or cache.default_expiration) = number of seconds to wait before expiring each item.</li>
		///		<li>cache.use_sliding_expiration = a boolean, true for resetting a cached item expiration each time it is accessed.</li>
		/// 	<li>regionPrefix = a string for prefixing the region name.</li>
		/// </ul>
		/// All parameters are optional. The defaults are an expiration of 300 seconds, no sliding expiration and no prefix.
		/// </remarks>
		/// <exception cref="ArgumentException">The "expiration" property could not be parsed.</exception>
		public CoreMemoryCache(string region, IDictionary<string, string> properties)
			: base(region, properties)
		{
		}

		/// <inheritdoc />
		public new Task<object> GetAsync(object key, CancellationToken cancellationToken)
			=> base.GetAsync(key, cancellationToken);

		/// <inheritdoc />
		public new Task PutAsync(object key, object value, CancellationToken cancellationToken)
			=> base.PutAsync(key, value, cancellationToken);

		/// <inheritdoc />
		public new Task RemoveAsync(object key, CancellationToken cancellationToken)
			=> base.RemoveAsync(key, cancellationToken);

		/// <inheritdoc />
		public new Task ClearAsync(CancellationToken cancellationToken)
			=> base.ClearAsync(cancellationToken);

		/// <inheritdoc />
		public new Task LockAsync(object key, CancellationToken cancellationToken)
			=> base.LockAsync(key, cancellationToken);

		/// <inheritdoc />
		public Task UnlockAsync(object key, CancellationToken cancellationToken)
			=> base.UnlockAsync(key, null, cancellationToken);

		/// <inheritdoc />
		public new string RegionName => base.RegionName;

		/// <inheritdoc />
		public new object Get(object key)
			=> base.Get(key);

		/// <inheritdoc />
		public new void Put(object key, object value)
			=> base.Put(key, value);

		/// <inheritdoc />
		public new void Remove(object key)
			=> base.Remove(key);

		/// <inheritdoc />
		public new void Clear()
			=> base.Clear();

		/// <inheritdoc />
		public new void Destroy()
			=> base.Destroy();

		/// <inheritdoc />
		public new void Lock(object key)
			=> base.Lock(key);

		/// <inheritdoc />
		public void Unlock(object key)
			=> base.Unlock(key, null);

		/// <inheritdoc />
		public new long NextTimestamp()
			=> base.NextTimestamp();

		/// <inheritdoc />
		public new int Timeout => base.Timeout;
	}

	/// <summary>
	/// Pluggable cache implementation using the Microsoft.Extensions.Caching.Memory classes.
	/// </summary>
	/// <remarks>
	/// Priority is not configurable because it is un-usable: the compaction on memory pressure feature has been
	/// removed from MemoryCache, only explicit compaction or size limit compaction may use priorities. But
	/// <see cref="CacheBase" /> API does not have a suitable method for triggering compaction, and size of each
	/// cached entry has to be user provided, which <see cref="CacheBase" /> API does not support.
	/// </remarks>
	public abstract class CoreMemoryCacheBase : CacheBase
	{
		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(CoreMemoryCache));

		// Using one single shared memory cache: it launches background task from time to time in order
		// to cleanup expired items. So we need to avoid running many memory cache instances, otherwise
		// they may launch many background tasks concurrently, which could be very detrimental for some
		// applications.
		private static readonly IMemoryCache Cache;

		private static readonly TimeSpan DefaultExpiration = TimeSpan.FromSeconds(300);
		private const bool _defaultUseSlidingExpiration = false;
		private static readonly string DefaultRegionPrefix = string.Empty;

		private string _fullRegion;

		private volatile CancellationTokenSource _clearToken = new CancellationTokenSource();
		private readonly ReaderWriterLockSlim _clearTokenLock = new ReaderWriterLockSlim();

		static CoreMemoryCacheBase()
		{
			var cacheOption = new MemoryCacheOptions();
			if (CoreMemoryCacheProvider.ExpirationScanFrequency.HasValue)
			{
				cacheOption.ExpirationScanFrequency = CoreMemoryCacheProvider.ExpirationScanFrequency.Value;
			}

			Cache = new MemoryCache(cacheOption);
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public CoreMemoryCacheBase()
			: this("nhibernate", null)
		{
		}

		/// <summary>
		/// Constructor with no properties.
		/// </summary>
		/// <param name="region">The region of the cache.</param>
		public CoreMemoryCacheBase(string region)
			: this(region, null)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="region">The region of the cache.</param>
		/// <param name="properties">Cache configuration properties.</param>
		/// <remarks>
		/// There are three (3) configurable parameters:
		/// <ul>
		///		<li>expiration (or cache.default_expiration) = number of seconds to wait before expiring each item.</li>
		///		<li>cache.use_sliding_expiration = a boolean, true for resetting a cached item expiration each time it is accessed.</li>
		/// 	<li>regionPrefix = a string for prefixing the region name.</li>
		/// </ul>
		/// All parameters are optional. The defaults are an expiration of 300 seconds, no sliding expiration and no prefix.
		/// </remarks>
		/// <exception cref="ArgumentException">The "expiration" property could not be parsed.</exception>
		public CoreMemoryCacheBase(string region, IDictionary<string, string> properties)
		{
			RegionName = region;
			Configure(properties);
		}

		/// <inheritdoc />
		public override string RegionName { get; }

		/// <summary>
		/// The expiration delay applied to cached items.
		/// </summary>
		public TimeSpan Expiration { get; private set; }

		/// <summary>
		/// Should the expiration delay be sliding?
		/// </summary>
		/// <value><see langword="true" /> for resetting a cached item expiration each time it is accessed.</value>
		public bool UseSlidingExpiration { get; private set; }

		private void Configure(IDictionary<string, string> props)
		{
			var regionPrefix = DefaultRegionPrefix;
			if (props == null)
			{
				Log.Warn("Configuring cache with default values");
				Expiration = DefaultExpiration;
				UseSlidingExpiration = _defaultUseSlidingExpiration;
			}
			else
			{
				Expiration = GetExpiration(props);
				UseSlidingExpiration = GetUseSlidingExpiration(props);
				regionPrefix = GetRegionPrefix(props);
			}

			_fullRegion = regionPrefix + RegionName;
		}

		private static string GetRegionPrefix(IDictionary<string, string> props)
		{
			if (props.TryGetValue("regionPrefix", out var result))
			{
				Log.Debug("new regionPrefix: {0}", result);
			}
			else
			{
				result = DefaultRegionPrefix;
				Log.Debug("no regionPrefix value given, using defaults");
			}

			return result;
		}

		private static TimeSpan GetExpiration(IDictionary<string, string> props)
		{
			var result = DefaultExpiration;
			if (!props.TryGetValue("expiration", out var expirationString))
			{
				props.TryGetValue(Cfg.Environment.CacheDefaultExpiration, out expirationString);
			}

			if (expirationString != null)
			{
				if (int.TryParse(expirationString, out var seconds))
				{
					result = TimeSpan.FromSeconds(seconds);
					Log.Debug("new expiration value: {0}", seconds);
				}
				else
				{
					Log.Error("error parsing expiration value '{0}'", expirationString);
					throw new ArgumentException($"could not parse expiration '{expirationString}' as a number of seconds");
				}
			}
			else
			{
				Log.Debug("no expiration value given, using defaults");
			}

			return result;
		}

		private static bool GetUseSlidingExpiration(IDictionary<string, string> props)
		{
			var sliding = PropertiesHelper.GetBoolean("cache.use_sliding_expiration", props, _defaultUseSlidingExpiration);
			Log.Debug("Use sliding expiration value: {0}", sliding);
			return sliding;
		}

		private object GetCacheKey(object key)
		{
			return new Tuple<string, object>(_fullRegion, key);
		}

		/// <inheritdoc />
		public override object Get(object key)
		{
			if (key == null)
			{
				return null;
			}

			var cacheKey = GetCacheKey(key);
			Log.Debug("Fetching object '{0}' from the cache.", cacheKey);

			return Cache.Get(cacheKey);
		}

		/// <inheritdoc />
		public override void Put(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key), "null key not allowed");
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value), "null value not allowed");
			}

			var cacheKey = GetCacheKey(key);
			var options = new MemoryCacheEntryOptions();
			if (UseSlidingExpiration)
				options.SlidingExpiration = Expiration;
			else
				options.AbsoluteExpirationRelativeToNow = Expiration;

			Log.Debug("putting item with key: {0}", cacheKey);
			_clearTokenLock.EnterReadLock();
			try
			{
				options.ExpirationTokens.Add(new CancellationChangeToken(_clearToken.Token));
				Cache.Set(cacheKey, value, options);
			}
			finally
			{
				_clearTokenLock.ExitReadLock();
			}
		}

		/// <inheritdoc />
		public override void Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			var cacheKey = GetCacheKey(key);
			Log.Debug("removing item with key: {0}", cacheKey);
			Cache.Remove(cacheKey);
		}

		/// <inheritdoc />
		public override void Clear()
		{
			_clearTokenLock.EnterWriteLock();
			try
			{
				_clearToken.Cancel();
				_clearToken.Dispose();
				_clearToken = new CancellationTokenSource();
			}
			finally
			{
				_clearTokenLock.ExitWriteLock();
			}
		}

		/// <inheritdoc />
		public override void Destroy()
		{
			Clear();
			_clearTokenLock.Dispose();
			_clearToken.Dispose();
		}

		/// <inheritdoc />
		public override object Lock(object key)
		{
			// Do nothing
			return null;
		}

		/// <inheritdoc />
		public override void Unlock(object key, object lockValue)
		{
			// Do nothing
		}

		/// <inheritdoc />
		public override long NextTimestamp()
		{
			return Timestamper.Next();
		}

		/// <inheritdoc />
		public override int Timeout => Timestamper.OneMs * 60000;
	}
}
