using System;
using System.Collections.Generic;
using System.Text;

namespace RightPoint.Business.Models
{

    public class Movies
    {
        public Search[] Search { get; set; }
        public string totalResults { get; set; }
        public string Response { get; set; }

        public string Error { get; set; }

    }

}
