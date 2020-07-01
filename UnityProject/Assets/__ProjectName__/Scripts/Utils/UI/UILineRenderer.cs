using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * http://wordpress.notargs.com/blog/blog/2015/08/30/unityugui%E3%81%A7%E6%9B%B2%E7%B7%9A%E3%82%92%E6%8F%8F%E7%94%BB%E3%81%99%E3%82%8B/
 */
 
[ExecuteInEditMode()]
[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : MonoBehaviour
{
    public Material material;
    public Color32 color = new Color32(255, 255, 255, 255);
    [Range(0.001f, 100)]
    public float width = 10;
    public List<Vector2> points;

    // 更新処理
    public void Update()
    {
        var canvasRenderer = GetComponent<CanvasRenderer>();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();


        var vertex = new UIVertex();
        vertex.color = color;

        for (int i = 0; i < points.Count - 1; i++)
        {
            // Pointを定義
            Vector2? point0;
            Vector2 point1 = points[i + 0];
            Vector2 point2 = points[i + 1];
            Vector2? point3;

            if (i - 1 < 0) point0 = null;
            else point0 = points[i - 1];

            if (i + 2 > points.Count - 1) point3 = null;
            else point3 = points[i + 2];

            // 頂点座標を計算
            var normal1 = CalcNormal(point0, point1, point2);
            var normal2 = CalcNormal(point1, point2, point3);

            vertices.Add(point1 - normal1 * width);
            uvs.Add(new Vector2(0, 0));
            colors.Add(color);

            vertices.Add(point2 - normal2 * width);
            uvs.Add(new Vector2(1, 0));
            colors.Add(color);

            vertices.Add(point2 + normal2 * width);
            uvs.Add(new Vector2(1, 1));
            colors.Add(color);

            vertices.Add(point1 + normal1 * width);
            uvs.Add(new Vector2(0, 1));
            colors.Add(color);
        }

        // 頂点を設定
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
        canvasRenderer.SetMesh(mesh);
        canvasRenderer.SetMaterial(material, Texture2D.whiteTexture);
    }

    // 線を太らせる方向を計算する
    Vector2 CalcNormal(Vector2? prev, Vector2 current, Vector2? next)
    {
        var dir = Vector2.zero;
        if (prev.HasValue) dir += prev.Value - current;
        if (next.HasValue) dir += current - next.Value;
        dir = new Vector2(-dir.y, dir.x).normalized;
        return dir;
    }
}