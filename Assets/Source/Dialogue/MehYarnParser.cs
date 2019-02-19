using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;


// Used by MehDialogueRunner to parse a yarn file into a list of instructions - Michel
public class MehYarnParser {

    // when parsing lines, if this is encountered, ignore it completley
    const string LINE_COMMENT = "//";
    MehYarnLexer _lexer;

    public MehYarnParser(){
        _lexer = new MehYarnLexer();
    }

    public Dialogue Load(string text)
    {
        // Parse raw text
        NodeSource[] source = ParseText(text);

        // parse NodeSources into actual nodes and dump into nodeTable
        var nodeTable = new Dictionary<string, Node>();
        foreach (NodeSource s in source)
        {
            nodeTable.Add(s._title, ParseNode(s));
        }

        return new Dialogue(nodeTable);

    }

    /// <summary>
    /// Parses raw string into NodeInfo - Michel
    /// </summary>
    NodeSource[] ParseText(string text)
    {

        var nodes = new List<NodeSource>();

        // check for the existence of at least one "---"+newline sentinel, which divides
        // the headers from the body

        // we use a regex to match either \r\n or \n line endings
        if (Regex.IsMatch(text, "---.?\n") == false)
        {
            // Error checking
            Utility.LogError("Error parsing input: text appears corrupt (no header sentinel)");
            return null;
        }
        
        using (var reader = new System.IO.StringReader(text))
        {
            string line;

            Regex titleRegex = new Regex(@"title: (.*)");
            while ((line = reader.ReadLine()) != null)
            {

                // Create a new node
                NodeSource node = new NodeSource();

                // Attempt to match title
                // --------------------------------------------------------------
                Match match = titleRegex.Match(line);
                if (match.Success)
                {
                    node._title = match.Groups[1].Value;
                }
                else
                {
                    Utility.LogError("Error parsing input: header title not found");
                    return null;
                    //break;
                }

                // Skip lines until we go past the header - Michel
                // --------------------------------------------------------------
                while ((line = reader.ReadLine()) != "---"){

                }

                // We're past the header; read the body
                // ----------------------------------------------------------------

                var lines = new List<string>();

                // Skip past header lines until we hit the end of node sentinel
                // or the end of the file
                while ((line = reader.ReadLine()) != "===" && line != null)
                {
                    // If the line starts with "//", skip that line - Michel
                    // TODO: maybe make it a little bit more robust, so that it detects it even when there's white space after it
                    if (line.StartsWith(LINE_COMMENT)) continue;
                    // if the line is empty, skip that line 
                    else if (line.Length == 0) continue;

                    lines.Add(line);
                }
                // Finished reading lines, dump them into NodeSource - Michel
                node._body = lines.ToArray();

                // And add this node to the list
                nodes.Add(node);

                // done with this particular node, moving on to next node

            }
        }

        return nodes.ToArray();
    }
    
    /// <summary>
    ///  Converts a Nodesource into a Node, parsing all relevant information into instructions - Michel
    /// </summary>
    Node ParseNode(NodeSource source)
    {
        // Convert the lines into a queue- Michel
        var lines = new Queue<string>(source._body);

        // Run the line queue through the lexer parser- Michel
        var block = _lexer.ParseLines(ref lines);

        Node node = new Node();
        node._title = source._title;
        node._execBlock = block;

        return node;
    }

    ///<summary> Temporary storage to be parsed into Nodes - Michel
    /// </summary>
    internal struct NodeSource
    {
        public string _title;
        public string[] _body;
    }

    public enum Token
    {
        // General Tokens
        TEXT,
        CMD,
        JUMP,

        // Command Tokens (Specific)
        GATE,
        SET,
        IF,

        // Character Tokens
        JEMISON, COOPER, ARMSTRONG,

        // Gate Tokens
        ENDGATE,

        // ElseIf Tokens
        ELSEIF, ELSE, ENDIF,

