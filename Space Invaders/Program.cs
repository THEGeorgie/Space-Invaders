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

        static int TARGET_FPS = 60;
        float deltaTime;

        SDL_Rect bulletRect;
        SDL_Rect playerRect;
        SDL_Rect collisionRect;

        nint renderer = IntPtr.Zero;
        nint window = IntPtr.Zero;
        nint texturePlayer = IntPtr.Zero;
        nint textureBullet = IntPtr.Zero;
        nint textureEnemy = IntPtr.Zero;

        SDL.SDL_Rect[] enemyRect = new SDL.SDL_Rect[3] {
                new SDL.SDL_Rect() { x = 100, y = 100, w = 70, h = 70 },   // Rectangle 1
                new SDL.SDL_Rect() { x = 150, y = 100, w = 70, h = 70 },  // Rectangle 2
                new SDL.SDL_Rect() { x = 200, y = 100, w = 70, h = 70 }   // Rectangle 3
        };

        public bool shoot = false;
        public bool enemySpawn = true;
        Program()
        {
            SDL_Init(SDL_INIT_VIDEO);
            SDL_CreateWindowAndRenderer(Program.WINDOW_WIDTH, Program.WINDOW_HEIGHT, 0, out window, out renderer);
            SDL_SetWindowTitle(window, "Space Invaders");
            texturePlayer = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 100, 100);
            textureEnemy = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 70, 70);
            
            playerRect.w = 100;
            playerRect.h = 100;
            playerRect.x = (WINDOW_WIDTH / 2) - (100 / 2);
            playerRect.y = 400;

            SDL.SDL_Rect[] enemyRect = new SDL.SDL_Rect[3] {
                new SDL.SDL_Rect() { x = 100, y = 100, w = 70, h = 70 },   // Rectangle 1
                new SDL.SDL_Rect() { x = 500, y = 100, w = 70, h = 70 },  // Rectangle 2
                new SDL.SDL_Rect() { x = 800, y = 100, w = 70, h = 70 }   // Rectangle 3
            };

            bulletRect.w = 7;
            bulletRect.h = 20;
            bulletRect.x = playerRect.x + 43;
            bulletRect.y = playerRect.y;

            deltaTime = (float)1 / TARGET_FPS;
        }
        ~Program() {
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
            bool loop = true;
            while (loop)
            {
                prag.update();

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
                                prag.movePlayer(-1);
                            }
                            if (GetKey(SDL_Keycode.SDLK_RIGHT))
                            {
                                prag.movePlayer(1);
                            }
                            if (GetKey(SDL_Keycode.SDLK_ESCAPE))
                            {
                                loop = false;
                            }
                            if (GetKey(SDL_Keycode.SDLK_SPACE))
                            {
                                prag.shoot = true;
                                prag.update();

                            }
                            break;
                    }
                }

                prag.present();
                
            }

            
            return 0;
        }
        void update() {
            

            SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            SDL_RenderClear(renderer);

            if (shoot == true) {
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

            SDL_SetRenderTarget(renderer, texturePlayer);
            SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
            SDL_RenderClear(renderer);

            SDL_SetRenderTarget(renderer, textureEnemy);
            SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
            SDL_RenderClear(renderer);

            SDL_SetRenderTarget(renderer, IntPtr.Zero);
        }

        void present() {
            if (shoot == true) {
                projectile();
            }
            if (enemySpawn) {
                for (int i = 0; i < enemyRect.Length; i++) {
                    if (SDL_IntersectRect(ref enemyRect[i], ref bulletRect, out collisionRect) == SDL_bool.SDL_TRUE) {
                        SDL_DestroyTexture(textureEnemy);
                        enemySpawn = false;
                    }
                    else {               
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
            SDL_Delay(40);
            bulletRect.y -= (int)buffer;
            if (bulletRect.y < 0)
            {
                shoot = false;
                SDL_DestroyTexture(textureBullet);
            }
            else
            {
                SDL_RenderCopy(renderer, textureBullet, IntPtr.Zero, ref bulletRect);
            }
            Console.WriteLine("the y of the bullet is: " + bulletRect.y);
        }
        void movePlayer(int xPos)
        {
            float buffer;
            buffer = (float)10 * TARGET_FPS * deltaTime;
            if (xPos == -1)
            {
                playerRect.x -= (int)buffer;
            }
            else if (xPos == 1)
            {
                playerRect.x += (int)buffer;
            }
        }
    }

    class Game
    {

    }
}
