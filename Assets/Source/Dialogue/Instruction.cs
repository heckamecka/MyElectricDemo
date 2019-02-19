using System.Collections;
using System.Collections.Generic;


public enum InstructionType { LINE, CMD_CUSTOM, JUMP, GATE, RETURN, ASSIGN, IFELSE }

public abstract class Instruction {

    // public int _lineNum; // not sure if i need this at all? - Michel

    public InstructionType _type;
}

public class LineInstruction : Instruction
{
    public string _line;

    public LineInstruction(string line)
    {
        _type = InstructionType.LINE;
        _line = line;
        //Utility.Log("Line Instruction: " + _line);
    }
}

public class CommandInstruction : Instruction
{
    public Command cmd;

    public CommandInstruction(string line)
    {
        _type = InstructionType.CMD_CUSTOM;
        cmd = new Command();
        cmd.text = line;
        //Utility.Log("Command Instruction: " + line);
    }
}

public class JumpInstruction : Instruction
{
    public string _destNode;

    public JumpInstruction(string destinationNode)
    {
        _type = InstructionType.JUMP;
        _destNode = destinationNode;
    }
}

public class GateInstruction : Instruction
{
    public GateInstruction()
    {
        _type = InstructionType.GATE;
    }

    private Dictionary<char, List<Instruction>> instructionBlocks =
        new Dictionary<char, List<Instruction>>()
        {
        { 'j', null }, // jemison block
        { 'c', null }, // cooper block
        { 'a', null }, // armstrong block
        { 'i', null }  // initializer block
        };


    /// <summary>
    /// Add instructions to the corresponding storage block in the class
    /// </summary>
    public void AddInstructions(List<Instruction> store, char marker)
    {
        instructionBlocks[marker] = store;
    }
    
    /// <summary>
    /// Return true if the gate instruction has the block
    /// </summary>
    public bool HasBlock(char marker)
    {
        return (instructionBlocks[marker] != null);
    }

    public void RemoveBlock (char marker)
    {
        instructionBlocks[marker] = null;
    }

    public List<Instruction> GetBlock(char marker)
    {
        return instructionBlocks[marker];
    }

}

public class AssignInstruction : Instruction
{
    string _dataKey;
    MehYarnParser.Token _assignmentType;
    Expression _expression;

    public AssignInstruction()
    {
        _type = InstructionType.ASSIGN;
    }

    public AssignInstruction(string key, MehYarnParser.Token assign, Expression expr)
    {
        _type = InstructionType.ASSIGN;
        _dataKey = key;
        _assignmentType = assign;
        _expression = expr;
    }

    public string GetKey() { return _dataKey; }

    public Value GetData() { return _expression.Evaluate(); }

}

public class IfElseInstruction : Instruction
{
    // Use matching idices to get the correct instruction block from the testing condition
    public List<Expression> _conditions;
    public List<List<Instruction>> _instrBlock;

    public IfElseInstruction()
    {
        _type = InstructionType.IFELSE;
        _conditions = new List<Expression>();
        _instrBlock = new List<List<Instruction>>();
    }

}