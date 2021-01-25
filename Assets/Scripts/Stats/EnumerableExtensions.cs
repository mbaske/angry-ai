using UnityEngine;
using System.Collections.Generic;

namespace MBaske
{
    public static class EnumerableExtensions
    {
        public static float MAD(this IEnumerable<int> values)
        {
            int n = 0;
            float mean = values.Average();
            float sum = 0;
            float mad = 0;
            
            foreach (int val in values)
            {
                n++;
                float delta = val - mean;
                sum += Mathf.Abs(delta);
            }

            if (n > 1)
            {
                mad = sum / (float)n;
            }
            
            return mad;
        }

        public static float MAD(this IEnumerable<float> values)
        {
            int n = 0;
            float mean = values.Average();
            float sum = 0;
            float mad = 0;
            
            foreach (float val in values)
            {
                n++;
                float delta = val - mean;
                sum += Mathf.Abs(delta);
            }

            if (n > 1)
            {
                mad = sum / (float)n;
            }
            
            return mad;
        }


        public static float StdDev(this IEnumerable<int> values, bool relative = false)
        {
            int n = 0;
            float mean = 0;
            float sum = 0;
            float stdDev = 0;
            
            foreach (int val in values)
            {
                n++;
                float delta = val - mean;
                mean += delta / (float)n;
                sum += delta * (val - mean);
            }

            if (n > 1 && !sum.Equals(0f))
            {
                stdDev = Mathf.Sqrt(sum / (float)(n - 1));
                if (relative)
                {
                    float avg = values.Average();
                    if (!avg.Equals(0f))
                    {
                         // TODO
                        stdDev /= avg;
                    }
                }
            }
            
            return stdDev;
        }

        public static float StdDev(this IEnumerable<float> values, bool relative = false)
        {
            int n = 0;
            float mean = 0;
            float sum = 0;
            float stdDev = 0;
            
            foreach (float val in values)
            {
                n++;
                float delta = val - mean;
                mean += delta / (float)n;
                sum += delta * (val - mean);
            }

            if (n > 1 && !sum.Equals(0f))
            {
                stdDev = Mathf.Sqrt(sum / (float)(n - 1));
                if (relative)
                {
                    float avg = values.Average();
                    if (!avg.Equals(0f))
                    {
                         // TODO
                        stdDev /= avg;
                    }
                }
            }
            
            return stdDev;
        }


        public static float Average(this IEnumerable<int> values)
        {
            int n = 0;
            float sum = 0;
            
            foreach (int val in values)
            {
                n++;
                sum += val;
            }

            if (n > 1)
            {
                return sum / (float)n;
            }
            
            return sum;
        }

        public static float Average(this IEnumerable<float> values)
        {
            int n = 0;
            float sum = 0;
            
            foreach (float val in values)
            {
                n++;
                sum += val;
            }

            if (n > 1)
            {
                return sum / (float)n;
            }
            
            return sum;
        }


        public static float Sum(this IEnumerable<int> values)
        {
            float sum = 0;

            foreach (int val in values)
            {
                sum += val;
            }

            return sum;
        }

        public static float Sum(this IEnumerable<float> values)
        {
            float sum = 0;

            foreach (float val in values)
            {
                sum += val;
            }

            return sum;
        }


        public static float Min(this IEnumerable<int> values)
        {
            float min = Mathf.Infinity;

            foreach (int val in values)
            {
                min = Mathf.Min(min, val);
            }

            return min;
        }

        public static float Min(this IEnumerable<float> values)
        {
            float min = Mathf.Infinity;

            foreach (float val in values)
            {
                min = Mathf.Min(min, val);
            }

            return min;
        }


        public static float Max(this IEnumerable<int> values)
        {
            float max = Mathf.NegativeInfinity;

            foreach (int val in values)
            {
                max = Mathf.Max(max, val);
            }

            return max;
        }

        public static float Max(this IEnumerable<float> values)
        {
            float max = Mathf.NegativeInfinity;

            foreach (float val in values)
            {
                max = Mathf.Max(max, val);
            }

            return max;
        }
    }
}