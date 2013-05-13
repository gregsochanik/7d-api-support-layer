using System.Collections.Generic;

namespace SevenDigital.ApiInt.ServiceStack.Services.Restrictions
{
	public interface IRestrictor
	{
		void AssertRestriction(KeyValuePair<string,string> restriction);
	}
}