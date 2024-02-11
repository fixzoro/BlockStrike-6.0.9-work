using System;
using UnityEngine;

public class MinMaxRangeAttribute : PropertyAttribute
{
	public MinMaxRangeAttribute(float minLimit, float maxLimit)
	{
		this.minLimit = minLimit;
		this.maxLimit = maxLimit;
	}
    
	public float minLimit;
    
	public float maxLimit;
}
