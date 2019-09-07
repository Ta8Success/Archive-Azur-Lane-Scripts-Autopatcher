using UnityEngine;

namespace Konpaku
{
    internal static class UiStyle
    {
        static UiStyle()
        {
            if (Style == null)
                Style = new GUIStyle[9];

            for (var i = 0; i < Style.Length; i++)
            {
                Style[i] = new GUIStyle
                {
                    normal =
                        {
                            textColor = Color.white
                        }
                };
            }

            Style[0].normal.background = ToTexture2D(new Color(0f, 0f, 0f, 0.65f));
            Style[0].padding = new RectOffset(4, 4, 4, 4);
            Style[0].margin = Style[0].padding;
            Style[1].normal.background = Style[0].normal.background;
            Style[1].fontSize = 36;
            Style[1].padding = new RectOffset(15, 15, 15, 15);
            Style[1].margin = Style[0].margin;
            Style[2].normal.background = Style[0].normal.background;
            Style[2].fontSize = 28;
            Style[2].padding = Style[1].padding;
            Style[2].margin = Style[0].margin;
            Style[3].normal.background = Style[0].normal.background;
            Style[3].fontSize = 36;
            Style[3].padding = Style[1].padding;
            Style[3].margin = Style[0].margin;
            Style[3].fontStyle = FontStyle.Italic;
            Style[4].normal.background = Style[0].normal.background;
            Style[4].fontSize = 28;
            Style[4].padding = Style[1].padding;
            Style[4].margin = Style[0].margin;
            Style[4].fontStyle = FontStyle.Italic;
            Style[5].normal.background = Style[0].normal.background;
            Style[5].fontSize = 36;
            Style[5].padding = Style[1].padding;
            Style[5].padding.right += 20;
            Style[5].margin = Style[0].margin;
            Style[6].normal.background = ToTexture2D(new Color(0f, 0f, 0f, 0.25f));
            Style[6].normal.textColor = new Color(255f, 255f, 255f, 0.4f);
            Style[6].fontSize = 36;
            Style[6].padding = Style[1].padding;
            Style[6].padding.right += 20;
            Style[6].margin = Style[0].margin;
            Style[7].normal.background = ToTexture2D(new Color(139f, 0f, 0f, 0.7f));
            Style[7].fontSize = 28;
            Style[7].padding = Style[1].padding;
            Style[7].margin = Style[0].margin;
            Style[7].alignment = TextAnchor.MiddleCenter;
            Style[8].normal.background = ToTexture2D(new Color(139f, 0f, 0f, 0.25f));
            Style[8].normal.textColor = new Color(255f, 255f, 255f, 0.4f);
            Style[8].fontSize = 28;
            Style[8].padding = Style[1].padding;
            Style[8].margin = Style[0].margin;
            Style[8].alignment = TextAnchor.MiddleCenter;
        }

        internal static GUIStyle[] Style { get; set; }

        private static Texture2D ToTexture2D(Color color)
        {
            var array = new Color[1];
            var texture2D = new Texture2D(1, 1);
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = color;
            }
            texture2D.SetPixels(array);
            texture2D.Apply();
            return texture2D;
        }
    }
}