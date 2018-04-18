using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonGenerator : MonoBehaviour
{
    //This first list contains every vertex of the mesh that we are going to render
    public List<Vector3> newVertices = new List<Vector3>();
    //The Triangles tell Unity how to build each section of the mesh joining the vertices
    public List<int> newTriangles = new List<int>();
    //The UV list tells Unity how the texture is aligned on each polygon
    public List<Vector2> newUV = new List<Vector2>();
    //A mesh is made up for the vertices,Triangles and UV's we define, after we make them up we'll save them as this mesh
    private Mesh mesh;
    // Use this for initialization

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(0, 0);
    private Vector2 tGrass = new Vector2(0, 1);
    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();
    private int colCount;

    private MeshCollider col;

    private int squareCount;
    public byte[,] blocks;
    public bool update = false;

    void Start()
    {
        col = GetComponent<MeshCollider>();
        mesh = GetComponent<MeshFilter>().mesh;
        GenTerrain();
        BuildMesh();
        UpdateMesh();

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

    }

    void UpdateMesh() {
        Mesh newMesh = new Mesh();
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh = newMesh;

        colVertices.Clear();
        colTriangles.Clear();
        colCount = 0;

        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        squareCount = 0;
        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();

    }

    void Update(){
        if (update){
            BuildMesh();
            UpdateMesh();
            update = false;
        }
    }

    void GenCollider(int x, int y){
        //topside
        if (Block(x, y + 1) == 0){
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 0));
            colVertices.Add(new Vector3(x, y, 0));

            ColliderTriangles();
            colCount++;
        }

        //botside
        if (Block(x, y - 1) == 0){
            colVertices.Add(new Vector3(x, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y - 1, 1));
            colVertices.Add(new Vector3(x, y - 1, 1));

            ColliderTriangles();
            colCount++;
        }

        //leftside
        if (Block(x-1, y) == 0){
            colVertices.Add(new Vector3(x, y - 1, 1));
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x, y, 0));
            colVertices.Add(new Vector3(x, y - 1, 0));

            ColliderTriangles();
            colCount++;
        }

        //rightside
        if (Block(x+1, y) == 0){
            colVertices.Add(new Vector3(x + 1, y, 1));
            colVertices.Add(new Vector3(x + 1, y - 1, 1));
            colVertices.Add(new Vector3(x + 1, y - 1, 0));
            colVertices.Add(new Vector3(x + 1, y, 0));

            ColliderTriangles();
            colCount++;
        }
    }

    void ColliderTriangles(){

        colTriangles.Add(colCount * 4);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 3);
        colTriangles.Add((colCount * 4) + 1);
        colTriangles.Add((colCount * 4) + 2);
        colTriangles.Add((colCount * 4) + 3);
    }

    void GenSquare(int x, int y, Vector2 texture)
    {
        newVertices.Add(new Vector3(x, y, 0));
        newVertices.Add(new Vector3(x + 1, y, 0));
        newVertices.Add(new Vector3(x + 1, y - 1, 0));
        newVertices.Add(new Vector3(x, y - 1, 0));

        newTriangles.Add(squareCount * 4);
        newTriangles.Add((squareCount * 4) + 1);
        newTriangles.Add((squareCount * 4) + 3);
        newTriangles.Add((squareCount * 4) + 1);
        newTriangles.Add((squareCount * 4) + 2);
        newTriangles.Add((squareCount * 4) + 3);

       newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
       newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
       newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
       newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));

        squareCount++;
    }

    void GenTerrain(){
        blocks = new byte[96, 128];

        for (int px = 0; px < blocks.GetLength(0); px++){
            //80=scale, 15=magnitude(height), 0=lowest, 1=so no change is applied
            int stone = Noise(px, 0, 80, 15, 1);
            stone += Noise(px, 0, 50, 30, 1);
            stone += Noise(px, 0, 10, 10, 1);
            stone += 75;

            print(stone);

            int dirt = Noise(px, 0, 100, 35, 1);
            dirt += Noise(px, 0, 50, 30, 1);
            dirt += 75;

            for (int py = 0; py < blocks.GetLength(1); py++){
                if (py < stone){
                    blocks[px, py] = 1;

                //Add dirt in random places
                if(Noise(px, py, 12, 16, 1) > 10){
                        blocks[px, py] = 2;
                    }
                    //Remove dirt and rock to make caves in certain places
                    if (Noise(px, py * 2, 16, 14, 1) > 10){
                        blocks[px, py] = 0;
                    }
                } else if (py < dirt){
                    blocks[px, py] = 2;
                }
            }
        }
    }

    int Noise (int x, int y, float scale, float mag, float exp){
        return (int)(Mathf.Pow((Mathf.PerlinNoise(x / scale, y / scale) * mag), (exp)));
    }

    byte Block(int x, int y)
    {

        if (x == -1 || x == blocks.GetLength(0) || y == -1 || y == blocks.GetLength(1))
        {
            return (byte)1;
        }

        return blocks[x, y];
    }

    void BuildMesh()
    {
        for (int px = 0; px < blocks.GetLength(0); px++)
        {
            for (int py = 0; py < blocks.GetLength(1); py++)
            {

                //If the block is not air
                if (blocks[px, py] != 0)
                {
                    //This wil apply the collider to every block other than air
                    GenCollider(px, py);

                    if (blocks[px, py] == 1)
                    {
                        GenSquare(px, py, tStone);
                    }
                    else if (blocks[px, py] == 2)
                    {
                        GenSquare(px, py, tGrass);
                    }
                }
            }
        }
    }
}