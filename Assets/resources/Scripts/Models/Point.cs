#region License
// ==============================================================================
// Project Space Copyright (C) 2016 Mathew Strauss
// ==============================================================================
#endregion

using System.Runtime.CompilerServices;

namespace ProjectSpace.Models {
    /// <summary>
    /// Represents a point on the game board, determined by an X and Y position.
    /// </summary>
    public class Point {

        /// <summary>
        /// X position of the point.
        /// </summary>
        public float X { get; set; }
        /// <summary>
        /// Y position of the point.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Point() {

        }

        /// <summary>
        /// Constructor that initializes the starting X and Y values.
        /// </summary>
        /// <param name="x">X position of the point</param>
        /// <param name="y">Y position of the point</param>
        public Point(float x, float y) {
            X = x;
            Y = y;
        }
        
        /// <summary>
        /// Determines the equality of two points. Equality is determined by the X and Y values.
        /// </summary>
        /// <param name="obj">Other point to compare</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            Point otherPoint = (Point) obj;
            return RuntimeHelpers.Equals(X, otherPoint.X) && RuntimeHelpers.Equals(Y, otherPoint.Y);
        }

        /// <summary>
        /// Gets the hascode of the point, defined by the X and Y values.
        /// </summary>
        /// <returns>Hashcode of the point</returns>
        public override int GetHashCode() {
            return X.GetHashCode() + Y.GetHashCode();
        }
    }
}