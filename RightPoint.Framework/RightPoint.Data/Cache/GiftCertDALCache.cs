using System;
using System.Web.Caching;

namespace EventInventory.Data.Cache
{
	public class GiftCertDALCache
	{
		private static Object _syncLock = new Object();

		public static GiftCertDAL.AllDiscountsRecordCollection GetAllDiscounts()
		{
			string cacheKey = CacheManager.GetCacheKey("GiftCertDALCache.GetAllDiscounts");
			GiftCertDAL.AllDiscountsRecordCollection returnValue;
			Boolean cacheObjectFound = CacheManager.TryGet<GiftCertDAL.AllDiscountsRecordCollection>(cacheKey, out returnValue);

			if (cacheObjectFound == false)
			{
				lock (_syncLock)
				{
					// Do it again to make sure no other thread set it.
					cacheObjectFound = CacheManager.TryGet<GiftCertDAL.AllDiscountsRecordCollection>(cacheKey, out returnValue);

					if (cacheObjectFound == false)
					{
						if (GiftCertDAL.TrySelectAllDiscounts(out returnValue) == false)
						{
							returnValue = new GiftCertDAL.AllDiscountsRecordCollection();
						}

						CacheManager.Add(cacheKey, returnValue, EventInventory.Caching.Configuration.Instance.CacheIntervalMedium, null, CacheItemPriority.Default, null, true);
					}
				}
			}

			return returnValue;
		}

		public static GiftCertDAL.AllActiveDiscountsRecordCollection GetAllActiveDiscounts(String type)
		{
			string cacheKey = CacheManager.GetCacheKey("GiftCertDALCache.GetAllActiveDiscounts");
			GiftCertDAL.AllActiveDiscountsRecordCollection allDiscounts = (GiftCertDAL.AllActiveDiscountsRecordCollection)CacheManager.Get(cacheKey);

			if (allDiscounts == null || allDiscounts.Count == 0)
			{
				lock (_syncLock)
				{
					if (allDiscounts == null || allDiscounts.Count == 0)
					{
						if (GiftCertDAL.TrySelectAllActiveDiscounts(out allDiscounts) == false)
						{
							allDiscounts = new GiftCertDAL.AllActiveDiscountsRecordCollection();
						}
					}
					CacheManager.Add(cacheKey, allDiscounts, EventInventory.Caching.Configuration.Instance.CacheIntervalMedium, null, CacheItemPriority.NotRemovable);
				}
			}
			GiftCertDAL.AllActiveDiscountsRecordCollection filteredDiscounts = new GiftCertDAL.AllActiveDiscountsRecordCollection();
			foreach (GiftCertDAL.AllActiveDiscountsRecord rec in allDiscounts)
			{
				if (rec.AttributeValue == type)
				{
					filteredDiscounts.Add(rec);
				}
			}
			return filteredDiscounts;
		}
	}
}
