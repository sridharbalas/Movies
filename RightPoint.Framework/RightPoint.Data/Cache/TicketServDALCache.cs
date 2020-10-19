using System;

namespace EventInventory.Data.Cache
{
	public sealed class TicketServDALCache
	{
		public static TicketServDAL.AllMarkupRuleTypesRecordCollection GetAllMarkupRuleTypes()
		{
			String cacheKey = CacheManager.GetCacheKey("TicketServDALCache.GetAllMarkupRuleTypes");
			TicketServDAL.AllMarkupRuleTypesRecordCollection returnValue = (TicketServDAL.AllMarkupRuleTypesRecordCollection) CacheManager.Get(cacheKey);

			if (returnValue == null)
			{
				TicketServDAL.TryGetAllMarkupRuleTypes(out returnValue);
				if (returnValue != null)
				{
					CacheManager.Add(cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration);
				}
			}

			return returnValue;
		}


		public static Int32? GetClientMarkupAssignment ( Int32 clientID )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetClientMarkupAssignment", clientID );
			Int32? markupID = (Int32?) CacheManager.Get( cacheKey );

			if ( markupID == null )
			{
				TicketServDAL.TryGetClientMarkupAssignment( clientID, out markupID );
				if ( markupID != null )
				{
					CacheManager.Add( cacheKey, markupID, EventInventory.Configuration.DefaultCacheExpiration );
				}
			}

			return markupID;
		}

		public static TicketServDAL.MarkupRulesRecordCollection GetMarkupRules ( Int32? markupID, Boolean? showOnlyActive )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetMarkupRules", markupID, showOnlyActive );
			TicketServDAL.MarkupRulesRecordCollection returnValue =
				(TicketServDAL.MarkupRulesRecordCollection) CacheManager.Get( cacheKey );

			if ( returnValue == null )
			{
				TicketServDAL.TryGetMarkupRules( markupID, showOnlyActive, out returnValue );
				if ( returnValue != null )
					CacheManager.Add( cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return returnValue;
		}

		public static TicketServDAL.MarkupRecord GetMarkup ( Int32? markupID )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetMarkup", markupID );
			TicketServDAL.MarkupRecord returnValue =
				(TicketServDAL.MarkupRecord) CacheManager.Get( cacheKey );

			if ( returnValue == null )
			{
				TicketServDAL.TryGetMarkup( markupID, out returnValue );
				if ( returnValue != null )
					CacheManager.Add( cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return returnValue;
		}

		public static TicketServDAL.DefaultMarkupRuleRecord GetDefaultMarkupRule ( Int32? markupID, Boolean isInternal )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetDefaultMarkupRule", markupID, isInternal );
			TicketServDAL.DefaultMarkupRuleRecord returnValue =
				(TicketServDAL.DefaultMarkupRuleRecord) CacheManager.Get( cacheKey );

			if ( returnValue == null )
			{
				TicketServDAL.TryGetDefaultMarkupRule( markupID, isInternal, out returnValue );
				if ( returnValue != null )
					CacheManager.Add( cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return returnValue;
		}

		public static TicketServDAL.ClientMarkupRoundingRulesRecordCollection GetClientMarkupRoundingRules ( Int32? clientID )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetClientMarkupRoundingRules", clientID );
			TicketServDAL.ClientMarkupRoundingRulesRecordCollection returnValue =
				(TicketServDAL.ClientMarkupRoundingRulesRecordCollection) CacheManager.Get( cacheKey );

			if ( returnValue == null )
			{
				TicketServDAL.TryGetClientMarkupRoundingRules( clientID, out returnValue );
				if ( returnValue != null )
					CacheManager.Add( cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return returnValue;
		}

		public static TicketServDAL.MarkupDiscountRulesRecordCollection GetMarkupDiscountRules ( Int32? markupID )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetMarkupDiscountRules", markupID );
			TicketServDAL.MarkupDiscountRulesRecordCollection returnValue =
				(TicketServDAL.MarkupDiscountRulesRecordCollection) CacheManager.Get( cacheKey );

			if ( returnValue == null )
			{
				TicketServDAL.TryGetMarkupDiscountRules( markupID, out returnValue );
				if ( returnValue != null )
					CacheManager.Add( cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return returnValue;
		}

		public static TicketServDAL.MarkupSeasonRulesRecordCollection GetMarkupSeasonRules ( Int32? markupID )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetMarkupSeasonRules", markupID );
			TicketServDAL.MarkupSeasonRulesRecordCollection returnValue =
				(TicketServDAL.MarkupSeasonRulesRecordCollection) CacheManager.Get( cacheKey );

			if ( returnValue == null )
			{
				TicketServDAL.TryGetMarkupSeasonRules( markupID, out returnValue );
				if ( returnValue != null )
					CacheManager.Add( cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return returnValue;
		}

		public static TicketServDAL.BrokerDiscountRulesByClientIDRecordCollection GetBrokerDiscountRulesByClientID ( Int32? clientID )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetBrokerDiscountRulesByClientID", clientID );
			TicketServDAL.BrokerDiscountRulesByClientIDRecordCollection returnValue =
				(TicketServDAL.BrokerDiscountRulesByClientIDRecordCollection) CacheManager.Get( cacheKey );

			if ( returnValue == null )
			{
				TicketServDAL.TryGetBrokerDiscountRulesByClientID( clientID, out returnValue );
				if ( returnValue != null )
					CacheManager.Add( cacheKey, returnValue, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return returnValue;
		}

		public static Boolean? GetUseTierCommissionByMarkupID ( Int32? markupID )
		{
			String cacheKey = CacheManager.GetCacheKey( "GetUseTierCommissionByMarkupID", markupID );
			Boolean? useTierCommissionByMarkupID = (Boolean) CacheManager.Get( cacheKey );

			if ( useTierCommissionByMarkupID == null )
			{
				TicketServDAL.TryGetUseTierCommissionByMarkupID( markupID, out useTierCommissionByMarkupID );
				if ( useTierCommissionByMarkupID != null )
					CacheManager.Add( cacheKey, useTierCommissionByMarkupID, EventInventory.Configuration.DefaultCacheExpiration );
			}

			return useTierCommissionByMarkupID;
		}
	}
}