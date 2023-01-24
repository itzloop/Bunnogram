using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bunnogram;
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

            public Level(int index, int id, int columnsCount, int rowsCount, string name, bool isPlayed)
            {
                this.id = id;
                this.index = index;
                this.columnsCount = columnsCount;
                this.rowsCount = rowsCount;
                this.name = name;
                this.filename = String.Format("{0}-{1}", id, name);
                this.isPlayed = isPlayed;
                this.levelNumber = index + 1;
            }
        }

        private string ds_filename = "bw";

        public GameObject levelsGrid;
        public Button levelPrefab;

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
            var dataset = Resources.Load<TextAsset>(ds_filename);
            var lines = dataset.text.Split('\n');

            var listOfLevels = new List<Level>();
            var columns = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                if (i > 10)
                {
                    break;
                }

                // 0-id,1-link,2-name,3-size,4-row,5-col
                var data = lines[i].Split(',');

                var level = new Level(i - 1, int.Parse(data[0]), int.Parse(data[5]), int.Parse(data[4]), data[2], false);

                listOfLevels.Add(level); // add this list into a big list


                Button buttonPrefab = Instantiate(levelPrefab);

                buttonPrefab.name = "Level " + level.levelNumber;
                (buttonPrefab.transform.Find("LevelNumber").GetComponent<Text>()).text =
                    (level.levelNumber).ToString();

                
                Sprite previewSprite = Resources.Load<Sprite>("bw-preview/" + level.filename);

                //Sprite nono = Resources.Load<Sprite>("bw/" + String.Format("{0} {1}", level.index, level.name));
                //CreatePixelatedImage(previewSprite, nono, level.levelNumber, level.name);

                if (!level.isPlayed)
                {
                    buttonPrefab.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        // open scene to level i

                        GameState.Instance.Update(level.levelNumber, Constants.LevelKey);
                        SceneManager.LoadScene("InGameScene");
                    });
                    Transform notPlayedTransform = buttonPrefab.transform.Find("NotPlayed");

                    (notPlayedTransform.Find("NonogramSize").GetComponent<Text>()).text =
                        String.Format("{0} x {1}", level.rowsCount, level.columnsCount);
                    notPlayedTransform.gameObject.SetActive(true);
                }
                else
                {
                    Transform playedTransform = buttonPrefab.transform.Find("Played");
                    playedTransform.gameObject.SetActive(true);
                    Image img = (playedTransform.Find("Nonogram").GetComponent<Image>());
                    img.sprite = previewSprite;
                    img.preserveAspect = true;
                    playedTransform.Find("Name").GetComponent<Text>().text = level.name;
                }


                buttonPrefab.transform.parent = levelsGrid.transform;
                buttonPrefab.transform.localPosition = new Vector3(0, 0, 0);
                buttonPrefab.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
            }
        }
    }
}