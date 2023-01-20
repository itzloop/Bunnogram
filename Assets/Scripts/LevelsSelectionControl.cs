using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LevelsSelectionControl : MonoBehaviour
    {
        private class Level
        {
            public int index { get; set; }
            public int columnsCount { get; set; }
            public int rowsCount { get; set; }
            public String name { get; set; }
            public String filename { get; set; }
            public bool isPlayed { get; set; }

            public Level(int index, int columnsCount, int rowsCount, string name, bool isPlayed)
            {
                this.index = index;
                this.columnsCount = columnsCount;
                this.rowsCount = rowsCount;
                this.name = name;
                this.filename = String.Format("{0} {1}", index, name);
                this.isPlayed = isPlayed;
            }
        }

        private string ds_filename = "bw";

        public GameObject levelsGrid;
        public Button levelPrefab;

        private Sprite resizePixels(Sprite sprite, float size)
        {
            Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                (int)sprite.textureRect.y,
                (int)sprite.textureRect.width,
                (int)sprite.textureRect.height);
            int spriteSize = (int)sprite.textureRect.width;
            int mlt = Mathf.CeilToInt(size / spriteSize);
            int finalSize = mlt * spriteSize;

            Color[] finalPixels = new Color[finalSize * finalSize];
            int mltto = (finalPixels.Length) / (pixels.Length);
            var wtv = pixels.SelectMany(px => Enumerable.Repeat(px, mltto).ToArray()).ToArray();
            List<List<Color>> colorsList = new List<List<Color>>();
            Color[,] colorsArray = new Color[spriteSize, spriteSize];
            for (int i = 0; i < spriteSize; i++)
            {
                for (int j = 0; j < spriteSize; j++)
                {
                    var clr = sprite.texture.GetPixel(i, j);
                    int idx = i * spriteSize + j;
                    //finalPixels[idx] =
                    for (int k = 0; k < mlt; k++)
                    {
                        
                    }
                }
            }


            var newTexture = new Texture2D(finalSize, finalSize);

            newTexture.SetPixels(finalPixels);
            newTexture.Apply();
            Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height),
                new Vector2(0.5f, 0.5f));
            


            return newSprite;
        }

        private void Start()
        {
            var dataset = Resources.Load<TextAsset>(ds_filename);
            var lines = dataset.text.Split('\n');

            var listOfLevels = new List<Level>();
            var columns = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                if (i > 100)
                {
                    break;
                }

                // 0-id,1-link,2-name,3-size,4-row,5-col
                var data = lines[i].Split(',');

                var level = new Level(i - 1, int.Parse(data[5]), int.Parse(data[4]), data[2], i % 2 == 0);

                listOfLevels.Add(level); // add this list into a big list


                Button buttonPrefab = Instantiate(levelPrefab);

                buttonPrefab.name = "Level " + level.index;
                (buttonPrefab.transform.Find("LevelNumber").GetComponent<Text>()).text =
                    (level.index).ToString();

                if (!level.isPlayed)
                {
                    buttonPrefab.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        // open scene to level i
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
                    Sprite sprite = Resources.Load<Sprite>("bw-preview/" + level.filename);
                    Image img = (playedTransform.Find("Nonogram").GetComponent<Image>());
                    img.sprite = sprite;
                }

                buttonPrefab.transform.parent = levelsGrid.transform;
                buttonPrefab.transform.localPosition = new Vector3(0, 0, 0);
                buttonPrefab.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
            }
        }
    }
}