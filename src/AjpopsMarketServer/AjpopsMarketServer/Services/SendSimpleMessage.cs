using RestSharp; // RestSharp v112.1.0
using RestSharp.Authenticators;

namespace AjpopsMarketServer.Services;

public interface ISendSimpleMessage
{
    Task<RestResponse> Send();
}

public class SendSimpleMessage : ISendSimpleMessage
{
    readonly string API_KEY;

    public SendSimpleMessage(IConfiguration configuration)
    {
        API_KEY = configuration["MAILGUN_API_KEY"] ?? string.Empty;
    }

    public async Task<RestResponse> Send()
    {
        var options = new RestClientOptions("https://api.mailgun.net")
        {
            Authenticator = new HttpBasicAuthenticator("api", API_KEY)
        };

        var client = new RestClient(options);
        var request = new RestRequest("/sandboxa285848852de448b96716bf2737db16d.mailgun.org/messages", Method.Post);
        request.AlwaysMultipartFormData = true;
        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxa285848852de448b96716bf2737db16d.mailgun.org>");
        request.AddParameter("to", "Raul Avila Catala <cl2raul66_dev_gt@hotmail.com>");
        request.AddParameter("subject", "Hello Raul Avila Catala");
        request.AddParameter("text", "Congratulations Raul Avila Catala, you just sent an email with Mailgun! You are truly awesome!");
        return await client.ExecuteAsync(request);
    }
}
