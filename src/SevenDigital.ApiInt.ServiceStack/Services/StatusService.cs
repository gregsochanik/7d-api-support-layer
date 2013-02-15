using System;
using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	[DefaultView("ServiceStatus")]
	public class StatusService : Service
	{
		public object Get(ApplicationState request)
		{
			request.ServerTime = DateTime.Now;
			return request;
		}
	}
}