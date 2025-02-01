using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedDictionary<Tkey, Tvalue> : Dictionary<Tkey, Tvalue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<Tkey> keys = new List<Tkey>();
    [SerializeField] private List<Tvalue> values = new List<Tvalue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach( KeyValuePair<Tkey, Tvalue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }

    }

    public void OnAfterDeserialize() 
    {
        Clear();

        if(keys.Count != values.Count)
        {
            //error message
            Debug.LogError("Keys do not match values, something is wrong");
        }

        for(int i = 0; i < keys.Count; i++)
        {
            Add(keys[i], values[i]);
        }

    }    


}
