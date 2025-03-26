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

        int rows, columns;
        int frame;      // The frame number (column) in the sequence to draw
        int frames;     // The number of frames for each direction, usually the same as columns
        int directionRow; // The row number containing the frames for the current direction
        int leftRow, rightRow, upRow, downRow; // The row number of each directional set of frames

        float width;    // the width of each frame
        float height;   // the height of each frame
        float speed;    // How fast the character sprite will travel
        float time;     // used to store elapsed time
        float frameSpeed; // Sets how fast player frames transition

        Vector2 playerLocation; // Stored the location of the players collision sprite
        Vector2 direction; // The directional vector of the player

        Rectangle playerCollisionRect; // The rectangle that will be used for player collision
        Rectangle playerDrawRect; // The rectangle that we will scale our player frame into
        Rectangle window;  // Dimensions of the game window

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            window = new Rectangle(0, 0, 800, 500);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            barriers = new List<Rectangle>();
            barriers.Add(new Rectangle(100, 100, 30, 150));
            barriers.Add(new Rectangle(10, 400, 300, 30));
            barriers.Add(new Rectangle(400, 200, 200, 30));
            barriers.Add(new Rectangle(700, 450, 30, 100));

            // Aids with processing of spritesheet
            rows = 9;
            columns = 9;
            upRow = 0;
            leftRow = 1;
            downRow = 2;
            rightRow = 3;

            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(20, 20, 30, 40);
            
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            keyboardState = Keyboard.GetState();




            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            foreach (Rectangle barrier in barriers)
                _spriteBatch.Draw(rectangleTexture, barrier, Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}