using System.Threading.Tasks;
using Xamarin.Essentials;

namespace sample;

public interface IPermissionHandler
{
    Task<PermissionStatus> RequestPermission<TPermission>()
        where TPermission : Permissions.BasePlatformPermission, new();
}

/// <summary>
/// Permission Handler
/// </summary>
public class PermissionHandler : IPermissionHandler
{
    /// <summary>
    /// Request Permission
    /// </summary>
    /// <typeparam name="TPermission">Base Platform Permission</typeparam>
    /// <returns>Permission Status</returns>
    public async Task<PermissionStatus> RequestPermission<TPermission>()
        where TPermission : Permissions.BasePlatformPermission, new()
    {
        var status = await Permissions.CheckStatusAsync<TPermission>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<TPermission>();
        }

        return status;
    }
}