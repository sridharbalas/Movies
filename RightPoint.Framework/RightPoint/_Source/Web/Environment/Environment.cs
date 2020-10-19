using System;
using System.Collections.Generic;
using RightPoint.Config;
using System.IO;

namespace RightPoint.Web.Environment
{
    /// <summary>
    /// Summary description for Environment.
    /// </summary>
    public sealed class Environment : ConfigElementCollectionItem
    {
        private const string SELL_TICKETS_EMAIL_KEY = "sellTickets";

		public MachineType MachineType;

		public readonly string ContentServerName;

		public readonly string ContentRootPath;

		public readonly string SecureContentServerName;

		public readonly string SecureContentRootPath;

		public readonly string WebServerName;

		public readonly string WebRootPath;

		public readonly String SearchTicketsNowComDomainUrl;

		public readonly String CustomerServiceDomainUrl;

		public readonly String SmtpServer;

		public String WebServerRoot
		{
			get { return String.Format( "http://{0}{1}", WebServerName, WebRootPath ); }
		}

    }

	public sealed class EnvironmentCollection : ConfigElementCollection<Environment>
	{
		public Environment this[MachineType machineType]
		{
			get { return this[machineType.ToString()]; }
		}
	}
}