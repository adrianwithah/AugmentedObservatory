using UnityEngine;

// JSON Wrapper utility in order to parse JSON in Unity.
public class JSONParser {
    public static T[] parseJSONArray<T>(string originalJSON) {
        string modifiedJSON = "{ \"array\": " + originalJSON + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (modifiedJSON);
        return wrapper.array;
    }
 
    [System.Serializable]
    public class Wrapper<T> {
        public T[] array;
    }

    public static T parseJSONObject<T>(string json) {
        return JsonUtility.FromJson<T>(json);
    }
}