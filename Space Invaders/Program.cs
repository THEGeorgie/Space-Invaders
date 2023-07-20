using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using SDL2;
using static SDL2.SDL;

namespace SI
{
    class Program
    {
        static int Main(String[] args)
        {


            int WINDOW_WIDTH = 800;
            int WINDOW_HEIGHT = 600;


            nint renderer = IntPtr.Zero;
            nint window = IntPtr.Zero;

            SDL_Init(SDL_INIT_VIDEO);

            SDL_CreateWindowAndRenderer(WINDOW_WIDTH, WINDOW_HEIGHT, 0, out window, out renderer);
            SDL_SetWindowTitle(window, "Space Invaders");

            SDL_Event events;
            IntPtr keyboardState;
            int arrayKeys;

            bool loop = true;
            while (loop)
            {

                SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
                SDL_RenderClear(renderer);

                keyboardState = SDL_GetKeyboardState(out arrayKeys);

                while (SDL_PollEvent(out events) == 1)
                {
                    switch (events.type)
                    {
                        case SDL_EventType.SDL_QUIT:
                            loop = false;
                            break;
                        case SDL_EventType.SDL_KEYDOWN:
                            break;
                    }
                }

                SDL_RenderPresent(renderer);
            }

            SDL_DestroyWindow(window);
            SDL_DestroyRenderer(renderer);
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
    }

    class Game
    {
        static int movePlayer(int xPos, int yPos, int targetFps, float deltaTime)
        {
            float buffer;
            if (xPos == -1 || yPos == -1)
            {
                buffer = (float)10 * targetFps * deltaTime;
                return (int)buffer;
            }
            else
            {
                return 0;
            }
        }
    }
}
