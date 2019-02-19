using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

// Type denominators - Michel 
public enum VarType
{
    // Null for undefined, non-explicit value
    Null, Int, Bool, Float
}

// Serializable Union type to store data in save data - Michel
[StructLayout(LayoutKind.Explicit)]
[System.Serializable]
public struct Value
{
    [FieldOffset(0)] public int i;
    [FieldOffset(0)] public bool b;
    [FieldOffset(0)] public float f;
    [FieldOffset(4)] public VarType type;

    // Constructors
    public Value(bool value)
    {
        i = 0;
        f = 0;
        // the two above don't matter since we're gonna override them anyways - Michel
        b = value;
        type = VarType.Bool;
    }
    public Value(int value)
    {
        b = true;
        f = 0;
        // the two above don't matter since we're gonna override them anyways - Michel
        i = value;
        type = VarType.Int;
    }
    public Value(float value)
    {
        b = false;
        i = 0;
        // the two above don't matter since we're gonna override them anyways - Michel
        f = value;
        type = VarType.Float;
    }
    public override string ToString()
    {
        switch (type)
        {
            case VarType.Int:
                return i.ToString();
            case VarType.Bool:
                return b.ToString();
            case VarType.Float:
                return f.ToString();
            case VarType.Null:
            default:
                return "null";
        }
    }

    // TODO probably should add some type of isEquals function


}

public class Utility : MonoBehaviour {

    //Value type checking
    public static bool IsOfType(Value val, VarType checkType)
    {
        if (val.type == VarType.Null)
        {
            val.type = checkType;
            return true;
        }
        else return (val.type == checkType);
    }

    public static bool AreOfType(Value l, Value r, VarType checkType)
    {
        return IsOfType(l,checkType) && IsOfType(r, checkType);
    }

    public static bool AreSameType(Value l, Value r)
    {
        return l.type == r.type;
    }

    // Utility Logs, for logging inside of non-monobehaviour objects- Michel
    public static void Log(string message)
    {
        Debug.Log("Util Log: " + message);
    }
    public static void LogError(string message)
    {
        Debug.LogError("Util Log: " + message);
    }
    public static void LogWarning(string message)
    {
        Debug.LogWarning("Util Log: " + message);
    }

    public static Transform FindGreatestParent(Transform child)
    {
        Transform t = child;
        while (t.parent != null)
        {
            t = t.parent;
        }
        return t;
    }
}
