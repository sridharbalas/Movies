using System;

namespace RightPoint.Business
{

	public enum SettingID : Int32
	{
		OmdbApiUrl = 1,
		ApiKey = 2,
        DefaultPageSize = 3
    }

	public static class MovieSettingsValues
	{

		/// <summary>
		/// End point URL to Open Movie Database third party API service
		/// </summary>
		public static System.String OmdbApiUrl
		{
			get
			{
				return CachedData.MovieSettings.GetSettingValue(SettingID.OmdbApiUrl).Trim();
			}
		}

		/// <summary>
		/// apiKey to access Open Movie Database third party API service
		/// </summary>
		public static System.String ApiKey
		{
			get
			{
				return CachedData.MovieSettings.GetSettingValue(SettingID.ApiKey).Trim();
			}
		}

		public static System.String DefaultPageSize
		{
			get
			{
				return CachedData.MovieSettings.GetSettingValue(SettingID.DefaultPageSize).Trim();
			}
		}
	}
}