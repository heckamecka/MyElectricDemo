using System.Collections;
using System.Collections.Generic;


public enum EffectsTag { WAVE, SHAKE, TRAIL }

// Stores data for custom effects
public class TextStyleData {
    
    // New Custom Tags
    static Dictionary<string, EffectsTag> _customTagPatterns;
    static public Dictionary<string, EffectsTag> customTagPatterns
    {
        get
        {
            if (_customTagPatterns != null) return _customTagPatterns;
            else
            {
                _customTagPatterns = new Dictionary<string, EffectsTag>();

                // ADD NEW CUSTOM PATTERNS HERE - Michel
                _customTagPatterns.Add(@"wavy", EffectsTag.WAVE);
                _customTagPatterns.Add(@"shake", EffectsTag.SHAKE);
                _customTagPatterns.Add(@"trail", EffectsTag.TRAIL);

                return _customTagPatterns;
            }
        }
    }

    static List<string> _styleTagPatterns;
    static public List<string> styleTagPatterns
    {
        get
        {
            if (_styleTagPatterns != null) return _styleTagPatterns;
            else
            {
                _styleTagPatterns = new List<string>();

                // ADD NEW STYLE PATTERNS HERE - Michel
                _styleTagPatterns.Add(@"(big)");
                _styleTagPatterns.Add(@"(small)");

                return _styleTagPatterns;
            }
        }
    }

    public List<MarkUp> _waveTags;
    public List<ShakeMarkUp> _shakeTags;
    public List<TrailMarkUp> _trailTags;

    public TextStyleData(bool debug = false)
    {
        _waveTags = new List<MarkUp>();
        _shakeTags = new List<ShakeMarkUp>();
        _trailTags = new List<TrailMarkUp>();

        if (debug)
        {
            _waveTags.Add(new MarkUp(3, 15));
            _waveTags.Add(new MarkUp(30, 40));
            
            _shakeTags.Add(new ShakeMarkUp(50, 60, 1.0f));
            _shakeTags.Add(new ShakeMarkUp(70, 80, 2.0f));
        }
    }

    public void AddTags(int start, int end, EffectsTag tag, string[] parameters)
    {
        switch (tag)
        {
            case EffectsTag.WAVE:

                _waveTags.Add(new MarkUp(start,end));
                break;

            case EffectsTag.SHAKE:

                float f = 1.0f;
                if (parameters.Length > 1)
                {
                    f = ParseFloat(parameters[1]);
                }
                _shakeTags.Add(new ShakeMarkUp(start, end, f));
                break;

            case EffectsTag.TRAIL:

                _trailTags.Add(new TrailMarkUp(start, end));
                break;
        }
    }

    // Parsing
    float ParseFloat(string str)
    {
        float f;
        if (!float.TryParse(str, out f)) Utility.LogError("Text Styling Parsing Error: Unable to convert " + str + " into a float.");
        return f;
    }

}
