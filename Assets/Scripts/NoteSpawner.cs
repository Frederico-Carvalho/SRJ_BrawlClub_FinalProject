using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

[System.Serializable]
public class NoteData
{
    public float time;
    public string key;
}

[System.Serializable]
public class SongData
{
    public List<NoteData> notes;
}

public class NoteSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject leftNotePrefab;
    public GameObject rightNotePrefab;
    public GameObject upNotePrefab;
    public GameObject downNotePrefab;

    public Transform spawnPointLeft;
    public Transform spawnPointRight;
    public Transform spawnPointUp;
    public Transform spawnPointDown;

    [Header("Song Settings")]
    public string songFileName = "song1";
    public float spawnOffset = 2f;

    private SongData songData;
    private int nextNoteIndex = 0;
    private float songTime = 0f;
    private bool isPlaying = false;

    void Start()
    {
        LoadChart();
    }

    void LoadChart()
    {
        string path = Path.Combine(Application.streamingAssetsPath, songFileName + ".json");
        string json = File.ReadAllText(path);
        songData = JsonUtility.FromJson<SongData>(json);
        Debug.Log("Chart carregado com " + songData.notes.Count + " notas");
    }

    void Update()
    {
        if (!isPlaying) return;

        songTime += Time.deltaTime;

        while (nextNoteIndex < songData.notes.Count)
        {
            NoteData note = songData.notes[nextNoteIndex];

            if (songTime >= note.time - spawnOffset)
            {
                SpawnNote(note);
                nextNoteIndex++;
            }
            else
            {
                break;
            }
        }
    }

    public void StartSpawning()
    {
        isPlaying = true;
        songTime = 0f;
    }

    void SpawnNote(NoteData note)
    {
        GameObject prefab = null;

        switch (note.key)
        {
            case "Left": prefab = leftNotePrefab; break;
            case "Right": prefab = rightNotePrefab; break;
            case "Up": prefab = upNotePrefab; break;
            case "Down": prefab = downNotePrefab; break;
        }

        if (prefab != null)
        {
            Transform spawnPoint = null;
            switch (note.key)
            {
                case "Left": spawnPoint = spawnPointLeft; break;
                case "Right": spawnPoint = spawnPointRight; break;
                case "Up": spawnPoint = spawnPointUp; break;
                case "Down": spawnPoint = spawnPointDown; break;
            }

            GameObject spawnedNote = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            spawnedNote.transform.SetParent(GameObject.Find("NoteHolder").transform);
            NoteObject noteObject = spawnedNote.GetComponent<NoteObject>();
            if (noteObject != null)
            {
                GameObject activatorObject = GameObject.FindWithTag("Activator");
                if (activatorObject != null)
                {
                    noteObject.activator = activatorObject.transform;
                }
            }
        }
    }
}