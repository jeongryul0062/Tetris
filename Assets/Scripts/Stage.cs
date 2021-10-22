using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage : MonoBehaviour
{
    [Header("Source")]
    public GameObject tilePrefab;
    public Transform backgroundNode;
    public Transform boardNode;
    public Transform tetrominoNode;
    public GameObject gameoverPanel;

    [Header("Setting")]
    public int boardWidth = 10;    //너비설정
    public int boardHeight = 20;   //높이설정
    public float fallingCycle = 0.5f; //테트리스 블록 낙하 주기
    public float moveCycle = 0.1f;



    private int halfWidth;
    private int halfHeight;
    private float prevFallTime;
    private float nextMoveTime;

    private bool isPause;

    private int currIndex;

    //Start -> 배경, 첫 테트로미노 생성
    private void Start(){
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);
        isPause = false;
        gameoverPanel.SetActive(false);

        CreateBackground();

        for (int i = 0; i < boardHeight; ++i){
            var col = new GameObject((boardHeight - i - 1).ToString());
            col.transform.position = new Vector3(0, halfHeight - i, 0);
            col.transform.parent = boardNode;
        }

        currIndex = CreateTetromino();
    }


    //Update 매 프레임마다
    void Update(){
        if(gameoverPanel.activeSelf){
            if(Input.GetMouseButtonDown(0)){
                SceneManager.LoadScene(0);
            }
        }
        else{
            Vector3 moveDir = Vector3.zero;  //이동 여부
            bool isRotate = false;           //회전 여부

            //조작
            //좌우 방향키 -> 이동
            //위 방향키 -> 회전
            //아래 방향키 -> 아래로 이동
            //이동은 누르고 있으면 연속으로 가능
            //회전은 누를때마다 한번만
            //스페이스바 -> 바닥까지 한번에 이동
            //esc -> 일시정지/해제
            
            if(Input.GetKey(KeyCode.LeftArrow)){
                if(Time.time > nextMoveTime){
                    nextMoveTime = Time.time + moveCycle;
                    moveDir.x = -1;
                }
            }
            else if(Input.GetKey(KeyCode.RightArrow)){
                if(Time.time > nextMoveTime){
                    nextMoveTime = Time.time + moveCycle;
                    moveDir.x = 1;
                }
            }
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                isRotate = true;
            }

            if (Input.GetKeyDown(KeyCode.Space)){
                while (MoveTetromino(Vector3.down, false)){}
            }
            if (Input.GetKeyDown(KeyCode.Escape)){
                if(!isPause){
                    Time.timeScale = 0;
                    isPause = true;
                    return;
                }
                else{
                    Time.timeScale = 1;
                    isPause = false;
                    return;
                }
            }

            

            //falling cycle마다 fall
            if(Time.time > prevFallTime + (Input.GetKey(KeyCode.DownArrow)?fallingCycle/15:fallingCycle)){
                prevFallTime = Time.time;
                
                moveDir.y = -1;
                isRotate = false;
            }
            //입력에 따라 이동
            if(moveDir != Vector3.zero || isRotate){
                
                MoveTetromino(moveDir,isRotate);
            }
        }
    }


    //타일 인스턴스 생성
    Tile createTile(Transform parent, Vector2 position, Color color, int order = 1){
        var block = Instantiate(tilePrefab);
        block.transform.parent = parent;
        block.transform.localPosition = position;
        
        var tile = block.GetComponent<Tile>();
        tile.color = color;
        tile.sortingOrder = order;

        return tile;
    }

    //배경 타일 생성
    void CreateBackground(){
        Color color = Color.gray;
        
        color.a = 0.5f;
        for(int x = -halfWidth; x<halfWidth; ++x){
            for(int y = halfHeight; y>-halfHeight; --y){
                createTile(backgroundNode, new Vector2(x,y), color, 0);
            }
        }

        color.a = 1.0f;
        for(int y = halfHeight; y>-halfHeight; --y){
            createTile(backgroundNode, new Vector2(-halfWidth-1,y),color,0);
            createTile(backgroundNode, new Vector2(halfWidth,y),color,0);
        }
    }

    //테트로미노 생성
    int CreateTetromino(){
        int index = Random.Range(0,7); //테트로미노 형태 결정

        Color32 color = Color.white;

        tetrominoNode.rotation = Quaternion.identity;

        tetrominoNode.position = new Vector2(0, halfHeight);

        switch (index){
            case 0: // I형
                color = new Color32(80, 188, 223, 255); //하늘색
                createTile(tetrominoNode, new Vector2(-2f, 0f), color);
                createTile(tetrominoNode, new Vector2(-1f, 0f), color);
                createTile(tetrominoNode, new Vector2(0f, 0f), color);
                createTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;
            case 1: // J형
                color = new Color32(8, 8, 189, 255); //파란색
                createTile(tetrominoNode, new Vector2(-1f, 1f), color);
                createTile(tetrominoNode, new Vector2(-1f, 0f), color);
                createTile(tetrominoNode, new Vector2(0f, 0f), color);
                createTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;
            case 2: // L형
                color = new Color32(255, 160, 72, 255); //주황색
                createTile(tetrominoNode, new Vector2(-1f, -1f), color);
                createTile(tetrominoNode, new Vector2(0f, -1f), color);
                createTile(tetrominoNode, new Vector2(1f, -1f), color);
                createTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;
            case 3: // O형
                color = new Color32(255, 212, 0, 255); //노란색
                createTile(tetrominoNode, new Vector2(0f, -1f), color);
                createTile(tetrominoNode, new Vector2(0f, 0f), color);
                createTile(tetrominoNode, new Vector2(1f, -1f), color);
                createTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;
            case 4: // S형
                color = new Color32(129, 193, 71, 255); //연두색
                createTile(tetrominoNode, new Vector2(-1f, -1f), color);
                createTile(tetrominoNode, new Vector2(0f, -1f), color);
                createTile(tetrominoNode, new Vector2(0f, 0f), color);
                createTile(tetrominoNode, new Vector2(1f, 0f), color);
                break;
            case 5: // Z형
                color = new Color32(204, 17, 30, 255); //빨간색
                createTile(tetrominoNode, new Vector2(-1f, 0f), color);
                createTile(tetrominoNode, new Vector2(0f, 0f), color);
                createTile(tetrominoNode, new Vector2(0f, -1f), color);
                createTile(tetrominoNode, new Vector2(1f, -1f), color);
                break;
            case 6: // T형
                color = new Color32(139, 0, 185, 255); //보라색
                createTile(tetrominoNode, new Vector2(-1f, -1f), color);
                createTile(tetrominoNode, new Vector2(0f, -1f), color);
                createTile(tetrominoNode, new Vector2(1f, -1f), color);
                createTile(tetrominoNode, new Vector2(0f, 0f), color);
                break;
        }
        return index;
    }

    private bool hitBoard;
    private bool xLeftOver;
    private bool xRightOver;
    private bool yOver;

    //테트로미노 이동
    bool MoveTetromino(Vector3 moveDir, bool isRotate){
        hitBoard = false;
        xLeftOver = false;
        xRightOver = false;
        yOver = false;
        Vector3 tempPos = tetrominoNode.transform.position;
        Quaternion tempRot = tetrominoNode.transform.rotation;

        tetrominoNode.transform.position += moveDir;
        
        //시계 방향으로 90도 회전
        if(isRotate){
            tetrominoNode.transform.rotation *= Quaternion.Euler(0,0,-90);
        }


        //이동 시에 범위를 벗어났다면 제자리로 되돌림
        //회전 시에 범위를 벗어났다면 회전은 수행하고 범위 내로 되돌림
        //회전 시에 보드 블록에 부딪히는 경우 회전이 수행되지 않음
        if(!CanMove(tetrominoNode)){
            if(isRotate){
                
                if(hitBoard){
                    tetrominoNode.transform.position = tempPos;
                    tetrominoNode.transform.rotation = tempRot;

                    return false;
                }
                
                if(currIndex == 0){
                    int xover = 0;
                    int yover = 0;
                    if(xLeftOver) xover = 2;
                    if(xRightOver) xover = -2;
                    if(yOver) yover = 2;

                    tetrominoNode.transform.position += new Vector3(xover,yover,0);
                }
                else{
                    int xover = 0;
                    int yover = 0;
                    if(xLeftOver) xover = 1;
                    if(xRightOver) xover = -1;
                    if(yOver) yover = 1;
                    tetrominoNode.transform.position += new Vector3(xover,yover,0);
                }
            }
            else{
                tetrominoNode.transform.position = tempPos;
                tetrominoNode.transform.rotation = tempRot;

                if((int)moveDir.y == -1 && (int)moveDir.x == 0 && isRotate == false){
                    AddToBoard(tetrominoNode);
                    CheckBoardCol();
                    currIndex = CreateTetromino();

                    if(!CanMove(tetrominoNode)){
                        gameoverPanel.SetActive(true);
                    }
                }

                
                return false;
            }
        }
        
        
        return true;
    }

    //이동 가능 여부 판정
    bool CanMove(Transform root){
        foreach(Transform node in root){
            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            if(x < 0 || x > boardWidth - 1 || y < 0)
            {
                if(y < 0){
                    yOver=true;
                }
                if(x < 0){
                    xLeftOver=true;
                }
                if(x > boardWidth - 1){
                    xRightOver=true;
                }
                return false;
            }

            var column = boardNode.Find(y.ToString());

            if(column != null && column.Find(x.ToString()) != null){
                hitBoard = true;
                return false;
            }
        }
        return true;
    }

    //바닥에 닿은 테트로미노를 보드로 변환
    void AddToBoard(Transform root){
        while (root.childCount>0){
            var node = root.GetChild(0);

            int x = Mathf.RoundToInt(node.transform.position.x + halfWidth);
            int y = Mathf.RoundToInt(node.transform.position.y + halfHeight - 1);

            node.parent = boardNode.Find(y.ToString());
            node.name = x.ToString();
        }
    }
    void CheckBoardCol(){
        bool isCleared = false;


        //가득찬 행을 체크하여 삭제
        foreach (Transform column in boardNode){
            if(column.childCount == boardWidth){
                foreach(Transform tile in column){
                    Destroy(tile.gameObject);
                }

                column.DetachChildren();
                isCleared = true;
            }
        }


        //빈 행이 생기면 아래로 내려서 채워주기
        if(isCleared){

            for(int i = 1; i < boardNode.childCount; ++i){
                var column = boardNode.Find(i.ToString());

                if(column.childCount == 0) continue;

                int emptyCol = 0;
                for(int j = i - 1; j >= 0; j--){
                    if(boardNode.Find(j.ToString()).childCount == 0){
                        emptyCol++;
                    }
                }

                if(emptyCol > 0){
                    var targetColumn = boardNode.Find((i-emptyCol).ToString());

                    while (column.childCount > 0){
                        Transform tile = column.GetChild(0);
                        tile.parent = targetColumn;
                        tile.transform.position += new Vector3(0, -emptyCol, 0);
                    }
                    column.DetachChildren();
                }
            }
        }
    }
}
