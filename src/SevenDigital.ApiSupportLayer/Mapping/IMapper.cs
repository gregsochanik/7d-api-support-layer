namespace SevenDigital.ApiSupportLayer.Mapping
{
	public interface IMapper<TInput, TOutput>
	{
		TOutput Map(TInput input);
	}
}