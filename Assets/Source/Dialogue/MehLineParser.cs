using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

//Code that filters through YarnSpinner's output and does an additional filtration for Meh-specific syntax. Maybe one day I'll feel fancy and roll this in as a proper modification to the yarnspinner lexer / parser, but for now I'm not about that shit
public static class MehLineParser {
    const string inlinePattern = @"{{([^{]*?)}}"; //Basic rule that finds and captures the contents of double brace enclosures. Does shortest match, and avoids that nasty third inward brace
    const string madlibsPattern = @"\(\(([^(]*?)\)\)"; // same as above, except for double parenthesis (( )) - Michel
    private enum ReplacementType {InlineCommand, MadlibsCommand}

    public static void ParseInlineCommands (string lineContent, out string printableText, out Dictionary<int, List<Command>> commandsByIndex) {
        printableText = "";
        commandsByIndex = new Dictionary<int, List<Command>>();

        //Go through the line, and find / store / remove the first regex match. Continue until you can't find a match.
        string remainingToProcess = lineContent;
        int indexOffset = 0;
        var match = Regex.Match("", "");
        ReplacementType replacementType;
        while(tryFindMatch(remainingToProcess, out match, out replacementType)) {
            var indexOfSource = indexOffset + match.Index - 1;

            // add a new command if there isn't 
            if (!commandsByIndex.ContainsKey(indexOfSource))
                commandsByIndex.Add(indexOfSource, new List<Command>());

            var command = new Command();
            switch (replacementType) {
                //If I find an {{inline command}}, add a command to the command buffer with its exact contents:
                case ReplacementType.InlineCommand:  command.text = match.Groups[1].Value; break;
                // If I find a ((madlibs command)), add a command, with the "madlibs " command signature and it's exact contents - Michel
                case ReplacementType.MadlibsCommand: command.text = "madlibs " + match.Groups[1].Value; break;
            }

            commandsByIndex[indexOfSource].Add(command);
            
            printableText += remainingToProcess.Substring(0, match.Index);

            remainingToProcess = remainingToProcess.Substring(match.Index + match.Captures[0].Length);
            indexOffset += match.Index;
        }
        printableText += remainingToProcess;
	}
    
    private static bool tryFindMatch(string target, out Match match, out ReplacementType replacementType) {
        if ((match = Regex.Match(target, inlinePattern)).Success) {
            replacementType = ReplacementType.InlineCommand;
            return true;
        }
        else if ((match = Regex.Match(target, madlibsPattern)).Success)
        {
            replacementType = ReplacementType.MadlibsCommand;
            return true;
        }
        replacementType = (ReplacementType) 0;
        return false;
    }

}
