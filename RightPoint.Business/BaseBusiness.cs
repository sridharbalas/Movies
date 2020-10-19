using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightPoint.Business
{
    public class BaseBusiness
    {
        const int PAGE_SIZE = 10;

        protected string OmdbApiUrl
        {
            get
            {
                return RightPoint.Business.MovieSettingsValues.OmdbApiUrl;
            }
        }

        protected string ApiKey
        {
            get
            {
                return RightPoint.Business.MovieSettingsValues.ApiKey;
            }
        }
        protected int GetDefaultPageSize()
        {

            int.TryParse(RightPoint.Business.MovieSettingsValues.DefaultPageSize, out int pageSize);
            pageSize = (pageSize == 0) ? PAGE_SIZE : pageSize;

            return pageSize;
        }

    }
}
