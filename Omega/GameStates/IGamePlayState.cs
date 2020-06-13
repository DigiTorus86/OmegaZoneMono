
namespace Omega.GameStates
{
    public interface IGamePlayState
    {
        void SetUpNewGame(GameOptions options);
        void LoadExistingGame();
        void StartGame();
    }
}
