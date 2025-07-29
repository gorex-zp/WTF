using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WtfApp.App2.Objects.Alive.Interface
{
    interface IMovable
    {
        Vector2 moveTo { get; set; }
        void Move();

    }
}
