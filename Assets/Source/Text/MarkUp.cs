using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkUp
{
    public int _start;
    public int _end;

    public MarkUp(int start, int end)
    {
        _start = start;
        _end = end;
    }
}

public class ShakeMarkUp : MarkUp
{
    public float _intensity;

    public ShakeMarkUp(int start, int end,float intensity = 1.0f) : base(start,end)
    {
        _intensity = intensity;
    }
}

public class TrailMarkUp : MarkUp
{
    public Dictionary<int, float> fractions;

    public TrailMarkUp(int start, int end) : base(start, end)
    {
        fractions = new Dictionary<int, float>();

        // getting the distance between the last character and the first 
        // (approximately, it's not guaranteed monospaced afterall, should work for most languages) - Michel
        float distance = (float)(end - start);

        for (int i = start; i < end; ++i)
        {               // based on it's position in the line of characters, turn it into a fraction
            float f = ((float)(i - start)) / distance;
            fractions.Add(i, f);
        }

    }
}