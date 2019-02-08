using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMountain : MonoBehaviour {

    public UnityEngine.UI.Image img;
    float satTreshhold = 0.3f; // насыщенность
    float brightTreshhold = 0.95f; // яркость
    int coreAvaregeColor = 3;
    CoreSeeker coreSeeker;

    public float size = 0.1f;
    public int gridSize = 512;

    private MeshFilter filter;
    Renderer rendererMesh;
    float[,] heightSquad;

    // Use this for initialization
    void Start () {

        coreSeeker = GetComponent<CoreSeeker>();

        coreSeeker.OnCoreFound += Generate;

        filter = GetComponent<MeshFilter>();

        rendererMesh = GetComponent<Renderer>();
    }
	

    void Generate(Texture2D texture)
    {

        // Texture2D texture = new Texture2D(1, 1);

        // texture.LoadRawTextureData(core);

        // texture.Apply();
        // Debug.Log("textureByte " + core.Length);
        Texture2D textureSet = new Texture2D(texture.width, texture.height);

       


        heightSquad = new float[gridSize+1,gridSize+1];
        int pixelwidthInSquad = texture.width / gridSize;
        int pixelHeightInSquad = texture.height / gridSize;

        coreAvaregeColor = Mathf.Min(pixelwidthInSquad, coreAvaregeColor);
        coreAvaregeColor = Mathf.Min(pixelHeightInSquad, coreAvaregeColor);
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                

                int i = (x + 1) * pixelwidthInSquad - pixelwidthInSquad / 2;
                int j = (y + 1) * pixelHeightInSquad - pixelHeightInSquad / 2;              

                Color color = GetAvarageColor(texture, j, i);

                float H = 0, S = 0, V = 0;
                Color.RGBToHSV(color,out H, out S, out V);
                //Debug.Log("color " + color);
                Debug.Log("H " + H + " S " + S + " V " + V);
                Color colorSet = new Color();
                if (S > satTreshhold && V < brightTreshhold)
                {
                    heightSquad[y, x] = (H);
                    Debug.Log("heightSquad[x, y] " + heightSquad[y, x]);

                    color = Color.HSVToRGB(H, S, V);
                }
                else
                {
                   
                   
                    heightSquad[y, x] = 0;
                    color = Color.HSVToRGB(H, 0, 255);

                }

                for (int indX = 0; indX < pixelHeightInSquad; indX++)
                {
                    for (int indY = 0; indY < pixelwidthInSquad; indY++)
                    {
                        textureSet.SetPixel(y* pixelwidthInSquad + indY, x* pixelHeightInSquad + indX, color);
                    }
                }
            }
        }


        textureSet.Apply();
        Sprite sprite = Sprite.Create(textureSet, new Rect(0, 0, textureSet.width, textureSet.height), new Vector2(.5f, .5f));

        img.sprite = sprite; //Image is a defined reference to an image component
        filter.mesh = GenerateMesh(heightSquad);
        rendererMesh.material.SetTexture("_MainTex", textureSet);

    }

        Color GetAvarageColor(Texture2D texture, int x, int y)
    {
        
        float cRed = 0, cBlue = 0, cGreen = 0;
        for (int i = 0; i < coreAvaregeColor; i++)
        {
            for (int j = 0; j < coreAvaregeColor; j++)
            {
                Color c = texture.GetPixel(x - coreAvaregeColor + i, y - coreAvaregeColor + j);
                cRed += c.r;
                cBlue += c.b;
                cGreen += c.g;
            }
        }
        cRed /= coreAvaregeColor * coreAvaregeColor;
        cBlue /= coreAvaregeColor * coreAvaregeColor;
        cGreen /= coreAvaregeColor * coreAvaregeColor;

        Color result = new Color(cRed, cGreen, cBlue);

        return result;
    }

    Mesh GenerateMesh(float[,] heightSquad)
    {
        Mesh mesh = new Mesh();

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();
        for (int x = 0; x < gridSize + 1; ++x)
        {
            for (int y = 0; y < gridSize + 1; ++y)
            {
                vertices.Add(new Vector3(-size * 0.5f + size * (x / ((float)gridSize)), heightSquad[x,y], -size * 0.5f + size * (y / ((float)gridSize))));
                normals.Add(Vector3.up);
                uvs.Add(new Vector2(x / (float)gridSize, y / (float)gridSize));
            }
        }

        var triangles = new List<int>();
        var vertCount = gridSize + 1;
        for (int i = 0; i < vertCount * vertCount - vertCount; ++i)
        {
            if ((i + 1) % vertCount == 0)
            {
                continue;
            }
            triangles.AddRange(new List<int>()
            {
                i + 1 + vertCount, i + vertCount, i,
                i, i + 1, i + vertCount + 1
            });
        }

        mesh.SetVertices(vertices);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        return mesh;
    }
}
