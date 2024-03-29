using ResumeRocketQuery.Domain.Services;

namespace ResumeRocketQuery.Domain.DataLayer
{
    public class AccountConfigurationStorage
    {
        public string AccountAlias { get; set; }
        public Authentication Authentication { get; set; }
    }
}
