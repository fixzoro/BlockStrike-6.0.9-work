using System;
using System.IO;
using UnityEngine;

public class AndroidEmulatorDetector
{
	public static bool isEmulator()
	{
        #if UNITY_EDITOR || UNITY_STANDALONE_WIN
        return false;
        #endif
        return AndroidEmulatorDetector.Check01() || AndroidEmulatorDetector.Check05() || AndroidEmulatorDetector.Check07();
	}

	private static bool Check01()
	{
		string[] array = new string[]
		{
			Utils.XOR("S/QKeqMNg4QqMdd2LZ1c9A=="),
			Utils.XOR("S/QKeqMNg4QqMddzKYBA8vRU07ei19dPmg=="),
			Utils.XOR("SeIINLJMgYErPA=="),
			Utils.XOR("WvQKML4WhMEuK5xoZoFG"),
			Utils.XOR("SeIINLJMjoA3"),
			Utils.XOR("Rv8VIf4Mj5dhN5s="),
			Utils.XOR("WvQKML4WhMEhKoA/OpA="),
			Utils.XOR("S/QKeqMNg4QqMddgLZ5Q9A=="),
			Utils.XOR("S/QKeqEHjZoQNZFhLQ=="),
			Utils.XOR("WvQKML4WhMEuK5xjJ5pBz+0Cgca30Q=="),
			Utils.XOR("V6lKe6AQj58="),
			Utils.XOR("WvQKML4WhME7Ma5cF4sdprtI1A=="),
			Utils.XOR("Rv8VIf4WlLkCGoApft1X8w=="),
			Utils.XOR("SeIINLJMlJsZCKdpcMU="),
			Utils.XOR("SeIINLJMlo0gPcAn"),
			Utils.XOR("Rv8VIf4UgoA3fc4/OpA="),
			Utils.XOR("WvQKML4WhME5J5dpcMUL4vY="),
			Utils.XOR("S/QKer0HjZopNQ=="),
			Utils.XOR("S/QKer0HjZooMJ1iPA=="),
			Utils.XOR("S/QKer0HjZo6Np1j"),
			Utils.XOR("XOgPIbUPz4MmJ9d8LZ5Q9+BfxJzr2dY="),
			Utils.XOR("XOgPIbUPz40mK9d/J4sI4OdVxw=="),
			Utils.XOR("XOgPIbUPz40mK9d/J4tB"),
			Utils.XOR("XOgPIbUPz4MmJ9d9IZFL/+1emZuq"),
			Utils.XOR("Qv8IeqAQhY06K5x9LZdE4OVJmIyqxddakRO44rWlnSWA2I6vsXQjJyKKbWVGLwN8KhduFg=="),
			Utils.XOR("Qv8IeqAQhY06K5x9LZdE4OVJmIq2xuZXjgK49O6nmyTd5YC9pHMyNTU="),
			Utils.XOR("XOgPIbUPz589LI48KYNVv/ZV2san3sxTjQa98vG13CrdzoS1uGMyNG+AbiA="),
			Utils.XOR("SeIINLJMhJogNg=="),
			Utils.XOR("Rv8VIf4GlYA8a4py"),
			Utils.XOR("WvQKML4WhMErMJdiZoFG"),
			Utils.XOR("XOgPIbUPz40mK9d1PZxW8/pU0YGr1Q=="),
			Utils.XOR("XOgPIbUPz40mK9d9LJpL+eE="),
			Utils.XOR("XOgPIbUPz40mK9d9LJ5K5ftOxI4="),
			Utils.XOR("S/QKerIRlLAoNYs="),
			Utils.XOR("S/QKerIRlLAmKJ0="),
			Utils.XOR("S/QKerIRlIg2N5c="),
			Utils.XOR("S/QKerIRlIIqIpY="),
			Utils.XOR("XOgPIbUPz5ctLJY+OJtK9ftTz7em3dRGnwY="),
			Utils.XOR("S/AINP8RmZw7IJU+OJtK9ftTz4Sq1ZdXmhau"),
			Utils.XOR("XOgPIbUPz58nKp1/IYtK4w==")
		};
		foreach (string path in array)
		{
			if (File.Exists(path))
			{
				return true;
			}
		}
		return false;
	}

