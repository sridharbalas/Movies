using System;
using System.Collections.Generic;
using RightPoint;
using RightPoint.Data;
using System.Web;
using System.Runtime;

namespace RightPoint.Business.CachedData
{

    public class MovieSettings
    {

        private static object _syncObject = new object();

        #region GetSettings
        public static MoviesDAL.MovieSettings_GetRecord GetSetting(SettingID SettingID)
        {

            Dictionary<int, MoviesDAL.MovieSettings_GetRecord> allSettings = MovieSettings.GetAllSettings();

            if (allSettings.TryGetValue((int)SettingID, out MoviesDAL.MovieSettings_GetRecord returnValue) == false)
            {
                lock (_syncObject)
                {
                    // Do it again to make sure another thread didn't load it.
                    if (allSettings.TryGetValue((int)SettingID, out returnValue) == false)
                    {
                        returnValue = null;
                    }
                }
            }

            return returnValue;

        }

        public static string GetSettingValue(SettingID SettingID)
        {
            string returnValue = string.Empty;
            try
            {
                MoviesDAL.MovieSettings_GetRecord setting = MovieSettings.GetSetting(SettingID);
                if (setting != null)
                    returnValue = setting.Value;
            }
            catch (Exception ex)
            {
				// Logging.Logger.LogError ( ex );
				throw ex;
            }
            return returnValue;
        }
        #endregion //GetSettings

        #region GetAllSettings

        public static Dictionary<int, MoviesDAL.MovieSettings_GetRecord> GetAllSettings()
        {

            // Dictionary<int, MoviesDAL.MovieSettings_GetRecord> returnValue = (Dictionary<int, MoviesDAL.MovieSettings_GetRecord>)CacheManager.Get(cacheKey);
            Dictionary<int, MoviesDAL.MovieSettings_GetRecord> returnValue = null;

            //if (returnValue == null)
            //{
                lock (_syncObject)
                {
                    // do it again to ensure another thread didn't load it.
                    //returnValue = (Dictionary<int, MoviesDAL.MovieSettings_GetRecord>)CacheManager.Get(cacheKey);

                    if (returnValue == null)
                    {
                        returnValue = new Dictionary<int, MoviesDAL.MovieSettings_GetRecord>();

                    if (MoviesDAL.TryMovieSettings_Get(out MoviesDAL.MovieSettings_GetRecordCollection allSettings) == true)
                    {
                        foreach (MoviesDAL.MovieSettings_GetRecord setting in allSettings)
                        {
                            if (returnValue.ContainsKey(setting.ID.GetValueOrDefault(0)) == false)
                                returnValue.Add(setting.ID.GetValueOrDefault(0), setting);
                        }
                    }

                    // CacheManager.Add(cacheKey, returnValue, RightPoint.Configuration.DefaultCacheExpiration, null, Caching.CacheItemPriority.NotRemovable);
                }
                }
            //}

            return returnValue;

        }

        #endregion //GetAllSettings

    }
}
