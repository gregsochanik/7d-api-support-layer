using System;
using ServiceStack.ServiceInterface;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Status
{
	public class StatusService : Service
	{
		public object Get(ApplicationState request)
		{
			request.ServerTime = DateTime.Now;
			return request;
		}
	}
}