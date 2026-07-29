using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;
using PlayMe.Scenes;

namespace PlayMe;

public class Game1 : Core
{    
    // The background theme song
    private Song _themeSong;

    
    public Game1() : base("Play me", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();

        // Start playing the background music.
        Audio.PlaySong(_themeSong);

        // Start the game with the title scene.
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        // Load the background theme music.
        _themeSong = Content.Load<Song>("audio/theme");
    }
}