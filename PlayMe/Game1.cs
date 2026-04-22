using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace PlayMe;

public class Game1 : Core
{
    private Sprite _title_board;
    private AnimatedSprite _button;

    public Game1() : base("Play me", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/title-atlas-definition.xml");

        // retrieve the slime region from the atlas.
        _title_board = atlas.CreateSprite("title_board");

        // retrieve the bat region from the atlas.
        _button = atlas.CreateAnimatedSprite("button-animation");
        _button.Scale = new Vector2(2.0f, 2.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        // Update the bat animated sprite.
        _button.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

         // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime sprite
        _title_board.Draw(SpriteBatch, Vector2.Zero);

        // Draw the bat sprite
        _button.Draw(SpriteBatch, new Vector2(_title_board.Width + 10, 0));

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}