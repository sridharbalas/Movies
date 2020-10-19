using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;
//using Enyim.Caching;
using System.IO;

namespace RightPoint
{
	/// <summary>
	/// Summary description for CacheManager.
	/// </summary>
	public sealed class CacheManager
	{
		// TODO: Move to config file.
		//private const int MaximumUncachableKeyQueueLength = 10000;
		//private const int NumberOfSecondsBeforeUncacheableKeyQueueCleanup = 300;

		private static Dictionary<string, int> _loggableItems = new Dictionary<string, int>();
		private static DateTime _lastLogCommit = DateTime.Now;

		private static Cache _cache = CreateRuntimeCache();
		private static bool _isCacheAvailable = _cache != null;
		//private static Enyim.Caching.MemcachedClient _memcachedClient = GetMemcachedClient();

		/// <summary>
		/// Any keys that are placed in here are not queable in a distributed cache.
		/// </summary>
		//private static Queue<string> _uncachableKeys = new Queue<string>();
		//private static DateTime _lastCleanupCheck = DateTime.Now;

		public static Int32 Count
		{
			get { return _cache.Count; }
		}

		public static List<String> Keys
		{
			get
			{
				List<String> keys = new List<string>();
				IDictionaryEnumerator cacheEnum = Cache.GetEnumerator();
				while ( cacheEnum.MoveNext() )
				{
					keys.Add( ((DictionaryEntry)cacheEnum.Current).Key.ToString() );
				}
				return keys;
			}
		}

		private CacheManager()
		{
			
		}

		/// <summary>
		/// Gets the cache.
		/// </summary>
		/// <value>The cache.</value>
		private static Cache Cache
		{
			get { return ( _cache ); }
		}

		/// <summary>
		/// Creates the runtime cache.
		/// </summary>
		/// <returns></returns>
		private static Cache CreateRuntimeCache()
		{
			_loggableItems = new Dictionary<string, int>();
			_lastLogCommit = DateTime.Now;
			//_uncachableKeys = new Queue<string>();
			//_lastCleanupCheck = DateTime.Now;

			Cache cache;

			if ( HttpContext.Current == null )
			{
				HttpRuntime httpRuntime = new HttpRuntime();
			}

			cache = HttpRuntime.Cache;

			// When ever the website is restarted, we want to remove all the data from the distributed caches 
			// to prevent object incompatibility.
			//if (Caching.Configuration.UseDistributedCaching == true)
			//{
			//    AddOperationToLog("FlushAll", "None");
			//    new MemcachedClient().FlushAll();
			//}

			return ( cache );
		}

		//private static MemcachedClient GetMemcachedClient()
		//{
		//    if (Caching.Configuration.UseDistributedCaching == true)
		//        return new MemcachedClient();
		//    else
		//        return null;
		//}
		
		#region Constructor Overloads

		//public Cache() : base() {} 
		/*
		public Cache(int initialSize) : base(initialSize) {}

		public Cache(bool caseInsensitive) : base(caseInsensitive) {}

		public Cache(int initialSize, bool caseInsensitive) : base(initialSize, caseInsensitive) {}
		*/

		#endregion

		#region Add Method Overloads

		public static void Add ( string key, object value, int secondsBeforeExpiration, CacheDependency dependencies,
								CacheItemPriority cachePriority, CacheItemRemovedCallback cacheItemRemovedCallback, Boolean cacheEmptyObject )
		{
			if ( cacheEmptyObject )
			{
				value = new EmptyCacheObject( value );
			}
			//bool distributedCacheSucceeded = false;


			//if (Caching.Configuration.UseDistributedCaching == true && DoesKeyExistInUncacheableKeyQueueList(key) == false)
			//{
			//    distributedCacheSucceeded = _memcachedClient.Store(Enyim.Caching.Memcached.StoreMode.Set, key, value, DateTime.Now.AddSeconds(secondsBeforeExpiration));
			//}
            
            //if (distributedCacheSucceeded == false && _isCacheAvailable == true)
            if (_isCacheAvailable == true && value != null)
            {
                Cache.Insert(key, value, dependencies, DateTime.Now.AddSeconds(secondsBeforeExpiration),
                Cache.NoSlidingExpiration, cachePriority, cacheItemRemovedCallback);

                //if (Caching.Configuration.UseDistributedCaching == true)
                //    AddKeyToUncacheableKeysQueue(key);
            }
		}

		public static void Add(string key, object value, int secondsBeforeExpiration, CacheDependency dependencies,
								CacheItemPriority cachePriority, CacheItemRemovedCallback cacheItemRemovedCallback)
		{
			Add( key, value, secondsBeforeExpiration, dependencies, cachePriority, cacheItemRemovedCallback, false );
		}

		public static void Add( string key, object value, int secondsBeforeExpiration, CacheDependency dependencies,
								CacheItemPriority cachePriority )
		{
			Add(key, value, secondsBeforeExpiration, dependencies, cachePriority, null);
		}

		public static void Add( string key, object value, int secondsBeforeExpiration, CacheDependency dependencies )
		{
			Add( key, value, secondsBeforeExpiration, dependencies, CacheItemPriority.Default );
		}

		public static void Add( string key, object value, int secondsBeforeExpiration )
		{
			Add( key, value, secondsBeforeExpiration, null );
		}

		#endregion

		/// <summary>
		/// Gets the cache key.
		/// </summary>
		/// <param name="methodIdentifier">The method identifier.</param>
		/// <param name="methodParameters">The method parameters.</param>
		/// <returns></returns>
		public static string GetCacheKey( string methodIdentifier, params object[] methodParameters )
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( methodIdentifier + "_" );
			foreach ( object obj in methodParameters )
			{
				if ( obj != null )
				{
					sb.Append( obj.ToString() );
				}

				sb.Append( "_" );
			}
			return sb.ToString().TrimEnd( '_' );
		}

