using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MapScripts
{
    public class GraphUIRenderer : MonoBehaviour
    {
        [Header("Layout")]
        public float xSpacing = 180f;
        public float ySpacing = 90f;
        public float nodeSize = 40f;
        public Vector2 margin = new Vector2(100f, 100f);
        public Vector2 startPoint = new Vector2(-500f, 0f);

        [Header("Button Colors and Font")]
        public Color rootColor = new Color(0.5f, 0.75f, 1f);
        public Color areaColor = Color.gray;
        public Color endColor = new Color(1f, 0.5f, 0.5f);
        public Color edgeColor = Color.black;
        public Font font = null;
        public int fontSize = 20;
    
        // Button Prefab and Edge Prefab
        public GameObject buttonPrefab;
        public Image edgePrefab;

        // References
        private Dictionary<string, RectTransform> nodeRects = new();
        private GenGraph graphRef;
        private GenNode playerPosition;
    
        public void DrawGraph(GenGraph graph)
        {
            RectTransform canvasRect = GetComponent<RectTransform>();
            graphRef = graph;

            // Group nodes by depth
            Dictionary<int, List<GenNode>> nodesByDepth = new();
            foreach (var node in graph.Nodes.Values)
            {
                if (!nodesByDepth.TryGetValue(node.Depth, out var list))
                {
                    list = new List<GenNode>();
                    nodesByDepth[node.Depth] = list;
                }
                list.Add(node);
            }

            // Draw nodes
            foreach (var kvp in nodesByDepth)
            {
                int depth = kvp.Key;
                List<GenNode> nodes = kvp.Value;
            
                float x = margin.x + depth * xSpacing;
                float startY = canvasRect.sizeDelta.y/4 - (nodes.Count-1)*ySpacing/2;
            
                for (int i = 0; i < nodes.Count; i++)
                {
                    GenNode node = nodes[i];
                
                    float y = startY + i * ySpacing;
                    Vector2 pos = new Vector2(x,y);

                    RectTransform rect = CreateNode(node, pos+startPoint);
                    nodeRects[node.Id] = rect;
                }
            }

            // Draw edges
            foreach (GenEdge edge in graph.Edges)
            {
                RectTransform from = nodeRects[edge.From];
                RectTransform to = nodeRects[edge.To];

                CreateEdge(from, to);
            }
        }

        /// <summary>
        /// Create Node creates an instance of the buttonPrefab at a position (that aligns with the algorithm above).
        /// This buttonPrefab will be passed all node information corresponding to it from the SelectorMapGenerator
        /// class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        RectTransform CreateNode(GenNode node, Vector2 position)
        {
            GameObject go = Instantiate(buttonPrefab, transform, false);

            // image 
            Image img = go.GetComponent<Image>();
            img.color = node.Type switch
            {
                "root" => rootColor,
                "end" => endColor,
                _ => areaColor
            };

            // determine position
            RectTransform rect = img.rectTransform;
            rect.sizeDelta = new Vector2(nodeSize, nodeSize);
            rect.anchoredPosition = position;

            // label
            GameObject label = new GameObject("Label", typeof(Text));
            label.transform.SetParent(go.transform, false);
            Text txt = label.GetComponent<Text>();
            txt.text = node.Id;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = font;
            txt.fontSize = fontSize;
            txt.color = Color.black;
            txt.rectTransform.sizeDelta = rect.sizeDelta;
        
            // pass node information
            // TODO: use adjacency matrix to tell SelectorButton code that the node is adjacent and therefore selectable
            SelectorButton sb = go.GetComponent<SelectorButton>();

            sb.nodeId = node.Id;
            sb.mapGenerator = GetComponent<SelectorMapGenerator>();

            if (node.isPlayerPosition)
            {
                playerPosition = node;
                return rect;
            }

            foreach (GenEdge edge in graphRef.Out[playerPosition.Id])
            {
                if (edge.To == node.Id)
                {
                    sb.isPlayerAdjacent = true;
                }
            }

            return rect;
        }

        /// <summary>
        /// Create Edge creates a new CanvasUI element named "Edge" that will take two Rect Transforms
        /// in order to "draw" a line between them using itself
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void CreateEdge(RectTransform from, RectTransform to)
        {
            GameObject go = new GameObject("Edge", typeof(Image));
            go.transform.SetParent(transform, false);

            Image img = go.GetComponent<Image>();
            img.color = edgeColor;

            RectTransform rect = img.rectTransform;

            Vector2 a = from.anchoredPosition;
            Vector2 b = to.anchoredPosition;
            Vector2 dir = b - a;

            float length = dir.magnitude;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            rect.sizeDelta = new Vector2(length - nodeSize, 3f);
            rect.anchoredPosition = a + dir * 0.5f;
            rect.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}