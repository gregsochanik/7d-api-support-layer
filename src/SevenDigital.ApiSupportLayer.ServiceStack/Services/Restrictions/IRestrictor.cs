using System.Collections.Generic;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Services.Restrictions
{
	public interface IRestrictor
	{
		void AssertRestriction(KeyValuePair<string,string> restriction);
	}
}