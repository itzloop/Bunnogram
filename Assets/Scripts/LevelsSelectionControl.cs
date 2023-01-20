using System;
using System.Collections.Generic;
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

            public Level(int index, int columnsCount, int rowsCount, string name)
            {
                this.index = index;
                this.columnsCount = columnsCount;
                this.rowsCount = rowsCount;
                this.name = name;
                this.filename = String.Format("{0} {1}", index, name);
                this.isPlayed = false;
            }
        }

        private string ds_filename = "bw";

        public GameObject levelsGrid;
        public Button levelPrefab;

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
                Debug.Log(data[2]);
                var level = new Level(i, int.Parse(data[5]), int.Parse(data[4]), data[2]);

                listOfLevels.Add(level); // add this list into a big list


                Button buttonPrefab = Instantiate(levelPrefab);

                buttonPrefab.name = "Level " + level.index;
                if (!level.isPlayed)
                {
                    buttonPrefab.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        // open scene to level i
                    });
                    (buttonPrefab.transform.Find("LevelNumber").GetComponent<Text>()).text =
                        (level.index).ToString();
                    (buttonPrefab.transform.Find("NotPlayed").Find("NonogramSize").GetComponent<Text>()).text =
                        String.Format("{0} x {1}", level.rowsCount, level.columnsCount);
                }

                buttonPrefab.transform.parent = levelsGrid.transform;
                buttonPrefab.transform.localPosition = new Vector3(0, 0, 0);
                buttonPrefab.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(levelsGrid.GetComponent<RectTransform>());
            }
        }
    }
}