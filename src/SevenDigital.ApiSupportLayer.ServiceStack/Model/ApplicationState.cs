using System;
using System.Runtime.Serialization;

namespace SevenDigital.ApiSupportLayer.ServiceStack.Model
{
	[DataContract]
	public class ApplicationState
	{
		[DataMember]
		public DateTime ServerTime { get; set; }
	}
}