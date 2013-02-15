using System;
using System.Runtime.Serialization;

namespace SevenDigital.ApiInt.ServiceStack.Model
{
	[DataContract]
	public class ApplicationState
	{
		[DataMember]
		public DateTime ServerTime { get; set; }
	}
}