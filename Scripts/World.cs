using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class World : MonoBehaviour {
    public byte[,,] data;
    public int worldX = 16;
    public int worldY = 16;
    public int worldZ = 16;

    public GameObject chunk;
    public Chunk[,,] chunks;
    public int chunkSize = 16;

    public static World Instance;
    public GameObject player;

    GameObject prefab_Health;
    public GameObject TestCube;

    private int scoreCount;
    public Text countText;
    public Text timerText;
    private float startTime;

    void Awake() {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        startTime = Time.time;
        player = GameObject.Find("Player");
        if (player != null) {
            scoreCount = 0;
            SetCountText();
            prefab_Health = Resources.Load("Health") as GameObject;
        }
        data = new byte[worldX, worldY, worldZ];

        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
                stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;
                int dirt = PerlinNoise(x, 100, z, 50, 2, 0) + 1; //+1 is to make sure minimum grass height is 1

                for (int y = 0; y < worldY; y++)
                {
                    if (y <= stone)
                    {
                        data[x, y, z] = 1;
                    }
                    else if (y <= dirt + stone)
                    {
                        data[x, y, z] = 2;
                    }
                }
            }
        }

        chunks = new Chunk[Mathf.FloorToInt(worldX / chunkSize),
    Mathf.FloorToInt(worldY / chunkSize),
    Mathf.FloorToInt(worldZ / chunkSize)];


    }

    int PerlinNoise(int x, int y, int z, float scale, float height, float power)
    {
        float rValue;
        rValue = Noise.Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
        rValue *= height;

        if (power != 0)
        {
            rValue = Mathf.Pow(rValue, power);
        }

        return (int)rValue;
    }

    public byte Block(int x, int y, int z)
    {

        if (x >= worldX || x < 0 || y >= worldY || y < 0 || z >= worldZ || z < 0)
        {
            return (byte)1;
        }

        return data[x, y, z];
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            float t = Time.time - startTime;

            string minutes = ((int)t / 60).ToString("00");
            string seconds = (t % 60).ToString("00");

            timerText.text = minutes + ":" + seconds;
        }
    }
    public void GenColumn(int x, int z)
    {

        for (int y = 0; y < chunks.GetLength(1); y++)
        {
            GameObject newChunk = Instantiate(chunk, new Vector3(x * chunkSize - 0.5f,
            y * chunkSize + 0.5f, z * chunkSize - 0.5f), new Quaternion(0, 0, 0, 0)) as GameObject;

            chunks[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
            chunks[x, y, z].worldGO = gameObject;
            chunks[x, y, z].chunkSize = chunkSize;
            chunks[x, y, z].chunkX = x * chunkSize;
            chunks[x, y, z].chunkY = y * chunkSize;
            chunks[x, y, z].chunkZ = z * chunkSize;

        }
    }
    public void UnloadColumn(int x, int z)
    {
        for (int y = 0; y < chunks.GetLength(1); y++){
            Object.Destroy(chunks[x, y, z].gameObject);
        }
    }
    public int GetHeight(Vector3 playerPos, int _yOffset = 0) {
        int _x = Mathf.RoundToInt( Mathf.Floor(playerPos.x / this.chunkSize));
        int _y = Mathf.RoundToInt(Mathf.Floor((playerPos.y-1) / this.chunkSize));
        int _z = Mathf.RoundToInt (Mathf.Floor(playerPos.z / this.chunkSize));
        //print("Ik ben nu in chunk" + new Vector3(_x, (_y - _yOffset), _z));

        if(this.chunks!=null &&
            _x < this.chunks.GetLength(0) &&
            _x >= 0 &&
            (_y - _yOffset) < this.chunks.GetLength(1) &&
            (_y - _yOffset) >= 0 &&
            _z < this.chunks.GetLength(2) &&
            _z >= 0) 
            {
            Chunk a = this.chunks[_x, (_y - _yOffset), _z];
            if (a != null) {
                Vector3 LocalPos = a.transform.InverseTransformPoint(playerPos);
                int _hLocal = a.GetHeight(LocalPos.x, LocalPos.z);
                if (_hLocal != -2) {
                    int _h = ((_y - _yOffset) * this.chunkSize) + _hLocal;
                    return _h;        
                }
                else return this.GetHeight(playerPos, (_yOffset + 1));
                }
            }
        return -1;
    }
    //Does damage on instantiated object(enemy) and checks if health = equal to zero & Destroys the enemy if health = 0(BaseEnemy script)
    //also substracts 1 from the counter(From script BasePlayer) & Adds 1 point to your score &Spawns healt if the random generated number is lower than x.
    public void EnemyDestroy(BaseEnemy tokill) {
        Vector3 EnemyPos = tokill.gameObject.transform.position;
        Destroy(tokill.gameObject);
        scoreCount++;
        SetCountText();
        if (Random.Range(0f, 1f) < .15f) {
            GameObject Health = Instantiate(prefab_Health) as GameObject;
            Health.transform.position = EnemyPos;
            BasePlayer bp = player.GetComponent<BasePlayer>();
            bp.counter--;
        }
    }
    //Set score to Score: Number
    void SetCountText() {
        if (countText != null) {
            countText.text = "" + scoreCount;
        }
    }
}
