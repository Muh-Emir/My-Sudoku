using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SudokuGenerator : MonoBehaviour
{
    public int[,] puzzle ={
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
        { 1, 2, 3, 4, 5, 6, 7, 8, 9 } };

    public List<string> sudoFirstText;
    public List<string> sudoLastText;

    public GameObject myCanvas;
    public GameObject theGrid;
    public List<GameObject> gridSlot;

    void Start()
    {
        GameCreate();
        int thisCounter = 0;
        int zeroCounter = 0;
        for (int y = 0; y < 9; y++)
        {
            int[] shuffle = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            shuffle = shuffle.OrderBy(item => Random.Range(0, 9)).ToArray();
            for (int x = 0; x < 9; x++)
            {
                puzzle[y, x] = shuffle[x];
            }
        }
        ShuffleCheck();
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (puzzle[y,x] == 0)
                {
                    SolveSudoku(puzzle, y, x);
                    zeroCounter++;
                }
                gridSlot[thisCounter].transform.GetChild(0).GetComponent<Text>().text = puzzle[y, x].ToString();
                thisCounter++;
            }
        }
        //gameObject.GetComponent<Sutest>().SolveSudoku(line,0,0);
        Debug.Log(zeroCounter);
    }

    void ShuffleCheck()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                for (int h = 0; h < 9; h++)
                {
                    if (h != y && puzzle[y, x] == puzzle[h, x] && puzzle[h, x] != 0)
                    {
                        puzzle[h, x] = 0;
                    }
                }
            }
        }
    }

    void GameCreate()
    {
        CanvasCreate();
    }

    void CanvasCreate()
    {
        myCanvas = new GameObject("Canvas");
        myCanvas.AddComponent(typeof(RectTransform));
        myCanvas.AddComponent(typeof(Canvas));
        myCanvas.AddComponent(typeof(CanvasScaler));
        myCanvas.AddComponent(typeof(GraphicRaycaster));
        myCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        myCanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent(typeof(EventSystem));
        eventSystem.AddComponent(typeof(StandaloneInputModule));

        theGrid = new GameObject("theGrid");
        theGrid.transform.parent = myCanvas.transform;
        theGrid.transform.localScale = Vector3.one;
        theGrid.AddComponent(typeof(RectTransform));
        theGrid.AddComponent(typeof(GridLayoutGroup));
        theGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width / 10, Screen.width / 10);
        theGrid.GetComponent<GridLayoutGroup>().spacing = new Vector2((Screen.width / 10) / 20, (Screen.width / 10) / 20);
        theGrid.GetComponent<GridLayoutGroup>().childAlignment = (TextAnchor)4;
        theGrid.GetComponent<GridLayoutGroup>().constraint = (GridLayoutGroup.Constraint)1;
        theGrid.GetComponent<GridLayoutGroup>().constraintCount = 9;

        GameObject theSlot = new GameObject("theSlot");
        theSlot.transform.parent = theGrid.transform;
        theSlot.transform.localScale = Vector3.one;
        theSlot.AddComponent(typeof(RectTransform));
        theSlot.AddComponent(typeof(CanvasRenderer));
        theSlot.AddComponent(typeof(Image));
        theSlot.AddComponent(typeof(Button));

        GameObject theText = new GameObject("theText");
        theText.transform.parent = theSlot.transform;
        theText.transform.localScale = Vector3.one;
        theText.AddComponent(typeof(RectTransform));
        theText.AddComponent(typeof(CanvasRenderer));
        theText.AddComponent(typeof(Text));
        theText.GetComponent<Text>().text = "0";
        theText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        theText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        theText.GetComponent<Text>().alignment = (TextAnchor)4;
        theText.GetComponent<Text>().resizeTextForBestFit = true;
        theText.GetComponent<Text>().color = Color.black;
        gridSlot.Add(theSlot);
        for (int i = 0; i < 80; i++)
        {
            gridSlot.Add(Instantiate(theSlot, theGrid.transform));
        }
    }

    void Update()
    {
        
    }

    public static bool SolveSudoku(int[,] puzzle, int row, int col)
    {
        if (row < 9 && col < 9)
        {
            if (puzzle[row, col] != 0)
            {
                if ((col + 1) < 9) return SolveSudoku(puzzle, row, col + 1);
                else if ((row + 1) < 9) return SolveSudoku(puzzle, row + 1, 0);
                else return true;
            }
            else
            {
                for (int i = 0; i < 9; ++i)
                {
                    if (IsAvailable(puzzle, row, col, i + 1))
                    {
                        puzzle[row, col] = i + 1;

                        if ((col + 1) < 9)
                        {
                            if (SolveSudoku(puzzle, row, col + 1)) return true;
                            else puzzle[row, col] = 0;
                        }
                        else if ((row + 1) < 9)
                        {
                            if (SolveSudoku(puzzle, row + 1, 0)) return true;
                            else puzzle[row, col] = 0;
                        }
                        else return true;
                    }
                }
            }

            return false;
        }
        else return true;
    }

    private static bool IsAvailable(int[,] puzzle, int row, int col, int num)
    {
        int rowStart = (row / 3) * 3;
        int colStart = (col / 3) * 3;

        for (int i = 0; i < 9; ++i)
        {
            if (puzzle[row, i] == num) return false;
            if (puzzle[i, col] == num) return false;
            if (puzzle[rowStart + (i % 3), colStart + (i / 3)] == num) return false;
        }

        return true;
    }
}