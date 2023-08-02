using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SudokuGenerator : MonoBehaviour
{
    private const int boardSize = 9;
    private const int subgridSize = 3;
    private int[,] sudokuPuzzle;
    private int[,] sudokuAnswer;

    public int hideCount;

    public GameObject myCanvas;
    public GameObject theGrid;
    public List<GameObject> gridSlot;
    public List<GameObject> numButtons;

    private GameObject selectedButton;
    private int selectedY, selectedX;

    void Start()
    {
        CanvasCreate();
        GenerateSudoku();
        HideNums(hideCount);
        FillTable();
    }

    void HideNums(int hideCount)
    {
        for (int i = 0; i < hideCount; i++)
        {
            int y = Random.Range(0, 9);
            int x = Random.Range(0, 9);
            if (sudokuPuzzle[y, x] != 0)
            {
                sudokuPuzzle[y, x] = 0;
            }
            else
            {
                y = Random.Range(0, 9);
                x = Random.Range(0, 9);
                if (sudokuPuzzle[y, x] != 0)
                {
                    sudokuPuzzle[y, x] = 0;
                }
            }
        }
    }

    void FillTable()
    {
        int thisCounter = 0;
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (sudokuPuzzle[y, x] != 0)
                {
                    gridSlot[thisCounter].GetComponentInChildren<Text>().text = sudokuPuzzle[y, x].ToString();
                }
                else
                {
                    gridSlot[thisCounter].GetComponentInChildren<Text>().text = "";
                }
                thisCounter++;
            }
        }
    }

    #region UIcanvas
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
        theGrid.GetComponent<RectTransform>().localPosition = new Vector3(0, Screen.height / 8, 0);

        GameObject theSlot = new GameObject("theSlot");
        theSlot.transform.parent = theGrid.transform;
        theSlot.transform.localScale = Vector3.one;
        theSlot.AddComponent(typeof(RectTransform));
        theSlot.AddComponent(typeof(CanvasRenderer));
        theSlot.AddComponent(typeof(Image));
        theSlot.AddComponent(typeof(Button));
        theSlot.GetComponent<Button>().onClick.AddListener(delegate { SlotSelect(0); });

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
            int ID = i + 1;
            gridSlot[i + 1].GetComponent<Button>().onClick.AddListener(delegate { SlotSelect(ID); });
        }

        GameObject btnGrid = new GameObject("theButton");
        btnGrid.transform.parent = myCanvas.transform;
        btnGrid.transform.localScale = Vector3.one;
        btnGrid.AddComponent(typeof(RectTransform));
        btnGrid.AddComponent(typeof(GridLayoutGroup));
        btnGrid.GetComponent<GridLayoutGroup>().cellSize = new Vector2(Screen.width / 7, Screen.width / 7);
        btnGrid.GetComponent<GridLayoutGroup>().spacing = new Vector2((Screen.width / 5) / 20, (Screen.width / 5) / 20);
        btnGrid.GetComponent<GridLayoutGroup>().childAlignment = (TextAnchor)4;
        btnGrid.GetComponent<GridLayoutGroup>().constraint = (GridLayoutGroup.Constraint)1;
        btnGrid.GetComponent<GridLayoutGroup>().constraintCount = 6;
        btnGrid.GetComponent<RectTransform>().localPosition = new Vector3(0, (Screen.height / 2 - Screen.height / 5) * -1, 0);

        GameObject theButton = new GameObject("theButton");
        theButton.transform.parent = btnGrid.transform;
        theButton.transform.localScale = Vector3.one;
        theButton.AddComponent(typeof(RectTransform));
        theButton.AddComponent(typeof(CanvasRenderer));
        theButton.AddComponent(typeof(Image));
        theButton.AddComponent(typeof(Button));
        theButton.GetComponent<Button>().onClick.AddListener(delegate { OnButtonDown(0); });

        GameObject btnText = new GameObject("theText");
        btnText.transform.parent = theButton.transform;
        btnText.transform.localScale = Vector3.one;
        btnText.AddComponent(typeof(RectTransform));
        btnText.AddComponent(typeof(CanvasRenderer));
        btnText.AddComponent(typeof(Text));
        btnText.GetComponent<Text>().text = "0";
        btnText.GetComponent<Text>().font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        btnText.GetComponent<Text>().fontStyle = FontStyle.Bold;
        btnText.GetComponent<Text>().alignment = (TextAnchor)4;
        btnText.GetComponent<Text>().resizeTextForBestFit = true;
        btnText.GetComponent<Text>().color = Color.black;

        numButtons.Add(theButton);

        for (int i = 0; i < 11; i++)
        {
            numButtons.Add(Instantiate(theButton, btnGrid.transform));
            int ID = i + 1;
            numButtons[i + 1].GetComponent<Button>().onClick.AddListener(delegate { OnButtonDown(ID); });
            if (i < 9)
            {
                numButtons[i].GetComponentInChildren<Text>().text = (i + 1).ToString();
            }
        }
    }
    #endregion

    public void SlotSelect(int buttonID)
    {
        selectedX = buttonID - (buttonID / 9 * 9);
        selectedY = buttonID / 9;

        Debug.Log("Button ID: " + buttonID + " -Puzzle : " + sudokuPuzzle[selectedY, selectedX] + " -Answer :" + sudokuAnswer[selectedY, selectedX]);

        if (sudokuPuzzle[selectedY, selectedX] == 0)
        {
            selectedButton = gridSlot[buttonID];

            for (int i = 0; i < gridSlot.Count; i++)
            {
                if (gridSlot[i].GetComponentInChildren<Text>().text == "")
                    gridSlot[i].GetComponent<Image>().color = Color.white;
            }
            if (selectedButton.GetComponentInChildren<Text>().text == "")
                selectedButton.GetComponent<Image>().color = Color.grey;

        }
    }

    public void OnButtonDown(int buttonID)
    {
        Debug.Log("Button ID: " + buttonID + " -Puzzle : " + sudokuPuzzle[selectedY, selectedX] + " -Answer :" + sudokuAnswer[selectedY, selectedX]);
        if (sudokuPuzzle[selectedY, selectedX] == 0 && buttonID + 1 == sudokuAnswer[selectedY, selectedX])
        {
            Debug.Log("Answer is True!");
            selectedButton.GetComponentInChildren<Text>().text = sudokuAnswer[selectedY, selectedX].ToString();
            selectedButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            Debug.Log("Answer is False!" + "Answer is = " + sudokuAnswer[selectedY, selectedX].ToString());
            selectedButton.GetComponent<Image>().color = Color.red;
        }
    }

    #region SudokuCreate

    private void GenerateSudoku()
    {
        sudokuPuzzle = new int[boardSize, boardSize];
        sudokuAnswer = new int[boardSize, boardSize];

        FillDiagonalSubgrids();

        if (SolveSudoku(0, 0))
        {
            Debug.Log("Sudoku board successfully generated!");
        }
        else
        {
            Debug.LogError("Failed to generate a Sudoku board.");
        }
    }

    private bool SolveSudoku(int row, int col)
    {
        if (row == boardSize)
        {
            row = 0;
            if (++col == boardSize)
                return true;
        }

        if (sudokuPuzzle[row, col] != 0)
            return SolveSudoku(row + 1, col);

        for (int num = 1; num <= boardSize; num++)
        {
            if (IsValidPlacement(row, col, num))
            {
                sudokuPuzzle[row, col] = num;
                sudokuAnswer[row, col] = num;

                if (SolveSudoku(row + 1, col))
                    return true;

                sudokuPuzzle[row, col] = 0;
                sudokuAnswer[row, col] = 0;
            }
        }

        return false;
    }

    private bool IsValidPlacement(int row, int col, int num)
    {
        return !IsInRow(row, num) && !IsInColumn(col, num) && !IsInSubgrid(row - row % subgridSize, col - col % subgridSize, num);
    }

    private bool IsInRow(int row, int num)
    {
        for (int col = 0; col < boardSize; col++)
        {
            if (sudokuPuzzle[row, col] == num)
                return true;
        }

        return false;
    }

    private bool IsInColumn(int col, int num)
    {
        for (int row = 0; row < boardSize; row++)
        {
            if (sudokuPuzzle[row, col] == num)
                return true;
        }

        return false;
    }

    private bool IsInSubgrid(int startRow, int startCol, int num)
    {
        for (int row = 0; row < subgridSize; row++)
        {
            for (int col = 0; col < subgridSize; col++)
            {
                if (sudokuPuzzle[row + startRow, col + startCol] == num)
                    return true;
            }
        }

        return false;
    }

    private void FillDiagonalSubgrids()
    {
        for (int row = 0; row < boardSize; row += subgridSize)
        {
            FillSubgrid(row, row);
        }
    }

    private void FillSubgrid(int row, int col)
    {
        int[] numbers = GetShuffledNumbers();

        for (int i = 0; i < subgridSize; i++)
        {
            for (int j = 0; j < subgridSize; j++)
            {
                sudokuPuzzle[row + i, col + j] = numbers[i * subgridSize + j];
                sudokuAnswer[row + i, col + j] = numbers[i * subgridSize + j];
            }
        }
    }

    private int[] GetShuffledNumbers()
    {
        int[] numbers = new int[boardSize];
        for (int i = 0; i < boardSize; i++)
        {
            numbers[i] = i + 1;
        }

        for (int i = 0; i < boardSize - 1; i++)
        {
            int randIndex = UnityEngine.Random.Range(i, boardSize);
            int temp = numbers[i];
            numbers[i] = numbers[randIndex];
            numbers[randIndex] = temp;
        }

        return numbers;
    }
    #endregion
}