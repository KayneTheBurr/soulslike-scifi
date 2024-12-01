using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorldItemDataBase : MonoBehaviour
{
    public static WorldItemDataBase instance;

    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();
    public WeaponItem unarmedWeapon;

    private List<Item> items = new List<Item>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }
        for (int i = 0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
    }


}
