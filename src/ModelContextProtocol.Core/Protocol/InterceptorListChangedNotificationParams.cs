namespace ModelContextProtocol.Protocol;

/// <summary>
/// Represents the parameters sent with an <see cref="NotificationMethods.InterceptorListChangedNotification"/> notification.
/// </summary>
/// <remarks>
/// This notification is sent when the list of available interceptors on a server changes,
/// indicating that clients should refresh their interceptor list if needed.
/// See the <see href="https://github.com/modelcontextprotocol/specification/blob/main/schema/">schema</see> for details.
/// </remarks>
public sealed class InterceptorListChangedNotificationParams
{
    // This notification has no parameters; its presence alone indicates the list has changed
}
