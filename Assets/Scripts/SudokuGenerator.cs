using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SudokuGenerator : MonoBehaviour
{
    public int[] shuffle = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public int[,] puzzle ={
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

    public int[,] answer ={
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0 } };

    public GameObject myCanvas;
    public GameObject theGrid;
    public List<GameObject> gridSlot;
    public List<GameObject> numButtons;

    public GameObject selectedButton;
    public int selectedY, selectedX;

    void Start()
    {
        GameCreate();
    }

    void GameCreate()
    {
        CanvasCreate();
        if (MakeSudoku(0, 0))
        {
            //HideNums();
            FillTable();
        }
    }

    void HideNums()
    {
        for (int i = 0; i < 20; i++)
        {
            int y = Random.Range(0, 9);
            int x = Random.Range(0, 9);
            if (puzzle[y, x] != 0)
            {
                puzzle[y, x] = 0;
            }
            else
            {
                y = Random.Range(0, 9);
                x = Random.Range(0, 9);
                if (puzzle[y, x] != 0)
                {
                    puzzle[y, x] = 0;
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
                if (puzzle[y, x] != 0)
                {
                    gridSlot[thisCounter].GetComponentInChildren<Text>().text = puzzle[y, x].ToString();
                }
                else
                {
                    gridSlot[thisCounter].GetComponentInChildren<Text>().text = "";
                }
                thisCounter++;
            }
        }
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

    void Update()
    {
        
    }

    public void SlotSelect(int buttonID)
    {
        selectedX = buttonID - (buttonID / 9 * 9);
        selectedY = buttonID / 9;

        if (puzzle[selectedY, selectedX] == 0)
        {
            selectedButton = gridSlot[buttonID];
            for (int i = 0; i < gridSlot.Count; i++)
            {
                gridSlot[i].GetComponent<Image>().color = Color.white;
            }
            selectedButton.GetComponent<Image>().color = Color.grey;
        }
    }

    public void OnButtonDown(int buttonID)
    {
        Debug.Log("Button : " + buttonID +"---"+ answer[selectedY,selectedX] + "---" + puzzle[selectedY,selectedX]);

        if (puzzle[selectedY,selectedX] == 0 && buttonID + 1 == answer[selectedY, selectedX])
        {
            Debug.Log("True");
            selectedButton.GetComponentInChildren<Text>().text = answer[selectedY, selectedX].ToString();
            selectedButton.GetComponent<Image>().color = Color.green;
        }
    }

    public bool MakeSudoku(int y, int x)
    {
        shuffle = shuffle.OrderBy(item => Random.Range(0, 9)).ToArray();
        for (int i = 0; i < 9; i++)
        {
            puzzle[0, i] = shuffle[i];
            answer[0, i] = shuffle[i];
        }
        shuffle = shuffle.OrderBy(item => Random.Range(0, 9)).ToArray();

        if (y < 9 && x < 9)
        {
            if (puzzle[y, x] != 0)
            {
                if ((x + 1) < 9) return MakeSudoku(y, x + 1);
                else if ((y + 1) < 9) return MakeSudoku(y + 1, 0);
                else return true;
            }
            else
            {
                for (int i = 0; i < 9; ++i)
                {
                    if (IsOk(puzzle, y, x, shuffle[i]))
                    {
                        puzzle[y, x] = shuffle[i];
                        answer[y, x] = shuffle[i];

                        if ((x + 1) < 9)
                        {
                            if (MakeSudoku(y, x + 1)) return true;
                            else puzzle[y, x] = 0;
                        }
                        else if ((y + 1) < 9)
                        {
                            if (MakeSudoku(y + 1, 0)) return true;
                            else puzzle[y, x] = 0;
                        }
                        else return true;
                    }
                }
            }

            return false;
        }
        else return true;
    }

    bool IsOk(int[,] puzzle, int y, int x, int num)
    {
        int rowStart = (y / 3) * 3;
        int colStart = (x / 3) * 3;

        for (int i = 0; i < 9; ++i)
        {
            if (puzzle[y, i] == num) return false;
            if (puzzle[i, x] == num) return false;
            if (puzzle[rowStart + (i % 3), colStart + (i / 3)] == num) return false;
        }

        return true;
    }
}