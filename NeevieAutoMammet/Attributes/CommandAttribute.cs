﻿using System;

namespace NeevieAutoMammet.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
	public CommandAttribute(string command)
	{
		Command = command;
	}

	public string Command { get; }
}