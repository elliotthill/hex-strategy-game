using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace HexStrategy
{
    public static class ChatConsole
    {
        public static Texture2D form;
        public static Rectangle bounds;

        public static Rectangle textBox;
        public static String message = " ";

        public static Boolean FieldIsActive = false;
        public static KeyboardState ks, ksOld;

        public static void LoadContent()
        {
            form = Textures.DarkBrown;
            bounds = new Rectangle(4, Core.screenY - 374, 300, 370);
            textBox = new Rectangle(4, bounds.Y + bounds.Height-19, bounds.Width, 18);
        }


        public static void Update()
        {
            if (!UserInterface.chatWindowActive)
                return;

            if (Core.Maths.IsVector2InsideRect(new Vector2(Core.mouseState.X, Core.mouseState.Y), bounds))
                UserInterface.isMouseOverUI = true;

            UserInterface.isEditingTextField = FieldIsActive;
            if (!FieldIsActive)
                return;

            ks = Keyboard.GetState();
            UpdateTextField();

            ksOld = ks;
        }

        public static void Draw(SpriteBatch sb)
        {
            if (!UserInterface.chatWindowActive)
            {
                return;
            }
            Fonts.medium.Spacing = 0.6f;


            sb.Draw(form, bounds, UserInterface.grey);
            Border.Draw(bounds, sb);

            DrawRecentLogs(sb);
            DrawCurrentMessage(sb);
            Fonts.medium.Spacing = 1.3f;
        }

        private static void DrawRecentLogs(SpriteBatch sb)
        {
            int i = 0;
            foreach (String str in Logger.Log)
            {
                String thisString = str;
                if (str.Length > 60)
                    thisString = str.Substring(0, 53) + "...";


                sb.DrawString(Fonts.large, thisString, new Vector2(10, bounds.Y + 4 + (i * 15)), Color.White);
                i++;
            }
        }


        private static void DrawCurrentMessage(SpriteBatch sb)
        {
            Color clr = UserInterface.grey;

            if (Core.Maths.IsVector2InsideRect(new Vector2(Core.mouseState.X, Core.mouseState.Y), textBox))
            {
                clr = UserInterface.transparentWhite;


                if (Core.leftClickLastFrame)
                {
                    FieldIsActive = true;
                    clr = Color.Blue;
                }


            }
            else
            {
                if (Core.leftClickLastFrame)
                {
                    FieldIsActive = false;
                }
            }
            if (FieldIsActive)
                sb.Draw(form, textBox, UserInterface.transparentBlack);
            else
                sb.Draw(form, textBox, clr);

            Border.Draw(textBox, sb);

            sb.DrawString(Fonts.medium, message, new Vector2(textBox.X, textBox.Y), Color.White);
        }
        private static void UpdateTextField()
        {

            if (!FieldIsActive || !UserInterface.chatWindowActive)
                return;


            Keys[] pressedKeys;
            pressedKeys = ks.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (ksOld.IsKeyUp(key))
                {
                    if (key == Keys.Back && message.Length > 0)
                        message = message.Remove(message.Length - 1, 1);
                    else
                        if (key == Keys.Space)
                            message = message.Insert(message.Length, " ");
                        else
                        {
                            if (key != Keys.LeftShift && key != Keys.RightShift && key != Keys.Enter && key != Keys.LeftControl && key != Keys.Back)
                            {
                                if (key == Keys.OemQuestion)
                                    message += "?";
                                else if (key == Keys.OemComma)
                                    message += ",";
                                else if (key == Keys.OemPeriod)
                                    message += ".";
                                else if (key == Keys.D1 && Core.keyboardState.IsKeyDown(Keys.LeftShift))
                                    message += "!";
                                else if (key == Keys.D2 && Core.keyboardState.IsKeyDown(Keys.LeftShift))
                                    message += "'";
                                else if (key == Keys.OemTilde && Core.keyboardState.IsKeyDown(Keys.LeftShift))
                                    message += "'";
                                else if (key == Keys.D7 && Core.keyboardState.IsKeyDown(Keys.LeftShift))
                                    message += "&";
                                else
                                {

                                    if (Core.keyboardState.IsKeyDown(Keys.LeftShift))
                                    {
                                        message += key.ToString().ToUpper();
                                    }
                                    else
                                        message += key.ToString().ToLower();
                                }

                            }



                            if (key == Keys.Enter)
                            {
                                if (message.Trim().Count() > 0)
                                {
                                    //Eval
                                    
                                    String messageLower = message.ToLower().Trim();

                                    switch (messageLower)
                                    {
                                        case "save":
                                            Core.Save();
                                            break;

                                        case "load":
                                            Core.Load();
                                            break;

                                        case "stop":
                                            Core.giveallFaction = null;
                                            break;


                                    }

                                    if (messageLower.StartsWith("giveall"))
                                    {
                                        String[] words = messageLower.Split(' ');
                                        String factionName = words[1];
                                        Faction faction = Core.FindFaction(words[1]);

                                        if (faction != null)
                                            Core.giveallFaction = faction;
                                    }
                                    else if (messageLower.StartsWith("give"))
                                    {
                                        String[] words = messageLower.Split(' ');

                                        //second word is faction
                                        try
                                        {
                                            String factionName = words[1];
                                            Faction faction = Core.FindFaction(words[1]);

                                            if (faction != null && Core.map.selectedHex != null)
                                                faction.AnnexHex(Core.map.selectedHex);
                                            
                                        }
                                        catch
                                        {
                                        }

                                    }
                                    else if (messageLower.StartsWith("rename") || messageLower.StartsWith("name"))
                                    {
                                        String[] words = message.Split(' ');
                                        String name = words[1];

                                        Core.map.selectedHex.hexData.name = name;
                                    }
                                    else if (messageLower.StartsWith("build"))
                                    {
                                        String[] words = messageLower.Split(' ');

                                        switch (words[1])
                                        {
                                            case "town":
                                                Core.map.selectedHex.hexData.buildingType = BuildingType.Town;
                                                break;
                                            case "castle":
                                                Core.map.selectedHex.hexData.buildingType = BuildingType.Fortified;
                                                break;
                                            case "church":
                                                Core.map.selectedHex.hexData.buildingType = BuildingType.Church;
                                                break;
                                            case "market":
                                                Core.map.selectedHex.hexData.buildingType = BuildingType.Market;
                                                break;
                                        }
                                    }
                                    else if (messageLower.StartsWith("recruit"))
                                    {
                                        String[] words = messageLower.Split(' ');

                                        String factionName = words[1];
                                        Faction faction = Core.FindFaction(words[1]);

                                        if (faction != null && Core.map.selectedHex != null)
                                            faction.armyList.Add(new Army(Core.map.selectedHex, faction));
                                    }

                                    FieldIsActive = false;
                                    Logger.AddMessage(message);
                                }
                                message = "";
                                
                            }
                        }
                }
            }


            //Delete key should not be toggled
            //if (ks.IsKeyDown(Keys.Back) && message.Length > 0)
            //{
            //        message = message.Remove(message.Length - 1, 1);
            // }
        }

    }
}
