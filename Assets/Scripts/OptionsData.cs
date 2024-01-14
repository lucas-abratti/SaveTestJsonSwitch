using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsData
{
    private const string fileName = "OptionsSaveData";

    [SerializeField] private float volume = 1.0f;

    [SerializeField] private int language = 3;

    public static string FileName => fileName;

    public float Volume { get => volume; set => volume = value; }
    public int Language { get => language; set => language = value; }

    public static OptionsData DefaultOptions()
    {
        return new OptionsData();
    }
}
