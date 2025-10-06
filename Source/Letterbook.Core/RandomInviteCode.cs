namespace Letterbook.Core;

public class RandomInviteCode : IInviteCodeGenerator
{
	private const string Pool = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private Random _random;
	private static object Lock = new();

	public RandomInviteCode(Random rand)
	{
		_random = rand;
	}

	public RandomInviteCode()
	{
		_random = Random.Shared;
	}

	public string Generate()
	{
		var seq = new char[14];
		var i = 0;
		lock (Lock)
		{
			i = Add(seq, i);
			seq[i] = '-';
			i = Add(seq, ++i);
			seq[i] = '-';
			Add(seq, ++i);
		}

		return string.Join("", seq);
	}

	private int Add(char[] s, int start)
	{
		var i = 0;
		for (; i < 4; i++)
		{
			s[start + i] = Pool[_random.Next(0, 36)];
		}

		return start + i;
	}
}