namespace SevenDigital.ApiInt.ServiceStack.Mapping
{
	public interface IMapper<TInput, TOutput>
	{
		TOutput Map(TInput input);
	}
}