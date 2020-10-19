using System;
using System.Collections.Generic;
using System.Text;

namespace EventInventory.Data.Cache
{
	public class TNow3DALCache
	{
		private static Object _lock = new Object();

		private static Dictionary<Byte, AllProductionsByWidgetConsumerIDRecordCollectionCache> _allProductions =
			new Dictionary<Byte, AllProductionsByWidgetConsumerIDRecordCollectionCache>();

		private static Dictionary<Byte, AllEventsByWidgetConsumerIDRecordCollectionCache> _allEvents =
			new Dictionary<Byte, AllEventsByWidgetConsumerIDRecordCollectionCache>();

		private static Dictionary<Byte, AllVenuesByWidgetConsumerIDRecordCollectionCache> _allVenues =
			new Dictionary<Byte, AllVenuesByWidgetConsumerIDRecordCollectionCache>();

		private static Dictionary<Byte, AllCategoriesByWidgetConsumerIDRecordCollectionCache> _allCategories =
			new Dictionary<Byte, AllCategoriesByWidgetConsumerIDRecordCollectionCache>();

		public static TNow3DAL.AllProductionsByWidgetConsumerIDRecordCollection GetAllProductionsByWidgetConsumerID ( Byte widgetConsumerId )
		{
			AllProductionsByWidgetConsumerIDRecordCollectionCache data = new AllProductionsByWidgetConsumerIDRecordCollectionCache();
			if ( _allProductions.ContainsKey( widgetConsumerId ) )
			{
				data = _allProductions[widgetConsumerId];
			}

			if ( data.Data == null
				|| data.Data.Count == 0
				|| data.LastCached <= DateTime.Now.AddMinutes( -EventInventory.Data.Configuration_New.Instance.CacheDurationDefault ) )
			{
				TNow3DAL.AllProductionsByWidgetConsumerIDRecordCollection coll;
				if ( TNow3DAL.TryGetAllProductionsByWidgetConsumerID( (Byte)widgetConsumerId, out coll ) )
				{
					coll.Sort( "EventDate" );
					lock ( _lock )
					{
						data.Data = coll;
						data.LastCached = DateTime.Now;
						if ( _allProductions.ContainsKey( widgetConsumerId ) )
						{
							_allProductions[widgetConsumerId] = data;
						}
						else
						{
							_allProductions.Add( widgetConsumerId, data );
						}
					}
				}
			}
			return data.Data;
		}

		public static TNow3DAL.AllEventsByWidgetConsumerIDRecordCollection GetAllEventsByWidgetConsumerID ( Byte widgetConsumerId )
		{
			AllEventsByWidgetConsumerIDRecordCollectionCache data = new AllEventsByWidgetConsumerIDRecordCollectionCache();
			if ( _allEvents.ContainsKey( widgetConsumerId ) )
			{
				data = _allEvents[widgetConsumerId];
			}

			if ( data.Data == null
				|| data.Data.Count == 0
				|| data.LastCached <= DateTime.Now.AddMinutes( -EventInventory.Data.Configuration_New.Instance.CacheDurationDefault ) )
			{
				TNow3DAL.AllEventsByWidgetConsumerIDRecordCollection coll;
				if ( TNow3DAL.TryGetAllEventsByWidgetConsumerID( widgetConsumerId, out coll ) )
				{
					lock ( _lock )
					{
						data.Data = coll;
						data.LastCached = DateTime.Now;
						if ( _allEvents.ContainsKey( widgetConsumerId ) )
						{
							_allEvents[widgetConsumerId] = data;
						}
						else
						{
							_allEvents.Add( widgetConsumerId, data );
						}
					}
				}
			}
			return data.Data;
		}

		public static TNow3DAL.AllVenuesByWidgetConsumerIDRecordCollection GetAllVenuesByWidgetConsumerID ( Byte widgetConsumerId )
		{
			AllVenuesByWidgetConsumerIDRecordCollectionCache data = new AllVenuesByWidgetConsumerIDRecordCollectionCache();
			if ( _allVenues.ContainsKey( widgetConsumerId ) )
			{
				data = _allVenues[widgetConsumerId];
			}

			if ( data.Data == null
				|| data.Data.Count == 0
				|| data.LastCached <= DateTime.Now.AddMinutes( -EventInventory.Data.Configuration_New.Instance.CacheDurationDefault ) )
			{
				TNow3DAL.AllVenuesByWidgetConsumerIDRecordCollection coll;
				if ( TNow3DAL.TryGetAllVenuesByWidgetConsumerID( widgetConsumerId, out coll ) )
				{
					lock ( _lock )
					{
						data.Data = coll;
						data.LastCached = DateTime.Now;
						if ( _allVenues.ContainsKey( widgetConsumerId ) )
						{
							_allVenues[widgetConsumerId] = data;
						}
						else
						{
							_allVenues.Add( widgetConsumerId, data );
						}
					}
				}
			}
			return data.Data;
		}

		public static TNow3DAL.AllCategoriesByWidgetConsumerIDRecordCollection GetAllCategoriesByWidgetConsumerID ( Byte widgetConsumerId )
		{
			AllCategoriesByWidgetConsumerIDRecordCollectionCache data = new AllCategoriesByWidgetConsumerIDRecordCollectionCache();
			if ( _allCategories.ContainsKey( widgetConsumerId ) )
			{
				data = _allCategories[widgetConsumerId];
			}

			if ( data == null
				|| data.Data == null
				|| data.Data.Count == 0
				|| data.LastCached <= DateTime.Now.AddMinutes( -EventInventory.Data.Configuration_New.Instance.CacheDurationDefault ) )
			{
				TNow3DAL.AllCategoriesByWidgetConsumerIDRecordCollection coll;
				if ( TNow3DAL.TryGetAllCategoriesByWidgetConsumerID( widgetConsumerId, out coll ) && coll != null && coll.Count > 0 )
				{
					lock ( _lock )
					{
						data.Data = coll;
						data.LastCached = DateTime.Now;
						if ( _allCategories.ContainsKey( widgetConsumerId ) )
						{
							_allCategories[widgetConsumerId] = data;
						}
						else
						{
							_allCategories.Add( widgetConsumerId, data );
						}
					}
				}
			}
			return data.Data;
		}

		private class AllProductionsByWidgetConsumerIDRecordCollectionCache
		{
			public TNow3DAL.AllProductionsByWidgetConsumerIDRecordCollection Data;
			public DateTime LastCached;
		}

		private class AllEventsByWidgetConsumerIDRecordCollectionCache
		{
			public TNow3DAL.AllEventsByWidgetConsumerIDRecordCollection Data;
			public DateTime LastCached;
		}

		private class AllVenuesByWidgetConsumerIDRecordCollectionCache
		{
			public TNow3DAL.AllVenuesByWidgetConsumerIDRecordCollection Data;
			public DateTime LastCached;
		}

		private class AllCategoriesByWidgetConsumerIDRecordCollectionCache
		{
			public TNow3DAL.AllCategoriesByWidgetConsumerIDRecordCollection Data;
			public DateTime LastCached;
		}
	}
}
