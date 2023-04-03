using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GameEngine
{
    class Key
    {
        public string name;
        public bool value;
        public int id;
        public int index;

        public Key(string name, bool value, int id, int index)
        {
            this.name = name;
            this.value = value;
            this.id = id;
            this.index = index;
        }
    }
    class Button
    {
        public string name;
        public bool value;
        public int index;

        public Button(string name, bool value, int index)
        {
            this.name = name;
            this.value = value;
            this.index = index;
        }
    }

    class Input
    {
        private static Key[]? keysDown;
        private static Button[]? buttonsDown;
        private static int wKeyIndex = 0;
        private static int aKeyIndex = 0;
        private static int sKeyIndex = 0;
        private static int dKeyIndex = 0;
        public static void Instantiate()
        {
            Keys[] keys = (Keys[])Enum.GetValues(typeof(Keys));
            keysDown = new Key[keys.Length];

            for (int i = 0; i < keys.Length; i++)
            {
                keysDown[i] = new Key(keys[i].ToString(), false, (int)keys[i], i);
            }

            MouseButton[] buttons = (MouseButton[])Enum.GetValues(typeof(MouseButton));
            buttonsDown = new Button[buttons.Length];

            for (int i = 0; i < buttons.Length; i++)
            {
                buttonsDown[i] = new Button(buttons[i].ToString(), false, i);
            }

            wKeyIndex = Key("W")!.index;
            aKeyIndex = Key("A")!.index;
            sKeyIndex = Key("S")!.index;
            dKeyIndex = Key("D")!.index;
        }

        public static int Vertical()
        {
            return (KeyDownByIndex(wKeyIndex) ? 1 : 0) - (KeyDownByIndex(sKeyIndex) ? 1 : 0);
        }
        public static int Horizontal()
        {
            return (KeyDownByIndex(dKeyIndex) ? 1 : 0) - (KeyDownByIndex(aKeyIndex) ? 1 : 0);
        }

        public static bool KeyDownByIndex(int keyIndex)
            => keysDown![keyIndex].value;
        public static Key? KeyByIndex(int keyIndex)
            => keysDown![keyIndex];
        public static bool ButtonDownByIndex(int keyIndex)
            => buttonsDown![keyIndex].value;
        public static Button? ButtonByIndex(int keyIndex)
            => buttonsDown![keyIndex];

        public static Key? Key(int keycode)
        {
            for (int i = 0; i < keysDown!.Length; i++)
                if (keysDown[i].id == keycode)
                    return keysDown[i];

            return null;
        }
        public static bool KeyDown(int keycode)
        {
            for (int i = 0; i < keysDown!.Length; i++)
                if (keysDown[i].id == keycode)
                    return keysDown[i].value;

            return false;
        }
        public static Key? Key(string key)
        {
            for (int i = 0; i < keysDown!.Length; i++)
                if (keysDown[i].name == key)
                    return keysDown[i];

            return null;
        }
        public static bool KeyDown(string key)
        {
            for (int i = 0; i < keysDown!.Length; i++)
                if (keysDown[i].name == key)
                    return keysDown[i].value;

            return false;
        }
        public static Button? Button(string key)
        {
            for (int i = 0; i < buttonsDown!.Length; i++)
                if (buttonsDown[i].name == key)
                    return buttonsDown[i];

            return null;
        }
        public static bool ButtonDown(string key)
        {
            for (int i = 0; i < buttonsDown!.Length; i++)
                if (buttonsDown[i].name == key)
                    return buttonsDown[i].value;

            return false;
        }


        private static Keys? previousKey;
        private static MouseButton? previousButton;
        public static void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (previousKey! == e.Key)
                return;

            previousKey = e.Key;
            for (int i = 0; i < keysDown!.Length; i++)
            {
                if ((int)e.Key == keysDown[i].id)
                {
                    keysDown[i].value = true;
                    return;
                }
            }
        }

        public static void OnKeyUp(KeyboardKeyEventArgs e)
        {
            previousKey = null;
            for (int i = 0; i < keysDown!.Length; i++)
            {
                if ((int)e.Key == keysDown[i].id)
                {
                    keysDown[i].value = false;
                    return;
                }
            }
        }

        public static void OnMouseUp(MouseButtonEventArgs e)
        {
            previousButton = null;
            for (int i = 0; i < buttonsDown!.Length; i++)
            {
                if (e.Button.ToString() == buttonsDown[i].name)
                {
                    buttonsDown[i].value = false;
                    return;
                }
            }
        }

        public static void OnMouseDown(MouseButtonEventArgs e)
        {
            if (previousButton! == e.Button)
                return;

            previousButton = e.Button;
            for (int i = 0; i < buttonsDown!.Length; i++)
            {
                if (e.Button.ToString() == buttonsDown[i].name)
                {
                    buttonsDown[i].value = true;
                    return;
                }
            }
        }
    }
}
