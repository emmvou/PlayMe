using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Scenes;
using MonoGameLibrary.Graphics;

namespace PlayMe.Scenes;

public class TitleScene : Scene
{
    private Sprite _title_board;
    private AnimatedSprite _button;
    private const string PLAY_TEXT = "Play";
    private const string ME_TEXT = "Me";
    private const string PRESS_ENTER_TEXT = "Press Enter To Start";

    // The font to use to render normal text.
    private SpriteFont _font;

    // The font used to render the title text.
    private SpriteFont _font5x;

    // The position to draw the play text at.
    private Vector2 _playTextPos;

    // The origin to set for the play text.
    private Vector2 _playTextOrigin;

    // The position to draw the me text at.
    private Vector2 _meTextPos;

    // The origin to set for the me text.
    private Vector2 _meTextOrigin;

    // The position to draw the press enter text at.
    private Vector2 _pressEnterPos;

    // The origin to set for the press enter text when drawing it.
    private Vector2 _pressEnterOrigin;

    // The texture used for the background pattern.
    private Texture2D _backgroundPattern;

    // The destination rectangle for the background pattern to fill.
    private Rectangle _backgroundDestination;

    // The offset to apply when drawing the background pattern so it appears to
    // be scrolling.
    private Vector2 _backgroundOffset;

    // The speed that the background pattern scrolls.
    private float _scrollSpeed = 50.0f;

    private SoundEffect _uiSoundEffect;
    private Panel _titleScreenButtonsPanel;
    private Panel _optionsPanel;
    private Button _optionsButton;
    private Button _optionsBackButton;

    public override void Initialize()
    {
        // LoadContent is called during base.Initialize().
        base.Initialize();

        // While on the title screen, we can enable exit on escape so the player
        // can close the game by pressing the escape key.
        Core.ExitOnEscape = true;

        // Set the position and origin for the play text.
        Vector2 size = _font5x.MeasureString(PLAY_TEXT);
        _playTextPos = new Vector2(640, 100);
        _playTextOrigin = size * 0.5f;

        // Set the position and origin for the me text.
        size = _font5x.MeasureString(ME_TEXT);
        _meTextPos = new Vector2(757, 207);
        _meTextOrigin = size * 0.5f;

        // Set the position and origin for the press enter text.
        size = _font.MeasureString(PRESS_ENTER_TEXT);
        _pressEnterPos = new Vector2(640, 620);
        _pressEnterOrigin = size * 0.5f;

        // Initialize the offset of the background pattern at zero.
        _backgroundOffset = Vector2.Zero;

        // Set the background pattern destination rectangle to fill the entire
        // screen background.
        _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

        InitializeUI();
    }

    public override void LoadContent()
    {
        // Load the font for the standard text.
        _font = Core.Content.Load<SpriteFont>("fonts/04B_30");

        // Load the font for the title text.
        _font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/title-atlas-definition.xml");

        // retrieve the slime region from the atlas.
        _title_board = atlas.CreateSprite("title_board");

        // retrieve the bat region from the atlas.
        _button = atlas.CreateAnimatedSprite("button-animation");
        _button.Scale = new Vector2(0.5f, 0.5f);

        // Load the background pattern texture.
        _backgroundPattern = Content.Load<Texture2D>("images/background-pattern");

        // Load the sound effect to play when ui actions occur.
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
    }

