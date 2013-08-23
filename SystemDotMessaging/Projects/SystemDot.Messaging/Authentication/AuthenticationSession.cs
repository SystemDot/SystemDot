using System;
using System.Diagnostics.Contracts;

namespace SystemDot.Messaging.Authentication
{
    public class AuthenticationSession : Equatable<AuthenticationSession>
    {
        public Guid Id { get; set; }

        public DateTime ExpiresOn { get; set; }

        public DateTime GracePeriodEndOn { get; set; }

        public AuthenticationSession()
        {
        }

        public AuthenticationSession(DateTime expiresOn, DateTime gracePeriodEndOn)
        {
            Id = Guid.NewGuid();
            ExpiresOn = expiresOn;
            GracePeriodEndOn = gracePeriodEndOn;
        }

        public override bool Equals(AuthenticationSession other)
        {
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}