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
            public int id { get; set; }
            public int index { get; set; }
            public int columnsCount { get; set; }
            public int rowsCount { get; set; }
            public String name { get; set; }
            public String filename { get; set; }
            public bool isPlayed { get; set; }
            public int levelNumber { get; set; }

            public Level(int index, int id, int columnsCount, int rowsCount, string name, string englishName,
                bool isPlayed)
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
        [SerializeField] private GameObject playedLevelPanel;
        [SerializeField] private GameObject playedLevelPanelImage;
        [SerializeField] private GameObject playedLevelPanelText;
        [SerializeField] private Button NextPageButton;
        [SerializeField] private Button PrevPageButton;

        private List<Level[]> _levels;
        private bool[] _playedLevels;
        private int levelsPerPage = 6;

        private int page;
        private Dictionary<int, Level[]> allPages;
        private TextAsset dataset;

        public void handleBackButton()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        public void handleBackFromPlayedLevelPanel()
        {
            playedLevelPanel.SetActive(false);
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

        private void HandlePage(int page)
        {
            if (allPages.ContainsKey(page))
            {
                // levels are loaded before
                Level[] currPageLevels = allPages[page];
                for (int i = 0; i < currPageLevels.Length; i++)
                {
                    LoadLevel(currPageLevels[i]);
                }
            }
            else
            {
                // load and save
                dataset = Resources.Load<TextAsset>(ds_filename);
                var currPageLevels = GetLevelsOfPage(page);
                allPages.Add(page, currPageLevels);
                GameState.Instance.Update(allPages, Constants.ListLevelsKey);
            }


            if (page == 1)
            {
                PrevPageButton.gameObject.SetActive(false);
            }
            else
            {
                PrevPageButton.gameObject.SetActive(true);
            }

            if (page > 250)
            {
                NextPageButton.gameObject.SetActive(false);
            }
        }

        public void NextPage()
        {
            page += 1;
            foreach (Transform child in levelsGrid.transform)
            {
                Destroy(child.gameObject);
            }

            HandlePage(page);
        }

        public void PrevPage()
        {
            page -= 1;
            foreach (Transform child in levelsGrid.transform)
            {
                Destroy(child.gameObject);
            }

            HandlePage(page);
        }


        private void Start()
        {
            _playedLevels = GameObject.Find("PlayerDataControl").GetComponent<PlayerDataControl>().PlayerData
                .playedLevels;
            page = GameState.Instance.Get<int>(Constants.LevelPageKey);

            NextPageButton.onClick.AddListener(NextPage);
            PrevPageButton.onClick.AddListener(PrevPage);
            try
            {
                allPages = GameState.Instance.Get<Dictionary<int, Level[]>>(Constants.ListLevelsKey);
            }
            catch (Exception _)
            {
                allPages = new Dictionary<int, Level[]>();
                GameState.Instance.Store(allPages, Constants.ListLevelsKey);
            }

            HandlePage(page);
        }

        private Level[] GetLevelsOfPage(int page)
        {
            int start = (page - 1) * levelsPerPage + 1;
            int end = start + levelsPerPage;
            List<Level> currLevels = new List<Level>();

            var lines = dataset.text.Split('\n');
            for (int i = start; i < end; i++)
            {
                var data = lines[i].Split(',');
                
                Level lvl = DataToLevel(data, i - 1);
                currLevels.Add(lvl);
                LoadLevel(lvl);
            }

            return currLevels.ToArray();
        }

        private Level DataToLevel(string[] data, int index)
        {
            bool isPlayed = false;
            if (_playedLevels != null && _playedLevels.Length > index)
            {
                isPlayed = _playedLevels[index];
            }
            else
            {
                Debug.Log("Could not find level in player data");
            }

            return new Level(index,
                int.Parse(data[1]),
                int.Parse(data[6]),
                int.Parse(data[5]),
                data[7], data[3],
                isPlayed);
        }

        private void LoadLevel(Level level)
        {
            // update is played
            _playedLevels = GameObject.Find("PlayerDataControl").GetComponent<PlayerDataControl>().PlayerData
                .playedLevels;
            bool isPlayed = false;
            if (_playedLevels != null && _playedLevels.Length > level.index)
            {
                isPlayed = _playedLevels[level.index];
            }

            level.isPlayed = isPlayed;
            
            
            var buttonPrefab = Instantiate(levelPrefab);

            buttonPrefab.name = "Level " + level.levelNumber;
            (buttonPrefab.transform.Find("LevelNumber").GetComponent<RTLTextMeshPro>()).text =
                String.Format("مرحله {0}", (level.levelNumber).ToString());

            Sprite previewSprite = Resources.Load<Sprite>("bw-preview/" + level.filename);

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
                buttonPrefab.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var panelImg = playedLevelPanelImage.GetComponent<Image>();
                    panelImg.sprite = previewSprite;
                    panelImg.preserveAspect = true;

                    playedLevelPanelText.GetComponent<RTLTextMeshPro>().text = level.name;
                    playedLevelPanel.SetActive(true);
                });

                var playedTransform = buttonPrefab.transform.Find("Played");
                playedTransform.gameObject.SetActive(true);
                var img = (playedTransform.Find("Nonogram").GetComponent<Image>());
                img.sprite = previewSprite;
                img.preserveAspect = true;
                playedTransform.Find("Name").GetComponent<RTLTextMeshPro>().text = level.name;
                
            }

            buttonPrefab.transform.SetParent(levelsGrid.transform);
            buttonPrefab.transform.localPosition = new Vector3(0, 0, 0);
            buttonPrefab.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
        }
    }
}