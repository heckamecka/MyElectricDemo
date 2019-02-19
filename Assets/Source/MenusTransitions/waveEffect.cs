using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waveEffect : MonoBehaviour {

    private Text thisText;

    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
    public Color color5;
    Color oldColor;
    Color newColor;

    float lastTimeChanged = 0f;
    float howOftenToChange = 0.5f;
    int colorCounter = 1;

    public float frequency = 8.0f;
    public float magnitude = 1.5f;
    public float offset;

    private Vector3 m_startPos;

    private void Start()
    {
        thisText = gameObject.GetComponent<Text>();

        m_startPos = transform.localPosition;

        colorCounter = 1;
        thisText.color = color1;
    }

    private void Update()
    {
        float y = Mathf.Sin((offset + Time.time) * frequency) * magnitude;
        transform.localPosition = m_startPos + new Vector3(0.0f, y, 0.0f);

        if (colorCounter > 5)
        {
            colorCounter = 1;
        }
        thisText.color = Color.Lerp(oldColor, newColor, Mathf.PingPong(Time.time, 1));
        if (colorCounter == 1)
        {
            oldColor = color1;
            newColor = color2;
        }
        if (colorCounter == 2)
        {
            oldColor = color2;
            newColor = color3;
        }
        if (colorCounter == 3)
        {
            oldColor = color3;
            newColor = color4;
        }
        if (colorCounter == 4)
        {
            oldColor = color4;
            newColor = color5;
        }
        if (colorCounter == 5)
        {
            oldColor = color5;
            newColor = color1;
        }
        if (Time.time - lastTimeChanged > howOftenToChange)
        {
            colorCounter = (colorCounter + 1);
            lastTimeChanged = Time.time;
        }
    }
}
