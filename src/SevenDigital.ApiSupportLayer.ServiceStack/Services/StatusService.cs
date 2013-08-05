using System;
using ServiceStack.ServiceInterface;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
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