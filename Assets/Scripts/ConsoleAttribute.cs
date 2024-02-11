using System;

public class ConsoleAttribute : Attribute
{
    public string[] commands;

    public ConsoleAttribute(params string[] value)
	{
		this.commands = value;
	}
}
