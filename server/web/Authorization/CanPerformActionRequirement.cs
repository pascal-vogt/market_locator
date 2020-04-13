namespace Web.Authorization
{
    using Microsoft.AspNetCore.Authorization;

    public class CanPerformActionRequirement : IAuthorizationRequirement
    {
        public string Action { get; }
        
        public CanPerformActionRequirement(string action)
        {
            Action = action;
        }
    }
}