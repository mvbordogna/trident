using System;
using System.Security.Claims;

namespace Trident.UI.Blazor.Models;

public class UserContext
{
    public UserContext(
        Guid userId,
        ClaimsPrincipal principal
        //,AccountModel account,
        //Person.Person profile,
        //NavigationModel navigationModel
        )
    {
        UserId = userId;
        Principal = principal;
        //Account = account;
        //Profile = profile;
        //UserNavigation = navigationModel;
    }

    public Guid InstanceId { get; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ClaimsPrincipal Principal { get; private set; }
    //public AccountModel Account { get; }
    //public Person.Person Profile { get; set; }
    //public NavigationModel UserNavigation { get; set; }
    public string DefaultLandingPage { get; set; }
}
