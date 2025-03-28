using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Monogame_Drawing_from_a_SpriteSheet
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D characterSpritesheet, rectangleTexture;
        List<Rectangle> barriers;

        KeyboardState keyboardState;

        int rows, columns;  // The number of rows and columns in the spritesheet
        int frame;          // The frame number (column) in the sequence to draw
        int frames;         // The number of frames for each direction, usually the same as columns
        int directionRow;   // The row number containing the frames for the current direction
        int leftRow, rightRow, upRow, downRow; // The row number of each directional set of frames
        int width;          // The width of each frame
        int height;         // The height of each frame

        float speed;        // How fast the character sprite will travel
        float time;         // used to store elapsed time
        float frameSpeed;   // Sets how fast player frames transition

        Vector2 playerLocation;     // Stored the location of the players collision sprite
        Vector2 playerDirection;    // The directional vector of the player

        Rectangle playerCollisionRect;  // The rectangle that will be used for player collision
        Rectangle playerDrawRect;       // The rectangle that we will scale our player frame into
        Rectangle window;               // Dimensions of the game window

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            this.Window.Title = $"Drawing from a Spritesheet";

            // Sizes the window
            window = new Rectangle(0, 0, 800, 500);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            // Creates obstacles
            barriers = new List<Rectangle>();
            barriers.Add(new Rectangle(100, 100, 30, 150));
            barriers.Add(new Rectangle(10, 400, 300, 30));
            barriers.Add(new Rectangle(400, 200, 200, 30));
            barriers.Add(new Rectangle(700, 450, 30, 100));

            // Aids with processing of spritesheet
            rows = 4;           // Number of rows in the spritesheet
            columns = 9;        // Number of columns n the spritesheet
            upRow = 0;          // Row number with upward facing sprites
            leftRow = 1;        // Row number with leftward facing sprites
            downRow = 2;        // Row number with downward facing sprites
            rightRow = 3;       // Row number with rightward facing sprites
            directionRow = downRow;   // We will start facing downward

            // Timing of frames
            time = 0.0f;        // This is our timer for advancing frames
            frameSpeed = 0.08f; // Time interval of each frame
            frames = 9;         // There are 9 frames for each direction (0-8)
            frame = 0;          // We will start with the idle frame

            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(20, 20, 20, 48);    // The collision rect should always be at the same place as playerLocation
            playerDrawRect = new Rectangle(20, 20, 50, 65);         // We will need to determine the horizontal and vertical translation for this so the hitbox is over the character region of our frame   
            speed = 1.5f;       // Sets how fast our player moves

            UpdateRects();  // Set the offsets of playerDrawRect
            
            base.Initialize();  // Content is loaded here

            // Determine the dimensions of the region of each sprite that will be grabbed from the spritesheet
            // This must be done after the content is loaded so the dimensions of the spritesheet are known
            width = characterSpritesheet.Width / columns;
            height = characterSpritesheet.Height / rows;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            characterSpritesheet = Content.Load<Texture2D>("skeleton_spritesheet");
            rectangleTexture = Content.Load<Texture2D>("rectangle");

        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Increases the frame for animation when the player is moving
            if (time > frameSpeed && playerDirection != Vector2.Zero)
            {
                time = 0f;
                frame = (frame + 1) % frames;  // ensures frame is a value 0-8
                // frame += 1;
                //if (frame >= frames)
                //    frame = 0;
            }

            // Handles player speed increase
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                speed = 2.5f;
                frameSpeed = 0.06f;
            }
            else
            {
                speed = 1.5f;
                frameSpeed = 0.08f;
            }

            // Moves the player
            SetPlayerDirection();
            playerLocation += playerDirection * speed;
            UpdateRects();

            // Keeps player from leaving the screen
            if (!window.Contains(playerCollisionRect))
            {
                playerLocation -= playerDirection * speed;
                UpdateRects();
            }

            // Handles collision detection with barriers
            foreach (Rectangle barrier in barriers)
                if (playerCollisionRect.Intersects(barrier))
                {
                    playerLocation -= playerDirection * speed;
                    UpdateRects();
                }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            
            //_spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.3f); // Draws collision hitbox
            _spriteBatch.Draw(characterSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            
            foreach (Rectangle barrier in barriers)
                _spriteBatch.Draw(rectangleTexture, barrier, Color.Black);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // This updates our collision and player drawing rectangles to match the players location
        // This needs to be called each time the playerLocation changes
        public void UpdateRects()
        {
            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.Location = new Point(playerCollisionRect.X - 15, playerCollisionRect.Y - 15); // the horizontal and vertical offset are both 15 pixels to align the skeleton with our hitbox
        }

        // This sets the players directional vector based on the keyboard state and
        // ensures that we are grabbing frames from the correct row of the spritesheet 
        // depending on the directional vector.
        protected void SetPlayerDirection()
        {
            // Sets directional vector
            playerDirection = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.A))
                playerDirection.X += -1;
            if (keyboardState.IsKeyDown(Keys.D))
                playerDirection.X += 1;
            if (keyboardState.IsKeyDown(Keys.W))
                playerDirection.Y += -1;
            if (keyboardState.IsKeyDown(Keys.S))
                playerDirection.Y += 1;

            // Chooses the correct row of frames to draw based on playerDirection
            if (playerDirection != Vector2.Zero)
            {
                playerDirection.Normalize(); // Sets the directional vector to the unit vector so the speed is 1 regardless of direction
                if (playerDirection.X < 0)  // Moving left
                    directionRow = leftRow;
                else if (playerDirection.X > 0)  // Moving right
                    directionRow = rightRow;
                else if (playerDirection.Y < 0)  // Moving up
                    directionRow = upRow;
                else
                    directionRow = downRow;
            }
            else
                frame = 0;  // If the player is not moving, set frame to the idle frame which is zero on our spritesheet

        }
    }
}