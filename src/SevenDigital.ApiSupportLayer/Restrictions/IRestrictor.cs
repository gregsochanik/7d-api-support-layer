using System.Collections.Generic;

namespace SevenDigital.ApiSupportLayer.Restrictions
{
	public interface IRestrictor
	{
		Restriction AssertRestriction(KeyValuePair<string, string> restriction);
	}
}