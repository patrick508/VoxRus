using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ModifyTerrain : MonoBehaviour {
    World world;
    GameObject cameraGO;
    public Camera myCam;
    public Shooting Player;
    public Image ActiveWapImage;
    public GameObject PlayerRef;

    void Start(){
        PlayerRef = GameObject.FindGameObjectWithTag("Player");
        world = gameObject.GetComponent<World>();
        cameraGO = GameObject.FindGameObjectWithTag("MainCamera");
    }

    //Checks playerposition and if within range spawns the column(chunks) if out of range deletes column
    public void LoadChunks(Vector3 playerPos, float distToLoad, float distToUnload){
        for (int x = 0; x < world.chunks.GetLength(0); x++){
            for (int z = 0; z < world.chunks.GetLength(2); z++){

                float dist = Vector2.Distance(new Vector2(x * world.chunkSize,
                z * world.chunkSize), new Vector2(playerPos.x, playerPos.z));
                if (dist < distToLoad){
                    if (world.chunks[x, 0, z] == null){
                        world.GenColumn(x, z);
                    }
                }
                else if (dist > distToUnload){
                    if (world.chunks[x, 0, z] != null){

                        world.UnloadColumn(x, z);
                    }
                }

            }
        }
    }
    //Weapon switching modes
    enum CheckWeapon {
        BULLETS = 0,
        BLOCKS = 1
    };
    CheckWeapon CheckWeap = CheckWeapon.BULLETS;
    void Update() {
        //Fades out the active image by x amount every frame (lowers Alpha)

        Vector3 CamPos = new Vector3(Camera.main.transform.position.x -200f, Camera.main.transform.position.y - 200f, Camera.main.transform.position.z);
        Vector3 RefPos = (PlayerRef != null) ? PlayerRef.transform.position : Camera.main.transform.position;

        //32 en 48 is hoever je de chunks ziet laden
        LoadChunks(RefPos, 60, 76);
        if (PlayerRef != null) {
            Color temp = ActiveWapImage.color;
            temp.a = temp.a - 0.045f;
            ActiveWapImage.color = temp;
        }
        if (PlayerRef != null) {
            switch (CheckWeap) {
                //When Bullets is active(Shoots bullets(1 does damage, 1 knocks enemy's back))
                case CheckWeapon.BULLETS:
                    {
                        Player.BulletShooting();
                        ActiveWapImage.overrideSprite = Sprite.Create(Resources.Load<Texture2D>("fireball"), ActiveWapImage.sprite.rect, ActiveWapImage.sprite.pivot);
                        if (Input.GetKeyDown("2")) {
                            CheckWeap = CheckWeapon.BLOCKS;
                            //Set Alpha to 1 so the image becomes visible
                            Color tempblock = ActiveWapImage.color;
                            tempblock.a = 1f;
                            ActiveWapImage.color = tempblock;
                        }
                    }
                    break;
                //When Blocks is active(Place & Destroy blocks)
                case CheckWeapon.BLOCKS:
                    {
                        BlockPlaceDeleteManager();
                        ActiveWapImage.overrideSprite = Sprite.Create(Resources.Load<Texture2D>("block"), ActiveWapImage.sprite.rect, ActiveWapImage.sprite.pivot);
                        if (Input.GetKeyDown("1")) {
                            CheckWeap = CheckWeapon.BULLETS;
                            //Set Alhpa to 1 so the image becomes visible
                            Color tempbullet = ActiveWapImage.color;
                            tempbullet.a = 1f;
                            ActiveWapImage.color = tempbullet;
                        }
                    }
                    break;
            }
        }
    }

     void BlockPlaceDeleteManager() {
        //left click
        if (Input.GetMouseButtonDown(0)) {
            ReplaceBlockCursor(0);
        }
        //Right click
        if (Input.GetMouseButtonDown(1)) {
            AddBlockCursor(1);
        }

    }
    //Replaces the block directly in front of the player
    public void ReplaceBlockCenter(float range, byte block) {

Ray ray = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
 RaycastHit hit;
 
 if (Physics.Raycast (ray, out hit)) {
   
  if(hit.distance<range){
   ReplaceBlockAt(hit, block);
  }
 }
  
    }

    //Adds the block specified directly in front of the player
    public void AddBlockCenter(float range, byte block) {

        Ray ray = new Ray(cameraGO.transform.position, cameraGO.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast (ray, out hit)){
            if (hit.distance < range){
                AddBlockAt(hit, block);
            }
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance), Color.green * 2);
        }
    }

    //Replaces the block specified where the mouse cursor is pointing
    public void ReplaceBlockCursor(byte block) {
        Ray ray = myCam.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;;
        if (Physics.Raycast (ray, out hit, 999f)){
            ReplaceBlockAt(hit, block);
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance),
            Color.green, 2);
        }
    }

    //Adds the block specified where the mouse cursor is pointing
    public void AddBlockCursor(byte block) {

        Ray ray = myCam.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2));
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit)){
            AddBlockAt(hit, block);
            Debug.DrawLine(ray.origin, ray.origin + (ray.direction * hit.distance),
            Color.green, 2);
        }
    }

    //removes a block at these impact coordinates, you can raycast against the terrain and call this with hit.point
    public void ReplaceBlockAt(RaycastHit hit, byte block) {
        Vector3 position = hit.point;
        position += (hit.normal * -0.5f);

        SetBlockAt(position, block);
    }

    //adds the specified block at these impact coordinates, you can raycast against the terrain and call this with hit.point
    public void AddBlockAt(RaycastHit hit, byte block) {
        Vector3 position = hit.point;
        position += (hit.normal * 0.5f);

        SetBlockAt(position, block);
    }

    //sets the specified block at these coordinates
    public void SetBlockAt(Vector3 position, byte block) {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);

        SetBlockAt(x, y, z, block);
    }

    //Sets the specified block at these coordinates
    public void SetBlockAt(int x, int y, int z, byte block) {
        //print("Adding block on: " + x + ", " + y + "," + z);

        world.data[x, y, z] = block;
        UpdateChunkAt(x, y, z);
    }

    //Updates the chunk containing this block
    public void UpdateChunkAt(int x, int y, int z) {

        int updateX = Mathf.FloorToInt(x / world.chunkSize);
        int updateY = Mathf.FloorToInt(y / world.chunkSize);
        int updateZ = Mathf.FloorToInt(z / world.chunkSize);

        //print("Update:" + updateX + "," + updateY + "," + updateZ);

        world.chunks[updateX, updateY, updateZ].update = true;

        if(x-(world.chunkSize*updateX)==0 && updateX != 0){
            world.chunks[updateX - 1, updateY, updateZ].update = true;
        }

        if(x-(world.chunkSize*updateX)==15 && updateX != world.chunks.GetLength(0) - 1){
            world.chunks[updateX + 1, updateY, updateZ].update = true;
        }
        if(y-(world.chunkSize*updateY)==0 && updateY != 0){
            world.chunks[updateX, updateY - 1, updateZ].update = true;
        }

        if(y-(world.chunkSize*updateY)==15 && updateY != world.chunks.GetLength(0) - 1){
            world.chunks[updateX, updateY + 1, updateZ].update = true;
        }

        if(z-(world.chunkSize*updateZ)==0 && updateZ != 0){
            world.chunks[updateX, updateY, updateZ - 1].update = true;
        }

        if(z-(world.chunkSize*updateZ)==15 && updateZ != world.chunks.GetLength(0) - 1){
            world.chunks[updateX, updateY, updateZ + 1].update = true;
        }
    }
}