using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace TestApp.Helpers;

internal static class EmailCheckingHelper
{
    internal static bool Check(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new(pattern);

            if (!regex.IsMatch(email))
                return false;

            string domain = email.Split('@')[1];
            IPHostEntry entry = Dns.GetHostEntry(domain);
            return entry.AddressList.Length > 0;
        }
        catch (SocketException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
