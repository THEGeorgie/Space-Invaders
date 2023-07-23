using SDL2;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace SI
{
    class Program
    {
        static int WINDOW_WIDTH = 800;
        static int WINDOW_HEIGHT = 600;

        static SDL_Rect bulletRect;
        static SDL_Rect playerRect;
        static SDL_Rect AreaRect;
        Program()
        {
            playerRect.w = 100;
            playerRect.h = 100;
            playerRect.x = (WINDOW_WIDTH / 2) - (100 / 2);
            playerRect.y = 400;

            bulletRect.w = 7;
            bulletRect.h = 20;
            bulletRect.x = playerRect.x;
            bulletRect.y = playerRect.y;  

            AreaRect.w = WINDOW_WIDTH;
            AreaRect.h = WINDOW_HEIGHT;
            AreaRect.x = 0;
            AreaRect.y = 0;
        }

        static int Main(String[] args)
        {
            Program prag = new Program();

            var renderer = IntPtr.Zero;
            var window = IntPtr.Zero;
            var texturePlayer = IntPtr.Zero;
            var textureBullet = IntPtr.Zero;

            SDL_Init(SDL_INIT_VIDEO);
            SDL_CreateWindowAndRenderer(Program.WINDOW_WIDTH, Program.WINDOW_HEIGHT, 0, out window, out renderer);
            SDL_SetWindowTitle(window, "Space Invaders");
            texturePlayer = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 100, 100);
            textureBullet = SDL_CreateTexture(renderer, SDL_PIXELFORMAT_ARGB8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 100, 100);

            SDL_Event events;
            IntPtr keyboardState;
            int arrayKeys;
            int TARGET_FPS = 60;

            float deltaTime = (float)1 / TARGET_FPS;
            bool loop = true;
            float buffer;
            while (loop)
            {
                SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                SDL_RenderClear(renderer);

                SDL_SetRenderTarget(renderer, textureBullet);
                SDL_SetRenderDrawColor(renderer, 255, 0, 0, 255);
                SDL_RenderClear(renderer);

                SDL_SetRenderTarget(renderer, texturePlayer);
                SDL_SetRenderDrawColor(renderer, 0, 255, 0, 255);
                SDL_RenderClear(renderer);
                
                SDL_SetRenderTarget(renderer, IntPtr.Zero);
                SDL_RenderDrawLine(renderer,AreaRect.x, AreaRect.y, bulletRect.x, bulletRect.y);

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
                                movePlayer(-1, TARGET_FPS, deltaTime);
                            }
                            if (GetKey(SDL_Keycode.SDLK_RIGHT))
                            {
                                movePlayer(1, TARGET_FPS, deltaTime);
                            }
                            if (GetKey(SDL_Keycode.SDLK_ESCAPE))
                            {
                                loop = false;
                            }
                            if (GetKey(SDL_Keycode.SDLK_SPACE))
                            {
                                buffer = (float)1 * TARGET_FPS * deltaTime;
                                Console.WriteLine("The x of bullet is " + bulletRect.x + " \n the y of the bullet is " + bulletRect.y);
                                while(WINDOW_WIDTH > bulletRect.x && WINDOW_HEIGHT > bulletRect.y) {
                                    bulletRect.y -= (int)buffer;
                                    SDL_RenderCopy(renderer, textureBullet, ref AreaRect, ref bulletRect);
                                    SDL_RenderCopy(renderer, texturePlayer, IntPtr.Zero, ref playerRect);
                                    SDL_RenderPresent(renderer);
                                }
                                
                                




                            }
                            break;
                    }
                }

                //SDL_RenderCopy(renderer, textureBullet, ref AreaRect, ref bulletRect);
                SDL_RenderCopy(renderer, texturePlayer, IntPtr.Zero, ref playerRect);
                SDL_RenderPresent(renderer);
            }

            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(renderer);
            SDL_DestroyTexture(textureBullet);
            SDL_DestroyTexture(texturePlayer);
            SDL_Quit();
            return 0;
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

        static void movePlayer(int xPos, int targetFps, float deltaTime)
        {
            float buffer;
            buffer = (float)10 * targetFps * deltaTime;
            if (xPos == -1)
            {
                playerRect.x -= (int)buffer;
            }
            else if (xPos == 1)
            {
                playerRect.x += (int)buffer;
            }
        }
        static void shoot(int targetFps, float deltaTime)
        {
            float buffer;
            buffer = (float)10 * targetFps * deltaTime;
            bulletRect.y += (int)buffer;
        }
    }

    class Game
    {

    }
}
