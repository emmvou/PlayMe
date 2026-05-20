using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;

namespace PlayMe;

public class Game1 : Core
{
    private Sprite _title_board;
    private AnimatedSprite _button;
    private Sprite _character;
    private AnimatedSprite _bullet;

    // Tracks the position of the character.
    private Vector2 _characterPosition;

    // Speed multiplier when moving.
    private const float MOVEMENT_SPEED = 5.0f;

    // Tracks the position of the character.
    private Vector2 _bulletPosition;

    // Tracks the velocity of the bullet.
    private Vector2 _bulletVelocity;

    private Keys _up_key = Keys.Z;
    private Keys _down_key = Keys.S;
    private Keys _left_key = Keys.Q;
    private Keys _right_key = Keys.D;

    public Game1() : base("Play me", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();

        // Set the initial position of the bullet to be 10px
        // to the right of the slime.
        _bulletPosition = new Vector2(_character.Width + 10, 0);

        // Assign the initial random velocity to the bat.
        AssignRandomBulletVelocity();
    }

    protected override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/title-atlas-definition.xml");

        // retrieve the slime region from the atlas.
        _title_board = atlas.CreateSprite("title_board");

        // retrieve the bat region from the atlas.
        _button = atlas.CreateAnimatedSprite("button-animation");
        _button.Scale = new Vector2(0.5f, 0.5f);

        _character = atlas.CreateSprite("character");

        _bullet = atlas.CreateAnimatedSprite("bullet-animation");
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here

        // Update the button animated sprite.
        _button.Update(gameTime);
        // Update the bullet animated sprite.
        _bullet.Update(gameTime);

        // Check for keyboard input and handle it.
        CheckKeyboardInput();

        // Check for gamepad input and handle it.
        CheckGamePadInput();

        // Create a bounding rectangle for the screen.
        Rectangle screenBounds = new Rectangle(
            0,
            0,
            GraphicsDevice.PresentationParameters.BackBufferWidth,
            GraphicsDevice.PresentationParameters.BackBufferHeight
        );

        // Creating a bounding circle for the character
        Circle characterBounds = new Circle(
            (int)(_characterPosition.X + (_character.Width * 0.5f)),
            (int)(_characterPosition.Y + (_character.Height * 0.5f)),
            (int)(_character.Width * 0.5f)
        );

        // Use distance based checks to determine if the character is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // move it back inside.
        if (characterBounds.Left < screenBounds.Left)
        {
            _characterPosition.X = screenBounds.Left;
        }
        else if (characterBounds.Right > screenBounds.Right)
        {
            _characterPosition.X = screenBounds.Right - _character.Width;
        }

        if (characterBounds.Top < screenBounds.Top)
        {
            _characterPosition.Y = screenBounds.Top;
        }
        else if (characterBounds.Bottom > screenBounds.Bottom)
        {
            _characterPosition.Y = screenBounds.Bottom - _character.Height;
        }

        // Calculate the new position of the bullet based on the velocity.
        Vector2 newBulletPosition = _bulletPosition + _bulletVelocity;

        // Create a bounding circle for the bullet.
        Circle bulletBounds = new Circle(
            (int)(newBulletPosition.X + (_bullet.Width * 0.5f)),
            (int)(newBulletPosition.Y + (_bullet.Height * 0.5f)),
            (int)(_bullet.Width * 0.5f)
        );

        Vector2 normal = Vector2.Zero;

        // Use distance based checks to determine if the bullet is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // reflect it about the screen edge normal.
        if (bulletBounds.Left < screenBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBulletPosition.X = screenBounds.Left;
        }
        else if (bulletBounds.Right > screenBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBulletPosition.X = screenBounds.Right - _bullet.Width;
        }

        if (bulletBounds.Top < screenBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBulletPosition.Y = screenBounds.Top;
        }
        else if (bulletBounds.Bottom > screenBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBulletPosition.Y = screenBounds.Bottom - _bullet.Height;
        }

        // If the normal is anything but Vector2.Zero, this means the bullet had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        if (normal != Vector2.Zero)
        {
            normal.Normalize();
            _bulletVelocity = Vector2.Reflect(_bulletVelocity, normal);
        }

