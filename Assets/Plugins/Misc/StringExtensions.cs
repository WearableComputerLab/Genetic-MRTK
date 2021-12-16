using System.Text;

public static class StringExtensions
{
	public static string Readable(this string s)
	{
		StringBuilder sb = new StringBuilder();

		bool previousIsWhiteSpace = false;

		for (int i = 0; i < s.Length; i++)
		{
			if (i == 0 || i == s.Length-1)
			{
				if (char.IsLetterOrDigit(s[i]))
				{
					sb.Append(s[i]);
				}
				continue;
			}

			if (!char.IsLetterOrDigit(s[i]))
			{
				if(!previousIsWhiteSpace)
				{
					sb.Append(' ');
					previousIsWhiteSpace = true;
				}
				continue;
			}
			else if (char.IsUpper(s[i]))
			{
				if(!previousIsWhiteSpace)
				{
					sb.Append(' ');
					previousIsWhiteSpace = true;
				}
			}

			sb.Append(s[i]);
			previousIsWhiteSpace = false;
		}

		return sb.ToString();
	}
}
