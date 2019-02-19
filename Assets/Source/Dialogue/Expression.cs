using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE:

// this is a hyper simplified expression system. If the anyone insists on having anything more
// complicated, well, we're gonna have to rewrite this whole thing - Michel
// I don't think I'm ever going to delete the yarn spinner files, so if you need help,
// look inside Parser.cs for the fucntion "Expression Parse(ParseNode parent, Parser p)" 
// or this wikipedia page here https://en.wikipedia.org/wiki/Shunting-yard_algorithm - Michel

public class Expression
{
    // type denominator
    enum Type { DATA, VALUE, FUNC }
    Type _type;
    //Pure Value
    Value _value;
    // Key to get a value from persistent Data
    string _dataKey;
    // function operation
    Expression _left;
    Expression _right;
    MehYarnParser.Token _op;

    // Constructors - Michel
    public Expression() { }

    public Expression(Value value)
    {
        _value = value;
        _type = Type.VALUE;
    }

    public Expression(string key)
    {
        _dataKey = key;
        _type = Type.DATA;
    }

    public Expression(Expression left, Expression right, MehYarnParser.Token op)
    {
        _left = left;
        _right = right;
        _op = op;
        _type = Type.FUNC;
    }
    // TODO add other constructor combinations?

    ///<summary> Evaluates the Expression to a Value </summary>
    public Value Evaluate()
    {
        switch (_type)
        {
            case Type.VALUE:
                return _value;
            case Type.DATA:
                return MehGameManager.instance.persistent.Get(_dataKey);
            case Type.FUNC:
                return FunctionEvaluate();
        }
        // Code should never reach this point
        return new Value();
    }

    // Called by Evaluate(), Decides what function is used based on token type, and uses it to determine the output value, 
    public Value FunctionEvaluate()
    {
        Value output = new Value();

        Value l = _left.Evaluate();
        Value r = _right.Evaluate();

        switch (_op)
        {
            // Straight Integer operations
            case MehYarnParser.Token.ADD:
            case MehYarnParser.Token.SUB:
                output.type = VarType.Int;
                output.i = IntInIntOut(l,r);
                break;
            // Straight Boolean operations
            case MehYarnParser.Token.AND:
            case MehYarnParser.Token.OR:
            case MehYarnParser.Token.NOT:
                output.type = VarType.Bool;
                output.b = BoolInBoolOut(l,r);
                break;
            // Int in bool out
            case MehYarnParser.Token.GREATER:
            case MehYarnParser.Token.LESS:
                output.type = VarType.Bool;
                output.b = IntInBoolOut(l, r);
                break;
            // Any in Bool out
            case MehYarnParser.Token.EQUAL:
            case MehYarnParser.Token.NOTEQUAL:
                output.type = VarType.Bool;
                output.b = AnyInBoolOut(l, r);
                break;
        }
        return output;
    }

    // Types of input/output value combinations
    #region IO Eval
    int IntInIntOut(Value l, Value r)
    {
        // check if either left or right are not integers - Michel
        if (!Utility.AreOfType(l, r, VarType.Int))
        {
            Utility.LogError("One or more values cannot be evaluated into an integerfor a [" + _op + "] operation");
            return 0;
        }

        switch (_op)
        {
            // Integer operations
            case MehYarnParser.Token.ADD: return l.i + r.i;
            case MehYarnParser.Token.SUB: return l.i - r.i;
            default:
                Utility.LogError("Token mismatch IiIo: " + _op);
                return 0;
        }
    }
     
    bool BoolInBoolOut(Value l, Value r)
    {
        if (!Utility.AreOfType(l, r, VarType.Bool))
        {
            Utility.LogError("One or more values cannot be evaluated into a boolean for a ["+ _op +"] operation");
            return false;
        }
        switch (_op)
        {
            case MehYarnParser.Token.AND: return l.b && r.b;
            case MehYarnParser.Token.OR:  return l.b || r.b;
            case MehYarnParser.Token.NOT: return !r.b;
            default:
                Utility.LogError("Token mismatch BiBo: " + _op);
                return false;
        }
    }

    bool IntInBoolOut(Value l, Value r)
    {
        // check if either left or right are not integers - Michel
        if (!Utility.AreOfType(l, r, VarType.Int))
        {
            Utility.LogError("One or more values cannot be evaluated into a integer for a [" + _op + "] operation");
            return false;
        }
        switch (_op)
        {
            case MehYarnParser.Token.GREATER: return l.i > r.i;
            case MehYarnParser.Token.LESS:    return l.i < r.i;
            default:
                Utility.LogError("Token mismatch IiBo: " + _op);
                return false;
        }
    }

    bool AnyInBoolOut(Value l, Value r)
    {
        if(!Utility.AreSameType(l,r))
        {
            Utility.LogError("Evaluation type mismatch for [" + _op + "] operation (Left is ["+r.type+"] while right is [" +l.type+"] )");
            return false;
        }
        switch (_op)
        {
            case MehYarnParser.Token.EQUAL:     if (l.type == VarType.Bool) return l.b == r.b; else return l.i == r.i;
            case MehYarnParser.Token.NOTEQUAL:  if (l.type == VarType.Bool) return l.b != r.b; else return l.i != r.i;
            default:
                Utility.LogError("Token mismatch AiBo: " + _op);
                return false;
        }



        return false;
    }
    #endregion
    
    public override string ToString()
    {
        switch (_type)
        {
            case Type.VALUE:
                return _value.ToString() ;
            case Type.DATA:
                return _dataKey;
            case Type.FUNC:
                return (_left.ToString() + " " + _op + " " + _right.ToString());
            default:
                return null;
        }
    }

}

