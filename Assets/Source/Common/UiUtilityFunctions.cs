using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public static class UiUtilityFunctions
{
    public static float UnitAsPercentageOfScreenWidth(this Camera cam, float widthPercent) {
        float percentOffset = cam.ViewportToWorldPoint(Vector3.right * widthPercent).x;
        float viewportOrigin   = cam.ViewportToWorldPoint(Vector3.zero).x;

        return percentOffset - viewportOrigin;
    }

    public static float UnitAsPercentageOfCanvasWidth(this Canvas canvas, float widthPercent)
    {
        Vector2 canvasRect = canvas.GetComponent<RectTransform>().sizeDelta;
        return widthPercent * canvasRect.x;
    }

    public static Vector3 WorldToConstantSizeCanvasSpace(this Canvas canvas, Camera cam, Vector3 worldPoint)
    {
        Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;
        Vector3 offset = canvas.transform.position - (Vector3)(canvasSize * 0.5f); //Normalize canvas (-1,1) to viewport (0, 1)
        Vector3 viewport = cam.WorldToViewportPoint(worldPoint);
        return offset + new Vector3(viewport.x * canvasSize.x, viewport.y * canvasSize.y, worldPoint.z);
    }

    public static Vector3 WorldToScreenScaledCanvasSpace(this Canvas canvas, Camera cam, Vector3 worldPoint)
    {
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        Vector2 actualScreenSize = new Vector2(Screen.width, Screen.height);

        //Hypothesis: If actual size == reference resolution, then canvas point = world point / pixels per unit
        //If actual size != reference resolution, then canvas point = (world point / pixels per unit) * (actualScreenSize / referenceRes)
        return (worldPoint * scaler.referencePixelsPerUnit) * (actualScreenSize.x / scaler.referenceResolution.x);
    }
}