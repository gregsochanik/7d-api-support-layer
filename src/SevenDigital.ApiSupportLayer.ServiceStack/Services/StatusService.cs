using System;
using ServiceStack.ServiceInterface;
using SevenDigital.ApiSupportLayer.ServiceStack.Model;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services
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