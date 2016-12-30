namespace ProjectSpace.Models
{
    public class Point
    {
        public Point()
        {

        }

        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        
        public float X { get; set; }
        public float Y { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (base.Equals(obj))
            {
                return true;
            }
            Point otherPoint = (Point)obj;
            return this.X == otherPoint.X && this.Y == otherPoint.Y;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }
    }
}