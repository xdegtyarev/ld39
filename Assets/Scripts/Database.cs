using System;
using UnityEngine;
using System.Collections.Generic;

public class Settings{
    public string key;
}

public class Database<T> where T:Settings {
    Dictionary<string,T> data;

    static Database<T> Instance;
    public static Database<T> GetInstance() {
        if (Instance == null) {
            Instance = new Database<T>();
            Instance.data = new Dictionary<string, T>();
        }
        return Instance;
    }

    public void Add(T o,bool overwrite = false) {
        try{
            if(overwrite && data.ContainsKey(o.key)){
                data.Remove(o.key);
            }
            data.Add(o.key, o);
        }catch(Exception e){
            Debug.LogErrorFormat( "[Settings::Database] {0} failed with {1}", o.key, e );
        }
    }

    public void Remove(string key){
        data.Remove(key);
    }

    public bool HasItem(string key) {
        return data.ContainsKey(key);
    }

    public T GetItem(string key) {
        if (data.ContainsKey(key)) {
            return data[key];
        } else {
	       Debug.LogWarningFormat( "[Settings::Database] no such key: {0} in {1} database", key, typeof(T).Name );
            // foreach (var o in data.Keys) {
            //     Debug.Log(o);
            // }
            return null;
        }
    }

    public List<T> GetAll(System.Predicate<T> predicate = null) {
        var list = new List<T>();
        if (predicate == null) {
            list.AddRange(data.Values);
        } else {
            foreach (var o in data.Values) {
                if (predicate(o)) {
                    list.Add(o);
                }
            }
        }
        return list;
    }


    public List<string> GetAllKeys(System.Predicate<T> predicate = null) {
        var list = new List<string>();
        if (predicate == null) {
            foreach (var o in data.Values) {
                list.Add(o.key);
            }
        } else {
            foreach (var o in data.Values) {
                if (predicate(o)) {
                    list.Add(o.key);
                }
            }
        }
        return list;
    }

    public void Clear() {
        data.Clear();
    }
}