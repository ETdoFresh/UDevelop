using UnityEngine;

namespace Jint
{
    public static class Util
    {
        public static int Add(int a, int b) => a + b;
        public static int Subtract(int a, int b) => a - b;
        public static int Multiply(int a, int b) => a * b;
        public static int Divide(int a, int b) => a / b;
        public static int Modulo(int a, int b) => a % b;
        
        public static float Add(float a, float b) => a + b;
        public static float Subtract(float a, float b) => a - b;
        public static float Multiply(float a, float b) => a * b;
        public static float Divide(float a, float b) => a / b;
        public static float Modulo(float a, float b) => a % b;
        
        public static Vector2 Add(Vector2 a, Vector2 b) => a + b;
        public static Vector2 Subtract(Vector2 a, Vector2 b) => a - b;
        public static Vector2 Multiply(float a, Vector2 b) => a * b;
        public static Vector2 Multiply(Vector2 a, float b) => a * b;
        public static Vector2 Divide(Vector2 a, float b) => a / b;
        
        public static Vector3 Add(Vector3 a, Vector3 b) => a + b;
        public static Vector3 Subtract(Vector3 a, Vector3 b) => a - b;
        public static Vector3 Multiply(float a, Vector3 b) => a * b;
        public static Vector3 Multiply(Vector3 a, float b) => a * b;
        public static Vector3 Divide(Vector3 a, float b) => a / b;

        public static bool IsNotNull(object obj) => obj as Object ?? obj != null;
        public static bool IsNull(object obj) => obj is Object uObj ? !uObj : obj == null;
    }
}