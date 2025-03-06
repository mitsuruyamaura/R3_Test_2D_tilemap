// Unity Command Pattern
// https://zenn.dev/yukio_1130/books/bbea639eb0827b/viewer/88e417
// https://qiita.com/kiku09020/items/bc38de3d2ee6baaa3bcd

public interface ICommand {
    void Execute();
    void Undo();
    void Release();
    void Initialize(params object[] args);
}