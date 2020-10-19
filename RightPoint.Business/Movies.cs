using Newtonsoft608.Json;
using RightPoint.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightPoint.Business
{
    public class Movies : BaseBusiness, IMovies
    {
        public int PageCount { get; private set; }

        public Models.Movies GetMovies(string searchTitle, string searchType, int pageNumber)
        {

            string omdbApiUrl = base.OmdbApiUrl;
            string apiKey = base.ApiKey;

            if (string.IsNullOrEmpty(omdbApiUrl) || (string.IsNullOrEmpty(apiKey)))
            {
                throw new ApplicationException("Configure Open Movie Database (OMDB) end point and api key to access movies list. They cannot be empty");
            }

            Models.Movies moviesModel = null;
            if (!string.IsNullOrEmpty(searchTitle))
            {
                string endPointURL = $"{omdbApiUrl}?apiKey={apiKey}&s={searchTitle}&type={searchType}&page={pageNumber}";
                RestClient restClient = new RestClient(endPointURL, RestClient.httpVerb.GET);
                string response = restClient.Get();
                moviesModel = JsonConvert.DeserializeObject<Models.Movies>(response);
                bool responseStatus = (moviesModel.Response == null) ? false : Convert.ToBoolean(moviesModel.Response);

                if (responseStatus && moviesModel != null && moviesModel.Search != null)
                {
                    int totalResults = Convert.ToInt32(moviesModel.totalResults);
                    this.PageCount = (int)Math.Ceiling((double)totalResults / (double)base.GetDefaultPageSize());

                }

            }

            return moviesModel;

        }

    }
}
