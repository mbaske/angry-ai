using UnityEngine;
using System;

public class CollisionArgs : EventArgs
{
    public const int ENTER = 0;
    public const int STAY = 1;
    public const int EXIT = 2;

    public Collision Collision { get; private set; }
    public int state { get; private set; }
    // Collision can be null which just means no collision after timeout.
    // Used for bullets.
    public string Tag => Collision == null ? Tags.NONE : Collision.gameObject.tag;

    public CollisionArgs(Collision collision, int state = 0)
    {
        Collision = collision;
        this.state = state;
    }
}