using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace RightPoint.Web.JSON
{
	public class JsonResponse
	{
		public JsonResponse()
		{

		}

		public JsonResponse(string error, int id, object result) : this()
		{
			this.error = error;
			this.id = id;
			this.result = result;
		}

		

		public string error;
		public int id;
		public object result;
	}
}