		public static bool Contains(string key)
		{
			return Get(key) != null;
		}

		/// <summary>
		/// Gets the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		public static object Get( string key )
		{
			object cachedObject = null;

			if (_isCacheAvailable == true
				//&& (
				//        Caching.Configuration.UseDistributedCaching == false
				//        || (Caching.Configuration.UseDistributedCaching == true && DoesKeyExistInUncacheableKeyQueueList(key) == true)
				//        || (Caching.Configuration.UseInProcessCacheWithDistributedCaching == true)
				//    )
				//&& DoesKeyExistInUncacheableKeyQueueList(key) == true
				)
			{
				cachedObject = Cache[key];
			}

			//if (cachedObject == null && Caching.Configuration.UseDistributedCaching == true && DoesKeyExistInUncacheableKeyQueueList(key) == false)
			//{
			//    cachedObject = _memcachedClient.Get(key);

			//    // Place the object into the local memory cache for retrieval for subsequent opeations. 
			//    // This will help relieve pressure from the serialization/deserialization from the distributed cache.
			//    if(cachedObject != null && Caching.Configuration.UseInProcessCacheWithDistributedCaching == true)
			//        Cache.Add(key, cachedObject, null, DateTime.Now.AddSeconds(Configuration.DefaultCacheExpiration), Cache.NoSlidingExpiration, CacheItemPriority.Low, null);
			//}

			return cachedObject;
		}

		[Obsolete("Using generics here created a performance issue for the pricing engine. Please revert back to object casting.")]
		public static T Get<T> ( String key )
		{
			return (T)Get( key );
		}

		/// <summary>
		/// Gets an object from cache, returns true if the object was wrapped in an EmptyCacheObject
		/// </summary>
		/// <typeparam name="T">Type of cached object</typeparam>
		/// <param name="key">The cache key of the object</param>
		/// <param name="cacheObject">The cached object</param>
		/// <returns>True if the object was wrapped in an EmptyCacheObject, else false</returns>
		public static Boolean TryGet<T> ( String key, out T cacheObject )
		{
			Object obj = Get( key );
			if ( obj is EmptyCacheObject )
			{
				cacheObject = (T)((EmptyCacheObject)obj).EmptyObject;
				return true;
			}

			cacheObject = (T)obj;
			return cacheObject != null;
		}

		public delegate object CachedOperation();

		/// <summary>
		/// Returns the cached result of the specified operation.
		/// The result will be cached for secondsBeforeExpiration using the specified key,
		/// and concurrent execution of the operation is prevented by locking on syncObject.
		/// </summary>
		public static object GetCachedResult(CacheManager.CachedOperation operation, string key, int secondsBeforeExpiration, object syncObject)
		{
			object returnValue = CacheManager.Get(key);
			if (returnValue == null)
			{
				lock (syncObject)
				{
					// Check the cache again to ensure that another thread
					// didn't load the desired value while this thread was waiting.
					returnValue = CacheManager.Get(key);
					if (returnValue == null)
					{
						returnValue = operation();
						CacheManager.Add(key, returnValue, secondsBeforeExpiration);
					}
				}
			}
			return returnValue;
		}

		/// <summary>
		/// Clears all cache.
		/// </summary>
		public static void ClearAllCache()
		{
			//if (Caching.Configuration.UseDistributedCaching == true)
			//{
			//    AddOperationToLog("ClearAll", "None");
			//    new MemcachedClient().FlushAll();
			//}

			if ( _isCacheAvailable )
			{
				IDictionaryEnumerator cacheEnum = Cache.GetEnumerator();
				while ( cacheEnum.MoveNext() )
				{
					string cacheKey = ( (DictionaryEntry) cacheEnum.Current ).Key.ToString();
					Cache.Remove( cacheKey );
				}
			}
		}

		/// <summary>
		/// Clears all cache.
		/// </summary>
		public static Boolean ClearCache(string cacheKey)
		{
			//if (Caching.Configuration.UseDistributedCaching == true)
			//{
			//    AddOperationToLog("Clear", cacheKey);
			//    _memcachedClient.Remove(cacheKey);
			//}

			Boolean returnValue = false;
			
			if (_isCacheAvailable)
			{
				if ( Get(cacheKey) != null )
				{
					object removedItem = Cache.Remove(cacheKey);
					if (removedItem != null)
						returnValue = true;
				}
			}
			return returnValue;
			
		}

		//private static bool DoesKeyExistInUncacheableKeyQueueList(string key)
		//{
		//    return _uncachableKeys.Contains(key);
		//}

		//private static void AddKeyToUncacheableKeysQueue(string key)
		//{
		//    _uncachableKeys.Enqueue(key);

		//    if (_lastCleanupCheck < DateTime.Now.AddSeconds(-1 * NumberOfSecondsBeforeUncacheableKeyQueueCleanup))
		//    {
		//        while (_uncachableKeys.Count > MaximumUncachableKeyQueueLength)
		//            _uncachableKeys.Dequeue();
		//    }

		//    AddOperationToLog("NonCacheable", key);
		//}
		
	}

	internal class EmptyCacheObject
	{
		private Object _emptyObject;

		public Object EmptyObject
		{
			get { return _emptyObject; }
			set { _emptyObject = value; }
		}

		public EmptyCacheObject ( Object emptyObject )
		{
			EmptyObject = emptyObject;
		}
	}
}