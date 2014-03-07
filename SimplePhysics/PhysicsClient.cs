﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jypeli;

namespace Jypeli.Physics
{
    /// <summary>
    /// Fysiikkarajapinta Jypelille.
    /// </summary>
    public class PhysicsClient : IPhysicsClient
    {
        /// <summary>
        /// Luo uuden kappaleen.
        /// PhysicsObject kutsuu syntyessään.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public IPhysicsBody CreateBody( IPhysicsObject owner, double width, double height, Shape shape )
        {
            return new PhysicsBody( width, height, shape ) { Owner = owner };
        }
    }
}