        //Operators
        // Arithmetic
        ADD, SUB,
        //Relational
        EQUAL, NOTEQUAL, GREATER, LESS,
        //Logical
        AND, OR, NOT,

        //Assignment
        ASSIGNMENT, // ADDASS, SUBASS,

        // Values
        NUM,
        TRUE,
        FALSE
    }
    // Given a queue of lines, returns a list of instructions
    // Identifies tokens and performs parsing based on those tokens - Michel
    class MehYarnLexer
    {
        // Enum to denote pattern states. I was using strings before but that was error prone so I swapped it out - Michel
        internal enum Pattern { GENERAL, COMMAND, GATE, IFELSE,OPERATOR, ASSIGNMENT, VALUE}
        Dictionary<Pattern, List<TokenRule>> patterns;        // stores Patterns with an enum as a key - Michel
        TokenRule cmdToken = new TokenRule(Token.CMD, @"<<([^<]*?)>>"); // used for gate and ifelse parsing - Michel
        
        public MehYarnLexer()
        {
            patterns = new Dictionary<Pattern, List<TokenRule>>();

            // ---------------------------
            // V V V ADD NEW PATTERNS HERE V V V
            // Order determines what gets matched first
            // ---------------------------
            
            // Reused Token Rules
            // TODO: modify these 3 regex patterns to be more permissive- Michel
            TokenRule jemisonToken = new TokenRule(Token.JEMISON, @"jemison");
            TokenRule cooperToken = new TokenRule(Token.COOPER, @"cooper");
            TokenRule armstrongToken = new TokenRule(Token.ARMSTRONG, @"armstrong");
            
            // General patterns
            patterns.Add(Pattern.GENERAL, new List<TokenRule>());
            patterns[Pattern.GENERAL].Add(new TokenRule(Token.CMD, @"<<([^<]*?)>>")); // matches <<*any thing>>
            patterns[Pattern.GENERAL].Add(new TokenRule(Token.JUMP, @"\[\[([^\[]*?)\]\]")); // matches [[*any thing]]- Michel
            patterns[Pattern.GENERAL].Add(new TokenRule(Token.TEXT, @".*"));

            // Command 
            patterns.Add(Pattern.COMMAND, new List<TokenRule>());
            patterns[Pattern.COMMAND].Add(new TokenRule(Token.GATE, @"gate"));
            patterns[Pattern.COMMAND].Add(new TokenRule(Token.SET, @"set ")); // empty space after text to avoid confusion with other set functions
            patterns[Pattern.COMMAND].Add(new TokenRule(Token.IF, @"if"));

            // gate patterns
            patterns.Add(Pattern.GATE, new List<TokenRule>());
            patterns[Pattern.GATE].Add(new TokenRule(Token.ENDGATE, @"endgate"));
            patterns[Pattern.GATE].Add(jemisonToken);
            patterns[Pattern.GATE].Add(cooperToken);
            patterns[Pattern.GATE].Add(armstrongToken);

            // ifelse patterns, the order matters here - Michel
            patterns.Add(Pattern.IFELSE, new List<TokenRule>());
            patterns[Pattern.IFELSE].Add(new TokenRule(Token.ELSEIF, @"elseif"));
            patterns[Pattern.IFELSE].Add(new TokenRule(Token.ELSE, @"else(?!if)"));
            patterns[Pattern.IFELSE].Add(new TokenRule(Token.ENDIF, @"endif"));

            patterns.Add(Pattern.OPERATOR, new List<TokenRule>());
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.ADD, @"(.+)\+(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.SUB, @"(.+)-(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.EQUAL, @"(.+)==(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.NOTEQUAL, @"(.+)!=(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.GREATER, @"(.+)>(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.LESS, @"(.+)<(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.AND, @"(.+)&&(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.OR, @"(.+)\|\|(.+)"));
            patterns[Pattern.OPERATOR].Add(new TokenRule(Token.NOT, @"(.*)!(.+)"));

            patterns.Add(Pattern.ASSIGNMENT, new List<TokenRule>());
            //patterns[Pattern.ASSIGNMENT].Add(new TokenRule(Token.ADDASS, @"(.+)+=(.+)"));
            //patterns[Pattern.ASSIGNMENT].Add(new TokenRule(Token.SUBASS, @"(.+)-=(.+)"));
            patterns[Pattern.ASSIGNMENT].Add(new TokenRule(Token.ASSIGNMENT, @"(.+)=(.+)"));

            patterns.Add(Pattern.VALUE, new List<TokenRule>());
            patterns[Pattern.VALUE].Add(new TokenRule(Token.TRUE, @"(?<!\w)true(?!\w)"));
            patterns[Pattern.VALUE].Add(new TokenRule(Token.FALSE, @"(?<!\w)false(?!\w)"));
            patterns[Pattern.VALUE].Add(new TokenRule(Token.NUM, @"(?<!\w)(\d+)(?!\w)"));

        }

        internal class TokenRule
        {
            public TokenRule(Token type, string pattern)
            {
                _type = type;
                _regex = new Regex(pattern);
            }
            public Token _type;
            public Regex _regex;
        }

        // Parses a queue of lines into a list of instructions
        public List<Instruction> ParseLines(ref Queue<string> lines)
        {
            var instructions = new List<Instruction>();

            // Iterate through lines- Michel
            while (lines.Count > 0)
            {
                string line = lines.Dequeue();

                // test every TokenRule in General Patterns against the line- Michel
                foreach (TokenRule rule in patterns[Pattern.GENERAL])
                {
                    var match = rule._regex.Match(line);

                    if (match.Success == false || match.Length == 0)
                        continue;
                    // Create an Instruction based on what General Pattern it matches - Michel
                    switch (rule._type)
                    {
                        case Token.CMD: // << >>
                            // There's additional parsing to be done, using the same lines- Michel
                            instructions.Add(ParseCommand(match.Groups[1].Value, ref lines));
                            break;
                        case Token.JUMP: // [[ ]]
                            instructions.Add(new JumpInstruction(match.Groups[1].Value));
                            break;
                        case Token.TEXT: // no special pattern found
                            instructions.Add(new LineInstruction(line));
                            break;
                        default:
                            // No Error handling here,
                            // because the default should really be Token.LINE
                            // and I can't think of a reason why it would reach here- Michel
                            break;
                    }

                    // Stop looping through pattern once we found a match- Michel
                    break;
                }
            }
            return instructions;
        }

        /// <summary>
        /// Parse any commands that uses the <<   >> pattern, and search for custom cmds
        /// </summary>
        /// <returns></returns>
        Instruction ParseCommand(string commandLine, ref Queue<string> lines)
        {
            foreach (var rule in patterns[Pattern.COMMAND])
            {
                // attmpet to find a pattern match
                var match = rule._regex.Match(commandLine);
                if (match.Success == false || match.Length == 0)
                    continue;

                switch (rule._type)
                {
                    case Token.GATE:
                        // Create pattern for a dialogue gate - Michel
                        return ParseGate(ref lines);
                    break;
                    case Token.SET: // Create pattern for a set instruction
                        return ParseSet(commandLine);
                        break;
                    case Token.IF: // Create pattern for an if else block instruction
                        return ParseIfElse(commandLine, ref lines);
                        break;
                }
            }
            // If there are no matches at all, assume that
            // it's a custom command pattern - Michel
            return new CommandInstruction(commandLine);
        }

        #region Parse Gate Code
        /// <summary>
        /// Parses a given set of lines and returns a gate
        /// </summary>
        GateInstruction ParseGate(ref Queue<string> lines)
        {
            Queue<string> tempStore = new Queue<string>();
            GateInstruction gate = new GateInstruction();

            char mark = 'i'; // i for init block, used to denote whether to store the instructions as jemison's lines, armstrong's or cooper's - Michel
            bool enqueue = true;
            Match cmdMatch;

            // go through each line in the queue
            while (lines.Count > 0) {
                string line = lines.Dequeue(); // remove the next line from the master queue
                enqueue = true;// mark it for add to tempStore - Michel 

                // looking for <<jemison>>, <<jemison>>,<<cooper>> and <<endgate>>
                // process it if if it matches the command token << >> - Michel
                cmdMatch = cmdToken._regex.Match(line);
                if (cmdMatch.Success)
                {
                    // Loop through all the GATE patterns to match a token to the line
                    foreach (var rule in patterns[Pattern.GATE])
                    {
                        var match = rule._regex.Match(cmdMatch.Groups[1].Value);
                        if (match.Success == false || match.Length == 0)
                            continue;

                        // A gate command is matched
                        // Mark to ignore enqueueing 
                        enqueue = false;

                        // if there are lines in the tempstore, add it to the gate, with the corresponding mark
                        if (tempStore.Count > 0) gate.AddInstructions(ParseLines(ref tempStore), mark);
                        tempStore.Clear();

                        // Either swap the mark, or return the gate
                        switch (rule._type)
                        {
                            case Token.JEMISON:
                                mark = 'j';
                                break;
                            case Token.ARMSTRONG:
                                mark = 'a';
                                break;
                            case Token.COOPER:
                                mark = 'c';
                                break;
                            case Token.ENDGATE:
                                return gate;
                                break;
                            default:
                                Utility.LogError("Parsing Error: Gate rule patter detected without appropriate switch condition");
                                break;
                        }
                        // break out of the regex match loop 
                        break;
                    }
                }
                // if there is no match with a gate pattern, enqueue the line for processing later
                if (enqueue)
                {
                    tempStore.Enqueue(line);
                }

            }

            // if i reached end of lines, that means that
            // no end gate was set, which is bad
            Utility.LogError("<<endgate>> was not found when parsing <<gate>>");
            return null;
        }
        #endregion

        #region Parse Set & IfEelse Code

        Expression ParseExpression(string str)
        {
            foreach (var rule in patterns[Pattern.OPERATOR])
            {
                // attempt to find a pattern match - Michel
                var match = rule._regex.Match(str);
                if (match.Success == false || match.Length == 0)
                    continue;
                Utility.Log("Parse Expr: " + str + " matched " + rule._regex);
                // if there's a match, try to convert the match groups into expressions, then make a function expression out of them.
                Expression lhs = ParseSingleExpr(match.Groups[1].Value);
                Expression rhs = ParseSingleExpr(match.Groups[2].Value);

                return new Expression(lhs, rhs, rule._type);
            }

            // if no match is found, assume that it's just a lonely value or data key
            return ParseSingleExpr(str);
        }

        // attempt to convert lhs and rhs into Values, based on @"false", @"true", @"(\d+)", if failed, then assme it's a datakey
        Expression ParseSingleExpr(string str)
        {
            foreach (var rule in patterns[Pattern.VALUE])
            {
                // attempt to find a pattern match - Michel
                var match = rule._regex.Match(str);
                if (match.Success == false || match.Length == 0)
                    continue;

                Value val = new Value();
                
                switch (rule._type)
                {
                    case Token.TRUE:
                        val.type = VarType.Bool;
                        val.b = true;
                        break;
                    case Token.FALSE:
                        val.type = VarType.Bool;
                        val.b = false;
                        break;
                    case Token.NUM:
                        val.type = VarType.Int;
                         int.TryParse(str, out val.i);
                        break;
                }

                return new Expression(val);
            }

            // Not match was found, assume it's a dataKey
            return new Expression(str);
        }

        // Parse the line into:
        // 1. Data key (for accessing data in PersistentData)
        // 2. Assignment operataion (whether it's =, += or -=)
        // 3. And the expression to be assigned
        AssignInstruction ParseSet(string line)
        {
            // get rid of the "set" at the beginning of the line - Michel
            string[] words = line.Split(null);
            words = words.Skip(1).ToArray();
            string str = string.Join("", words); // also gets rid of whitespace - Michel
            
            foreach (var rule in patterns[Pattern.ASSIGNMENT])
            {
                // attempt to find a pattern match - Michel
                var match = rule._regex.Match(str);
                if (match.Success == false || match.Length == 0)
                    continue;
                
                // Extract key string from regex parameters - Michel
                string key = match.Groups[1].Value;

                // Extract expression string from regex parameters and parse into Expression - Michel
                Expression expr = ParseExpression(match.Groups[2].Value);

                // return the assignment instruction
                return new AssignInstruction(key, rule._type, expr);

            }

            Utility.LogError("<<set>> instruction parameter error: " + line);
            return new AssignInstruction();
        }

        // IfElseInstruction has a List of conditions(List<Expression>) and a List of "instruction blocks" (List<List<Instruction>>)
        // Logic for ParseIfElse goes like this:
        // 1. Store the first if condition into the IfElseInstruction at index 0
        // 2. Move lines from queue parameter into tempStore
        // 3. Once we find something that matches "elseif" or "else", we convert
        // the lines into an instruction list and store it at index 0, then we store
        // elseif condition at index 1 (or just the value "true" if it's an "else")
        // 4. Repeat 2 and 3 until we match and "endif", then just store the last instruction block in the last index
        // (which should match the index of the last condition) - Michel
        IfElseInstruction ParseIfElse(string initialLine, ref Queue<string> lines)
        {

            Queue<string> tempStore = new Queue<string>();
            IfElseInstruction ifelse = new IfElseInstruction();

            // Parse first line, and store it inside of the ifelse instruction
            ifelse._conditions.Add(ParseIfElseCondition(initialLine));
            
            bool enqueue = true;
            Match cmdMatch;

            // go through each line in the queue
            while (lines.Count > 0)
            {
                string line = lines.Dequeue(); // remove the next line from the master queue
                enqueue = true; // mark it for add to tempStore - Michel 

                // process it if if it matches the command token << >>, looking for <<elseif []>>, <<else>>, <<endif>>
                cmdMatch = cmdToken._regex.Match(line);
                if (cmdMatch.Success)
                {
                    // Loop through all the IFELSE patterns to match a token to the line
                    foreach (var rule in patterns[Pattern.IFELSE])
                    {
                        var match = rule._regex.Match(cmdMatch.Groups[1].Value);
                        if (match.Success == false || match.Length == 0)
                            continue;

                        // An ifelse command is matched, do not enqueue the line
                        enqueue = false;
                        // Parse the tempStore lines into instruction and store it in IfElseInstruction
                        ifelse._instrBlock.Add(ParseLines(ref tempStore));
                        tempStore.Clear();


                        // store the condition, or return IfElseInstruction
                        switch (rule._type)
                        {
                            case Token.ELSEIF:
                                ifelse._conditions.Add(ParseIfElseCondition(cmdMatch.Groups[1].Value));
                                break;
                            case Token.ELSE:
                                ifelse._conditions.Add(new Expression(new Value(true)));
                                break;
                            case Token.ENDIF:
                                return ifelse;
                                break;
                            default:
                                Utility.LogError("Parsing Error: IfElse Token detected without appropriate switch condition");
                                break;
                        }
                        // break out of the regex match loop 
                        break;
                    }
                }
                // if there is no match at all, enqueue the line for processing later - Michel
                if (enqueue)
                {
                    tempStore.Enqueue(line);
                }

            }

            // if i reached end of lines, that means that
            // no endif was set, which is bad - Michel
            Utility.LogError("<<endif>> was not found when parsing <<if []>>");
            return null;
        }

        Expression ParseIfElseCondition(string line)
        {
            // Gets rid of the first word of the line- Michel
            string[] words = line.Split(null);
            words = words.Skip(1).ToArray();
            string str = string.Join("", words); // also gets rid of whitespace - Michel
            return ParseExpression(str);
        }

        #endregion

    }




}