    public override void Update(GameTime gameTime)
    {
        // If the user presses enter, switch to the game scene.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
        {
            Core.ChangeScene(new GameScene());
        }

        // Update the button animated sprite.
        _button.Update(gameTime);

        // Update the offsets for the background pattern wrapping so that it
        // scrolls down and to the right.
        float offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        _backgroundOffset.X -= offset;
        _backgroundOffset.Y -= offset;

        // Ensure that the offsets do not go beyond the texture bounds so it is
        // a seamless wrap.
        _backgroundOffset.X %= _backgroundPattern.Width;
        _backgroundOffset.Y %= _backgroundPattern.Height;

        GumService.Default.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

        // Draw the background pattern first using the PointWrap sampler state.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
        Core.SpriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
        Core.SpriteBatch.End();

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the title sprite
        _title_board.Draw(Core.SpriteBatch, Vector2.Zero);

        // Draw the button sprite
        _button.Draw(Core.SpriteBatch, new Vector2(_button.Width + 10, 0));

        // // The color to use for the drop shadow text.
        // Color dropShadowColor = Color.Black * 0.5f;

        // // Draw the play text slightly offset from it is original position and
        // // with a transparent color to give it a drop shadow.
        // Core.SpriteBatch.DrawString(_font5x, PLAY_TEXT, _playTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // // Draw the play text on top of that at its original position.
        // Core.SpriteBatch.DrawString(_font5x, PLAY_TEXT, _playTextPos, Color.White, 0.0f, _playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // // Draw the me text slightly offset from it is original position and
        // // with a transparent color to give it a drop shadow.
        // Core.SpriteBatch.DrawString(_font5x, ME_TEXT, _meTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _meTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // // Draw the me text on top of that at its original position.
        // Core.SpriteBatch.DrawString(_font5x, ME_TEXT, _meTextPos, Color.White, 0.0f, _meTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // // Draw the press enter text.
        // Core.SpriteBatch.DrawString(_font, PRESS_ENTER_TEXT, _pressEnterPos, Color.White, 0.0f, _pressEnterOrigin, 1.0f, SpriteEffects.None, 0.0f);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        
    if (_titleScreenButtonsPanel.IsVisible)
    {
        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // The color to use for the drop shadow text.
        Color dropShadowColor = Color.Black * 0.5f;

        // Draw the Dungeon text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow
        Core.SpriteBatch.DrawString(_font5x, PLAY_TEXT, _playTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Dungeon text on top of that at its original position
        Core.SpriteBatch.DrawString(_font5x, PLAY_TEXT, _playTextPos, Color.White, 0.0f, _playTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Slime text slightly offset from it is original position and
        // with a transparent color to give it a drop shadow
        Core.SpriteBatch.DrawString(_font5x, ME_TEXT, _meTextPos + new Vector2(10, 10), dropShadowColor, 0.0f, _meTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Draw the Slime text on top of that at its original position
        Core.SpriteBatch.DrawString(_font5x, ME_TEXT, _meTextPos, Color.White, 0.0f, _meTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }

    GumService.Default.Draw();
    }

    private void CreateTitlePanel()
    {
        // Create a container to hold all of our buttons
        _titleScreenButtonsPanel = new Panel();
        _titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _titleScreenButtonsPanel.AddToRoot();

        var startButton = new Button();
        startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        startButton.X = 50;
        startButton.Y = -12;
        startButton.Width = 70;
        startButton.Text = "Start";
        startButton.Click += HandleStartClicked;
        _titleScreenButtonsPanel.AddChild(startButton);

        _optionsButton = new Button();
        _optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsButton.X = -50;
        _optionsButton.Y = -12;
        _optionsButton.Width = 70;
        _optionsButton.Text = "Options";
        _optionsButton.Click += HandleOptionsClicked;
        _titleScreenButtonsPanel.AddChild(_optionsButton);

        startButton.IsFocused = true;
    }

    private void HandleStartClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Change to the game scene to start the game.
        Core.ChangeScene(new GameScene());
    }

    private void HandleOptionsClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        
        // Set the title panel to be invisible.
        _titleScreenButtonsPanel.IsVisible = false;

        // Set the options panel to be visible.
        _optionsPanel.IsVisible = true;

        // Give the back button on the options panel focus.
        _optionsBackButton.IsFocused = true;
    }

    private void CreateOptionsPanel()
    {
        _optionsPanel = new Panel();
        _optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
        _optionsPanel.IsVisible = false;
        _optionsPanel.AddToRoot();

        var optionsText = new TextRuntime();
        optionsText.X = 10;
        optionsText.Y = 10;
        optionsText.Text = "OPTIONS";
        _optionsPanel.AddChild(optionsText);

        var musicLabel = new Label();
        musicLabel.Text = "Music";
        musicLabel.X = 35;
        musicLabel.Y = 35;
        _optionsPanel.AddChild(musicLabel);

        var musicSlider = new Slider();
        musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
        musicSlider.Y = 30f;
        musicSlider.Minimum = 0;
        musicSlider.Maximum = 1;
        musicSlider.Value = Core.Audio.SongVolume;
        musicSlider.SmallChange = .1;
        musicSlider.LargeChange = .2;
        musicSlider.ValueChanged += HandleMusicSliderValueChanged;
        musicSlider.ValueChangeCompleted += HandleMusicSliderValueChangeCompleted;
        _optionsPanel.AddChild(musicSlider);

        var sfxLabel = new Label();
        sfxLabel.Text = "SFX";
        sfxLabel.X = 20;
        sfxLabel.Y = 80;
        _optionsPanel.AddChild(sfxLabel);

        var sfxSlider = new Slider();
        sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
        sfxSlider.Y = 93;
        sfxSlider.Minimum = 0;
        sfxSlider.Maximum = 1;
        sfxSlider.Value = Core.Audio.SoundEffectVolume;
        sfxSlider.SmallChange = .1;
        sfxSlider.LargeChange = .2;
        sfxSlider.ValueChanged += HandleSfxSliderChanged;
        sfxSlider.ValueChangeCompleted += HandleSfxSliderChangeCompleted;
        _optionsPanel.AddChild(sfxSlider);

        _optionsBackButton = new Button();
        _optionsBackButton.Text = "BACK";
        _optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        _optionsBackButton.X = -28f;
        _optionsBackButton.Y = -10f;
        _optionsBackButton.Click += HandleOptionsButtonBack;
        _optionsPanel.AddChild(_optionsBackButton);
    }

    private void HandleSfxSliderChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global sound effect volume to the value of the slider.;
        Core.Audio.SoundEffectVolume = (float)slider.Value;
    }

    private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
    {
        // Play the UI Sound effect so the player can hear the difference in audio.
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleMusicSliderValueChanged(object sender, EventArgs args)
    {
        // Intentionally not playing the UI sound effect here so that it is not
        // constantly triggered as the user adjusts the slider's thumb on the
        // track.

        // Get a reference to the sender as a Slider.
        var slider = (Slider)sender;

        // Set the global song volume to the value of the slider.
        Core.Audio.SongVolume = (float)slider.Value;
    }

    private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    private void HandleOptionsButtonBack(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Set the title panel to be visible.
        _titleScreenButtonsPanel.IsVisible = true;

        // Set the options panel to be invisible.
        _optionsPanel.IsVisible = false;

        // Give the options button on the title panel focus since we are coming
        // back from the options screen.
        _optionsButton.IsFocused = true;
    }

    private void InitializeUI()
    {
        // Clear out any previous UI in case we came here from
        // a different screen:
        GumService.Default.Root.Children.Clear();

        CreateTitlePanel();
        CreateOptionsPanel();
    }

}
