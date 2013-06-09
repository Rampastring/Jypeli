﻿#region MIT License
/*
 * Copyright (c) 2013 University of Jyväskylä, Department of Mathematical
 * Information Technology.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

/*
 * Authors: Tero Jäntti, Tomi Karppinen, Janne Nikkanen.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli.Controls;
using Microsoft.Xna.Framework;

namespace Jypeli
{
    public partial class Game : ControlContexted
    {
        private ListenContext _context = new ListenContext() { Active = true };
        private GamePad[] _gamePads;
        private bool initialized;

        /// <summary>
        /// Näppäimistö
        /// </summary>
        public Keyboard Keyboard { get; private set; }

        /// <summary>
        /// Hiiri
        /// </summary>
        public Mouse Mouse { get; private set; }

        /// <summary>
        /// Kosketusnäyttö
        /// </summary>
        public TouchPanel TouchPanel { get; private set; }

        /// <summary>
        /// Ensimmäinen peliohjain.
        /// </summary>
        public GamePad ControllerOne { get { return _gamePads[0]; } }

        /// <summary>
        /// Toinen peliohjain.
        /// </summary>
        public GamePad ControllerTwo { get { return _gamePads[1]; } }

        /// <summary>
        /// Kolmas peliohjain.
        /// </summary>
        public GamePad ControllerThree { get { return _gamePads[2]; } }

        /// <summary>
        /// Neljäs peliohjain.
        /// </summary>
        public GamePad ControllerFour { get { return _gamePads[3]; } }

        /// <summary>
        /// Pelin pääohjainkonteksti.
        /// </summary>
        public ListenContext ControlContext
        {
            get { return Instance._context; }
        }

        public bool IsModal
        {
            get { return false; }
        }

        private void InitControls()
        {
            Keyboard = new Keyboard();
            Mouse = new Mouse();
            TouchPanel = new TouchPanel();

            _gamePads = new GamePad[4];
            _gamePads[0] = new GamePad( PlayerIndex.One );
            _gamePads[1] = new GamePad( PlayerIndex.Two );
            _gamePads[2] = new GamePad( PlayerIndex.Three );
            _gamePads[3] = new GamePad( PlayerIndex.Four );

            initialized = true;
        }

        private void UpdateControls( Time gameTime )
        {
            //if ( !initialized ) return;

            Keyboard.Update();
            Mouse.Update();
            TouchPanel.Update();

            for ( int i = 0; i < _gamePads.Length; i++ )
            {
                _gamePads[i].Update();
                
                if (!Game.Instance.IsPaused)
                    _gamePads[i].UpdateVibrations( gameTime );
            }
        }

        private void ActivateObject( ControlContexted obj )
        {
            obj.ControlContext.Active = true;

            if ( obj.IsModal )
            {
                Game.Instance.ControlContext.SaveFocus();
                Game.Instance.ControlContext.Active = false;

                foreach ( Layer l in Layers )
                {
                    foreach ( IGameObject lo in l.Objects )
                    {
                        ControlContexted co = lo as ControlContexted;
                        if ( lo == obj || co == null )
                            continue;

                        co.ControlContext.SaveFocus();
                        co.ControlContext.Active = false;
                    }
                }
            }
        }

        private void DeactivateObject( ControlContexted obj )
        {
            obj.ControlContext.Active = false;

            if ( obj.IsModal )
            {
                Game.Instance.ControlContext.RestoreFocus();

                foreach ( Layer l in Layers )
                {
                    foreach ( IGameObject lo in l.Objects )
                    {
                        ControlContexted co = lo as ControlContexted;
                        if ( lo == obj || co == null )
                            continue;

                        co.ControlContext.RestoreFocus();
                    }
                }
            }
        }
    }
}