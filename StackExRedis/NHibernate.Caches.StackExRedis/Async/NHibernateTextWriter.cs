﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.IO;
using System.Text;

namespace NHibernate.Caches.StackExRedis
{
	using System.Threading.Tasks;
	internal partial class NHibernateTextWriter : TextWriter
	{

		public override Task WriteAsync(string value)
		{
			try
			{
				Write(value);
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task WriteLineAsync(string value)
		{
			try
			{
				WriteLine(value);
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public override Task WriteLineAsync()
		{
			try
			{
				WriteLine();
				return Task.CompletedTask;
			}
			catch (System.Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}
	}
}
