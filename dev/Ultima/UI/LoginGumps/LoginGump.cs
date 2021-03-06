﻿/***************************************************************************
 *   LoginGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Security;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps
{
    public delegate void LoginEvent(string server, int port, string account, SecureString password);

    enum LoginGumpButtons
    {
        QuitButton = 0,
        LoginButton = 1,
        CreditsButtons = 2
    }
    enum LoginGumpTextFields
    {
        AccountName = 0,
        Password = 1
    }

    public class LoginGump : Gump
    {
        public event LoginEvent OnLogin;

        public LoginGump()
            : base(0, 0)
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            int hue = 902; // dark grey
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 0x0588));
            AddControl(new ResizePic(this, 96, 285, 0x13BE, 492, 190));
            AddControl(new GumpPic(this, 0, 0, 0x157C, 0)); // 0x2329 - upper-left border graphic
            AddControl(new GumpPic(this, 294, 42, 0x058A, 0)); // 0x2329 - castle graphic

            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)LoginGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;
            // Log in to Ultima Online
            AddControl(new TextLabelAscii(this, 230, 305, 2, hue, provider.GetString(3000038)));
            // Account Name
            AddControl(new TextLabelAscii(this, 181, 346, 2, hue, provider.GetString(3000099)));
            // Password
            AddControl(new TextLabelAscii(this, 181, 386, 2, hue, provider.GetString(3000103)));
            // name field
            TextEntry g1 = new TextEntry(this, 332, 346, 200, 20, 0, (int)LoginGumpTextFields.AccountName, 32, Settings.Login.UserName);
            g1.LeadingHtmlTag = "<basefont color=#000000><big>";
            AddControl(new ResizePic(this, g1));
            AddControl(g1);
            // password field
            TextEntry g2 = new TextEntry(this, 332, 386, 200, 20, 0, (int)LoginGumpTextFields.Password, 32, "");
            g2.IsPasswordField = true;
            g2.LeadingHtmlTag = "<basefont color=#000000><big>";
            AddControl(new ResizePic(this, g2));
            AddControl(g2);
            // login button
            AddControl(new Button(this, 550, 439, 5540, 5542, ButtonTypes.Activate, 0, (int)LoginGumpButtons.LoginButton));
            ((Button)LastControl).GumpOverID = 5541;
            // Version information
            AddControl(new HtmlGumpling(this, 120, 440, 400, 20, 0, 0, "<center><medium><outline><font color='#DDDDDD'>" + Utility.VersionString));

            // flag graphic
            AddControl(new GumpPic(this, 0, 0, 0x15A0, 0));
            // credits
            AddControl(new Button(this, 10, 450, 0x15A8, 0x15A7, ButtonTypes.Activate, 0, (int)LoginGumpButtons.CreditsButtons));

            IsUncloseableWithRMB = true;
        }

        public override void OnButtonClick(int buttonID)
        {
            string accountName = GetTextEntry((int)LoginGumpTextFields.AccountName);
            string password = GetTextEntry((int)LoginGumpTextFields.Password);

            switch ((LoginGumpButtons)buttonID)
            {
                case LoginGumpButtons.QuitButton:
                    UltimaGame.IsRunning = false;
                    break;
                case LoginGumpButtons.LoginButton:
                {
                    SecureString secureStr = new SecureString();
                    if (password.Length > 0)
                    {
                        foreach (char c in password.ToCharArray())
                        {
                            secureStr.AppendChar(c);
                        }
                    }

                    OnLogin(Settings.Login.ServerAddress, Settings.Login.ServerPort, accountName, secureStr);
                    break;
                }
                case LoginGumpButtons.CreditsButtons:
                    UserInterface.AddControl(new CreditsGump(), 0, 0);
                    break;
            }

            Settings.Login.UserName = accountName;
        }
        public override void OnKeyboardReturn(int textID, string text)
        {
            OnButtonClick((int)LoginGumpButtons.LoginButton);
        }
    }
}
