using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bunnogram;
using UniRx;
using UnityEngine;

public class PlayerDataControl : MonoBehaviour
{
    // Start is called before the first frame update
    private PlayerData playerData;
    private string playerDataPath;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        playerDataPath = Application.persistentDataPath + "/playerdata.json";
        if (File.Exists(playerDataPath))
        {
            string data = File.ReadAllText(playerDataPath);
            playerData = JsonUtility.FromJson<PlayerData>(data);
            Debug.Log("Loaded Data from " + playerDataPath);
        }
        else
        {
            // build with def values
            
            bool[] levels = Enumerable.Repeat(false, Constants.numberOfLevels).ToArray();
            
            playerData = new PlayerData(levels, true, true);
            saveToJson(playerData);
            Debug.Log("Saved data to " + playerDataPath);
        }
        //GameState.Instance.Store(new ReactiveProperty<bool[]>(playerData.playedLevels), Constants.PlayerDataKey);
        //GameState.Instance.Store(new ReactiveProperty<bool>(playerData.bgMusicOn), Constants.BgMusicOnKey);
        //GameState.Instance.Store(new ReactiveProperty<bool>(playerData.soundFXOn), Constants.SoundFXOnKey);
        
    }

    public PlayerData PlayerData
    {
        get => playerData;
        set => playerData = value;
    }

    public void SavePlayedLevel(int lvlIndex)
    {
        playerData.playedLevels[lvlIndex] = true;
        saveToJson(playerData);
    }

    public void SaveMusicOnState(bool state)
    {
        playerData.bgMusicOn = state;
        saveToJson(playerData);
    }
    
    public void SaveSoundFXOnState(bool state)
    {
        playerData.soundFXOn = state;
        saveToJson(playerData);
    }

    private void saveToJson(PlayerData newPlayerData)
    {
        string newData = JsonUtility.ToJson(newPlayerData);
        File.WriteAllText(playerDataPath, newData);
    }

}

[System.Serializable]
public class PlayerData
{
    public bool[] playedLevels;
    public bool bgMusicOn;
    public bool soundFXOn;

    public PlayerData(bool[] playedLevels, bool bgMusicOn, bool soundFXOn)
    {
        this.playedLevels = playedLevels;
        this.bgMusicOn = bgMusicOn;
        this.soundFXOn = soundFXOn;
    }
}