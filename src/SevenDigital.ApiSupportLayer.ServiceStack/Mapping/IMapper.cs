namespace SevenDigital.ApiSupportLayer.ServiceStack.Mapping
{
	public interface IMapper<TInput, TOutput>
	{
		TOutput Map(TInput input);
	}
}