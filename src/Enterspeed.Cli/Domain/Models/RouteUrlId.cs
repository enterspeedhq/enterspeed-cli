using System.Text.RegularExpressions;
using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public sealed class RouteUrlId : Id
{
    public RouteUrlId(string idValue)
        : base(idValue)
    {
    }

    public string EnvironmentGuid { get; set; }
    public string UrlPath { get; set; }
    public string DomainGuid { get; set; }

    public static string From(string environmentGuid, string domainGuid, string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(environmentGuid))
        {
            throw new InvalidIdFormatException("Missing environment id");
        }

        return $"{IdBase}Environment/{environmentGuid}/Domain/{domainGuid}/Route/Url{Clean(url)}";
    }

    public static bool TryParse(string routeId, out RouteUrlId result)
    {
        try
        {
            result = Parse(routeId);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public static RouteUrlId Parse(string routeId)
    {
        CheckIdValidity(routeId);

        var environment = EnvironmentId.Parse(routeId);

        var values = GetIdValues(routeId);

        var domainGuid = values.ContainsKey("Domain")
            ? values["Domain"]
            : null;

        var path = routeId.Split($"{environment.IdValue}/Domain/{domainGuid}/Route/Url", StringSplitOptions.RemoveEmptyEntries);
        var urlPath = path.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(urlPath))
        {
            throw new InvalidIdFormatException("Missing url");
        }

        return new RouteUrlId(From(environment.EnvironmentGuid.ToString(), domainGuid, urlPath))
        {
            EnvironmentGuid = environment.EnvironmentGuid.ToString(),
            UrlPath = urlPath,
            DomainGuid = domainGuid
        };
    }

    private static bool IsUrlValid(string url) => url.StartsWith("http", StringComparison.Ordinal) || url.StartsWith("/", StringComparison.Ordinal);

    private static string Clean(string url)
    {
        if (!IsUrlValid(url))
        {
            throw new UriFormatException($"URL is invalid: {url}");
        }

        // Remove http/https
        var output = Regex.Replace(url, @"^(?:http(?:s)?:\/\/)?", string.Empty, RegexOptions.IgnoreCase);

        // Remove query
        if (output.Contains('?'))
        {
            output = output.Split('?')[0];
        }

        // Remove hashes
        if (output.Contains('#'))
        {
            output = output.Split('#')[0];
        }

        // Remove trailing slash
        if (output.EndsWith('/'))
        {
            output = output.TrimEnd('/');
        }

        var pathIndex = output.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
        output = pathIndex == -1
            ? "/"
            : output[pathIndex..];

        output = output.ToLowerInvariant();

        return output;
    }
}
