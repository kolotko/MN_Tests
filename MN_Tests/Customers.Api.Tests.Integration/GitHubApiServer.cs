using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Customers.Api.Tests.Integration;

public class GitHubApiServer : IDisposable
{
    private WireMockServer _server;
    public string Url => _server.Url!;

    public void Start()
    {
        _server = WireMockServer.Start();
    }
    
    public void SetupUser(string username)
    {
        _server.Given(Request.Create()
                .WithPath($"/users/{username}")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(@$"{{
                  ""login"": ""{username}"",
                  ""id"": 96433920,
                  ""node_id"": ""U_kgDOBb93AA"",
                  ""avatar_url"": ""https://avatars.githubusercontent.com/u/96433920?v=4"",
                  ""gravatar_id"": """",
                  ""url"": ""https://api.github.com/users/{username}"",
                  ""html_url"": ""https://github.com/{username}"",
                  ""followers_url"": ""https://api.github.com/users/{username}/followers"",
                  ""following_url"": ""https://api.github.com/users/{username}/following{{/other_user}}"",
                  ""gists_url"": ""https://api.github.com/users/{username}/gists{{/gist_id}}"",
                  ""starred_url"": ""https://api.github.com/users/{username}/starred{{/owner}}{{/repo}}"",
                  ""subscriptions_url"": ""https://api.github.com/users/{username}/subscriptions"",
                  ""organizations_url"": ""https://api.github.com/users/{username}/orgs"",
                  ""repos_url"": ""https://api.github.com/users/{username}/repos"",
                  ""events_url"": ""https://api.github.com/users/{username}/events{{/privacy}}"",
                  ""received_events_url"": ""https://api.github.com/users/{username}/received_events"",
                  ""type"": ""User"",
                  ""user_view_type"": ""public"",
                  ""site_admin"": false,
                  ""name"": null,
                  ""company"": null,
                  ""blog"": """",
                  ""location"": null,
                  ""email"": null,
                  ""hireable"": null,
                  ""bio"": null,
                  ""twitter_username"": null,
                  ""public_repos"": 11,
                  ""public_gists"": 0,
                  ""followers"": 0,
                  ""following"": 0,
                  ""created_at"": ""2021-12-20T15:30:40Z"",
                  ""updated_at"": ""2024-11-18T17:07:39Z""
                }}")
                .WithHeader("content-type", "application/json; charset=utf-8")
                .WithStatusCode(200));
    } 
    
    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}