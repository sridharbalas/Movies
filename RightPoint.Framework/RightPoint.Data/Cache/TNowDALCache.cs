using System;
using System.Web.Caching;

namespace EventInventory.Data.Cache
{
	public class TNowDALCache
	{
		private static Object _syncLock = new Object();

        public static TNowDAL.AllBrokersRecordCollection GetAllBrokers()
		{
            string cacheKey = CacheManager.GetCacheKey("TNowDALCache.GetAllBrokers");
            TNowDAL.AllBrokersRecordCollection returnValue = (TNowDAL.AllBrokersRecordCollection)CacheManager.Get(cacheKey);

            if (returnValue == null)
			{
                lock (_syncLock)
				{
					// Do it again to make sure no other thread set it.
                    returnValue = (TNowDAL.AllBrokersRecordCollection)CacheManager.Get(cacheKey);

                    if (returnValue == null)
					{
                        if (TNowDAL.TrySelectAllBrokers(out returnValue) == false)
						{
							returnValue = new TNowDAL.AllBrokersRecordCollection();
						}

                        CacheManager.Add(cacheKey, returnValue, EventInventory.Configuration.DefaultOneHourCacheExpiration, null, CacheItemPriority.NotRemovable);
					}
				}
			}

			return returnValue;
		}

        public static TNowDAL.AllSegmentSpecsRecordCollection GetAllSegmentSpecs()
		{
            String cacheKey = CacheManager.GetCacheKey("TNowDALCache.GetAllSegmentSpecs");
            TNowDAL.AllSegmentSpecsRecordCollection returnValue = (TNowDAL.AllSegmentSpecsRecordCollection)CacheManager.Get(cacheKey);

            if (returnValue == null)
			{
                lock (_syncLock)
				{
					// Do it again to make sure no other thread set it.
                    returnValue = (TNowDAL.AllSegmentSpecsRecordCollection)CacheManager.Get(cacheKey);

                    if (returnValue == null)
					{
                        if (TNowDAL.TryGetAllSegmentSpecs(out returnValue) == false)
						{
							returnValue = new TNowDAL.AllSegmentSpecsRecordCollection();
						}

                        CacheManager.Add(cacheKey, returnValue, EventInventory.Configuration.DefaultOneHourCacheExpiration, null, CacheItemPriority.NotRemovable);
					}
				}
			}

			return returnValue;
		}

		public static TNowDAL.AllProductionDetailRecordCollection SelectAllProductionDetail ()
		{
			String cacheKey = CacheManager.GetCacheKey( "TNowDALCache.SelectAllProductionDetail" );
			TNowDAL.AllProductionDetailRecordCollection collection = (TNowDAL.AllProductionDetailRecordCollection)CacheManager.Get( cacheKey );
			if ( collection == null )
			{
				lock ( _syncLock )
				{
					collection = (TNowDAL.AllProductionDetailRecordCollection)CacheManager.Get( cacheKey );
					if ( collection == null )
					{
						if ( TNowDAL.TrySelectAllProductionDetail( out collection ) )
						{
							CacheManager.Add( cacheKey, collection, EventInventory.Configuration.DefaultOneHourCacheExpiration, null, CacheItemPriority.NotRemovable );
						}
					}
				}
			}
			return collection;
		}

		public static TNowDAL.AllEventsRecordCollection SelectAllEvents ()
		{
			String cacheKey = CacheManager.GetCacheKey( "TNowDALCache.SelectAllEvents" );
			TNowDAL.AllEventsRecordCollection collection = (TNowDAL.AllEventsRecordCollection)CacheManager.Get( cacheKey );
			if ( collection == null )
			{
				lock ( _syncLock )
				{
					collection = (TNowDAL.AllEventsRecordCollection)CacheManager.Get( cacheKey );
					if ( collection == null )
					{
						if ( TNowDAL.TrySelectAllEvents( out collection ) )
						{
							CacheManager.Add( cacheKey, collection, EventInventory.Configuration.DefaultOneHourCacheExpiration, null, CacheItemPriority.NotRemovable );
						}
					}
				}
			}
			return collection;
		}
	}
}