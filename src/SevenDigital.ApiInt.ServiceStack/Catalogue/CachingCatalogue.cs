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
	public class CachingCatalogue : ICatalogue
	{
		private readonly ICatalogApiFactory _factory;
		private readonly ICacheClient _cacheClient;

		public CachingCatalogue(ICatalogApiFactory factory, ICacheClient cacheClient)
		{
			_factory = factory;
			_cacheClient = cacheClient;
		}

		public Track GetATrack(string countryCode, int id)
		{
			var key = CacheKeys.Track(countryCode, id);
			return GetSet(key, () => _factory.TrackApi().WithParameter("country", countryCode).WithParameter("imagesize", "100").ForTrackId(id).Please());
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
			return GetSet(key, () => _factory.ReleaseApi().WithParameter("country", countryCode).WithParameter("imagesize", "100").ForReleaseId(id).Please());
		}

		public List<Track> GetAReleaseTracks(string countryCode, int id)
		{
			var key = CacheKeys.ReleaseTracks(countryCode, id);
			return GetSet(key, () => _factory.ReleaseTracksApi().WithPageSize(100).WithParameter("country", countryCode).WithParameter("imagesize", "100").ForReleaseId(id).Please().Tracks);
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