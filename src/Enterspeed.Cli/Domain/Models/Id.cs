using Enterspeed.Cli.Domain.Exceptions;

namespace Enterspeed.Cli.Domain.Models;

public abstract class Id : ValueObject
{
    protected Id(string idValue) => IdValue = idValue;

    public string IdValue { get; set; }
    protected static string IdBase => "gid://";

    protected static void CheckIdValidity(string id)
    {
        if (!(id?.StartsWith("gid://", StringComparison.Ordinal) ?? false))
        {
            throw new InvalidIdFormatException("Invalid Gid");
        }
    }

    protected static IDictionary<string, string> GetIdValues(string idValue)
    {
        if (string.IsNullOrWhiteSpace(idValue))
        {
            throw new InvalidIdFormatException("Null gid");
        }

        var gid = idValue.Replace("gid://", string.Empty);

        var values = gid.Split(
            new[]
            {
                "/"
            }, StringSplitOptions.RemoveEmptyEntries);

        var output = new Dictionary<string, string>();

        for (var i = 0; i < values.Length; i += 2)
        {
            var key = values[i];
            if (values.Length > i + 1)
            {
                output.Add(key, values[i + 1]);
            }
        }

        return output;
    }

    protected static void ValidateOrder(IDictionary<string, string> idValues, params string[] order)
    {
        var keys = idValues.Keys.Where(order.Contains).ToArray();

        if (keys.Where((key, i) => order[i] != key).Any())
        {
            throw new InvalidIdFormatException("Order");
        }
    }

    protected static Guid GetValidatedGuid(string guidAsString)
    {
        if (!Guid.TryParse(guidAsString, out var guid))
        {
            throw new InvalidIdFormatException("Guid");
        }

        return guid;
    }

    public override string ToString() => IdValue;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return IdValue;
    }
}