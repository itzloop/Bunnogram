using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bunnogram;
using RTLTMPro;
using ScriptableObjects;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LevelsSelectionControl : MonoBehaviour
    {
        private class Level
        {
            public  int id { get; set; }
            public int index { get; set; }
            public int columnsCount { get; set; }
            public int rowsCount { get; set; }
            public String name { get; set; }
            public String filename { get; set; }
            public bool isPlayed { get; set; }
            public int levelNumber { get; set; }

            public Level(int index, int id, int columnsCount, int rowsCount, string name, string englishName, bool isPlayed)
            {
                this.id = id;
                this.index = index;
                this.columnsCount = columnsCount;
                this.rowsCount = rowsCount;
                this.name = name;
                this.filename = String.Format("{0}-{1}", id, englishName);
                this.isPlayed = isPlayed;
                this.levelNumber = index + 1;
            }
        }

        private string ds_filename = "bw";

        [SerializeField] GameObject levelsGrid;
        [SerializeField] Button levelPrefab;

        private List<Level> _levels;
        private bool[] _playedLevels;

        public void handleBackButton()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        private void CreatePixelatedImage(Sprite previewSprite, Sprite originalSprite, int levelNumber, string name)
        {
            PixelatedImage img = ScriptableObject.CreateInstance<PixelatedImage>();
            img.sprite = previewSprite;
            img.bounds = new Vector2Int(originalSprite.texture.width, originalSprite.texture.height);

            List<Vector2Int> backgroundPixels = new List<Vector2Int>();
            for (int i = originalSprite.texture.width - 1; i >= 0; i--)
            {
                for (int j = 0; j < originalSprite.texture.height; j++)
                {
                    if (originalSprite.texture.GetPixel(i, j) == Color.white)
                    {
                        backgroundPixels.Add(new Vector2Int(originalSprite.texture.height - 1 - j, i));
                    }
                }
            }

            img.backgroundPixels = backgroundPixels;

            img.levelName = name;
            AssetDatabase.CreateAsset(img, $"Assets/Resources/Levels/Level_{levelNumber:000}.asset");
        }

        private void Start()
        {
            _playedLevels = GameObject.Find("PlayerDataControl").GetComponent<PlayerDataControl>().PlayerData.playedLevels;
            try
            {
                _levels = GameState.Instance.Get<List<Level>>(Constants.ListLevelsKey);
            }
            catch (Exception _)
            {
                Debug.Log($"levels has not been loaded. loading levels...");
                _levels = null;
            }
            if (_levels != null && _levels.Count > 0)
            {
                Debug.Log("levels had been cached. don't load them");
                LoadLevels(_levels);
                return;
            }
            
            _levels = CreateLevels();
            GameState.Instance.Store(_levels, Constants.ListLevelsKey);
            LoadLevels(_levels);
        }
        

        private List<Level> CreateLevels()
        {
            var dataset = Resources.Load<TextAsset>(ds_filename);
            var lines = dataset.text.Split('\n');
            var columns = 0;
            var levels = new List<Level>();
            
            for (int i = 1; i < lines.Length; i++)
            {
                if (i > Constants.numberOfLevels)
                {
                    break;
                }

                // 0-id,1-idx, 2-link,3-name,4-size,5-row,6-col,7-persian
                var data = lines[i].Split(',');
                bool isPlayed = false;
                int index = i - 1;
                if (_playedLevels != null && _playedLevels.Length > index)
                {
                    isPlayed = _playedLevels[index];
                }
                else
                {
                    Debug.Log("Could not find level in player data");
                }
                    
                var level = new Level(index, 
                    int.Parse(data[1]), 
                    int.Parse(data[6]), 
                    int.Parse(data[5]),
                    data[7],data[3],
                    isPlayed);

                levels.Add(level); // add this list into a big list


                // create the level button
                var buttonPrefab = Instantiate(levelPrefab);
                
                buttonPrefab.name = "Level " + level.levelNumber;
                (buttonPrefab.transform.Find("LevelNumber").GetComponent<RTLTextMeshPro>()).text =
                    String.Format("مرحله {0}", (level.levelNumber).ToString());
                
                Sprite previewSprite = Resources.Load<Sprite>("bw-preview/" + level.filename);

                //Sprite nono = Resources.Load<Sprite>("bw/" + String.Format("{0} {1}", level.id, data[3]));
                //CreatePixelatedImage(previewSprite, nono, level.levelNumber, level.name);
        
                if (!level.isPlayed)
                {
                    buttonPrefab.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        // open scene to level i
                        GameState.Instance.Get<ReactiveProperty<int>>(Constants.LevelKey).Value = level.levelNumber;
                        SceneManager.LoadScene("InGameScene");
                    });
                    
                    var notPlayedTransform = buttonPrefab.transform.Find("NotPlayed");
                    (notPlayedTransform.Find("NonogramSize").GetComponent<RTLTextMeshPro>()).text =
                        $"{level.rowsCount} در {level.columnsCount}";
                    notPlayedTransform.gameObject.SetActive(true);
                }
                else
                {
                    var playedTransform = buttonPrefab.transform.Find("Played");
                    playedTransform.gameObject.SetActive(true);
                    var img = (playedTransform.Find("Nonogram").GetComponent<Image>());
                    img.sprite = previewSprite;
                    img.preserveAspect = true;
                    playedTransform.Find("Name").GetComponent<RTLTextMeshPro>().text = level.name;
                }

                buttonPrefab.transform.parent = levelsGrid.transform;
                buttonPrefab.transform.localPosition = new Vector3(0, 0, 0);
                buttonPrefab.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
            }


            return levels;
        }

        private void LoadLevels(List<Level> levels)
        {
            for (var i = 0; i < levels.Count; i++)
            {
                // Don't load all levels
                if (i > Constants.numberOfLevels) break;
                
                var level = levels[i];
                var buttonPrefab = Instantiate(levelPrefab);
                 
                buttonPrefab.name = "Level " + level.levelNumber;
                (buttonPrefab.transform.Find("LevelNumber").GetComponent<RTLTextMeshPro>()).text =
                    String.Format("مرحله {0}", (level.levelNumber).ToString());
                 
                Sprite previewSprite = Resources.Load<Sprite>("bw-preview/" + level.filename);

                
                bool isPlayed = false;
                if (_playedLevels != null && _playedLevels.Length > i)
                {
                    isPlayed = _playedLevels[i];
                }

                level.isPlayed = isPlayed;
         
                if (!level.isPlayed)
                {
                    buttonPrefab.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        // open scene to level i
                        GameState.Instance.Get<ReactiveProperty<int>>(Constants.LevelKey).Value = level.levelNumber;
                        SceneManager.LoadScene("InGameScene");
                    });
                     
                    var notPlayedTransform = buttonPrefab.transform.Find("NotPlayed");
                    (notPlayedTransform.Find("NonogramSize").GetComponent<RTLTextMeshPro>()).text =
                        $"{level.rowsCount} در {level.columnsCount}";
                    notPlayedTransform.gameObject.SetActive(true);
                }
                else
                {
                    var playedTransform = buttonPrefab.transform.Find("Played");
                    playedTransform.gameObject.SetActive(true);
                    var img = (playedTransform.Find("Nonogram").GetComponent<Image>());
                    img.sprite = previewSprite;
                    img.preserveAspect = true;
                    playedTransform.Find("Name").GetComponent<RTLTextMeshPro>().text = level.name;
                }
 
                buttonPrefab.transform.parent = levelsGrid.transform;
                buttonPrefab.transform.localPosition = new Vector3(0, 0, 0);
                buttonPrefab.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
                
            }
        }
    }
}