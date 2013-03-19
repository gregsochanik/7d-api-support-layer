using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.CacheAccess;
using SevenDigital.Api.Schema.ReleaseEndpoint;
using SevenDigital.Api.Schema.TrackEndpoint;
using SevenDigital.Api.Wrapper;
using SevenDigital.ApiInt.Catalogue;

namespace SevenDigital.ApiInt.ServiceStack.Catalogue
{
	public interface IFluentApiTriggers
	{
		T SingleRequest<T>(IFluentApi<T> fluentApi);
		T MultipleRequestBasedOnCountryCodeList<T>(IFluentApi<T> fluentApi);
	}

	public class CachingCatalogue : ICatalogue
	{
		private readonly ICatalogApiFactory _factory;
		private readonly ICacheClient _cacheClient;
		private readonly IFluentApiTriggers _fluentApiTriggers;

		public CachingCatalogue(ICatalogApiFactory factory, ICacheClient cacheClient, IFluentApiTriggers fluentApiTriggers)
		{
			_factory = factory;
			_cacheClient = cacheClient;
			_fluentApiTriggers = fluentApiTriggers;
		}

		public Track GetATrack(string countryCode, int id)
		{
			var key = CacheKeys.Track(countryCode, id);
			var forTrackId = _factory.TrackApi().WithParameter("imagesize", "100").ForTrackId(id);
			return GetSet(key, () => _fluentApiTriggers.MultipleRequestBasedOnCountryCodeList(forTrackId));
		}

		public Track GetATrackWithPrice(string countryCode, int id)
		{
			var aTrack = GetATrack(countryCode, id);
			var aReleaseTracks = GetAReleaseTracks(countryCode, aTrack.Release.Id);
			return aReleaseTracks.FirstOrDefault(x => x.Id == id);
		}

		public Release GetARelease(string countryCode, int id)
		{
			var key = CacheKeys.Release(countryCode, id);
			var forReleaseId = _factory.ReleaseApi().WithParameter("imagesize", "100").ForReleaseId(id);
			return GetSet(key, () => _fluentApiTriggers.MultipleRequestBasedOnCountryCodeList(forReleaseId));
		}

		public List<Track> GetAReleaseTracks(string countryCode, int id)
		{
			var key = CacheKeys.ReleaseTracks(countryCode, id);
			var forReleaseId = _factory.ReleaseTracksApi().WithPageSize(100).WithParameter("imagesize", "100").ForReleaseId(id);
			return GetSet(key, () => _fluentApiTriggers.MultipleRequestBasedOnCountryCodeList(forReleaseId).Tracks);
		}

		private T GetSet<T>(string key, Func<T> retrieveEntity) where T : class
		{
			var cachedEntity = _cacheClient.Get<T>(key);
			if (cachedEntity == null)
			{
				cachedEntity = retrieveEntity();
				_cacheClient.Set(key, cachedEntity, TimeSpan.FromDays(1));
			}
			return cachedEntity;
		}
	}
}