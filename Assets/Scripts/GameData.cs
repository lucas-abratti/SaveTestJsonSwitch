using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private const string fileName = "GameSaveData";

    [SerializeField] private string name = "User123";
    [SerializeField] private int level = 0;
    [SerializeField] private Vector3 position = Vector3.zero;

    public static string FileName => fileName;

    public int Level { get => level; set => level = value; }
    public string Name { get => name; set => name = value; }
    public Vector3 Position { get => position; set => position = value; }

    public static GameData DefaultOptions()
    {
        return new GameData();
    }
}
