using SDL2;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace SI
{
    class Program
    {

        static int WINDOW_WIDTH = 800;
        static int WINDOW_HEIGHT = 600;
        bool loop = true;
        int checkFront = 1;
        int checkBack = 0;

        Random rnd = new Random();
        int randShoot;
        int randShootEnemy;

        static int TARGET_FPS = 60;
        float deltaTime;

        SDL_Rect bulletRect;
        SDL_Rect playerRect;
        SDL_Rect collisionRect;
        SDL_Rect collisionBrRect;
        SDL_Rect collisionBlRect;
        SDL_Rect collisionPlayerRect;
        SDL_Rect borderRightRect;
        SDL_Rect borderLeftRect;

        nint renderer = IntPtr.Zero;
        nint window = IntPtr.Zero;
        nint texturePlayer = IntPtr.Zero;
        nint textureBullet = IntPtr.Zero;
        nint textureBulletEnemy = IntPtr.Zero;
        nint textureEnemy = IntPtr.Zero;

        SDL.SDL_Rect[] enemyRect = new SDL.SDL_Rect[3] {
                new SDL.SDL_Rect() { x = 100, y = 100, w = 70, h = 70 },
                new SDL.SDL_Rect() { x = 200, y = 100, w = 70, h = 70 },
                new SDL.SDL_Rect() { x = 300, y = 100, w = 70, h = 70 }
        };

        SDL.SDL_Rect[] bulletEnemyRect = new SDL.SDL_Rect[3] {
                new SDL.SDL_Rect() { x = 100, y = 100, w = 7, h = 20 },
                new SDL.SDL_Rect() { x = 200, y = 100, w = 7, h = 20 },
                new SDL.SDL_Rect() { x = 300, y = 100, w = 7, h = 20 }
        };

        bool[] enemySpawn = new bool[3] { true, true, true };

        public bool shoot = false;
        Program()
        {
            SDL_Init(SDL_INIT_VIDEO);
            SDL_CreateWindowAndRenderer(Program.WINDOW_WIDTH, Program.WINDOW_HEIGHT, 0, out window, out renderer);
            SDL_SetWindowTitle(window, "Space Invaders");
            texturePlayer = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 100, 100);
            textureEnemy = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 70, 70);

            bulletEnemyRect[0].x = enemyRect[0].x + 28;
            bulletEnemyRect[1].x = enemyRect[1].x + 28;
            bulletEnemyRect[2].x = enemyRect[2].x + 28;
            bulletEnemyRect[0].y = enemyRect[0].y;
            bulletEnemyRect[1].y = enemyRect[1].y;
            bulletEnemyRect[2].y = enemyRect[2].y;

            playerRect.w = 100;
            playerRect.h = 100;
            playerRect.x = (WINDOW_WIDTH / 2) - (100 / 2);
            playerRect.y = 400;

            borderLeftRect.x = 0;
            borderLeftRect.y = 0;
            borderLeftRect.w = 40;
            borderLeftRect.h = WINDOW_HEIGHT;

            borderRightRect.x = 760;
            borderRightRect.y = 0;
            borderRightRect.w = 40;
            borderRightRect.h = WINDOW_HEIGHT;

            bulletRect.w = 7;
            bulletRect.h = 20;
            bulletRect.x = playerRect.x + 43;
            bulletRect.y = playerRect.y;

            deltaTime = (float)1 / TARGET_FPS;
        }
        ~Program()
        {
            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyTexture(textureBullet);
            SDL_DestroyTexture(texturePlayer);
            SDL_Quit();
        }

        static int Main(String[] args)
        {
            Program prag = new Program();

            SDL_Event events;
            IntPtr keyboardState;
            int arrayKeys;

            UInt32 timeold = SDL_GetTicks();
            UInt32 timenew;
            UInt32 ticks;
            bool loop = true;
            while (loop)
            {
                prag.update();
                timenew = SDL_GetTicks();
                ticks = timeold - timenew;

                keyboardState = SDL_GetKeyboardState(out arrayKeys);

                while (SDL_PollEvent(out events) == 1)
                {
                    switch (events.type)
                    {
                        case SDL_EventType.SDL_QUIT:
                            loop = false;
                            break;
                        case SDL_EventType.SDL_KEYDOWN:
                            if (GetKey(SDL_Keycode.SDLK_LEFT))
                            {
                                prag.move(-1);
                            }
                            if (GetKey(SDL_Keycode.SDLK_RIGHT))
                            {
                                prag.move(1);
                            }
                            if (GetKey(SDL_Keycode.SDLK_ESCAPE))
                            {
                                loop = false;
                            }
                            if (GetKey(SDL_Keycode.SDLK_SPACE))
                            {
                                prag.shoot = true;

                            }
                            break;
                    }
                }

                SDL_Delay(30);
                timeold = timenew;
                prag.present((int)ticks);

            }


            return 0;
        }
        void update()
        {


            SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL_RenderClear(renderer);

            if (shoot == true)
            {
                textureBullet = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 100, 100);
                SDL_SetRenderTarget(renderer, textureBullet);
                SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
                SDL_RenderClear(renderer);
            }
            else
            {
                bulletRect.y = playerRect.y;
                bulletRect.x = playerRect.x + 47;
            }
            if (randShoot <= 10){
                textureBulletEnemy = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 100, 100);
                SDL_SetRenderTarget(renderer, textureBulletEnemy);
                SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
                SDL_RenderClear(renderer);
            }

            SDL_SetRenderTarget(renderer, texturePlayer);
            SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
            SDL_RenderClear(renderer);

            SDL_SetRenderTarget(renderer, textureEnemy);
            SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            SDL_RenderClear(renderer);



            SDL_SetRenderTarget(renderer, IntPtr.Zero);
            SDL_SetRenderDrawColor(renderer, 0, 0, 255, 255);
            SDL_RenderDrawRect(renderer, ref borderLeftRect);
            SDL_RenderDrawRect(renderer, ref borderRightRect);
        }

        void present(int ticks)
        {


            if (shoot == true)
            {
                // the bullent gets spawned
                projectile();
            }
            randShoot = rnd.Next(10);
            Console.WriteLine(randShoot);
            projectileEnemy();

            for (int i = 0; i < enemyRect.Length; i++)
            {
                if (enemySpawn[i])
                {
                    // checks if it hit anythinf and if it did the enemy dosent spawn and gets removed from
                    // the rendering pipeline if not the enemy keeps moving and being rendered
                    if (SDL_IntersectRect(ref enemyRect[i], ref bulletRect, out collisionRect) == SDL_bool.SDL_TRUE)
                    {
                        Console.WriteLine("Killed enemy num." + i);
                        enemySpawn[i] = false;
                        enemyRect[i].w = 0;
                        enemyRect[i].h = 0;
                    }
                    else
                    {
                        moveEnemy();
                        SDL_RenderCopy(renderer, textureEnemy, IntPtr.Zero, ref enemyRect[i]);
                    }
                }
            }

            SDL_RenderCopy(renderer, texturePlayer, IntPtr.Zero, ref playerRect);
            SDL_RenderPresent(renderer);
        }
        static bool GetKey(SDL.SDL_Keycode _keycode)
        {
            int arraySize;
            bool isKeyPressed = false;
            IntPtr origArray = SDL.SDL_GetKeyboardState(out arraySize);
            byte[] keys = new byte[arraySize];
            byte keycode = (byte)SDL.SDL_GetScancodeFromKey(_keycode);
            Marshal.Copy(origArray, keys, 0, arraySize);
            isKeyPressed = keys[keycode] == 1;
            return isKeyPressed;
        }
        void projectile()
        {
            float buffer;
            //Changing the bullet speed
            buffer = (float)15 * TARGET_FPS * deltaTime;
            bulletRect.y -= (int)buffer;
            //If it goes over the window surface the bullet gets destryoed if not the bullet contniues being renderd
            for (int i = 0; i < enemyRect.Length - 1; i++)
            {
                if (bulletRect.y < 0 || SDL_IntersectRect(ref enemyRect[i], ref bulletRect, out collisionRect) == SDL_bool.SDL_TRUE)
                {
                    shoot = false;
                    SDL_DestroyTexture(textureBullet);
                }
                else
                {
                    SDL_RenderCopy(renderer, textureBullet, IntPtr.Zero, ref bulletRect);
                }
            }
        }
        void projectileEnemy()
        {
            float buffer;
            //Changing the bullet speed
            buffer = (float)15 * TARGET_FPS * deltaTime;
            for (int i = 0; i < enemyRect.Length - 1; i++)
            {
                if (randShoot <= 9)
                {
                    bulletEnemyRect[i].y += (int)buffer;
                    
                    if (bulletEnemyRect[i].y < 0 || SDL_IntersectRect(ref enemyRect[i], ref bulletEnemyRect[i], out collisionRect) == SDL_bool.SDL_TRUE)
                    {
                        SDL_DestroyTexture(textureBullet);
                    }
                    else
                    {
                        SDL_RenderCopy(renderer, textureBulletEnemy, IntPtr.Zero, ref bulletEnemyRect[i]);
                    }
                }
            }
        }
        void move(int xPos)
        {
            float buffer;
            buffer = (float)10 * TARGET_FPS * deltaTime;
            if (xPos == -1 && SDL_IntersectRect(ref playerRect, ref borderLeftRect, out collisionPlayerRect) != SDL_bool.SDL_TRUE)
            {
                playerRect.x -= (int)buffer;
            }
            else if (xPos == 1 && SDL_IntersectRect(ref playerRect, ref borderRightRect, out collisionPlayerRect) != SDL_bool.SDL_TRUE)
            {
                playerRect.x += (int)buffer;
            }
        }
        void moveEnemy()
        {
            float buffer;
            int len = enemyRect.Length;
            int speed = 1;
            buffer = (float)speed * TARGET_FPS * deltaTime;
            switch (loop)
            {
                case true:
                    if (enemySpawn[len - checkFront] == true)
                    {
                        if (SDL_IntersectRect(ref enemyRect[len - checkFront], ref borderRightRect, out collisionBrRect) != SDL_bool.SDL_TRUE)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                enemyRect[i].x += (int)buffer;
                                if (collisionBrRect.w == 0)
                                {
                                    loop = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        checkFront++;
                    }
                    break;
                case false:
                    if (enemySpawn[checkBack] == true)
                    {
                        if (SDL_IntersectRect(ref enemyRect[checkBack], ref borderLeftRect, out collisionBlRect) != SDL_bool.SDL_TRUE)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                enemyRect[i].x -= (int)buffer;
                                if (collisionBlRect.w == 0)
                                {
                                    loop = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        checkBack++;
                    }
                    break;
            }

        }

    }

    class Game
    {

    }
}
