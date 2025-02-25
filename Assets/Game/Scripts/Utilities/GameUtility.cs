using System.Collections.Generic;
using UnityEngine;
namespace GameUtility
{
    public static class GameUtility
    {
        public static readonly string PLAYER_TAG = "Player";
        static System.Text.StringBuilder builder = new System.Text.StringBuilder();
        static readonly string zero = "0";
        private static int globalSeed;
        public static string GetId(int id)
        {
            builder.Clear();
            builder.Append(id <= 9 ? zero + zero : (id <= 99 ? zero : string.Empty)).Append(id);
            return builder.ToString();
        }
        public static int GetSeed()
        {
            return globalSeed==0? UnityEngine.Random.seed:globalSeed;
        }
        public static void SetSeed(int seed)
        {
            globalSeed = seed;
        }
        static string[] placeTemplates = { "st", "nd", "rd", "th" };
        public static string GetPlace(int rank)
        {
            return rank + placeTemplates[Mathf.Min(rank - 1, 3)];
        }
        public static string MinimizePrice(int price)
        {
            if (price < 1000) return price.ToString();
            string result;
            float p = (price / 1000f);
            p = ((int)(p * 10)) / 10f;
            result = p+"K";

            return result;
        }
        public static int Pow(int a,int b)
        {
            int result=a;
            while (b >= 1)
            {
                result *= a;
                b--;
            }

            return result;

        }


        public static float GetAngle(Vector2 a, Vector2 b)
        {
            var dir = b - a;
            dir.Normalize();
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        public static float GetAngle(Transform a, Transform b)
        {
            var dir = b.position - a.position;
            dir.Normalize();
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        public static float GetAngle(Vector3 dir)
        {
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        public static Vector2 ConvertDir(float angle)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        }

        public static void ResetTransformation(this Transform trans)
        {
            trans.position = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }
      
      
        public static void Log(object message)
        {
#if UNITY_EDITOR
            Debug.Log(message.ToString());
#endif
        }
        public static void LogError(object message)
        {
            Debug.LogError(message.ToString());
        }
        public static void LogWarning(object message)
        {
            Debug.LogWarning(message.ToString());
        }

        public static bool RandomBoolean(float value)
        {
            return UnityEngine.Random.value <= value;
        }

        public static float GetRange(Transform a, Transform b)
        {
            var posa = a.position;
            var posb = b.position;

            return (posa - posb).magnitude;
        }
        public static float GetRange(Vector3 a, Vector3 b)
        {
            var posa = a;
            var posb = b;

            return (posa - posb).magnitude;
        }
        public static Vector3 GetRandomPositionByRaycast(Vector3 start, float radius)
        {
            Vector3 pos = default;
            var hitCount = 0;
            do
            {
                var rd = UnityEngine.Random.insideUnitCircle.normalized * radius;
                pos = start + new Vector3(rd.x, rd.y, 0);
                var hitResults = new RaycastHit2D[1];
                hitCount = Physics2D.RaycastNonAlloc(pos, Vector2.zero, hitResults, Mathf.Infinity);
            } while (hitCount > 0);

            return pos;
        }

        public static Vector3 GetRandomPositionWithRaycast(Vector3 start, float radius)
        {
            Vector3 pos = default;
            for (int i = 0; i < 1000; i++)
            {
                Vector2 rd = UnityEngine.Random.insideUnitCircle.normalized * radius;
                pos = start + new Vector3(rd.x, rd.y, 0);

                RaycastHit2D hit = Physics2D.Raycast(new Vector3(pos.x, pos.y, -10), Vector3.forward, 50f);
                if (hit.transform == null)
                {
                    break;
                }
            }

            return pos;
        }
        public static List<Vector3> GetRandomPositionWithoutOverlapping(Vector3 starting, Vector2 size,
       float distanceBetweenPosition, int positionNumber, int ignoreMask = 0, bool isUseMask = false)
        {
            var points = new List<Vector3>();
            int countTry = 0;
            for (var i = 0; i < positionNumber;)
            {
                var randX = UnityEngine.Random.Range(-size.x / 2f, size.x / 2);
                var randY = UnityEngine.Random.Range(-size.y / 2f, size.y / 2);
                var rd = new Vector2(randX, randY);
                var point = starting + new Vector3(rd.x, rd.y, 0);
                countTry++;
                if (points.Count == 0)
                {
                    points.Add(point);
                    i++;
                    continue;
                }
                for (var j = 0; j < points.Count; j++)
                {
                    if ((point - points[j]).sqrMagnitude > distanceBetweenPosition * distanceBetweenPosition)
                    {
                        bool canAdd = true;
                        if (isUseMask)
                        {
                            if (IsOverlapPoint(point, ignoreMask))
                            {
                                canAdd = false;
                                break;
                            }
                        }
                        if (canAdd)
                        {
                            if (j == points.Count - 1)
                            {
                                points.Add(point);
                                i++;
                                countTry = 0;
                            }

                            continue;
                        }
                    }

                    if (countTry > 10)
                    {
                        points.Add(point);
                        i++;
                        countTry = 0;
                        break;
                    }

                    break;
                }
            }

            return points;
        }

        public static bool IsOverlapPoint(Vector3 point, LayerMask mask)
        {
            return Physics2D.OverlapPoint(point, mask);
        }

        public static Vector2 GetPositionImpactRaycast(Vector3 point, Vector2 dir, float distance, LayerMask ground)
        {
            var rch = Physics2D.Raycast(point, dir, distance, ground);
            if (rch.collider)
            {
                return rch.point;
            }
            return Vector3.zero;
        }
        public static Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float initAngle,float gravityScale=1)
        {
            Vector3 p = target;

            float gravity = Physics.gravity.magnitude*gravityScale;
            // Selected angle in radians
            float angle = initAngle * Mathf.Deg2Rad;

            // Positions of this object and the target on the same plane
            Vector3 planarTarget = new Vector3(p.x, 0);
            Vector3 planarPostion = new Vector3(source.x, 0);

            // Planar distance between objects
            float distance = Vector3.Distance(planarTarget, planarPostion);
            // Distance along the y axis between objects
            float yOffset = source.y - p.y;

            //limit the init velocity by 22
            float initialVelocity = Mathf.Min((1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset)),22);

            Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

            // Rotate our velocity to match the direction between the two objects
            float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
            Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

            if (target.x < source.x)
            {
                finalVelocity.x = -finalVelocity.x;
            }
            return finalVelocity * 1;
        }
        public static bool LaunchAngle(float speed, float distance, float yOffset, float gravity, out float angle0, out float angle1)
        {
            angle0 = angle1 = 0;

            float speedSquared = speed * speed;

            float operandA = Mathf.Pow(speed, 4);
            float operandB = gravity * (gravity * (distance * distance) + (2 * yOffset * speedSquared));

            // Target is not in range
            if (operandB > operandA)
                return false;

            float root = Mathf.Sqrt(operandA - operandB);

            angle0 = Mathf.Atan((speedSquared + root) / (gravity * distance));
            angle1 = Mathf.Atan((speedSquared - root) / (gravity * distance));

            return true;
        }