        _bulletPosition = newBulletPosition;

        if (characterBounds.Intersects(bulletBounds))
        {
            // Divide the width and height of the screen into equal columns and
            // rows based on the width and height of the bullet.
            int totalColumns = GraphicsDevice.PresentationParameters.BackBufferWidth / (int)_bullet.Width;
            int totalRows = GraphicsDevice.PresentationParameters.BackBufferHeight / (int)_bullet.Height;

            // Choose a random row and column based on the total number of each
            int column = Random.Shared.Next(0, totalColumns);
            int row = Random.Shared.Next(0, totalRows);

            // Change the bullet position by setting the x and y values equal to
            // the column and row multiplied by the width and height.
            _bulletPosition = new Vector2(column * _bullet.Width, row * _bullet.Height);

            // Assign a new random velocity to the bullet
            AssignRandomBulletVelocity();
        }

        base.Update(gameTime);
    }

    private void AssignRandomBulletVelocity()
    {
        // Generate a random angle.
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector.
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        // Multiply the direction vector by the movement speed.
        _bulletVelocity = direction * MOVEMENT_SPEED;
    }

    private void CheckKeyboardInput()
    {
        // Get the state of keyboard input
        KeyboardState keyboardState = Keyboard.GetState();

        // If the space key is held down, the movement speed increases by 1.5
        float speed = MOVEMENT_SPEED;
        if (Input.Keyboard.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;
        }

        // If the W or Up keys are down, move the slime up on the screen.
        if (Input.Keyboard.IsKeyDown(_up_key) || Input.Keyboard.IsKeyDown(Keys.Up))
        {
            _characterPosition.Y -= speed;
        }

        // if the S or Down keys are down, move the slime down on the screen.
        if (Input.Keyboard.IsKeyDown(_down_key) || Input.Keyboard.IsKeyDown(Keys.Down))
        {
            _characterPosition.Y += speed;
        }

        // If the A or Left keys are down, move the slime left on the screen.
        if (Input.Keyboard.IsKeyDown(_left_key) || Input.Keyboard.IsKeyDown(Keys.Left))
        {
            _characterPosition.X -= speed;
        }

        // If the D or Right keys are down, move the slime right on the screen.
        if (Input.Keyboard.IsKeyDown(_right_key) || Input.Keyboard.IsKeyDown(Keys.Right))
        {
            _characterPosition.X += speed;
        }
    }

    private void CheckGamePadInput()
    {
        GamePadInfo gamePadOne = Input.GamePads[(int)PlayerIndex.One];

        // If the A button is held down, the movement speed increases by 1.5
        // and the gamepad vibrates as feedback to the player.
        float speed = MOVEMENT_SPEED;
        if (gamePadOne.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            gamePadOne.SetVibration(1.0f, TimeSpan.FromSeconds(1));
        }
        else
        {
            gamePadOne.StopVibration();
        }

        // Check thumbstick first since it has priority over which gamepad input
        // is movement.  It has priority since the thumbstick values provide a
        // more granular analog value that can be used for movement.
        if (gamePadOne.LeftThumbStick != Vector2.Zero)
        {
            _characterPosition.X += gamePadOne.LeftThumbStick.X * speed;
            _characterPosition.Y -= gamePadOne.LeftThumbStick.Y * speed;
        }
        else
        {
            // If DPadUp is down, move the slime up on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadUp))
            {
                _characterPosition.Y -= speed;
            }

            // If DPadDown is down, move the slime down on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadDown))
            {
                _characterPosition.Y += speed;
            }

            // If DPapLeft is down, move the slime left on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadLeft))
            {
                _characterPosition.X -= speed;
            }

            // If DPadRight is down, move the slime right on the screen.
            if (gamePadOne.IsButtonDown(Buttons.DPadRight))
            {
                _characterPosition.X += speed;
            }
        }
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

         // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the title sprite
        _title_board.Draw(SpriteBatch, Vector2.Zero);

        _character.Draw(SpriteBatch, _characterPosition);

        // Draw the button sprite
        _button.Draw(SpriteBatch, new Vector2(_character.Width + 10, 0));

        // Draw the bullet sprite.
        _bullet.Draw(SpriteBatch, _bulletPosition);

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}