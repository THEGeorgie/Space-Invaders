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
        int randShoot = 0;
        int randShootEnemy = 0;

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
                new SDL.SDL_Rect() { x = 100, y = 100, w = 50, h = 50 },
                new SDL.SDL_Rect() { x = 200, y = 100, w = 50, h = 50 },
                new SDL.SDL_Rect() { x = 300, y = 100, w = 50, h = 50 }
        };

        SDL.SDL_Rect[] bulletEnemyRect = new SDL.SDL_Rect[3] {
                new SDL.SDL_Rect() { x = 100, y = 100, w = 7, h = 20 },
                new SDL.SDL_Rect() { x = 200, y = 100, w = 7, h = 20 },
                new SDL.SDL_Rect() { x = 300, y = 100, w = 7, h = 20 }
        };

        SDL.SDL_Rect[,] wallRect = new SDL.SDL_Rect[,] {
            {
                new SDL.SDL_Rect() { x = 100, y = 390, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 110, y = 390, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 120, y = 390, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 130, y = 390, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 140, y = 390, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 150, y = 390, w = 10, h = 20 },
            },
            {
                new SDL.SDL_Rect() { x = 100, y = 410, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 110, y = 410, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 120, y = 410, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 130, y = 410, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 140, y = 410, w = 10, h = 20 },
                new SDL.SDL_Rect() { x = 150, y = 410, w = 10, h = 20 },
            },
        };

        SDL.SDL_Rect[,] wallCollisionRect = new SDL.SDL_Rect[,]
        {
            {
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
            },
            {
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
                new SDL.SDL_Rect(),
            }
        };
        SDL.SDL_Rect[,] wallCollision1Rect;
        SDL.SDL_Rect[,] wallCollision2Rect;
        SDL.SDL_Rect[,] wallCollision3Rect;
        SDL.SDL_Rect[,] wallCollision4Rect;
        SDL.SDL_Rect[,] wallCollision5Rect;

        SDL.SDL_Rect[,] wallRect1 = new SDL.SDL_Rect[,]
        {

        };
        SDL.SDL_Rect[,] wallRect2 = new SDL.SDL_Rect[,]
        {

        };
        SDL.SDL_Rect[,] wallRect3 = new SDL.SDL_Rect[,]
        {

        };
        SDL.SDL_Rect[,] wallRect4 = new SDL.SDL_Rect[,]
        {

        };
        SDL.SDL_Rect[,] wallRect5 = new SDL.SDL_Rect[,]
        {

        };

        nint[] clusterWall;

        bool[] enemySpawn = new bool[3] { true, true, true };

        public bool shoot = false;
        public bool shootEnemy = false;
        public bool terminate = false;
        Program()
        {
            SDL_Init(SDL_INIT_VIDEO);
            SDL_CreateWindowAndRenderer(Program.WINDOW_WIDTH, Program.WINDOW_HEIGHT, 0, out window, out renderer);
            SDL_SetWindowTitle(window, "Space Invaders");
            texturePlayer = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 50, 50);
            textureEnemy = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 50, 50);

            bulletEnemyRect[0].x = enemyRect[0].x + 28;
            bulletEnemyRect[1].x = enemyRect[1].x + 28;
            bulletEnemyRect[2].x = enemyRect[2].x + 28;
            bulletEnemyRect[0].y = enemyRect[0].y;
            bulletEnemyRect[1].y = enemyRect[1].y;
            bulletEnemyRect[2].y = enemyRect[2].y;

            wallRect1 = wallRect.Clone() as SDL.SDL_Rect[,];
            wallRect2 = wallRect.Clone() as SDL.SDL_Rect[,];
            wallRect3 = wallRect.Clone() as SDL.SDL_Rect[,];
            wallRect4 = wallRect.Clone() as SDL.SDL_Rect[,];
            wallRect5 = wallRect.Clone() as SDL.SDL_Rect[,];

            wallCollision1Rect = wallCollisionRect.Clone() as SDL.SDL_Rect[,];
            wallCollision2Rect = wallCollisionRect.Clone() as SDL.SDL_Rect[,];
            wallCollision3Rect = wallCollisionRect.Clone() as SDL.SDL_Rect[,];
            wallCollision4Rect = wallCollisionRect.Clone() as SDL.SDL_Rect[,];
            wallCollision5Rect = wallCollisionRect.Clone() as SDL.SDL_Rect[,];

            for (int i = 0; i < wallRect1.Length / 2; i++)
            {
                wallRect1[0, i].x += 100;
                wallRect1[1, i].x += 100;
                wallRect2[0, i].x += 200;
                wallRect2[1, i].x += 200;
                wallRect3[0, i].x += 300;
                wallRect3[1, i].x += 300;
                wallRect4[0, i].x += 400;
                wallRect4[1, i].x += 400;
                wallRect5[0, i].x += 500;
                wallRect5[1, i].x += 500;
            }

            playerRect.w = 50;
            playerRect.h = 50;
            playerRect.x = (WINDOW_WIDTH / 2) - (100 / 2);
            playerRect.y = 500;

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
            int countEnemySpawn = 0;

            UInt32 timeold = SDL_GetTicks();
            UInt32 timenew;
            UInt32 ticks;
            bool loop = true;
            while (loop)
            {
                Console.WriteLine(prag.shootEnemy);
                prag.update();
                timenew = SDL_GetTicks();
                ticks = timeold - timenew;
                if (prag.terminate == true)
                {
                    break;
                }


                if (prag.shootEnemy == false)
                {
                    prag.randShoot = prag.rnd.Next(20);
                    if (prag.randShoot == 10)
                    {
                        Console.WriteLine(prag.randShoot);

                    }
                    for (int i = 0; i < prag.enemySpawn.Length; i++)
                    {
                        if (prag.enemySpawn[i] == true)
                        {
                            countEnemySpawn++;
                        }
                    }
                    Console.WriteLine(countEnemySpawn);

                    switch (countEnemySpawn)
                    {
                        case 3:
                            prag.randShootEnemy = prag.rnd.Next(0, 3);
                            break;
                        case 2:
                            prag.randShootEnemy = prag.rnd.Next(0, 2);
                            break;
                        case 1:
                            prag.randShootEnemy = 0;
                            break;
                    }
                    countEnemySpawn = 0;
                    if (prag.randShoot == 10)
                    {
                        prag.shootEnemy = true;
                    }
                }



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
            textureBulletEnemy = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 100, 100);


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
            if (shootEnemy == true)
            {
                SDL_SetRenderTarget(renderer, textureBulletEnemy);
                SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
                SDL_RenderClear(renderer);
            }
            else
            {
                bulletEnemyRect[0].x = enemyRect[0].x + 28;
                bulletEnemyRect[1].x = enemyRect[1].x + 28;
                bulletEnemyRect[2].x = enemyRect[2].x + 28;
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

            SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);

            for (int i = 0; i < wallRect.Length / 2; i++)
            {
                SDL_RenderDrawRect(renderer, ref wallRect[0, i]);
                SDL_RenderDrawRect(renderer, ref wallRect[1, i]);
                SDL_RenderDrawRect(renderer, ref wallRect1[0, i]);
                SDL_RenderDrawRect(renderer, ref wallRect1[1, i]);
                SDL_RenderDrawRect(renderer, ref wallRect2[0, i]);
                SDL_RenderDrawRect(renderer, ref wallRect2[1, i]);
                SDL_RenderDrawRect(renderer, ref wallRect3[0, i]);
                SDL_RenderDrawRect(renderer, ref wallRect3[1, i]);
                SDL_RenderDrawRect(renderer, ref wallRect4[0, i]);
                SDL_RenderDrawRect(renderer, ref wallRect4[1, i]);
                SDL_RenderDrawRect(renderer, ref wallRect5[0, i]);
                SDL_RenderDrawRect(renderer, ref wallRect5[1, i]);
            }


        }

        void present(int ticks)
        {


            if (shoot == true)
            {
                // the bullent gets spawned
                projectile();
            }
            if (shootEnemy == true)
            {
                projectileEnemy();
            }

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
            for (int i = 0; i < wallRect.Length / 2; i++)
            {
                if (SDL_IntersectRect(ref wallRect[0, i], ref bulletEnemyRect[randShootEnemy], out wallCollisionRect[0, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect[0, i].h = 0;
                    wallRect[0, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }
                if (SDL_IntersectRect(ref wallRect[1, i], ref bulletEnemyRect[randShootEnemy], out wallCollisionRect[1, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect[1, i].h = 0;
                    wallRect[1, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }

                if (SDL_IntersectRect(ref wallRect1[0, i], ref bulletEnemyRect[randShootEnemy], out wallCollision1Rect[0, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect1[0, i].h = 0;
                    wallRect1[0, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }
                if (SDL_IntersectRect(ref wallRect1[1, i], ref bulletEnemyRect[randShootEnemy], out wallCollision1Rect[1, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect1[1, i].h = 0;
                    wallRect1[1, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }

                if (SDL_IntersectRect(ref wallRect2[0, i], ref bulletEnemyRect[randShootEnemy], out wallCollision2Rect[0, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect2[0, i].h = 0;
                    wallRect2[0, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }
                if (SDL_IntersectRect(ref wallRect2[1, i], ref bulletEnemyRect[randShootEnemy], out wallCollision2Rect[1, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect2[1, i].h = 0;
                    wallRect2[1, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }

                if (SDL_IntersectRect(ref wallRect3[0, i], ref bulletEnemyRect[randShootEnemy], out wallCollision3Rect[0, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect3[0, i].h = 0;
                    wallRect3[0, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }
                if (SDL_IntersectRect(ref wallRect3[1, i], ref bulletEnemyRect[randShootEnemy], out wallCollision3Rect[1, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect3[1, i].h = 0;
                    wallRect3[1, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }

                if (SDL_IntersectRect(ref wallRect4[0, i], ref bulletEnemyRect[randShootEnemy], out wallCollision4Rect[0, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect4[0, i].h = 0;
                    wallRect4[0, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }
                if (SDL_IntersectRect(ref wallRect4[1, i], ref bulletEnemyRect[randShootEnemy], out wallCollision4Rect[1, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect4[1, i].h = 0;
                    wallRect4[1, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }

                if (SDL_IntersectRect(ref wallRect5[0, i], ref bulletEnemyRect[randShootEnemy], out wallCollision5Rect[0, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect5[0, i].h = 0;
                    wallRect5[0, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
                }
                if (SDL_IntersectRect(ref wallRect5[1, i], ref bulletEnemyRect[randShootEnemy], out wallCollision5Rect[1, i]) == SDL_bool.SDL_TRUE)
                {
                    wallRect5[1, i].h = 0;
                    wallRect5[1, i].w = 0;
                    SDL_DestroyTexture(textureBullet);
                    shootEnemy = false;
                    randShoot = 0;
                    randShootEnemy = 0;
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

            //If it goes beyond the window surface the bullet gets destryoed if not the bullet contniues being renderd
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

            bulletEnemyRect[randShootEnemy].y += (int)buffer;

            if (bulletEnemyRect[randShootEnemy].y > 600)
            {
                SDL_DestroyTexture(textureBullet);
                shootEnemy = false;
                randShoot = 0;
                randShootEnemy = 0;
            }
            else if (SDL_IntersectRect(ref playerRect, ref bulletEnemyRect[randShootEnemy], out collisionRect) == SDL_bool.SDL_TRUE)
            {
                SDL_DestroyTexture(textureBullet);
                shootEnemy = false;
                terminate = true;

            }
            else
            {
                SDL_RenderCopy(renderer, textureBulletEnemy, IntPtr.Zero, ref bulletEnemyRect[randShootEnemy]);
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