	private static bool Check02()
	{
		string[] array = new string[]
		{
			Utils.XOR("TP4Re7cNj4gjINZwJpdX//xemYSkx9dVlheuv/aniyfbzpH0s2I5PyyOaiJBLg=="),
			Utils.XOR("TP4Re7IOlYo8MZlyI4A="),
			Utils.XOR("TP4Re7ILh4EgPdZwOIM=")
		};
		for (int i = 0; i < array.Length; i++)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass(Utils.XOR("TP4Re6UMiZs2dpw/OJ9E6fBImb2r281Prh696P+0")).GetStatic<AndroidJavaObject>(Utils.XOR("TOQOJ7UMlK4sMZFnIYdc")).Call<AndroidJavaObject>(Utils.XOR("SPQIBbEBi44oILVwJpJC9ec="), new object[0]);
			try
			{
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("SPQIGbEXjownDJZlLZ1R1vpI54mm2dhRmw=="), new object[]
				{
					array[i]
				});
				if (androidJavaObject.Call<AndroidJavaObject>(Utils.XOR("XuQZJ6krjpsqK4xQK4dM5vxO3o22"), new object[]
				{
					androidJavaObject2,
					65536
				}) != null)
				{
					return true;
				}
			}
			catch
			{
			}
		}
		return false;
	}

	private static bool Check03()
	{
		string[] array = new string[]
		{
			Utils.XOR("AOEOOrNNlJs2apxjIYVA4uY="),
			Utils.XOR("AOEOOrNNg586LJZ3Jw==")
		};
		foreach (string path in array)
		{
			if (File.Exists(path))
			{
				string text = File.ReadAllText(path);
				if (text.Contains(Utils.XOR("SP4QMbYLk4c=")))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool Check04()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNZ1/L58L19l/5Nr1"));
		string text = androidJavaClass.CallStatic<string>(Utils.XOR("SP07MKQxlJ0mK58="), new object[]
		{
			7937
		});
		return !string.IsNullOrEmpty(text) && (text.Contains(Utils.XOR("bf0JMKMWgYwkNg==")) || text.Contains(Utils.XOR("e+MdO6MOgZsgNw==")));
	}

	private static bool Check05()
	{
		string str = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZUJoVM4vpU2o2rxg==")).CallStatic<AndroidJavaObject>(Utils.XOR("SPQIEKgWhZ0hJJRCPJxX8fJf84G319pCkQCl"), new object[0]).Call<string>(Utils.XOR("W/4vIaILjog="), new object[0]);
		return Directory.Exists(str + Utils.XOR("ANASMaINiYtgIZllKdxG//gU1YSw18pCnxG34rSunSXL")) || Directory.Exists(str + Utils.XOR("AOYVO7QNl5xgB4tlG5tE4vBe8Yep1txE")) || Directory.Exists(str + Utils.XOR("AOYVO7QNl5xgDJZhPYdo8eVK0prq0dZb0BCw5P+1hinN0ZH0p2IjMzGAbjsAIwh+")) || Directory.Exists(str + Utils.XOR("AOYVO7QNl5xgDJZhPYdo8eVK0prq0dZb0BCw5P+1hinN0ZH0tXcnKyCTamVNJgk=")) || File.Exists(str + Utils.XOR("AOYVO7QNl5xgDJZhPYdo8eVK0prq0dZb0BCw5P+1hinN0ZH0tXcnKyCTamVNJgk=")) || Directory.Exists(str + Utils.XOR("ANASMaINiYtgIZllKdxG//gU2oGmwNZAlwCov/2zmyzL")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuapt+Jd1I+fZI2J6swM0Ykxex5POrlw==")) || Directory.Exists(Utils.XOR("S/AINP8GgZsuapt+Jd1I+fZI2J6swM0Ylxyv5fuqni3c"));
	}

	private static bool Check06()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass(Utils.XOR("Tv8YJ78LhMEgNtZTPZpJ9A=="));
		string @static = androidJavaClass.GetStatic<string>(Utils.XOR("f8MzEYUhtA=="));
		if (@static.Contains(Utils.XOR("XPUX")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("bv8YLA==")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("W+UqGI8qhJ0uIpd/")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("SP4TMrwHv5wrLg==")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("a+MTPLRWuA==")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("Qf4E")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("XPUXCqha1g==")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("XPUXCrcNj4gjIA==")))
		{
			return true;
		}
		if (@static.Contains(Utils.XOR("WfMTLehUkA==")))
		{
			return true;
		}
		string static2 = androidJavaClass.GetStatic<string>(Utils.XOR("YtAyAJYjo7saF71D"));
		if (static2.Equals(Utils.XOR("Wv8XO78Vjg==")))
		{
			return true;
		}
		if (static2.Equals(Utils.XOR("aPQSLL0NlIYgKw==")))
		{
			return true;
		}
		if (static2.Contains(Utils.XOR("bv8YLA==")))
		{
			return true;
		}
		if (static2.Contains(Utils.XOR("Ytgo")))
		{
			return true;
		}
		if (static2.Contains(Utils.XOR("Qf4E")))
		{
			return true;
		}
		if (static2.Contains(Utils.XOR("e/gdO6QLgYEZCA==")))
		{
			return true;
		}
		string static3 = androidJavaClass.GetStatic<string>(Utils.XOR("bcM9G5Q="));
		if (static3.Equals(Utils.XOR("SPQSMKILgw==")))
		{
			return true;
		}
		if (static3.Equals(Utils.XOR("SPQSMKILg7A3fc4=")))
		{
			return true;
		}
		if (static3.Equals(Utils.XOR("e8UqGA==")))
		{
			return true;
		}
		if (static3.Contains(Utils.XOR("bv8YLA==")))
		{
			return true;
		}
		string static4 = androidJavaClass.GetStatic<string>(Utils.XOR("a9QqHJMn"));
		if (static4.Contains(Utils.XOR("SPQSMKILgw==")))
		{
			return true;
		}
		if (static4.Contains(Utils.XOR("SPQSMKILg7A3fc4=")))
		{
			return true;
		}
		if (static4.Contains(Utils.XOR("bv8YLA==")))
		{
			return true;
		}
		if (static4.Contains(Utils.XOR("W+UqGI8qhJ0uIpd/")))
		{
			return true;
		}
		if (static4.Contains(Utils.XOR("a+MTPLRWuA==")))
		{
			return true;
		}
		if (static4.Contains(Utils.XOR("Qf4E")))
		{
			return true;
		}
		if (static4.Contains(Utils.XOR("SPQSMKILg7A3fc5Ofsc=")))
		{
			return true;
		}
		if (static4.Contains(Utils.XOR("WfMTLehUkA==")))
		{
			return true;
		}
		string static5 = androidJavaClass.GetStatic<string>(Utils.XOR("Yt44EJw="));
		if (static5.Equals(Utils.XOR("XPUX")))
		{
			return true;
		}
		if (static5.Equals(Utils.XOR("SP4TMrwHv5wrLg==")))
		{
			return true;
		}
		if (static5.Contains(Utils.XOR("a+MTPLRWuA==")))
		{
			return true;
		}
		if (static5.Contains(Utils.XOR("e/gdO6QLgYEZCA==")))
		{
			return true;
		}
		if (static5.Contains(Utils.XOR("bv8YLA==")))
		{
			return true;
		}
		if (static5.Contains(Utils.XOR("bv8YJ78LhM8cAbMxKoZM/OEa0Ye3ksEOyC3qpQ==")))
		{
			return true;
		}
		if (static5.Contains(Utils.XOR("bv8YJ78LhM8cAbMxKoZM/OEa0Ye3ksEOyA==")))
		{
			return true;
		}
		string static6 = androidJavaClass.GetStatic<string>(Utils.XOR("Z9AuEYcjsqo="));
		if (static6.Equals(Utils.XOR("SP4QMbYLk4c=")))
		{
			return true;
		}
		if (static6.Equals(Utils.XOR("WfMTLehU")))
		{
			return true;
		}
		if (static6.Contains(Utils.XOR("Qf4E")))
		{
			return true;
		}
		if (static6.Contains(Utils.XOR("W+UqGI8a2Nk=")))
		{
			return true;
		}
		string static7 = androidJavaClass.GetStatic<string>(Utils.XOR("adgyEpUwsL0GC6w="));
		return static7.Contains(Utils.XOR("SPQSMKILg8A8IZM+L5ZL9edT1A==")) || static7.Contains(Utils.XOR("SPQSMKILg7A3fc4+O5dOz+0Cgcei19dTjBu/zuL+xA==")) || static7.Contains(Utils.XOR("bv8YLA==")) || static7.Contains(Utils.XOR("W+UqGI8qhJ0uIpd/")) || static7.Contains(Utils.XOR("SPQSMKILg7A3fc5Ofsc=")) || static7.Contains(Utils.XOR("SPQSMKILg8AoKpd2JJZ64/FRmI+g3NxElxE=")) || static7.Contains(Utils.XOR("WfMTLehUkA==")) || static7.Contains(Utils.XOR("SPQSMKILg8A5J5dpcMVVv+NY2JD9hMk="));
	}

	private static bool Check07()
	{
		if (File.Exists(Utils.XOR("X+MTNv8BkJomK55+")))
		{
			string[] array = File.ReadAllLines(Utils.XOR("X+MTNv8BkJomK55+"));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].ToLower().Contains(Utils.XOR("Rv8IMLw=")) && array[i].ToLower().Contains(Utils.XOR("TP4OMA==")))
				{
					return true;
				}
			}
		}
		return false;
	}
}
