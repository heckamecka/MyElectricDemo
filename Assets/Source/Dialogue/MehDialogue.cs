using System.Collections;
using System.Collections.Generic;

public class Dialogue
{
    private Dictionary<string, Node> _nodeTable;
    public Dialogue(Dictionary<string, Node> nodeTable)
    {
        _nodeTable = nodeTable;
    }

    // easy get for node table - Michel
    public Node this[string key]
    {
        get { return _nodeTable[key]; }
    }

}

/// <summary>
/// Node inside of a dialogue tree, stores title and executable block.
/// </summary>
public class Node {
    public string _title;
    public List<Instruction> _execBlock;
}

public struct Command { public string text; }
