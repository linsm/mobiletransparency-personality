using Microsoft.AspNetCore.Authorization;

public class TreeManagerRequirementAuthorizationHandler : AuthorizationHandler<TreeManagerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TreeManagerRequirement requirement)
        {
            if (context.User.IsInRole(Role.TreeManager.ToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class TreeManagerRequirement : IAuthorizationRequirement{
        
    }