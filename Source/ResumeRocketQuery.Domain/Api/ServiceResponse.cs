namespace ResumeRocketQuery.Domain.Api
{
    public class ServiceResponse<T>
    {
        public ResponseMetadata ResponseMetadata { get; set; }
        public bool Succeeded { get; set; }
        public T Result { get; set; }
    }
}
