using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;

namespace Utilities
{
    /// <summary>
    /// Struct to make a triangle.
    /// </summary>
    public struct Triangle
    {
        public float angle;

        private float sideALength;
        private float sideBLength;
        private float sideCLength;

        //Getters for the side lengths.
        public float SideALength { get => sideALength; }
        public float SideBLength { get => sideBLength; }
        public float SideCLength { get => sideCLength; }

        /// <summary>
        /// Creates a right triangle where theta = angle, side a = a.
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="angle"></param>
        public Triangle(float b, float angle)
        {
            this.angle = angle;

            sideBLength = b;
            sideALength = sideBLength * Mathf.Tan(angle * Mathf.Deg2Rad);
            sideCLength = Mathf.Sqrt(Mathf.Pow(sideALength, 2) + Mathf.Pow(sideBLength, 2));
        }
    }

    public static class UnityUtilities
    {
        #region Triangle Functions

        public enum drawDirection
        {
            up, down, right, left
        }

        /// <summary>
        /// Draws a given triangle starting at origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="shape"></param>
        public static void DrawRightTriangle(Vector3 origin, Triangle shape, Transform parent)
        {
            Vector3 point;
            Vector3 point2;


            point = origin + (parent.forward * shape.SideALength);
            point2 = (parent.forward * shape.SideALength) + (parent.right * shape.SideBLength);

            //Draws side A.
            UnityEngine.Debug.DrawLine(origin, point);

            //Draws side B.
            UnityEngine.Debug.DrawLine(point, point2);

            //Draws side C.
            UnityEngine.Debug.DrawLine(origin, point2);
        }

        /// <summary>
        /// Draws a given triangle starting at origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="shape"></param>
        public static void Draw2DRightTriangle(Vector3 origin, Triangle shape, Transform parent)
        {
            Vector3 point;
            Vector3 point2;

            //Draws side A.
            point = origin + (parent.up * shape.SideALength);
            UnityEngine.Debug.DrawLine(origin, point);

            //Draws side B.
            point2 = (parent.up * shape.SideALength) + (parent.right * shape.SideBLength);
            UnityEngine.Debug.DrawLine(point, point2);

            //Draws side C.
            UnityEngine.Debug.DrawLine(origin, point2);
        }

        /// <summary>
        /// Draws a given triangle starting at origin.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="shape"></param>
        public static void Draw2DRightTriangle(Vector3 origin, Triangle shape, Transform parent, drawDirection drawDir)
        {
            Vector3 point;
            Vector3 point2;
            Vector3 dir;
            Vector3 right;

            switch (drawDir) //Find a better way to do this
            {
                case drawDirection.up:
                    dir = parent.up;
                    right = parent.right;
                    break;
                case drawDirection.down:
                    dir = -parent.up;
                    right = parent.right;
                    break;
                case drawDirection.left:
                    dir = -parent.right;
                    right = -parent.up;
                    break;
                case drawDirection.right:
                    dir = parent.right;
                    right = -parent.up;
                    break;
                default:
                    dir = parent.up;
                    right = parent.right;
                    break;
            }

            //Draws side A.
            point = origin + (dir * shape.SideALength);
            UnityEngine.Debug.DrawLine(origin, point);

            //Draws side B.
            point2 = (dir * shape.SideALength) + (right * shape.SideBLength);
            UnityEngine.Debug.DrawLine(point, point2);

            //Draws side C.
            UnityEngine.Debug.DrawLine(origin, point2);
        }

        /// <summary>
        /// Draws a non-right triangle with starting with theta = 2 * halfAngle.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="halfAngle"></param>
        /// <param name="length"></param>
        public static void DrawTriangle(Vector3 origin, float halfAngle, float length)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Vectors

        /// <summary>
        /// Returns the input vector with x component changed
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        /// <summary>
        /// Returns the input vector with y component changed
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        /// <summary>
        /// Returns the input vector with z component changed
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }

        /// <summary>
        /// Returns a vector3 where x = vector.x, y = 0, and z = vector.y
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Vector2To3(this Vector2 vector)
        {
            return new Vector3(vector.x, 0f, vector.y);
        }

        /// <summary>
        /// Returns a vector3 where x = vector.x, y = input var y, and z = vector.y
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 Vector2To3(this Vector2 vector, float y)
        {
            return new Vector3(vector.x, y, vector.y);
        }

        /// <summary>
        /// Returns the midpoint of two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2 Midpoint(Vector2 a, Vector2 b)
        {
            return (a + b) / 2f;
        }

        /// <summary>
        /// Returns the midpoint of two vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 Midpoint(Vector3 a, Vector3 b)
        {
            return (a + b) / 2f;
        }

        #endregion

        #region Math

        /// <summary>
        /// The difference between two floats
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Difference(float x, float y)
        {
            return Mathf.Abs(x - y);
        }

        /// <summary>
        /// The difference between two integers
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int Difference(int x, int y)
        {
            return Mathf.Abs(x - y);
        }

        /// <summary>
        /// Gets a random point within a certain radius from the center point
        /// </summary>
        /// <param name="centerPoint"> The center of the circle of valid locations </param>
        /// <param name="radius"> The extent from the center defining how large the circle of valid locations is </param>
        /// <returns></returns>
        public static Vector3 GetPointInRadius(Vector3 centerPoint, float radius)
        {
            Vector3 movePoint = new();
            movePoint.x = Random.Range(centerPoint.x - radius, centerPoint.x + radius);
            movePoint.y = centerPoint.y;
            movePoint.z = Random.Range(centerPoint.z - radius, centerPoint.z + radius);

            return movePoint;
        }

        #endregion

        #region Mesh Rendering

        //Add square drawing function

        //Add triangle drawing function

        #endregion

        #region Misc

        /// <summary>
        /// Returns true if the value is not null, false if the value is null.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsValid<T>(T obj)
        {
            return obj != null;
        }

        #endregion
    }

    public class BinarySearcher<T>
    {
        /// <summary>
        /// Generic binary search algorithm
        /// </summary>
        /// <param name="obj"> Array to search through </param>
        /// <param name="high"> Upper index of array </param>
        /// <param name="low"> Lower index of array </param>
        /// <param name="target"> The object to find in the array </param>
        /// <returns></returns>
        public static T BinarySearch(T[] obj, int high, int low, string target)
        {
            int mid = (low + high) / 2;
            System.IComparable compare = (System.IComparable)obj[mid];
            int compareResult = compare.CompareTo(target);

            if (compareResult == 0)
            {
                return obj[mid];
            }
            else if(compareResult > 0)
            {
                return BinarySearch(obj, high, mid + 1, target);
            }
            else
            {
                return BinarySearch(obj, mid - 1, low, target);
            }
        }
    }
}


