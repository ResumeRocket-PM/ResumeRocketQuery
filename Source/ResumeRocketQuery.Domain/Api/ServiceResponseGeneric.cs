namespace ResumeRocketQuery.Domain.Api
{
    public class ServiceResponseGeneric<T> : ServiceResponse
    {
        public T Result { get; set; }
    }
}
