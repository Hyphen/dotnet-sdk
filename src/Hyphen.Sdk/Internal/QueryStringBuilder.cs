using System.Text;
using System.Web;

namespace Hyphen.Sdk;

internal class QueryStringBuilder
{
	readonly StringBuilder result = new();

	public void Add(string key, object? value)
	{
		if (value is null)
			return;

		if (result.Length == 0)
			result.Append('?');
		else
			result.Append('&');

		result.Append(HttpUtility.UrlEncode(key));
		result.Append('=');
		result.Append(HttpUtility.UrlEncode(value.ToString()));
	}

	public override string ToString() => result.ToString();
}
