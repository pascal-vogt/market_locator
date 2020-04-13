namespace Web.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Database;
    using Microsoft.AspNetCore.Authorization;

    public class CanPerformActionHandler : AuthorizationHandler<CanPerformActionRequirement>
    {
        private readonly DatabaseContext _databaseContext;

        public CanPerformActionHandler(DatabaseContext databaseContext)
        {
            this._databaseContext = databaseContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanPerformActionRequirement requirement)
        {
            var email = context.User.Identity?.Name ?? "anon";
            var hasPermission = this._databaseContext.AllowedGlobalActions.Any(o => o.Action == requirement.Action && (o.Email == email || o.Email == "anon"));
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}