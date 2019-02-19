using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class RichTextParser{

    const string RICHTEXT_PATTERN = @"<([^<]*?)>"; // pattern for matching style tags - Michel
    const string ENDTAG_PATTERN = @"\/(.*)";


    enum TagType { CUSTOM, REPLACE }

    public static void ParseTextStyle(string lineContent, out string outputText, out TextStyleData data)
    {
        outputText = "";
        data = new TextStyleData();
        
        var tagStore = new Stack<string>();
        var indexStore = new Stack<int>();

        //Go through the line, and find all rich text tags encased in < > and remove them for processing - Michel
        string remainingToProcess = lineContent;
        int indexOffset = 0;
        var match = Regex.Match("", "");
        while ((match = Regex.Match(remainingToProcess, RICHTEXT_PATTERN)).Success)
        {
            var indexOfSource = indexOffset + match.Index - 1;

            // store everything in there to be processed- Michel
            tagStore.Push(match.Groups[1].Value);
            indexStore.Push(indexOfSource +1);
            
            // add the text before the match to printable text- Michel
            outputText += remainingToProcess.Substring(0, match.Index);

            // remove text mark up from string to parse- Michel
            remainingToProcess = remainingToProcess.Substring(match.Index + match.Groups[0].Length);
            indexOffset += match.Index;
        }

        outputText += remainingToProcess;
        
        // generate dictionary to keep track of the end indices of custom mark ups - Michel
        var customTagStacks = new Dictionary<EffectsTag, Stack<int>>();
        foreach (EffectsTag tag in System.Enum.GetValues(typeof(EffectsTag)))
        {
            customTagStacks.Add(tag, new Stack<int>());
        }

        // Process all tags, starting from the back so that they don't affect indexing - Michel
        while (tagStore.Count > 0)
        {
            string process = tagStore.Pop();
            int index = indexStore.Pop();
            bool endTag = false;

            // if it's an end tag, remove the '/' from the process string, and set process as just the string 
            if((match = Regex.Match(process, ENDTAG_PATTERN)).Success)
            {
                endTag = true;
                process = match.Groups[1].Value;
            }
            bool skip = false;

            // STYLE TAG, replace it and reinsert it
            // --------------------------------------------------------------------------
            #region Style Tag
            foreach(string s in TextStyleData.styleTagPatterns)
            {
                if(Regex.Match(process, s).Success){
                    if (endTag)
                    {
                        outputText = outputText.Insert(index , "</style>");
                    }
                    else
                    {
                        outputText = outputText.Insert(index, "<style=" + process + ">");
                    }
                    // skip to the next tag
                    skip = true;
                }
            }
            if (skip) continue;
            #endregion


            // CUSTOM TAG try to create a MarkUp with start and end tags and insert it into text style data
            // --------------------------------------------------------------------------
            #region custom tag
            // split string by spaces, for mark ups that have parameters - Michel
            string[] w = process.Split(null);
            // check if it's a custom tag
            if (TextStyleData.customTagPatterns.ContainsKey(w[0]))
            {
                EffectsTag tag = TextStyleData.customTagPatterns[w[0]];
                if (endTag) // Add it the index to the tag marked stack and wait for a start tag - Michel
                {
                    customTagStacks[tag].Push(index); 
                }
                else
                {
                    if (customTagStacks[tag].Count != 0) // Add tag with the start index, end index, tag, and list of parameter
                    {
                        data.AddTags(index, customTagStacks[tag].Pop(), tag, w);
                    }
                    else
                    {
                        Utility.LogError("Found "+ tag + " startintg tag without an end tag");
                    }
                }
                // skip to the next tag
                skip = true;
            }
            if (skip) continue;
            #endregion

            // STANDART TAG just reinsert it, remembering to add '/' for end tags because we removed it earlier - Michel
            // --------------------------------------------------------------------------
            if (endTag) process = "/" + process;

            outputText = outputText.Insert(index, "<"+process+">");

        }

        //Error checking for if there's dangling tags without closing statements - Michel
        foreach(var stack in customTagStacks)
        {
            if (stack.Value.Count > 0)
            {
                Debug.LogError("Found a " + stack.Key + " ending tag without a starting tag");
            }
        }

    }


}
