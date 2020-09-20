using System.Drawing;

namespace HGE.Graphics
{
    /// <summary>
    /// Particle structure based on integer
    /// </summary>
    public struct ParticleA
    {
        public Pixel Color;
        public bool IsDead;

        public int X_Direction => Direction != null ? Direction.X : -1;
        public int Y_Direction => Direction != null ? Direction.Y : -1;
        public int X_Position => Position != null ? Position.X : -1;
        public int Y_Position => Position != null ? Position.Y : -1;

        public Point Direction;
        public Point Position;
    }
    /// <summary>
    /// Particle structure based on floating point
    /// </summary>
    public struct ParticleB
    {
        public Pixel Color;
        public bool IsDead;

        public float X_Direction => Direction != null ? Direction.X : -1;
        public float Y_Direction => Direction != null ? Direction.Y : -1;
        public float X_Position => Position != null ? Position.X : -1;
        public float Y_Position => Position != null ? Position.Y : -1;

        public PointF Direction;
        public PointF Position;
    }
}
