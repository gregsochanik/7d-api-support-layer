using System.Collections.Generic;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using SevenDigital.Api.Schema.LockerEndpoint;
using SevenDigital.Api.Wrapper.Exceptions;
using SevenDigital.ApiInt.Locker;
using SevenDigital.ApiInt.Mapping;
using SevenDigital.ApiInt.Model;
using SevenDigital.ApiInt.ServiceStack.Model;

namespace SevenDigital.ApiInt.ServiceStack.Services
{
	public class LockerCheckService : Service
	{
		private readonly IPurchaseItemMapper _mapper;
		private readonly ILockerBrowser _lockerBrowser;

		public LockerCheckService(ILockerBrowser lockerBrowser, IPurchaseItemMapper mapper)
		{
			_mapper = mapper;
			_lockerBrowser = lockerBrowser;
		}

		public object Get(LockerCheckRequest lockerCheckRequest)
		{
			var accessToken = this.TryGetOAuthAccessToken();

			try
			{
				var response = _lockerBrowser.GetLockerItem(accessToken, lockerCheckRequest);

				if (response.LockerReleases.Count < 1)
					return null;

				return BuildLockerCheckResponse(lockerCheckRequest, response.LockerReleases);
			} 	
			catch(ApiException ex)
			{
				throw HttpError.NotFound(ex.Message);
			}
		}

		private LockerCheckResponse BuildLockerCheckResponse(LockerCheckRequest request, IEnumerable<LockerRelease> lockerReleases)
		{
			return new LockerCheckResponse
			{
				OriginalRequest = request,
				Item = _mapper.Map(request, lockerReleases)
			};
		}
	}
}