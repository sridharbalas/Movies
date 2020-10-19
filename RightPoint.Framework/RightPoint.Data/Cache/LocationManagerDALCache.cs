using System;
using System.Collections.Generic;
using System.Text;

namespace EventInventory.Data.Cache
{
	public sealed class LocationManagerDALCache
	{
		public static LocationManagerDAL.AllCountriesRecordCollection GetAllCountries ()
		{
			String cacheKey = CacheManager.GetCacheKey( "GetAllCountries" );
			LocationManagerDAL.AllCountriesRecordCollection records =
				(LocationManagerDAL.AllCountriesRecordCollection)CacheManager.Get( cacheKey );
			if ( records == null || records.Count == 0 )
			{
				LocationManagerDAL.TrySelectAllCountries( out records );
				if ( records != null && records.Count > 0 )
				{
					CacheManager.Add( cacheKey, records, EventInventory.Configuration.DefaultCacheExpiration );
				}
			}
			return records;
		}
	}
}
