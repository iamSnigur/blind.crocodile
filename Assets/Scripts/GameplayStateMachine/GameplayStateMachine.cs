using BlindCrocodile.Core.StateMachine;

namespace BlindCrocodile.GameplayStates
{
    public class GameplayStateMachine : AbstractStateMachine<IGameplayState>
    {
        // main menu state ?
        // InLobbyState - when player joins the lobby (when player is not ready to play)
        // PreRoundState - state for players which are waiting for drawing
        // RoundEndState - show round's summary

        //          Ready - just add flag to player's object IsReady
        // PlayerRole - Artist, Guessor

        // (for players who Guessor)
        // DrawingState
        // ComparisonState

        // (for player who Artist)
        // Art preparation
        // wait

        public GameplayStateMachine()
        {
            _states = new()
            {
                [typeof(InLobbyState)] = new InLobbyState(),
                [typeof(PreRoundState)] = new PreRoundState(),
                [typeof(DrawingState)] = new DrawingState(),
                [typeof(ComparisonState)] = new ComparisonState(),
                [typeof(ArtistSatisfactionState)] = new ArtistSatisfactionState(),
                [typeof(RoundEndState)] = new RoundEndState(),
            };
        }
    }
}