        /// <summary>
        /// Calculates the initial launch speed required to hit a target at distance with elevation yOffset.
        /// </summary>
        /// <param name="distance">Planar distance from origin to the target</param>
        /// <param name="yOffset">Elevation of the origin with respect to the target</param>
        /// <param name="gravity">Downwards force of gravity in m/s^2</param>
        /// <param name="angle">Initial launch angle in radians</param>
        public static float LaunchSpeed(float distance, float yOffset, float gravity, float angle)
        {
            float speed = (distance * Mathf.Sqrt(gravity) * Mathf.Sqrt(1 / Mathf.Cos(angle))) / Mathf.Sqrt(2 * distance * Mathf.Sin(angle) + 2 * yOffset * Mathf.Cos(angle));

            return speed;
        }

        public static Vector2[] ProjectileArcPoints(int iterations, float speed, float distance, float gravity, float angle)
        {
            float iterationSize = distance / iterations;

            float radians = angle;

            Vector2[] points = new Vector2[iterations + 1];

            for (int i = 0; i <= iterations; i++)
            {
                float x = iterationSize * i;
                float t = x / (speed * Mathf.Cos(radians));
                float y = -0.5f * gravity * (t * t) + speed * Mathf.Sin(radians) * t;

                Vector2 p = new Vector2(x, y);

                points[i] = p;
            }

            return points;
        }
        public static Vector2 RotationToVector(float rotationAngle)
        {
            return new Vector2(Mathf.Cos(rotationAngle * Mathf.Deg2Rad), Mathf.Sin(rotationAngle * Mathf.Deg2Rad));
        }
        public static Color GetColor(string hex, bool pre = false)
        {
            var news = $"#{hex}";
            ColorUtility.TryParseHtmlString(news, out var c);
            c.a = 1;
            return c;
        }

        public static float GetRandomAngle(float upper, float lower)
        {
            var diff = (upper - lower);
            return lower + UnityEngine.Random.Range(1, (int)diff);
        }
    }
}