using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CellState
{
    public int state;
    public int column;
    public int row;

    public CellState(int state, int column, int row)
    {
        this.state = state;
        this.column = column;
        this.row = row;
    }
}

public class GridManager : MonoBehaviour
{
    public GridLayoutGroup mainGrid;
    public GridLayoutGroup columnGrid;
    public GridLayoutGroup rowGrid;
    public RectTransform preview;
    public GameObject cellPrefab;
    public GameObject hintPrefab;

    public int rows;
    public int columns;
    public bool[] solutions;

    public GameObject logicClearPanel;

    [HideInInspector] public int current_Column;
    [HideInInspector] public int current_Row;
    [HideInInspector] public bool stageClear = false;
    [HideInInspector] public bool isClicking = false;
    // 0:체크 해제, 1:체크, 2:X 체크, 3:X 해제
    [HideInInspector] public int clickType = -1;

    [HideInInspector] public List<List<int>> columnHints = new List<List<int>>();
    [HideInInspector] public List<List<int>> rowHints = new List<List<int>>();
    [HideInInspector] public int columnHintSize;
    [HideInInspector] public int rowHintSize;

    private Stack<CellState[]> undoStack = new Stack<CellState[]>();
    private Stack<CellState[]> redoStack = new Stack<CellState[]>();

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            switch(GameManager.instance.current_Difficulty)
            {
                case NNG_Difficulty.Easy:
                    rows = 10;
                    columns = 10;
                    break;
                case NNG_Difficulty.Normal:
                    rows = 15;
                    columns = 15;
                    break;
                case NNG_Difficulty.Hard:
                    rows = 20;
                    columns = 20;
                    break;
            }
        }
        else
        {
            rows = 10;
            columns = 10;
        }

        stageClear = false;
        solutions = new bool[rows * columns];
        GetComponent<GridLayoutGroup>().constraintCount = columns;

        for(int i=0; i<solutions.Length; i++)
        {
            solutions[i] = Random.Range(0, 2)==1;
        }

        GridSetting();
        GenerateGrid();
        SaveState();

        AudioManager.instance.PlayBgm(AudioManager.instance.bgmClips_Game[Random.Range(1,14)]);
        AudioManager.instance.bgmPlayer.loop = true;
        AudioManager.instance.bgmPlayer.DOFade(AudioManager.instance.bgmVolume, 0.7f);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            Redo();
        }
    }

    // 그리드 세팅
    private void GridSetting()
    {
        ColumnHintCalculate();
        RowHintCalculate();
        preview.localScale = new Vector3(rowGrid.constraintCount, columnGrid.transform.childCount / columnGrid.constraintCount, 1);
    }
    // 컬럼 그리드 설정
    private void ColumnHintCalculate()
    {
        for(int i=0; i<rows; i++)
        {
            List<int> temp = new List<int>(); // column hint 임시 저장 리스트
            int count = 0; // 연속되는 hint의 수

            // 한 column씩 hint 계산
            for(int j=0; j<columns; j++)
            {
                if(solutions[j*rows + i])
                {
                    count++;
                }

                // 연속되던 hint가 끊기면 이전의 값을 리스트에 저장
                else
                {
                    if(count != 0)
                    {
                        temp.Add(count);
                        count = 0;
                    }
                }
            }

            // 한 column이 모두 끝났을 때

            // 마지막 연속되는 hint를 리스트에 추가
            if(count != 0)
                temp.Add(count);

            // hint가 아예 없는 column일 경우 0을 추가
            if(temp.Count == 0)
                temp.Add(0);

            // 한 컬럼에 대한 힌트 리스트를 columnHints에 추가
            columnHints.Add(temp);

            // 컬럼 힌트 크기 갱신
            if(columnHintSize < temp.Count)
                columnHintSize = temp.Count;
        }

        // 각 컬럼 힌트 리스트에서 빈 공간을 0으로 채움
        for(int i=0; i<columns; i++)
        {
            int num = columnHintSize-columnHints[i].Count;
            for(int j=0; j<num; j++)
            {
                columnHints[i].Insert(0, 0);
            }
        }

        columnGrid.constraintCount = columns;
        mainGrid.spacing = new Vector2(mainGrid.spacing.x, 50 * (columns + columnHintSize - 2));

        // 힌트 오브젝트 생성
        for(int i=0; i<columns; i++)
        {
            for(int j=0; j<columnHintSize; j++)
            {
                GameObject hint = Instantiate(hintPrefab, columnGrid.transform);
                
                if(columnHints[i][j] == 0)
                    hint.transform.GetChild(0).gameObject.SetActive(false);

                else
                    hint.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = columnHints[i][j].ToString();
            }
        }
        
    }
    // 로우 그리드 설정
    private void RowHintCalculate()
    {
        for(int i=0; i<columns; i++)
        {
            List<int> temp = new List<int>(); // row hint 임시 저장 리스트
            int count = 0; // 연속되는 hint의 수

            // 한 row씩 hint 계산
            for(int j=0; j<rows; j++)
            {
                if(solutions[i*columns + j])
                {
                    count++;
                }

                // 연속되던 hint가 끊기면 이전의 값을 리스트에 저장
                else
                {
                    if(count != 0)
                    {
                        temp.Add(count);
                        count = 0;
                    }
                }
            }

            // 한 row가 모두 끝났을 때

            // 마지막 연속되는 hint를 리스트에 추가
            if(count != 0)
                temp.Add(count);

            // hint가 아예 없는 row일 경우 0을 추가
            if(temp.Count == 0)
                temp.Add(0);

            // 한 로우에 대한 힌트 리스트를 rowwHints에 추가
            rowHints.Add(temp);

            // 로우 힌트 크기 갱신
            if(rowHintSize < temp.Count)
                rowHintSize = temp.Count;
        }

        for(int i=0; i<rows; i++)
        {
            int num = rowHintSize-rowHints[i].Count;
            for(int j=0; j<num; j++)
            {
                rowHints[i].Insert(0, 0);
            }
        }

        rowGrid.constraintCount = rowHintSize;
        mainGrid.spacing = new Vector2(50 * (rows + rowHintSize - 2), mainGrid.spacing.y);

        for(int i=0; i<rows; i++)
        {
            for(int j=0; j<rowHintSize; j++)
            {
                GameObject hint = Instantiate(hintPrefab, rowGrid.transform);
                
                if(rowHints[i][j] == 0)
                    hint.transform.GetChild(0).gameObject.SetActive(false);

                else
                    hint.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = rowHints[i][j].ToString();
            }
        }
    }

    // 셀 생성
    private void GenerateGrid()
    {
        for(int i=0; i<rows; i++)
        {
            for(int j=0; j<columns; j++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);
                cell.GetComponent<Cell>().column = j;
                cell.GetComponent<Cell>().row = i;
                cell.transform.name = (i * columns + j).ToString();
            }
        }
    }

    // 정답 확인
    public void CheckSolution()
    {
        bool isCorrect = true;

        
        for(int i=0; i<solutions.Length; i++)
        {
            Cell cell = transform.GetChild(i).GetComponent<Cell>();
            if (cell.IsFilled() != solutions[i])
            {
                isCorrect = false;
                break;
            }
        }
        

        if(isCorrect)
        {
            stageClear = true;
            for(int i=0; i<solutions.Length; i++)
            {
                transform.GetChild(i).GetComponent<Cell>().Clear(solutions[i]);
            }
            logicClearPanel.SetActive(true);
        }
    }

    // 셀 강조 표시
    public void CellHighlight(int _column, int _row)
    {
        current_Column = _column;
        current_Row = _row;
        
        for(int i=0; i<solutions.Length; i++)
        {
            Cell cell = transform.GetChild(i).GetComponent<Cell>();
            if(cell.state == 1)
                continue;
            
            else if (cell.column == current_Column && cell.row == current_Row)
                cell.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f);

            else if(cell.column == current_Column || cell.row == current_Row)
                cell.GetComponent<Image>().color = Color.gray;
            
            else
                cell.GetComponent<Image>().color = Color.white;
            
        }
    }

    public void HintAutoCheck(int _column, int _row)
    {
        CheckColumnHints(_column);
        CheckRowHints(_row);
    }
    private void CheckColumnHints(int _column)
    {
        List<int> currentColumnHints = columnHints[_column];
        currentColumnHints.RemoveAll(i => i==0);

        List<int> filledBlocks = new List<int>();
        int count = 0;

        // Count the filled blocks in the column
        for (int i = 0; i < rows; i++)
        {
            Cell cell = transform.GetChild(i * columns + _column).GetComponent<Cell>();
            if (cell.IsFilled())
            {
                count++;
            }
            else if (count > 0)
            {
                filledBlocks.Add(count);
                count = 0;
            }
        }
        if (count > 0)
        {
            filledBlocks.Add(count);
        }

        // Compare filled blocks with hints
        bool columnCorrect = CompareHints(currentColumnHints, filledBlocks);

        // Update hints
        for (int i = 0; i < columnHintSize; i++)
        {
            Hint hint = columnGrid.transform.GetChild(_column * columnHintSize + i).GetComponent<Hint>();
            hint.AutoCheck(columnCorrect);
        }
    }
    private void CheckRowHints(int _row)
    {
        List<int> currentRowHints = rowHints[_row];
        currentRowHints.RemoveAll(i => i==0);

        List<int> filledBlocks = new List<int>();
        int count = 0;

        // Count the filled blocks in the row
        for (int i = 0; i < columns; i++)
        {
            Cell cell = transform.GetChild(_row * columns + i).GetComponent<Cell>();
            if (cell.IsFilled())
            {
                count++;
            }
            else if (count > 0)
            {
                filledBlocks.Add(count);
                count = 0;
            }
        }
        if (count > 0)
        {
            filledBlocks.Add(count);
        }

        // Compare filled blocks with hints
        bool rowCorrect = CompareHints(currentRowHints, filledBlocks);

        // Update hints
        for (int i = 0; i < rowHintSize; i++)
        {
            Hint hint = rowGrid.transform.GetChild(_row * rowHintSize + i).GetComponent<Hint>();
            hint.AutoCheck(rowCorrect);
        }
    }
    private bool CompareHints(List<int> hints, List<int> filledBlocks)
    {
        if (hints.Count != filledBlocks.Count) 
        {
            return false;
        }

        for (int i = 0; i < hints.Count; i++)
        {
            if (hints[i] != filledBlocks[i]) 
            {
                return false;
            }
        }
        
        return true;
    }

    public void SaveState()
    {
        CellState[] currentState = new CellState[rows * columns];
        for (int i = 0; i < rows * columns; i++)
        {
            Cell cell = transform.GetChild(i).GetComponent<Cell>();
            currentState[i] = new CellState(cell.state, cell.column, cell.row);
        }
        undoStack.Push(currentState);
        redoStack.Clear(); // 새로운 상태 저장 시, redo 스택 초기화
    }

    public void Undo()
    {
        if(!stageClear)
        {
            if(undoStack.Count > 1)
            {
                redoStack.Push(undoStack.Pop());
                ApplyState(undoStack.Peek());
    
                foreach(Transform cell in transform)
                {
                    cell.GetComponent<Cell>().UpdateCell();
                    cell.GetComponent<Image>().color =  cell.GetComponent<Cell>().state == 1? 
                        new Color(0.25f,0.25f,0.25f) : Color.white;
                }
    
                for(int i = 0; i < columns; i++)
                {
                    CheckColumnHints(i);
                }
                for(int i = 0; i < rows; i++)
                {
                    CheckRowHints(i);
                }
            }
            CellHighlight(-1, -1);
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[1]);
        }
    }

    public void Redo()
    {
        if(!stageClear)
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push(redoStack.Pop());
                ApplyState(undoStack.Peek());

                foreach(Transform cell in transform)
                {
                    cell.GetComponent<Cell>().UpdateCell();
                    cell.GetComponent<Image>().color =  cell.GetComponent<Cell>().state == 1? 
                        new Color(0.25f,0.25f,0.25f) : Color.white;
                }

                for(int i = 0; i < columns; i++)
                {
                    CheckColumnHints(i);
                }
                for(int i = 0; i < rows; i++)
                {
                    CheckRowHints(i);
                }
            }
            CellHighlight(-1, -1);
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        }
    }

    public void Reset()
    {
        if(!stageClear)
        {
            foreach(Transform item in transform)
            {
                Cell cell = item.GetComponent<Cell>();
                cell.state = 0;
                cell.UpdateCell();
            }
            foreach(Transform item in columnGrid.transform)
            {
                Hint hint = item.GetComponent<Hint>();

                if(hint.isAutoChecked)
                    hint.AutoCheck(false);
                if(hint.isChecked)
                {
                    hint.isChecked = false;
                    hint.Check();
                }
            }
            foreach(Transform item in rowGrid.transform)
            {
                Hint hint = item.GetComponent<Hint>();

                if(hint.isAutoChecked)
                    hint.AutoCheck(false);
                if(hint.isChecked)
                {
                    hint.isChecked = false;
                    hint.Check();
                }
            }
            SaveState();
            CellHighlight(-1, -1);
            AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[8]);
        }
    }

    private void ApplyState(CellState[] state)
    {
        for (int i = 0; i < state.Length; i++)
        {
            Cell cell = transform.GetChild(i).GetComponent<Cell>();
            cell.state = state[i].state;
            cell.UpdateCell();
        }
        CheckSolution();
    }

    // 정답률이 95% 이상일 때, 바로 정답 처리 후 종료
    public void CheckButton()
    {
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
        if (IsValidSolution())
        {
            stageClear = true;
            for (int i = 0; i < solutions.Length; i++)
            {
                transform.GetChild(i).GetComponent<Cell>().Clear(solutions[i]);
            }
            logicClearPanel.SetActive(true);
        }
    }

    private bool IsValidSolution()
    {
        // 각 열 검증
        for (int column = 0; column < columns; column++)
        {
            List<int> filledBlocks = new List<int>();
            int count = 0;

            for (int row = 0; row < rows; row++)
            {
                Cell cell = transform.GetChild(row * columns + column).GetComponent<Cell>();
                if (cell.IsFilled())
                {
                    count++;
                }
                else if (count > 0)
                {
                    filledBlocks.Add(count);
                    count = 0;
                }
            }
            if (count > 0)
                filledBlocks.Add(count);

            // 힌트와 블록 비교
            List<int> currentColumnHints = columnHints[column];
            currentColumnHints.RemoveAll(i => i == 0);
            if (!CompareHints(currentColumnHints, filledBlocks))
                return false;
        }

        // 각 행 검증
        for (int row = 0; row < rows; row++)
        {
            List<int> filledBlocks = new List<int>();
            int count = 0;

            for (int column = 0; column < columns; column++)
            {
                Cell cell = transform.GetChild(row * columns + column).GetComponent<Cell>();
                if (cell.IsFilled())
                {
                    count++;
                }
                else if (count > 0)
                {
                    filledBlocks.Add(count);
                    count = 0;
                }
            }
            if (count > 0)
                filledBlocks.Add(count);

            // 힌트와 블록 비교
            List<int> currentRowHints = rowHints[row];
            currentRowHints.RemoveAll(i => i == 0);
            if (!CompareHints(currentRowHints, filledBlocks))
                return false;
        }

        return true;
    }

    public void HomeButton()
    {
        GameManager.instance.isStageSelected = false;
        GameManager.instance.isStageClear = false;
        GameManager.instance.isSequnceActivate = false;
        //GameManager.instance.deckList.Clear();
        StartCoroutine(SceneLoader.instance.LoadScene(SceneLoader.instance.ReturnLoadScene(), 1));
        AudioManager.instance.PlaySfx(AudioManager.instance.sfxClips_Main[2]);
    }
}
