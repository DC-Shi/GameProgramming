using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XnaCards;

namespace ProgrammingAssignment6
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const int WindowWidth = 800;
        const int WindowHeight = 600;

        // max valid blockjuck score for a hand
        const int MaxHandValue = 21;

        // deck and hands
        Deck deck;
        List<Card> dealerHand = new List<Card>();
        List<Card> playerHand = new List<Card>();

        // hand placement
        const int TopCardOffset = 100;
        const int HorizontalCardOffset = 150;
        const int VerticalCardSpacing = 125;

        // messages
        SpriteFont messageFont;
        const string ScoreMessagePrefix = "Score: ";
        Message playerScoreMessage;
        Message dealerScoreMessage;
        Message winnerMessage;
        List<Message> messages = new List<Message>();

        // message placement
        const int ScoreMessageTopOffset = 25;
        const int HorizontalMessageOffset = HorizontalCardOffset;
        Vector2 winnerMessageLocation = new Vector2(WindowWidth / 2,
            WindowHeight / 2);

        // menu buttons
        Texture2D quitButtonSprite;
        List<MenuButton> menuButtons = new List<MenuButton>();

        // menu button placement
        const int TopMenuButtonOffset = TopCardOffset;
        const int QuitMenuButtonOffset = WindowHeight - TopCardOffset;
        const int HorizontalMenuButtonOffset = WindowWidth / 2;
        const int VerticalMenuButtonSpacing = 125;

        // use to detect hand over when player and dealer didn't hit
        bool playerHit = false;
        bool dealerHit = false;

        // game state tracking
        static GameState currentState = GameState.WaitingForPlayer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution and show mouse
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            // Show mouse cursor
            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // create and shuffle deck
            deck = new Deck(Content, 0, 0);
            deck.Shuffle();


            // first player card
            var oneCard = deck.TakeTopCard();
            oneCard.FlipOver();
            playerHand.Add(oneCard);

            // first dealer card
            var oneDealerCard = deck.TakeTopCard();
            dealerHand.Add(oneDealerCard);

            // second player card
            var twoCard = deck.TakeTopCard();
            twoCard.FlipOver();
            playerHand.Add(twoCard);

            // second dealer card
            var twoDealerCard = deck.TakeTopCard();
            twoDealerCard.FlipOver();
            dealerHand.Add(twoDealerCard);


            // load sprite font, create message for player score and add to list
            messageFont = Content.Load<SpriteFont>(@"fonts\Arial24");
            playerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString(),
                messageFont,
                new Vector2(HorizontalMessageOffset, ScoreMessageTopOffset));
            messages.Add(playerScoreMessage);

            // load quit button sprite for later use
            quitButtonSprite = Content.Load<Texture2D>(@"graphics\quitbutton");

            // create hit button and add to list
            var hitButtonSprite = Content.Load<Texture2D>(@"graphics\hitbutton");
            menuButtons.Add(new MenuButton(hitButtonSprite, new Vector2(HorizontalMenuButtonOffset, TopMenuButtonOffset), GameState.PlayerHitting));

            // create stand button and add to list
            var standButtonSprite = Content.Load<Texture2D>(@"graphics\standbutton");
            menuButtons.Add(new MenuButton(standButtonSprite, new Vector2(HorizontalMenuButtonOffset, TopMenuButtonOffset + VerticalMenuButtonSpacing), GameState.WaitingForDealer));

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // update menu buttons as appropriate
            foreach (var button in menuButtons)
                button.Update(Mouse.GetState());


            // game state-specific processing
            switch (currentState)
            {
                // Do nothing here, player change the state by clicking the button.
                case GameState.WaitingForPlayer:
                    break;
                // Player hit, so draw one card and wait for dealer
                case GameState.PlayerHitting:
                    playerHit = true;
                    var one = deck.TakeTopCard();
                    one.FlipOver();
                    playerHand.Add(one);
                    // update score
                    messages.Clear();
                    playerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString(),
                        messageFont,
                        new Vector2(HorizontalMessageOffset, ScoreMessageTopOffset));
                    messages.Add(playerScoreMessage);
                    ChangeState(GameState.WaitingForDealer);
                    break;
                // Dealer would hit if current score is less than 17, else dealer stand
                case GameState.WaitingForDealer:
                    if (GetBlockjuckScore(dealerHand) < 17)
                        ChangeState(GameState.DealerHitting);
                    else
                        ChangeState(GameState.CheckingHandOver);
                    break;
                case GameState.DealerHitting:
                    dealerHit = true;
                    playerHit = true;
                    var two = deck.TakeTopCard();
                    two.FlipOver();
                    dealerHand.Add(two);
                    ChangeState(GameState.CheckingHandOver);
                    break;
                case GameState.CheckingHandOver:
                    if (GetBlockjuckScore(playerHand) > MaxHandValue
                        || GetBlockjuckScore(dealerHand) > MaxHandValue
                        || (!playerHit && !dealerHit))
                    {
                        // Flip over dealer's first card
                        if (!dealerHand[0].FaceUp)
                            dealerHand[0].FlipOver();
                        // Update dealer score message and player score message
                        messages.Clear();
                        playerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(playerHand).ToString(),
                            messageFont,
                            new Vector2(HorizontalMessageOffset, ScoreMessageTopOffset));
                        messages.Add(playerScoreMessage);
                        dealerScoreMessage = new Message(ScoreMessagePrefix + GetBlockjuckScore(dealerHand).ToString(),
                            messageFont,
                            new Vector2(WindowWidth - HorizontalMessageOffset, ScoreMessageTopOffset));
                        messages.Add(dealerScoreMessage);
                        int playerScore = GetBlockjuckScore(playerHand);
                        int dealerScore = GetBlockjuckScore(dealerHand);
                        string result = "";
                        if (playerScore > MaxHandValue && dealerScore > MaxHandValue)
                            result = "Tie!";
                        if (playerScore <= MaxHandValue && dealerScore > MaxHandValue)
                            result = "Player Win!";
                        if (playerScore > MaxHandValue && dealerScore <= MaxHandValue)
                            result = "Dealer Win!";
                        if (playerScore <= MaxHandValue && dealerScore <= MaxHandValue)
                        {
                            if (playerScore == dealerScore)
                                result = "Tie!";
                            if (playerScore > dealerScore)
                                result = "Player Win!";
                            if (playerScore < dealerScore)
                                result = "Dealer Win!";
                        }
                        winnerMessage = new Message(result, messageFont, winnerMessageLocation);
                        messages.Add(winnerMessage);

                        menuButtons.Clear();
                        menuButtons.Add(new MenuButton(quitButtonSprite, new Vector2(HorizontalMenuButtonOffset, QuitMenuButtonOffset), GameState.Exiting));
                        ChangeState(GameState.DisplayingHandResults);
                    }
                    else
                    {
                        playerHit = false;
                        dealerHit = false;
                        ChangeState(GameState.WaitingForPlayer);
                    }

                    break;
                // Do nothing here, waiting for player click to exit.
                case GameState.DisplayingHandResults:
                    break;
                case GameState.Exiting:
                    Exit();
                    break;
            }



            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Goldenrod);

            spriteBatch.Begin();

            // draw hands
            DrawHands(playerHand, spriteBatch, HorizontalCardOffset);
            DrawHands(dealerHand, spriteBatch, WindowWidth - HorizontalCardOffset);


            // draw messages
            foreach (var mes in messages)
                mes.Draw(spriteBatch);


            // draw menu buttons
            if(currentState == GameState.WaitingForPlayer || currentState == GameState.DisplayingHandResults)
                foreach (var button in menuButtons)
                    button.Draw(spriteBatch);


            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// Draw the cards of one hand. It would automatically adjust cards to fit in window
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="X"></param>
        private void DrawHands(List<Card> hand, SpriteBatch spriteBatch, int X)
        {
            int vertialSpace = VerticalCardSpacing;
            // Adjust the space if the last card is out of window.
            if (TopCardOffset + (hand.Count - 1) * vertialSpace + TopCardOffset > WindowHeight)
                vertialSpace = (WindowHeight - TopCardOffset * 2) / (hand.Count - 1);
            // Draw the cards with position.
            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].X = X;
                hand[i].Y = TopCardOffset + i * vertialSpace;
                hand[i].Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Calculates the Blockjuck score for the given hand
        /// </summary>
        /// <param name="hand">the hand</param>
        /// <returns>the Blockjuck score for the hand</returns>
        private int GetBlockjuckScore(List<Card> hand)
        {
            // add up score excluding Aces
            int numAces = 0;
            int score = 0;
            foreach (Card card in hand)
            {
                if (card.Rank != Rank.Ace)
                {
                    score += GetBlockjuckCardValue(card);
                }
                else
                {
                    numAces++;
                }
            }

            // if more than one ace, only one should ever be counted as 11
            if (numAces > 1)
            {
                // make all but the first ace count as 1
                score += numAces - 1;
                numAces = 1;
            }

            // if there's an Ace, score it the best way possible
            if (numAces > 0)
            {
                if (score + 11 <= MaxHandValue)
                {
                    // counting Ace as 11 doesn't bust
                    score += 11;
                }
                else
                {
                    // count Ace as 1
                    score++;
                }
            }

            return score;
        }

        /// <summary>
        /// Gets the Blockjuck value for the given card
        /// </summary>
        /// <param name="card">the card</param>
        /// <returns>the Blockjuck value for the card</returns>
        private int GetBlockjuckCardValue(Card card)
        {
            switch (card.Rank)
            {
                case Rank.Ace:
                    return 11;
                case Rank.King:
                case Rank.Queen:
                case Rank.Jack:
                case Rank.Ten:
                    return 10;
                case Rank.Nine:
                    return 9;
                case Rank.Eight:
                    return 8;
                case Rank.Seven:
                    return 7;
                case Rank.Six:
                    return 6;
                case Rank.Five:
                    return 5;
                case Rank.Four:
                    return 4;
                case Rank.Three:
                    return 3;
                case Rank.Two:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Changes the state of the game
        /// </summary>
        /// <param name="newState">the new game state</param>
        public static void ChangeState(GameState newState)
        {
            currentState = newState;
        }
    }
}
