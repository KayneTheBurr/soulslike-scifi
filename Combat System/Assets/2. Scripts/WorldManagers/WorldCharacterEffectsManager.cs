using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;

    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;
    public GameObject bloodSplatterVFX;

    [SerializeField] List<InstantCharacterEffect> instantEffects;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GenerateEffectIDs();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void GenerateEffectIDs()
    {
        for (int i = 0; i < instantEffects.Count; ++i)
        {
            instantEffects[i].instantEffectID = i;
        }


    }





}
