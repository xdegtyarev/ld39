using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class StreamingAssetManager : MonoBehaviour {
    public static StreamingAssetManager Instance;
    JsonSerializerSettings jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, MissingMemberHandling = MissingMemberHandling.Ignore };
    List<Object> settings;
    // Texture2D decorAtlas;
    private static bool isDataLoaded;
    private string streamingAssetsPath;
    private string persistentDataPath;

    void Awake() {
        if ( Instance == null ) {
            Instance = this;
            streamingAssetsPath = Application.streamingAssetsPath;
            persistentDataPath = Application.persistentDataPath;
            LoadData();
            // LoadSprites( ProjectsPaths.decorImagesPathExt, 8192 );
            // LoadSprites( ProjectsPaths.backgroundImagesPathExt, 8192 );
            DontDestroyOnLoad( gameObject );
        } else {
            if ( this != Instance ) {
                Destroy( this );
            }
        }
    }

    public T GetSettings<T>() where T: ScriptableObject {
        foreach (var o in settings) {
            if (o is T) {
                return o as T;
            }
        }
        return null;
    }

    void LoadData() {
        settings = new List<Object>();
        foreach ( Object o in Resources.LoadAll( "Settings/" ) ) {
            settings.Add( Instantiate( o ) );
        }
        LoadInternalData();
        Debug.Log("[StreamingAssetManager] All Data Loaded");
        isDataLoaded = true;
    }


    void LoadInternalData() {
        Debug.Log("[StreamingAssetManager] LoadInternal Started");

        LoadData<TiledMap>(streamingAssetsPath, "Maps");
        Debug.Log("[StreamingAssetManager] LoadInternal Complete");
    }

    private void LoadData<T>(string root, string path) where T: Settings {
        var directoryInfo = new DirectoryInfo(Path.Combine(root, path));
        if(!directoryInfo.Exists){
            directoryInfo.Create();
        }
        foreach (var o in directoryInfo.GetFiles( "*.json", SearchOption.AllDirectories )) {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(o.FullName), jsonSettings);
            if(string.IsNullOrEmpty(data.key)){
                data.key = o.Name.Substring(0, o.Name.LastIndexOf("."));
            }
            Database<T>.GetInstance().Add(data);
            Debug.Log("Added "  + data.GetType().Name + " with key: " + data.key);
        }
    }
}

