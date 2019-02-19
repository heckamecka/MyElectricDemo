using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEffect : MonoBehaviour {

    [SerializeField] TextMeshProUGUI _tmpText;
    [SerializeField] private TextEffectData _effectsData;
    
    // runtime data
    private TextStyleData _styleData;

    private CharacterData[] charData;
    private TMP_MeshInfo[] _meshInfoCache;
    private TMP_TextInfo _textInfo;

    private Matrix4x4 matrix;
   
    // wave vars
    private float curveOffset;

    // Use this for initialization
    void Awake () {

        _tmpText.ForceMeshUpdate();
        _textInfo = _tmpText.textInfo;
        _meshInfoCache = _textInfo.CopyMeshInfoVertexData();
        _tmpText.renderMode = TextRenderFlags.DontRender;

        _styleData = new TextStyleData();

        charData = new CharacterData[_tmpText.text.Length];
        InitCharData();
	}
	
	// Update is called once per frame
	void Update () {
        
        // Debug Update
        //_tmpText.maxVisibleCharacters = maxVisChar;

        curveOffset = Time.time % 1.0f;

        ResetCharData();

        WaveEffect();

        ShakeEffect();

        TrailEffect();

        ModifyText();
        
    }

    /// <summary>
    /// Sets the text for the Text Mesh Pro component
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetText(string str, Color color)
    {
        // A kinda hack, turns on AutoRendering for 1 frame so that text updates properly
        // before turning it back off, TODO: I should probably post this onto the forums - Michel
        _tmpText.renderMode = TextRenderFlags.Render;

        int store = _tmpText.maxVisibleCharacters;
        _tmpText.maxVisibleCharacters = int.MaxValue;

        _tmpText.color = color;
        SetTextHelper(str);

        _tmpText.maxVisibleCharacters = store;
        yield return null;


        _tmpText.renderMode = TextRenderFlags.DontRender;
    }

    /// <summary>
    /// Sets the text for the Text Mesh Pro component
    /// </summary>
    public IEnumerator SetText(string str)
    {
        yield return SetText(str, _tmpText.color);
    }

    void SetTextHelper(string str)
    {

        // process the incoming string, and also set style data - Michel
        string output;
        RichTextParser.ParseTextStyle(str, out output, out _styleData);

        // plug text into TMP component - Michel
        _tmpText.text = output;

        // Update meshInfoCache
        _tmpText.ForceMeshUpdate();// force update so the data in TMP component updates - Michel
        _textInfo = _tmpText.textInfo;
        _meshInfoCache = _textInfo.CopyMeshInfoVertexData();

        // Update perCharData
        charData = new CharacterData[_tmpText.text.Length];
        InitCharData();
        _tmpText.maxVisibleCharacters = 99999;
    }

    void ResetCharData()
    {
        for (int i = 0; i < charData.Length; ++i)
        {
            charData[i].offset = Vector2.zero;
            charData[i].scale = 1.0f;
            charData[i].rotation = 0.0f;
        }
    }

    void InitCharData()
    {
        for(int i = 0; i < charData.Length; ++i)
        {
            charData[i] = new CharacterData();
            charData[i].offset = Vector2.zero;
            charData[i].scale = 1.0f;
            charData[i].rotation = 0.0f;
        }
    }

    // Modify Text, based on the CharacterData[] that we should have created before
    void ModifyText() {

        // A lot of this code I copy and pasted from TextMeshPro examples - Michel
        for (int i = 0; i < _tmpText.text.Length; i++)
        {
            TMP_CharacterInfo charInfo = _textInfo.characterInfo[i];

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!charInfo.isVisible)
                continue;
            
            // Get the index of the material used by the current character.
            int materialIndex = charInfo.materialReferenceIndex;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = charInfo.vertexIndex;

            // Get the cached vertices of the mesh used by this text element (character or sprite).
            Vector3[] sourceVertices = _meshInfoCache[materialIndex].vertices;

            // Determine the center point of each character at the baseline.
            Vector2 charMidBaseline = new Vector2((sourceVertices[vertexIndex + 0].x + sourceVertices[vertexIndex + 2].x) / 2, charInfo.baseLine);
            // Determine the center point of each character.
            //Vector2 charMidBaseline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            // Need to translate all 4 vertices of each quad to aligned with middle of character / baseline.
            // This is needed so the matrix TRS is applied at the origin for each character.
            Vector3 offset = charMidBaseline;

            Vector3[] destinationVertices = _textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;
            
            // Use Char data to modify the verticies
            matrix = Matrix4x4.TRS(charData[i].offset, Quaternion.Euler(0, 0, charData[i].rotation), Vector3.one * charData[i].scale);

            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;

        }



        // Push changes into meshes
        for (int i = 0; i < _textInfo.meshInfo.Length; i++)
        {
            _textInfo.meshInfo[i].mesh.vertices = _textInfo.meshInfo[i].vertices;
            _textInfo.meshInfo[i].mesh.uv = _meshInfoCache[i].uvs0;
            _textInfo.meshInfo[i].mesh.uv2 = _meshInfoCache[i].uvs2;
            _textInfo.meshInfo[i].mesh.colors32 = _meshInfoCache[i].colors32;

            _tmpText.UpdateGeometry(_textInfo.meshInfo[i].mesh, i);
        }

        // update mesh here, because modifying max visible character can cause the text to reset
        _tmpText.ForceMeshUpdate();

    }

    void WaveEffect()
    {
        // loop through all the wave tags - Michel
        for (int i = 0; i < _styleData._waveTags.Count; ++i)
        {
            int count = 0;

            // Get the start and end of the wave from textData
            for (int j = _styleData._waveTags[i]._start; j < _styleData._waveTags[i]._end; ++j)
            {
                // Loop through between 0 and character per curve, getting the percentage of the number - Michel
                float percentage = ((float)count % _effectsData.waveFrequency) / _effectsData.waveFrequency;

                // add it to the curve offset, and deduct 1.0f if it exceeds 1.0f- Michel
                float x = (percentage + curveOffset) % 1.0f;

                // use the float to get a y position from the _data wave curve - Michel
                charData[j].offset.y += _effectsData.waveCurve.Evaluate(x) * _effectsData.waveAmplitude;

                // increment count
                count++;
            }
        }
    }

    void ShakeEffect()
    {
        // loop through all the tags - Michel
        for (int i = 0; i < _styleData._shakeTags.Count; ++i)
        {
            // Get the start and end of the shake from textData
            for (int j = _styleData._shakeTags[i]._start; j < _styleData._shakeTags[i]._end; ++j)
            {
                // Modify the offset by a random point in a circle, the default shake strength, and the given intensity - Michel
                charData[j].offset += Random.insideUnitCircle * _effectsData.defaultShakeStrength * _styleData._shakeTags[i]._intensity;
            }
        }
    }
    
    void TrailEffect()
    {
        // loop through all the tags - Michel
        for (int i = 0; i < _styleData._trailTags.Count; ++i)
        {
            // Get the start and end of the shake from textData
            for (int j = _styleData._trailTags[i]._start; j < _styleData._trailTags[i]._end; ++j)
            {
                // based on that fraction, get a scale from the trail curve in effects data
                charData[j].scale *= _effectsData.trailCurve.Evaluate(_styleData._trailTags[i].fractions[j]);
            }
        }
    }
    // Stores modified data of a single character
    class CharacterData
    {
        public Vector2 offset = Vector2.zero;
        public float scale = 1.0f;
        public float rotation = 0.0f;
    }


}

