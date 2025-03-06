using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CursorController : MonoBehaviour
{
    public Tilemap tilemap; // タイルマップ
    public GameObject cursorPrefab; // カーソルのプレハブ
    private GameObject cursorInstance;
    private Vector3Int currentPosition; // カーソルの現在位置
    
    [SerializeField] private float moveInterval = 0.2f; // 移動の間隔(秒)
    //[SerializeField] private CameraController cameraController;

    private void Start() {
        //InitializeCursor();
        //cameraController.SetDefaultCameraFollow(cursorInstance.transform);
    }

    /// <summary>
    /// カーソルを初期化し、キー入力の購読を開始
    /// </summary>
    /// <param name="initialPosition">default = new Vector3Int(0, 0, 0)</param>
    public GameObject InitializeCursor(GameManager gameManager, Vector3Int initialPosition = default) {
        // 初期カーソル位置設定
        currentPosition = initialPosition;
        cursorInstance = Instantiate(cursorPrefab, tilemap.CellToWorld(currentPosition), Quaternion.identity);

        this.UpdateAsObservable()
            .Where(_ => gameManager.CommandState.Value == CommandStateType.CursorMove)
            .Where(_ => Input.GetKey(KeyCode.UpArrow))
            .ThrottleFirst(System.TimeSpan.FromSeconds(moveInterval))
            .Subscribe(_ => MoveCursor(Vector3Int.up));

        this.UpdateAsObservable()
            .Where(_ => gameManager.CommandState.Value == CommandStateType.CursorMove)
            .Where(_ => Input.GetKey(KeyCode.DownArrow))
            .ThrottleFirst(System.TimeSpan.FromSeconds(moveInterval))
            .Subscribe(_ => MoveCursor(Vector3Int.down));

        this.UpdateAsObservable()
            .Where(_ => gameManager.CommandState.Value == CommandStateType.CursorMove)
            .Where(_ => Input.GetKey(KeyCode.LeftArrow))
            .ThrottleFirst(System.TimeSpan.FromSeconds(moveInterval))
            .Subscribe(_ => MoveCursor(Vector3Int.left));

        this.UpdateAsObservable()
            .Where(_ => gameManager.CommandState.Value == CommandStateType.CursorMove)
            .Where(_ => Input.GetKey(KeyCode.RightArrow))
            .ThrottleFirst(System.TimeSpan.FromSeconds(moveInterval))
            .Subscribe(_ => MoveCursor(Vector3Int.right));

        //cameraController.SetDefaultCameraFollow(cursorInstance.transform);

        return cursorInstance;
    }

    //private void Update() {
    //    // 十字キーでカーソルを移動
    //    if (Input.GetKey(KeyCode.UpArrow)) {
    //        MoveCursor(Vector3Int.up);
    //    }
    //    if (Input.GetKey(KeyCode.DownArrow)) {
    //        MoveCursor(Vector3Int.down);
    //    }
    //    if (Input.GetKey(KeyCode.LeftArrow)) {
    //        MoveCursor(Vector3Int.left);
    //    }
    //    if (Input.GetKey(KeyCode.RightArrow)) {
    //        MoveCursor(Vector3Int.right);
    //    }
    //}

    /// <summary>
    /// カーソルの移動
    /// </summary>
    /// <param name="direction"></param>
    private void MoveCursor(Vector3Int direction) {
        // 新しい位置を計算
        currentPosition += direction;

        // タイルマップの範囲内に収める処理(例: タイルマップの範囲が決まっていればその範囲内に制限)
        if (IsPositionValid(currentPosition)) {
            cursorInstance.transform.position = tilemap.CellToWorld(currentPosition);

            // 移動用にタイルの色が変更されている場合には、元の色に戻す

        }
    }

    /// <summary>
    /// タイルマップの範囲内かどうかをチェック
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private bool IsPositionValid(Vector3Int position) {
        // タイルが存在するか確認
        return tilemap.HasTile(position);
    }

    public Vector3Int GetCursorPosition() {
        return currentPosition;
    }
}