using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MBaske
{
    public class Graph : MonoBehaviour
    {
        public class Line
        {
            public bool IsFlat { get; set; }
            public bool ShowValue { get; private set; }

            public Color Color { get; private set; }
            public string RGB { get; private set; }
            public string RGB_Avg { get; private set; }

            public StatsBuffer Buffer { get; private set; }
            public bool HasLabel => Buffer.Name.Length > 0;
            public bool IsDrawable => Buffer.IsEnabled && Buffer.Length > 1;

            public Line(StatsBuffer buffer, Color color, bool showValue)
            {
                Buffer = buffer;
                Color = color;
                RGB = "#" + ColorUtility.ToHtmlStringRGB(color);
                // Dimmed for average value.
                RGB_Avg = "#" + ColorUtility.ToHtmlStringRGB(color * 0.5f);
                ShowValue = showValue;
            }
        }

        public string Name = "Graph";
        public Vector2 Size = new Vector2(636, 60);
        public bool FitBounds { get; set; }

        private readonly List<Line> m_Lines = new List<Line>();
        private readonly StringBuilder m_SB = new StringBuilder();
        private const string c_Format = "0.###"; // TODO overflow

        private float m_MinVal = Mathf.Infinity;
        private float m_MaxVal = Mathf.NegativeInfinity;

        private Rect m_DrawArea;
        private Text m_TextDescr;
        private Text m_TextMin;
        private Text m_TextMax;
        private Text m_TextVal;

        private const int c_DescrWidth = 200;
        private const int c_MinMaxWidth = 42;
        private const int c_LineHeight = 14;

        private float m_yCenter;
        private float m_yScale;

        public Graph Add(StatsBuffer data, Color color, bool showValue = false)
        {
            m_Lines.Add(new Line(data, color, showValue));
            UpdateLayout();
            return this;
        }

        public void UpdateLayout()
        {
            var nLabels = m_Lines.Where(x => x.HasLabel).Count();
            var reqHeightLabels = (nLabels + 1) * c_LineHeight + 1;
            var nValues = m_Lines.Where(x => x.ShowValue).Count();
            var reqHeightValues = (nValues + 1) * 2 * c_LineHeight + 3;
            var reqHeight = Mathf.Max(reqHeightLabels, reqHeightValues);
            Size.y = Mathf.Max(Size.y, reqHeight);

            // 5px margin between values and lines.
            m_DrawArea = new Rect(c_DescrWidth, 0, Size.x - c_MinMaxWidth - c_DescrWidth - 5, Size.y);
            m_yCenter = m_DrawArea.height * -0.5f;

            var rect = this.GetComponent<RectTransform>();
            rect.sizeDelta = Size;

            // Background padding 2px above and below.
            rect = transform.GetChild(0).GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Size.x, Size.y + 4);

            m_TextDescr = transform.GetChild(1).GetComponent<Text>();
            rect = m_TextDescr.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(c_DescrWidth, Size.y);

            m_TextMin = transform.GetChild(2).GetComponent<Text>();
            rect = m_TextMin.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(c_MinMaxWidth, Size.y);

            m_TextMax = transform.GetChild(3).GetComponent<Text>();
            rect = m_TextMax.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(c_MinMaxWidth, Size.y);

            m_TextVal = transform.GetChild(4).GetComponent<Text>();
            rect = m_TextVal.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(c_MinMaxWidth, Size.y);

            UpdateText();
        }

        public void ResetBounds()
        {
            m_MinVal = Mathf.Infinity;
            m_MaxVal = Mathf.NegativeInfinity;
        }

        private void UpdateText()
        {
            m_SB.Clear();
            m_SB.AppendLine($"<color=#ffffff>{Name.ToUpper()}</color>");
            foreach (Line line in m_Lines)
            {
                if (line.IsDrawable && line.HasLabel)
                {
                    m_SB.AppendLine($"<color={line.RGB}>- {line.Buffer.Name}</color>");
                }
            }
            m_TextDescr.text = m_SB.ToString();
            m_TextMin.text = "";
            m_TextMax.text = "";
            m_TextVal.text = "";
        }

        private void OnRenderObject()
        {
            UpdateText();

            if (FitBounds)
            {
                ResetBounds();
            }

            float minTime = Mathf.Infinity;
            float maxTime = Mathf.NegativeInfinity;

            bool isFlatGraph = true;
            bool drawGraph = false;

            foreach (Line line in m_Lines)
            {
                if (line.IsDrawable)
                {
                    drawGraph = true;

                    float min = line.Buffer.Min;
                    float max = line.Buffer.Max;

                    m_MinVal = Mathf.Min(m_MinVal, min);
                    m_MaxVal = Mathf.Max(m_MaxVal, max);
                    minTime = Mathf.Min(minTime, line.Buffer.Start);
                    maxTime = Mathf.Max(maxTime, line.Buffer.End);

                    line.IsFlat = min == max;
                    isFlatGraph = isFlatGraph && line.IsFlat;
                }
            }

            if (drawGraph)
            {
                m_yScale = 0;

                if (!isFlatGraph)
                {
                    m_TextMin.text = m_MinVal.ToString(c_Format);
                    m_TextMax.text = m_MaxVal.ToString(c_Format);
                    m_yScale = m_DrawArea.height / (m_MaxVal - m_MinVal);
                }

                m_SB.Clear();
                CreateLineMaterial();
                lineMaterial.SetPass(0);
                GL.PushMatrix();
                GL.MultMatrix(transform.localToWorldMatrix);
                GL.Begin(GL.LINES);

                foreach (Line line in m_Lines)
                {
                    if (line.IsDrawable)
                    {
                        GL.Color(line.Color);
                        string sVal = line.Buffer.Current.ToString(c_Format);

                        if (line.IsFlat)
                        {
                            float y = isFlatGraph ? m_yCenter
                                : (line.Buffer.Current - m_MinVal) * m_yScale - m_DrawArea.height;
                            GL.Vertex3(m_DrawArea.xMin, y, 0);
                            GL.Vertex3(m_DrawArea.xMax, y, 0);

                            if (line.ShowValue)
                            {
                                m_SB.AppendLine($"<color={line.RGB}>{sVal}</color>");
                            }
                        }
                        else
                        {
                            TimedQueueItem<float> previous = default;
                            foreach (var current in line.Buffer.Items())
                            {
                                if (previous.Time > 0)
                                {
                                    GL.Vertex3(
                                        MapTime(minTime, maxTime, previous.Time),
                                        (previous.Value - m_MinVal) * m_yScale - m_DrawArea.height,
                                        0);
                                    GL.Vertex3(
                                        MapTime(minTime, maxTime, current.Time),
                                        (current.Value - m_MinVal) * m_yScale - m_DrawArea.height,
                                        0);
                                }
                                previous = current;
                            }

                            if (line.ShowValue)
                            {
                                string sAvg = line.Buffer.Average.ToString(c_Format);
                                m_SB.AppendLine($"<color={line.RGB}>{sVal}</color>");
                                m_SB.AppendLine($"<color={line.RGB_Avg}>{sAvg}</color>");
                            }
                        }
                    }
                }
                GL.End();
                GL.PopMatrix();
                m_TextVal.text = m_SB.ToString();
            }
        }

        private float MapTime(float min, float max, float t)
        {
            return Mathf.Lerp(m_DrawArea.xMin, m_DrawArea.xMax, Mathf.InverseLerp(min, max, t));
        }

        private static Material lineMaterial;
        private static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                // Turn on alpha blending.
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off.
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes.
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }
    }
}