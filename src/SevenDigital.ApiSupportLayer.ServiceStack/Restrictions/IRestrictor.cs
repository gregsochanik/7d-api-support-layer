using System.Collections.Generic;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Restrictions
{
	public interface IRestrictor
	{
		void AssertRestriction(KeyValuePair<string,string> restriction);
	}
}