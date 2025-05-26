using System.Collections;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 position = new Vector2(5, 5);
    [SerializeField] private Vector2 size = new Vector2(85, 25);
    [SerializeField] private Color textColor = Color.black;
    [SerializeField] private int fontSize = 18;
    [SerializeField] bool isEnabled = true;

    private int frameCount = 0;
    private float timePassed = 0f;
    private float averageFPS = 0f;
    private float fpsDuration = 0.5f; // Duration in seconds over which to average FPS.
    private WaitForSeconds countWait;

    private IEnumerator Start()
    {
        if (isEnabled)
        {
            countWait = new WaitForSeconds(0.01f);
            GUI.depth = 2;

            while (true)
            {
                frameCount++;
                timePassed += Time.unscaledDeltaTime;

                if (timePassed >= fpsDuration)
                {
                    averageFPS = frameCount / timePassed;
                    frameCount = 0;
                    timePassed -= fpsDuration;
                }

                yield return countWait;
            }
        }
    }

    private void OnGUI()
    {
        string text = $"FPS: {Mathf.Round(averageFPS)}";
        Texture black = Texture2D.linearGrayTexture;
        Rect rect = new Rect(position.x, position.y, size.x, size.y);
        GUI.DrawTexture(rect, black, ScaleMode.StretchToFill);
        GUI.color = textColor;
        GUI.skin.label.fontSize = fontSize;
        GUI.Label(rect, text);
    }
}