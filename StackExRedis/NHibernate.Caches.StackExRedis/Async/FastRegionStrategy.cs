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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace NHibernate.Caches.StackExRedis
{
	using System.Threading;
	public partial class FastRegionStrategy : AbstractRegionStrategy
	{

		/// <inheritdoc />
		public override Task ClearAsync(CancellationToken cancellationToken)
		{
			throw new NotSupportedException(
				$"{nameof(ClearAsync)} operation is not supported, if it cannot be avoided use {nameof(DefaultRegionStrategy)}.");
		}
	}
}