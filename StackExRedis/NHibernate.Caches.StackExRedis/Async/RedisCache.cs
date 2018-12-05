﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Cache;

namespace NHibernate.Caches.StackExRedis
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class RedisCache : CacheBase
	{

		/// <inheritdoc />
		public override Task<object> GetAsync(object key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return RegionStrategy.GetAsync(key, cancellationToken);
		}

		/// <inheritdoc />
		public override Task<object[]> GetManyAsync(object[] keys, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object[]>(cancellationToken);
			}
			return RegionStrategy.GetManyAsync(keys, cancellationToken);
		}

		/// <inheritdoc />
		public override Task PutAsync(object key, object value, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return RegionStrategy.PutAsync(key, value, cancellationToken);
		}

		/// <inheritdoc />
		public override Task PutManyAsync(object[] keys, object[] values, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return RegionStrategy.PutManyAsync(keys, values, cancellationToken);
		}

		/// <inheritdoc />
		public override Task RemoveAsync(object key, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return RegionStrategy.RemoveAsync(key, cancellationToken);
		}

		/// <inheritdoc />
		public override Task ClearAsync(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return RegionStrategy.ClearAsync(cancellationToken);
		}

		/// <inheritdoc />
		public override async Task<object> LockAsync(object key, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await (RegionStrategy.LockAsync(key, cancellationToken)).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task<object> LockManyAsync(object[] keys, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await (RegionStrategy.LockManyAsync(keys, cancellationToken)).ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override Task UnlockAsync(object key, object lockValue, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return RegionStrategy.UnlockAsync(key, (string)lockValue, cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		/// <inheritdoc />
		public override Task UnlockManyAsync(object[] keys, object lockValue, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return RegionStrategy.UnlockManyAsync(keys, (string) lockValue, cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
