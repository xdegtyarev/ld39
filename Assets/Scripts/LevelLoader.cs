using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TiledMap: Settings {
    public int width;
    public int height;
    public int tilewidth;
    public int tileheight;
    public string orientation;
    public TiledLayer[] layers;
}

public enum TiledLayerType {
    Unknown,
    TileLayer,
    ObjectGroup,
    ImageLayer
}

public class TiledLayer {
    public int width;
    public int height;
    public string name;
    public string type;
    public int[] data;
    [JsonIgnore] public TiledLayerType tiledLayerType {
        get {
            if (type == "tilelayer")
                return TiledLayerType.TileLayer;
            else if (type == "objectgroup")
                return TiledLayerType.ObjectGroup;
            else if (type == "imagelayer")
                return TiledLayerType.ImageLayer;
            else
                return TiledLayerType.Unknown;
        }
    }
}

[System.Serializable]
public class TileSet: Settings{
    public GameObject[] prefab;
}

public class LevelLoader : MonoBehaviour {
	public static LevelLoader Instance;
	[SerializeField] int maxLevelHeight;
    [SerializeField] string testLevelKey;
    [SerializeField] TileSet[] tileSets;
    [SerializeField] GameObject hero;
    [SerializeField] Power[] powerLines;

    public Transform checkpoint;

   	Vector3 lastCheckpoint;
    bool checkpointInited = false;
    public TiledMap currentLevelData;
    public Transform enter;
    public Transform exit;

    void Awake(){
    	Instance = this;
    	foreach(var o in tileSets){
    		Database<TileSet>.GetInstance().Add(o);
    	}
        ResetCheckpoint();
    }

    public void ResetCheckpoint(){
        checkpoint = enter;
    }

    public void SetCheckpoint(Transform checkpointTransform){
        checkpoint = checkpointTransform;
    }

    void Start() {
    	Application.targetFrameRate = 100;
        // LoadLevel(testLevelKey);
        ReSpawnHero();
    }

    public void ReSpawnHero(){
        AudioController.Play("Level_start");
    	hero.transform.position = checkpoint.transform.position+Vector3.up;
        hero.GetComponent<Character>().RestoreEnergy();
        foreach(var o in powerLines){
            if(o.transform!=checkpoint){
                o.Restore();
            }
        }
    }

    public TileSet GetTileSet(string tileSetName){
    	return Database<TileSet>.GetInstance().GetItem(string.IsNullOrEmpty(tileSetName) ? "default" : tileSetName);
    }

    public void LoadLevel(string level) {
        if (Database<TiledMap>.GetInstance().HasItem(level)) {
            currentLevelData = Database<TiledMap>.GetInstance().GetItem(level);
            foreach (var layer in currentLevelData.layers) {
                Debug.Log("LayerName:" + layer.name + " of type: " + layer.type);
                if (layer.tiledLayerType == TiledLayerType.TileLayer) {
                	var tileSet = GetTileSet(null);
                    for (int i = 0; i < layer.data.Length; i++) {
                        if (layer.data[i] > 0) {
                        	// if(layer.data[i]>=tileSet.prefab.Length){
                        	// 	Debug.Log("Cannot place tile: " + layer.data[i]);
                        	// }
                        	// if(!checkpointInited && layer.data[i] == 1){
                        	// 	checkpointInited = true;
                        	// 	lastCheckpoint = new Vector3(i%layer.width,1f,i/layer.width);
                        	// }
                            // PlaceTile(tileSet.prefab[layer.data[i]], (i%layer.width)-(layer.width*0.5f), 0, (i/layer.width)-(layer.width*0.5f), transform);
                            PlaceScaledCubeTile(layer.data[i], (i%layer.width)-(layer.width*0.5f), (i/layer.width)-(layer.width*0.5f), transform);
                        }
                    }
                }
            }
        } else {
            Debug.Log("[LevelLoader] No such level: " + level);
        }
    }
    [SerializeField] GameObject cubePrefab;
    public void PlaceCubeTile(int height,float x, float z, Transform parent){
    	float y = 0.5f;
    	for(int i = 0; i<height; i++, y+=1f){
    		Instantiate(cubePrefab, new Vector3(x,y,z),Quaternion.identity,parent);
    	}
    }

    public void PlaceScaledCubeTile(int height,float x, float z, Transform parent){
    	var go = Instantiate(cubePrefab, new Vector3(x,height*0.5f+0.5f,z),Quaternion.identity,parent);
    	go.transform.localScale = new Vector3(1,height,1);

    }

    public GameObject PlaceTile(GameObject go, float x,float y,float z, Transform parent){
    	return Instantiate(go, new Vector3(x,y,z), Quaternion.identity, parent);
    }
}
